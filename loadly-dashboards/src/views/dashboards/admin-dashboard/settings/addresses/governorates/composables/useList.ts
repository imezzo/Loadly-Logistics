import type { Governorate } from '@/types/admin-dashboard/response/governorate'
import type { Pagination } from '@/types/response/pagination'
import { useGovernorateStore } from '@/stores/admin-dashboard/governorate'
import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'

export function useList() {
  const governorateStore = useGovernorateStore()
  const { governoratesList } = storeToRefs(governorateStore)

  const { isLoading, setLoading } = useLoading()

  const govPagination = computed<Pagination | null>(() => governoratesList.value?.pagination || null)

  const {
    currentPage,
    itemsPerPage,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(govPagination)

  async function fetchGovernorates() {
    setLoading(true)

    const { success, data } = await governorateStore.getList({
      pagination: {
        page: currentPage.value,
        size: itemsPerPage.value,
      },
    })

    setLoading(false)
    governoratesList.value = success ? data : null
  }

  watch(itemsPerPage, () => {
    currentPage.value = 1
    fetchGovernorates()
  })

  watch(currentPage, () => {
    fetchGovernorates()
  })

  const governorates = computed<Governorate[]>(() => governoratesList.value?.governorates || [])

  return {
    governorates,
    fetchGovernorates,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  }
}

