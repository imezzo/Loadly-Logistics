import type { BroadcastNotification } from '@/types/admin-dashboard/response/broadcast-notification'
import type { Pagination } from '@/types/response/pagination'
import { useBroadcastNotificationStore } from '@/stores/admin-dashboard/broadcast-notification'
import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'

export function useList() {
  const broadcastNotificationStore = useBroadcastNotificationStore()
  const { broadcastNotificationsList, filters } = storeToRefs(broadcastNotificationStore)

  const { isLoading, setLoading } = useLoading()

  const broadcastNotificationsPagination = computed<Pagination | null>(
    () => broadcastNotificationsList.value?.pagination || null,
  )

  const {
    currentPage,
    itemsPerPage,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(broadcastNotificationsPagination)

  async function fetchNotifications() {
    setLoading(true)

    const { success, data } = await broadcastNotificationStore.getList({
      pagination: {
        page: currentPage.value,
        size: itemsPerPage.value,
      },
      search: filters.value.search,
    })

    setLoading(false)
    broadcastNotificationsList.value = success ? data : null
  }

  watch(itemsPerPage, () => {
    currentPage.value = 1
    fetchNotifications()
  })

  watch(currentPage, () => {
    fetchNotifications()
  })

  const notifications = computed<BroadcastNotification[]>(
    () => broadcastNotificationsList.value?.notifications || [],
  )

  return {
    fetchNotifications,
    filters,
    isLoading,
    nextPage,
    notifications,
    pagination,
    prevPage,
    resetFilters: broadcastNotificationStore.resetFilters,
    setPage,
    updateFilters: broadcastNotificationStore.updateFilters,
    updateItemsPerPage,
  }
}
