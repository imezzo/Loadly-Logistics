import { useLoading } from '@/composables/useLoading'
import { useDocumentStore } from '@/stores/admin-dashboard/document'
import type { ApiDocumentInfo } from '@/types/admin-dashboard/response/document'

export function useDocuments(
  emit: (event: 'update', data: Record<string, unknown>) => void
) {
  const documentStore = useDocumentStore()
  const { setLoading } = useLoading()
  const documents = ref<ApiDocumentInfo[]>([])
  const fileInputs = ref<Record<number, HTMLInputElement>>({})
  const documentTemplateIds = ref<number[]>([])

  async function fetchDocuments() {
    setLoading(true)
    const { data } = await documentStore.getList({
      pagination: { page: 1, size: 10 },
      entityId: 2,
    })

    setLoading(false)
    documents.value = data.documents
    documentTemplateIds.value = data.documents.map((doc: { id?: number }) => doc.id || 0)
  }

  function triggerFileInput(index: number): void {
    fileInputs.value[index]?.click()
  }

  function handleFileChange(event: Event, index: number): void {
    const input = event.target as HTMLInputElement
    const file = input.files?.[0]
    if (file && documents.value[index]) {
      documents.value[index].file = file
      updateFormData()
    }
  }

  function handleDrop(event: DragEvent, index: number): void {
    const file = event.dataTransfer?.files[0]
    if (file && documents.value[index]) {
      documents.value[index].file = file
      updateFormData()
    }
  }

  function removeFile(index: number): void {
    if (documents.value[index]) {
      documents.value[index].file = undefined
      const input = fileInputs.value[index]
      if (input) {
        input.value = ''
      }
      updateFormData()
    }
  }

  function buildDocumentData(doc: ApiDocumentInfo, index: number) {
    const templateId = documentTemplateIds.value[index] || doc.id || 0
    if (!templateId) return null

    const companyDocId = (doc.id && doc.id !== templateId) ? doc.id : 0
    const metadata = {
      expirationDate: doc.expirationDate,
      status: doc.status,
    }

    return {
      templateId,
      companyDocId,
      companyDocument: {
        id: companyDocId,
        documentId: templateId,
        ...metadata,
        fileUrl: doc.fileUrl || null,
      },
      legacyDocument: companyDocId ? { id: templateId, ...metadata } : null,
    }
  }

  function updateFormData(): void {
    const filesData: Record<string, File | undefined> = {}
    const documentsData: { id: number; expirationDate?: string; status?: string }[] = []
    const companyDocuments: {
      id: number
      documentId: number
      expirationDate?: string
      status?: string
      fileUrl?: string | null
    }[] = []

    documents.value.forEach((doc, index) => {
      filesData[`document_${index}`] = doc.file

      if (doc.file || doc.fileUrl) {
        const docData = buildDocumentData(doc, index)
        if (docData) {
          companyDocuments.push(docData.companyDocument)
          if (docData.legacyDocument) {
            documentsData.push(docData.legacyDocument)
          }
        }
      }
    })

    emit('update', { ...filesData, _documents: documentsData, documents: companyDocuments })
  }

  function updateDocument(index: number, doc: Partial<ApiDocumentInfo>): void {
    if (!documents.value[index]) return
    documents.value[index] = { ...documents.value[index], ...doc }
    updateFormData()
  }

  function updateDocumentExpirationDate(index: number, date: string): void {
    if (!documents.value[index]) return
    documents.value[index].expirationDate = date
    updateFormData()
  }

  function updateDocumentStatus(index: number, status: string): void {
    if (!documents.value[index]) return
    documents.value[index].status = status
    updateFormData()
  }

  function findDocumentByTemplateId(templateId: number): ApiDocumentInfo | null {
    const index = documentTemplateIds.value.findIndex(id => id === templateId)

    return index >= 0 ? documents.value[index] || null : null
  }

  function updateDocumentMetadata(
    doc: ApiDocumentInfo,
    metadata: { expirationDate?: string; status?: string; fileUrl?: string | null; id?: number; documentId?: number }
  ): void {
    if (metadata.expirationDate) doc.expirationDate = metadata.expirationDate
    if (metadata.status) doc.status = metadata.status
    if (metadata.fileUrl) doc.fileUrl = metadata.fileUrl
    if (metadata.id && metadata.documentId && metadata.id !== metadata.documentId) {
      doc.id = metadata.id
    }
  }

  function getSavedFiles(formValues?: Record<string, unknown>): void {
    if (!formValues) return

    documents.value.forEach((doc, index) => {
      const file = formValues[`document_${index}`] as File | undefined
      if (file instanceof File) doc.file = file
    })

    const documentsData = formValues._documents as { id: number; expirationDate?: string; status?: string }[] | undefined
    documentsData?.forEach((docData) => {
      const doc = findDocumentByTemplateId(docData.id)
      if (doc) updateDocumentMetadata(doc, docData)
    })

    const savedDocuments = formValues.documents as {
      id: number
      documentId: number
      expirationDate?: string
      status?: string
      fileUrl?: string | null
    }[] | undefined

    savedDocuments?.forEach((savedDoc) => {
      const doc = findDocumentByTemplateId(savedDoc.documentId)
      if (doc) updateDocumentMetadata(doc, savedDoc)
    })
  }

  function validateRequiredDocuments(): { isValid: boolean; missingDocuments: string[] } {
    const missingDocuments: string[] = []

    documents.value.forEach((doc) => {
      const hasFile = !!doc.file
      const hasFileUrl = !!doc.fileUrl
      const hasDocument = hasFile || hasFileUrl

      if (doc.required && !hasDocument) {
        missingDocuments.push(doc.label)
      }
    })

    return {
      isValid: missingDocuments.length === 0,
      missingDocuments,
    }
  }

  return {
    documents,
    fileInputs,
    fetchDocuments,
    triggerFileInput,
    handleFileChange,
    handleDrop,
    removeFile,
    validateRequiredDocuments,
    updateDocument,
    updateDocumentExpirationDate,
    updateDocumentStatus,
    getSavedFiles,
  }
}

