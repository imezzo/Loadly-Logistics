import { useNotificationStore } from "@/stores/notification"

export function useNotifications(params?: {
  page?: number
  exclude?: number
  eventType?: number
}) {
  const route = useRoute()

  const notificationStore = useNotificationStore()
  const { notifications } = storeToRefs(notificationStore)

  const unreadCount = ref(0)
  const showNotificationMenu = ref(false)
  const isNotificationLoading = ref(false)

  const hasUnreadNotifications = computed(() => unreadCount.value > 0)

  async function loadUnreadCount(): Promise<void> {
    const { data } = await notificationStore.fetchUnreadCount()
    unreadCount.value = data.count
  }

  async function markAllAsRead(): Promise<void> {
    isNotificationLoading.value = true
    await notificationStore.markAllAsRead()
    loadUnreadCount()
    isNotificationLoading.value = false
  }

  async function loadNotifications(page?: number): Promise<void> {
    const { data } = await notificationStore.fetchNotifications({ ...params, page: page ? page : undefined})
    notifications.value = data
  }

  async function onScrollToEnd(): Promise<void> {
    const { page = 1, size = 1 } = notifications.value?.pagination ?? {}

    if (page >= size || isNotificationLoading.value) return
    isNotificationLoading.value = true

    const { data } = await notificationStore.fetchNotifications({ ...params, page: page + 1 })
    notifications.value = {
      ...notifications.value,
      data: [...notifications.value?.data ?? [], ...data.data],
      pagination: data.pagination,
    }

    isNotificationLoading.value = false
  }

  onMounted(loadUnreadCount)

  watch(route, loadUnreadCount)

  watch(showNotificationMenu, async (open): Promise<void> => {
    if (open) {
      await loadNotifications()
    }
  })

  return {
    hasUnreadNotifications,
    isNotificationLoading,
    markAllAsRead,
    notifications,
    onScrollToEnd,
    showNotificationMenu,
    unreadCount,
  }
}
