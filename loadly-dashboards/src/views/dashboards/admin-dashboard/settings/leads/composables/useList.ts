import type { Lead } from '@/types/admin-dashboard/response/lead/lead'
import type { Pagination } from '@/types/response/pagination'
import { useLeadSettingsStore } from '@/stores/admin-dashboard/lead-settings'
import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'

export function useList() {
  const leadSettingsStore = useLeadSettingsStore()
  const { leadsList } = storeToRefs(leadSettingsStore)

  const leadsPagination = computed<Pagination | null>(() => leadsList.value?.pagination || null)
  const leads = computed<Lead[]>(() => leadsList.value?.leads || [])

  const {
    currentPage,
    itemsPerPage,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(leadsPagination)
  const { isLoading, setLoading } = useLoading()

  async function fetchLeads(): Promise<void> {
    setLoading(true)
    const { success, data } = await leadSettingsStore.getList({
      page: currentPage.value,
      size: itemsPerPage.value,
    })
    setLoading(false)

    leadsList.value = success ? data : null
  }

  watch(itemsPerPage, () => {
    currentPage.value = 1
    fetchLeads()
  })

  watch(currentPage, fetchLeads)

  return {
    fetchLeads,
    isLoading,
    leads,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  }
}
