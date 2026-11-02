import { useLoading } from '@/composables/useLoading'
import { useDocumentStore } from '@/stores/admin-dashboard/document'
import type { ApiDocumentInfo } from '@/types/admin-dashboard/response/document'

export interface VehicleDocument {
  label: string
  required: boolean
  file?: File
  fileUrl?: string | null
  uploadState?: 'idle' | 'uploading' | 'success' | 'error'
  uploadProgress?: number
  uploadError?: string
  status?: string
  expirationDate?: string
  id?: number
}

type InitialDocumentsSource =
  | Ref<ApiDocumentInfo[] | undefined>
  | (() => ApiDocumentInfo[] | undefined)

export function useVehicleDocuments(
  emit?: (event: 'update', data: Record<string, unknown>) => void,
  initialDocumentsSource?: InitialDocumentsSource
) {
  const { t } = useI18n()
  const documentStore = useDocumentStore()
  const { setLoading } = useLoading()
  const documents = ref<VehicleDocument[]>([])

  const requiredDocuments = computed(() => documents.value.filter(doc => doc.required))
  const optionalDocuments = computed(() => documents.value.filter(doc => !doc.required))

  function getInitialDocuments(): ApiDocumentInfo[] | undefined {
    if (!initialDocumentsSource) return undefined

    return typeof initialDocumentsSource === 'function'
      ? initialDocumentsSource()
      : initialDocumentsSource.value
  }

  function mergeVehicleDocumentsFromApi(
    fetchedDocs: VehicleDocument[],
    apiDocuments: ApiDocumentInfo[]
  ): VehicleDocument[] {
    const byDocumentId = new Map<number, ApiDocumentInfo>()
    apiDocuments.forEach((d) => {
      const key = d.documentId ?? d.id
      if (key != null) byDocumentId.set(key, d)
    })

    return fetchedDocs.map((doc) => {
      const fromApi = doc.id != null ? byDocumentId.get(doc.id) : undefined
      if (!fromApi) return doc

      const hasFile = !!fromApi.fileUrl

      return {
        ...doc,
        fileUrl: fromApi.fileUrl ?? doc.fileUrl,
        expirationDate: fromApi.expirationDate ?? doc.expirationDate,
        status: fromApi.status ?? doc.status,
        uploadState: hasFile ? 'success' as const : doc.uploadState,
        uploadProgress: hasFile ? 100 : doc.uploadProgress ?? 0,
      }
    })
  }

  async function fetchDocuments() {
    setLoading(true)
    const { data } = await documentStore.getList({
      pagination: { page: 1, size: 100 },
      entityId: 4,
    })

    setLoading(false)
    let list = data.documents.map((doc: VehicleDocument) => ({
      label: doc.label,
      required: doc.required,
      file: undefined,
      fileUrl: doc.fileUrl || null,
      uploadState: doc.fileUrl ? 'success' : 'idle',
      uploadProgress: 0,
      id: doc.id,
    }))

    const apiDocuments = getInitialDocuments()
    if (apiDocuments?.length) {
      list = mergeVehicleDocumentsFromApi(list, apiDocuments)
    }

    documents.value = list
  }

  function updateDocument(index: number, doc: Partial<VehicleDocument>): void {
    if (!documents.value[index]) return
    documents.value[index] = { ...documents.value[index], ...doc }
    updateFormData()
  }

  function handleFileChange(file: File, index: number): void {
    if (!documents.value[index]) return

    const MAX_FILE_SIZE = 10 * 1024 * 1024
    if (file.size > MAX_FILE_SIZE) {
      documents.value[index].uploadState = 'error'
      documents.value[index].uploadError = t('error.documentUploadFailed')

      return
    }

    documents.value[index].file = file
    documents.value[index].uploadState = 'uploading'
    documents.value[index].uploadProgress = 0
    documents.value[index].uploadError = undefined

    simulateUpload(index)

    updateFormData()
  }

  function handleUpload(data: { file: File; status?: string; expiryDate?: string }, index: number): void {
    if (!documents.value[index]) return

    const MAX_FILE_SIZE = 10 * 1024 * 1024
    if (data.file.size > MAX_FILE_SIZE) {
      documents.value[index].uploadState = 'error'
      documents.value[index].uploadError = t('error.documentUploadFailed')

      return
    }

    documents.value[index].file = data.file
    documents.value[index].status = data.status
    documents.value[index].expirationDate = data.expiryDate
    documents.value[index].uploadState = 'uploading'
    documents.value[index].uploadProgress = 0
    documents.value[index].uploadError = undefined

    simulateUpload(index)

    updateFormData()
  }

  function simulateUpload(index: number): void {
    const interval = setInterval(() => {
      if (!documents.value[index]) {
        clearInterval(interval)

        return
      }

      documents.value[index].uploadProgress = (documents.value[index].uploadProgress || 0) + 10
      if ((documents.value[index].uploadProgress ?? 0) >= 100) {
        clearInterval(interval)
        documents.value[index].uploadState = 'success'
        documents.value[index].uploadProgress = 100
      }
    }, 400)
  }

  function retryUpload(index: number): void {
    if (!documents.value[index]?.file) return
    documents.value[index].uploadState = 'uploading'
    documents.value[index].uploadProgress = 0
    documents.value[index].uploadError = undefined
    simulateUpload(index)
  }

  function removeFile(index: number): void {
    if (!documents.value[index]) return
    documents.value[index].file = undefined
    documents.value[index].fileUrl = null
    documents.value[index].uploadState = 'idle'
    documents.value[index].uploadProgress = 0
    documents.value[index].uploadError = undefined
    updateFormData()
  }

  function updateFormData(): void {
    const filesData: Record<string, File | undefined> = {}
    const documentsData: ApiDocumentInfo[] = []

    documents.value.forEach((doc, index) => {
      if (doc.file) {
        filesData[`document_${index}`] = doc.file
      }
      documentsData.push({
        id: doc.id,
        documentId: doc.id,
        label: doc.label,
        required: doc.required,
        expirationDate: doc.expirationDate,
        status: doc.status,
        fileUrl: doc.fileUrl ?? undefined,
      })
    })

    emit?.('update', { ...filesData, _documents: documentsData })
  }

  function validateRequiredDocuments(): { isValid: boolean; missingDocuments: string[] } {
    const missingDocuments: string[] = []

    documents.value.forEach((doc) => {
      if (doc.required) {
        const hasFile = !!doc.file
        const hasFileUrl = !!doc.fileUrl
        const isUploadingOrSuccess = doc.uploadState === 'uploading' || doc.uploadState === 'success'

        if (!hasFile && !hasFileUrl && !isUploadingOrSuccess) {
          missingDocuments.push(doc.label)
        }
      }
    })

    return {
      isValid: missingDocuments.length === 0,
      missingDocuments,
    }
  }

  return {
    documents,
    requiredDocuments,
    optionalDocuments,
    fetchDocuments,
    updateDocument,
    handleFileChange,
    handleUpload,
    removeFile,
    retryUpload,
    validateRequiredDocuments,
  }
}
