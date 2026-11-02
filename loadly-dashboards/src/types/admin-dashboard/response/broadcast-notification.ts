import type { LookupItem } from '@/types/response/lookup'
import type { Pagination } from '@/types/response/pagination'
import { formatDateLocal } from '@/utils/date'

export interface BroadcastNotification {
  attachments: BroadcastNotificationAttachment[]
  body: string
  channel: string
  createdAt: string
  id: number
  modelIds: number[] | null
  models: LookupItem[]
  modelType: string
  subject: string
}

export interface ApiBroadcastNotificationModel {
  id: number
  name: string
}

export interface BroadcastNotificationsListResponse {
  pagination?: Pagination
  notifications: BroadcastNotification[]
}

export interface ApiBroadcastNotification {
  addedBy: number
  attachments: BroadcastNotificationAttachment[] | null
  body: string
  channel: string
  createdAt: string
  id: number
  modelIds: number[] | null
  models: ApiBroadcastNotificationModel[] | null
  modelType: string
  status: string
  subject: string
  updatedAt: string
}

export interface ApiBroadcastNotificationsListResponse {
  data: ApiBroadcastNotification[]
  pagination: Pagination
}

export function broadcastNotificationAdapter(data: ApiBroadcastNotification): BroadcastNotification {
  return {
    attachments: data.attachments ?? [],
    body: data.body,
    channel: data.channel,
    createdAt: formatDateLocal(data.createdAt),
    id: data.id,
    modelIds: data.modelIds,
    models: data.models?.map(model => ({ label: model.name, value: String(model.id) })) ?? [],
    modelType: data.modelType,
    subject: data.subject,
  }
}

export function broadcastNotificationsListResponseAdapter(
  response: ApiBroadcastNotificationsListResponse,
): BroadcastNotificationsListResponse {
  return {
    notifications: response.data.map(broadcastNotificationAdapter),
    pagination: response.pagination,
  }
}

export interface BroadcastNotificationAttachment {
  id: number
  url: string
}

export interface PublicBroadcastNotification {
  attachments: BroadcastNotificationAttachment[]
  body: string
  sentAt: string
  subject: string
}

export interface ApiPublicBroadcastNotification {
  attachments: BroadcastNotificationAttachment[] | null
  body: string
  sentAt: string
  subject: string
}

export function publicBroadcastNotificationAdapter(
  data: ApiPublicBroadcastNotification,
): PublicBroadcastNotification {
  return {
    attachments: data.attachments ?? [],
    body: data.body,
    sentAt: data.sentAt,
    subject: data.subject,
  }
}
