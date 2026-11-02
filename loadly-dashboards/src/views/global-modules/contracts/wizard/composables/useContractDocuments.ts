import { useLoading } from '@/composables/useLoading'
import { useDocumentStore } from '@/stores/admin-dashboard/document'
import type { ApiDocumentInfo, DocumentInfo } from '@/types/admin-dashboard/response/document'

export interface ContractDocument {
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
  uploadedRecordId?: number
}

export function useContractDocuments(
  emit?: (event: 'update', data: Record<string, unknown>) => void
) {
  const { t } = useI18n()
  const documentStore = useDocumentStore()
  const { setLoading } = useLoading()
  const documents = ref<ContractDocument[]>([])

  const requiredDocuments = computed(() => documents.value.filter(doc => doc.required))
  const optionalDocuments = computed(() => documents.value.filter(doc => !doc.required))

  async function fetchDocuments() {
    setLoading(true)
    const { data } = await documentStore.getList({
      pagination: { page: 1, size: 100 },
      entityId: 6,
    })

    setLoading(false)
    documents.value = data.documents.map((doc: ContractDocument) => ({
      label: doc.label,
      required: doc.required,
      file: undefined,
      fileUrl: doc.fileUrl || null,
      uploadState: doc.fileUrl ? 'success' : 'idle',
      uploadProgress: 0,
      id: doc.id,
    }))
  }

  function updateDocument(index: number, doc: Partial<ContractDocument>): void {
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
        id: doc.uploadedRecordId,
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

  function restoreFormValues(formValues: Record<string, unknown>): void {
    const docsMeta = formValues._documents as DocumentInfo[] | undefined
    if (!docsMeta || !Array.isArray(docsMeta)) return

    documents.value.forEach((doc, index) => {
      const typeId = doc.id
      const meta = docsMeta.find(
        (m) => (m.documentId != null && m.documentId === typeId) || m.id === typeId
      )
      doc.expirationDate = meta?.expirationDate ?? doc.expirationDate
      doc.status = meta?.status ?? doc.status
      if (meta?.fileUrl) {
        doc.fileUrl = meta.fileUrl
        doc.uploadState = 'success'
        doc.uploadProgress = 100
      }
      if (meta?.id != null) {
        doc.uploadedRecordId = meta.id
      }

      const file = formValues[`document_${index}`] as File | undefined
      if (file instanceof File) {
        doc.file = file
        doc.uploadState = 'success'
        doc.uploadProgress = 100
      }
    })
  }

  return {
    documents,
    requiredDocuments,
    optionalDocuments,
    fetchDocuments,
    restoreFormValues,
    updateDocument,
    handleFileChange,
    handleUpload,
    removeFile,
    retryUpload,
    validateRequiredDocuments,
  }
}

