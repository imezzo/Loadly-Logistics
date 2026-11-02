import type {
  BroadcastNotificationSendRequest,
  BroadcastNotificationsListRequest,
} from '@/types/admin-dashboard/request/broadcast-notifications'
import {
  broadcastNotificationSendRequestAdapter,
  broadcastNotificationsListRequestAdapter,
} from '@/types/admin-dashboard/request/broadcast-notifications'
import {
  broadcastNotificationAdapter,
  broadcastNotificationsListResponseAdapter,
  publicBroadcastNotificationAdapter,
} from '@/types/admin-dashboard/response/broadcast-notification'
import type {
  BroadcastNotification,
  BroadcastNotificationsListResponse,
} from '@/types/admin-dashboard/response/broadcast-notification'
import { api } from '@/utils/api'

export const useBroadcastNotificationStore = defineStore('broadcastNotification', () => {
  const broadcastNotificationsList = ref<BroadcastNotificationsListResponse | null>(null)
  const broadcastNotification = ref<BroadcastNotification | null>(null)
  const filters = ref({
    search: '',
  })

  async function getList(data: BroadcastNotificationsListRequest) {
    return api({
      url: '/broadcast-notifications',
      method: 'GET',
      params: broadcastNotificationsListRequestAdapter(data),
      responseAdapter: broadcastNotificationsListResponseAdapter,
    })
  }

  async function get(id: string | number) {
    return api({
      url: `/broadcast-notifications/${id}`,
      method: 'GET',
      responseAdapter: broadcastNotificationAdapter,
    })
  }

  async function getPublic(code: string) {
    return api({
      url: `${import.meta.env.VITE_API_BASE_URL}/public/broadcast-notifications/${code}`,
      method: 'GET',
      responseAdapter: publicBroadcastNotificationAdapter,
    })
  }

  async function send(data: BroadcastNotificationSendRequest) {
    return api({
      url: '/broadcast-notifications/send',
      method: 'POST',
      data: broadcastNotificationSendRequestAdapter(data),
    })
  }

  function updateFilters(newFilters: { search: string }) {
    filters.value = { ...filters.value, ...newFilters }
  }

  function resetFilters() {
    updateFilters({ search: '' })
  }

  return {
    broadcastNotification,
    broadcastNotificationsList,
    filters,
    get,
    getList,
    getPublic,
    resetFilters,
    send,
    updateFilters,
  }
})
