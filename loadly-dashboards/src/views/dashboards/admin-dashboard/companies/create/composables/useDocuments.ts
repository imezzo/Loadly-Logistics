import { useLoading } from '@/composables/useLoading'
import { useDocumentStore } from '@/stores/admin-dashboard/document'

export interface Document {
  label: string
  required: boolean
  file?: File
  id?: number
  expirationDate?: string
  status?: string
}

export function useDocuments(
  emit: (event: 'update', data: Record<string, unknown>) => void
) {
  const documentStore = useDocumentStore()
  const { setLoading } = useLoading()
  const documents = ref<Document[]>([])
  const fileInputs = ref<Record<number, HTMLInputElement>>({})

  async function fetchDocuments() {
    setLoading(true)
    const { data } = await documentStore.getList({
      pagination: { page: 1, size: 10 },
      entityId: 2,
    })

    setLoading(false)
    documents.value = data.documents
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

  function updateFormData(): void {
    const filesData: Record<string, File | undefined> = {}
    const documentsData: { id: number; expirationDate?: string; status?: string }[] = []
    documents.value.forEach((doc, index) => {
      filesData[`document_${index}`] = doc.file
      if (doc.id) {
        documentsData.push({
          id: doc.id,
          expirationDate: doc.expirationDate,
          status: doc.status,
        })
      }
    })
    emit('update', { ...filesData, _documents: documentsData })
  }

  function updateDocument(index: number, doc: Partial<Document>): void {
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

  function getSavedFiles(formValues?: Record<string, unknown>): void {
    if (!formValues) return

    documents.value.forEach((doc, index) => {
      const file = formValues[`document_${index}`] as File | undefined
      if (file instanceof File) doc.file = file
    })

    const documentsData = formValues._documents as { id: number; expirationDate?: string; status?: string }[]
    documentsData?.forEach((docData) => {
      const doc = documents.value.find(d => d.id === docData.id)
      if (!doc) return
      if (docData.expirationDate) doc.expirationDate = docData.expirationDate
      if (docData.status) doc.status = docData.status
    })
  }

  function validateRequiredDocuments(): { isValid: boolean; missingDocuments: string[] } {
    const missingDocuments: string[] = []

    documents.value.forEach((doc) => {
      if (doc.required && !doc.file) {
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

