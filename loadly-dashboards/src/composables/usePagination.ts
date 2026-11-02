import type { Pagination } from '@/types/response/pagination'

export function usePagination(pagination: Ref<Pagination | null> | ComputedRef<Pagination | null>) {
  const currentPage = ref(1)
  const itemsPerPage = ref(20)

  const paginationEntries = computed<Pagination>(() => ({
    size: itemsPerPage.value,
    page: pagination.value?.page || 0,
    pages: pagination.value?.pages || 0,
    items: pagination.value?.items || 0,
  }))

  function setPage(page: number) {
    if (page >= 1 && page <= paginationEntries.value.pages) {
      currentPage.value = page
    }
  }

  function nextPage() {
    if (currentPage.value < paginationEntries.value.pages) {
      currentPage.value++
    }
  }

  function prevPage() {
    if (currentPage.value > 1) {
      currentPage.value--
    }
  }

  function updateItemsPerPage(value: number) {
    itemsPerPage.value = value
    currentPage.value = 1
  }

  return {
    currentPage,
    itemsPerPage,
    pagination: paginationEntries,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  }
}
