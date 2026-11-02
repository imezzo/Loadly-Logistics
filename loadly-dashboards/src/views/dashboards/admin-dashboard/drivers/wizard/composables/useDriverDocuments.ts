import { useLoading } from '@/composables/useLoading'
import { useDocumentStore } from '@/stores/admin-dashboard/document'
import type { ApiDocumentInfo, Document } from '@/types/admin-dashboard/response/document'
import type { DriverDocument } from '@/types/admin-dashboard/response/driver/driver-detail'

export interface DriverDocumentItem {
  label: string
  required: boolean
  file?: File
  fileUrl?: string | null
  uploadState?: 'idle' | 'uploading' | 'success' | 'error'
  uploadProgress?: number
  uploadError?: string
  status?: string
  expirationDate?: string
  documentId?: number
  id?: number
}

type InitialDocuments = ComputedRef<
  DriverDocument[] | DriverDocumentItem[] | undefined>
  | Ref<DriverDocument[] | DriverDocumentItem[] | undefined>
  | DriverDocument[]
  | DriverDocumentItem[]

export function useDriverDocuments(
  emit?: (event: 'submit', data: Record<string, unknown>) => void,
  initialDocuments?: InitialDocuments
) {
  const { t } = useI18n()
  const documentStore = useDocumentStore()
  const { setLoading } = useLoading()
  const documents = ref<DriverDocumentItem[]>([])

  const requiredDocuments = computed(() => documents.value.filter(doc => doc.required))
  const optionalDocuments = computed(() => documents.value.filter(doc => !doc.required))

  const initialDocumentsRef = computed(() => unref(initialDocuments))

  function isDriverDocumentItem(doc: DriverDocument | DriverDocumentItem): doc is DriverDocumentItem {
    return 'label' in doc && 'required' in doc
  }

  function mapDocumentToItem(doc: Document): DriverDocumentItem {
    return {
      label: doc.label,
      required: doc.required,
      file: undefined,
      fileUrl: null,
      uploadState: 'idle',
      uploadProgress: 0,
      id: doc.id,
      documentId: doc.id,
    }
  }

  function getDocumentKey(doc: DriverDocument | DriverDocumentItem): number {
    return doc.documentId || doc.id || 0
  }

  function createInitialDocumentsMap(
    initialDocs: (DriverDocument | DriverDocumentItem)[]
  ): Map<number, DriverDocument | DriverDocumentItem> {
    const map = new Map<number, DriverDocument | DriverDocumentItem>()
    initialDocs.forEach((doc) => {
      const key = getDocumentKey(doc)
      if (key) {
        map.set(key, doc)
      }
    })

    return map
  }

  function mergeDriverDocumentItem(
    initialDoc: DriverDocumentItem,
    fetchedDoc: DriverDocumentItem
  ): DriverDocumentItem {

    return {
      ...initialDoc,
      id: initialDoc.id || fetchedDoc.id,
      documentId: initialDoc.documentId || initialDoc.id || fetchedDoc.id,
    }
  }

  function getUploadState(initialDoc: DriverDocument): DriverDocumentItem['uploadState'] {
    if (initialDoc.fileUrl) return 'success'

    if (initialDoc.file) return 'uploading'

    return 'idle'
  }

  function mergeDriverDocument(
    initialDoc: DriverDocument,
    fetchedDoc: DriverDocumentItem
  ): DriverDocumentItem {
    const hasFile = !!initialDoc.fileUrl || !!initialDoc.file

    return {
      ...fetchedDoc,
      file: initialDoc.file,
      fileUrl: initialDoc.fileUrl,
      status: initialDoc.status?.toString(),
      expirationDate: initialDoc.expiryDate,
      documentId: initialDoc.documentId || initialDoc.id || fetchedDoc.id,
      id: initialDoc.id || fetchedDoc.id,
      uploadState: getUploadState(initialDoc),
      uploadProgress: hasFile ? 100 : 0,
    }
  }

  function mergeDocuments(
    fetchedDocs: DriverDocumentItem[],
    initialDocs: (DriverDocument | DriverDocumentItem)[]
  ): DriverDocumentItem[] {
    const initialDocsMap = createInitialDocumentsMap(initialDocs)

    return fetchedDocs.map((fetchedDoc: DriverDocumentItem) => {
      const initialDoc = initialDocsMap.get(fetchedDoc.id || 0)
      if (!initialDoc) return fetchedDoc

      return isDriverDocumentItem(initialDoc)
        ? mergeDriverDocumentItem(initialDoc, fetchedDoc)
        : mergeDriverDocument(initialDoc, fetchedDoc)
    })
  }

  async function fetchDocuments() {
    setLoading(true)
    const { data } = await documentStore.getList({
      pagination: { page: 1, size: 100 },
      entityId: 3,
    })
    setLoading(false)

    const fetchedDocs = data.documents.map(mapDocumentToItem)
    const currentInitialDocuments = initialDocumentsRef.value

    documents.value = currentInitialDocuments?.length
      ? mergeDocuments(fetchedDocs, currentInitialDocuments)
      : fetchedDocs
  }

  function updateDocument(index: number, doc: Partial<DriverDocumentItem>): void {
    if (!documents.value[index]) return
    documents.value[index] = { ...documents.value[index], ...doc }
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
        label: doc.label,
        required: doc.required,
        expirationDate: doc.expirationDate,
        status: doc.status,
      })
    })

    emit?.('submit', { ...filesData, _documents: documentsData })
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
      isValid: !missingDocuments.length,
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
    updateFormData,
  }
}
