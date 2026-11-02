import type { District } from '@/types/admin-dashboard/response/district'
import type { Pagination } from '@/types/response/pagination'
import { useDistrictStore } from '@/stores/admin-dashboard/district'
import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'

export function useList() {
  const districtStore = useDistrictStore()
  const { districtsList } = storeToRefs(districtStore)

  const { isLoading, setLoading } = useLoading()

  const districtPagination = computed<Pagination | null>(() => districtsList.value?.pagination || null)

  const {
    currentPage,
    itemsPerPage,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(districtPagination)

  async function fetchDistricts() {
    setLoading(true)

    const { success, data } = await districtStore.getList({
      pagination: {
        page: currentPage.value,
        size: itemsPerPage.value,
      },
    })

    setLoading(false)
    districtsList.value = success ? data : null
  }

  watch(itemsPerPage, () => {
    currentPage.value = 1
    fetchDistricts()
  })

  watch(currentPage, () => {
    fetchDistricts()
  })

  const districts = computed<District[]>(() => districtsList.value?.districts || [])

  return {
    districts,
    fetchDistricts,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  }
}

