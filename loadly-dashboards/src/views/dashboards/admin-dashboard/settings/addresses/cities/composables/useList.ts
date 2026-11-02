import type { City } from '@/types/admin-dashboard/response/city'
import type { Pagination } from '@/types/response/pagination'
import { useCityStore } from '@/stores/admin-dashboard/city'
import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'

export function useList() {
  const cityStore = useCityStore()
  const { citiesList } = storeToRefs(cityStore)

  const { isLoading, setLoading } = useLoading()

  const cityPagination = computed<Pagination | null>(() => citiesList.value?.pagination || null)

  const {
    currentPage,
    itemsPerPage,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(cityPagination)

  async function fetchCities() {
    setLoading(true)

    const { success, data } = await cityStore.getList({
      pagination: {
        page: currentPage.value,
        size: itemsPerPage.value,
      },
    })

    setLoading(false)
    citiesList.value = success ? data : null
  }

  watch(itemsPerPage, () => {
    currentPage.value = 1
    fetchCities()
  })

  watch(currentPage, () => {
    fetchCities()
  })

  const cities = computed<City[]>(() => citiesList.value?.cities || [])

  return {
    cities,
    fetchCities,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  }
}

