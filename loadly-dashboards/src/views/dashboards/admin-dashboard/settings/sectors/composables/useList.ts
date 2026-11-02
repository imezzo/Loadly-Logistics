import type { Sector } from '@/types/admin-dashboard/response/sector/sector'
import type { Pagination } from '@/types/response/pagination'
import { useSectorSettingsStore } from '@/stores/admin-dashboard/sector-settings'
import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'

export function useList() {
  const sectorSettingsStore = useSectorSettingsStore()
  const { sectorsList } = storeToRefs(sectorSettingsStore)

  const sectorPagination = computed<Pagination | null>(() => sectorsList.value?.pagination || null)
  const sectors = computed<Sector[]>(() => sectorsList.value?.sectors || [])

  const {
    currentPage,
    itemsPerPage,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(sectorPagination)
  const { isLoading, setLoading } = useLoading()

  async function fetchSectors(): Promise<void> {
    setLoading(true)
    const { success, data } = await sectorSettingsStore.getList({
      page: currentPage.value,
      size: itemsPerPage.value,
    })
    setLoading(false)

    sectorsList.value = success ? data : null
  }

  watch(itemsPerPage, () => {
    currentPage.value = 1
    fetchSectors()
  })

  watch(currentPage, fetchSectors)

  return {
    fetchSectors,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    sectors,
    setPage,
    updateItemsPerPage,
  }
}
