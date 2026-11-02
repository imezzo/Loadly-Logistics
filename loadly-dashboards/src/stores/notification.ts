import { notificationsResponseAdapter } from '@/types/response/notification'
import type { NotificationsResponse } from '@/types/response/notification'
import { api } from '@/utils/api'

export const useNotificationStore = defineStore('notification', () => {
  const unreadCount = ref(0)
  const notifications = ref<NotificationsResponse | null>(null)

  async function fetchUnreadCount() {
    const response = await api({
      url: '/notifications/unread-count',
      method: 'GET',
    })

    if (response.success && response.data) {
      unreadCount.value = response.data.count
    }

    return response
  }

  async function fetchNotifications(params?: {
    page?: number
    pageSize?: number
    exclude?: number
    eventType?: number
  }) {
    return await api({
      url: '/notifications',
      method: 'GET',
      params,
      responseAdapter: notificationsResponseAdapter,
    })
  }

  async function markAllAsRead() {
    return await api({
      url: '/notifications/read-all',
      method: 'PATCH',
    })
  }

  return {
    fetchNotifications,
    fetchUnreadCount,
    markAllAsRead,
    notifications,
    unreadCount,
  }
})
