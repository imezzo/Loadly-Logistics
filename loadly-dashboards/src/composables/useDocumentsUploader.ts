import { useDocumentStore } from '@/stores/admin-dashboard/document'
import { useEnum } from './useEnum'
import type { Document } from '@/types/admin-dashboard/response/document'
import type { SelectOption } from '@/types/ui'
import type { DocumentUploaderItem } from '@/types/documents.types'

export interface DocumentsEntity {
  documents: DocumentUploaderItem[]
}

function defaultMapDocument(doc: Document): DocumentUploaderItem {
  return {
    documentId: doc.id,
    required: doc.required,
    label: doc.nameEn,
  }
}

export function useDocumentsUploader<T extends DocumentsEntity>(entityRef: Ref<T | null>) {
  const { enums } = useEnum()

  const documentStore = useDocumentStore()
  const { documentsList } = storeToRefs(documentStore)

  const validationError = ref<string | null>(null)

  const documentStatusOptions = computed<SelectOption[]>(() => enums.value?.DocumentVerificationStatus ?? [])

  async function fetchDocuments(entityId: number): Promise<void> {
    const { data } = await documentStore.getList({
      entityId,
      pagination: { page: 1, size: 100 },
    })

    documentsList.value = data
  }

  function onDocumentUpdate(index: number, document: DocumentUploaderItem): void {
    if (!entityRef.value || !document) return

    entityRef.value.documents[index] = {
      ...entityRef.value.documents[index],
      file: document.file,
      fileUrl: undefined,
      status: document.status,
      expirationDate: document.expirationDate,
      id: document.file
        ? document.id || entityRef.value.documents[index]?.id
        : entityRef.value.documents[index]?.id,
    }
  }

  function initializeDocuments(): void {
    const missingDocuments = !entityRef.value
      ? documentsList.value?.documents
      : documentsList.value?.documents
        .filter((document) => !entityRef.value?.documents.find((d) => d.documentId === document.id))

    entityRef.value = {
      ...entityRef.value,
      documents: [
        ...(entityRef.value?.documents ?? []),
        ...(missingDocuments?.map(defaultMapDocument) ?? []),
      ],
    } as T
  }

  return {
    fetchDocuments,
    documentStatusOptions,
    validationError,
    onDocumentUpdate,
    initializeDocuments,
  }
}
