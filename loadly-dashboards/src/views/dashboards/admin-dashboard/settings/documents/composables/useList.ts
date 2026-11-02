import type { Document } from '@/types/admin-dashboard/response/document'
import type { Pagination } from '@/types/response/pagination'
import { useDocumentStore } from '@/stores/admin-dashboard/document'
import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'

export function useList() {
  const documentStore = useDocumentStore()
  const { documentsList } = storeToRefs(documentStore)

  const { isLoading, setLoading } = useLoading()

  const documentPagination = computed<Pagination | null>(() => documentsList.value?.pagination || null)

  const {
    currentPage,
    itemsPerPage,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(documentPagination)

  async function fetchDocuments() {
    setLoading(true)

    const { success, data } = await documentStore.getList({
      pagination: {
        page: currentPage.value,
        size: itemsPerPage.value,
      },
    })

    setLoading(false)
    documentsList.value = success ? data : null
  }

  watch(itemsPerPage, () => {
    currentPage.value = 1
    fetchDocuments()
  })

  watch(currentPage, () => {
    fetchDocuments()
  })

  const documents = computed<Document[]>(() => documentsList.value?.documents || [])

  return {
    documents,
    fetchDocuments,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  }
}
