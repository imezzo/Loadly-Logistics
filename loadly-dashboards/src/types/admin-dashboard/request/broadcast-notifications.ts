import type { ApiPagination, Pagination } from '@/types/request/pagination'
import { apiPaginationAdapter } from '@/types/request/pagination'
export interface BroadcastNotificationSendRequest {
  attachments?: File | null
  body: string
  channel: number
  modelIds: string[]
  modelType: string
  subject: string
}

export function broadcastNotificationSendRequestAdapter(data: BroadcastNotificationSendRequest): FormData {
  const formData = new FormData()

  formData.append('body', data.body)
  formData.append('channel', String(data.channel))
  formData.append('modelType', data.modelType)
  formData.append('subject', data.subject)
  formData.append('sendToAllUsers', data.modelIds.length ? 'false' : 'true')
  data.modelIds?.forEach(id => formData.append('modelIds', id))

  if (data.attachments) {
    formData.append('attachments', data.attachments)
  }

  return formData
}

export interface BroadcastNotificationsListRequest {
  pagination: Pagination
  search?: string
}

export interface ApiBroadcastNotificationsListRequest extends ApiPagination {
  search?: string
}

export function broadcastNotificationsListRequestAdapter(
  data: BroadcastNotificationsListRequest,
): ApiBroadcastNotificationsListRequest {
  return {
    ...apiPaginationAdapter(data.pagination),
    ...(data.search ? { search: data.search } : {}),
  }
}
