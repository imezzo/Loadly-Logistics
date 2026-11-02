import { extractISODate } from "@/utils/date"
import type { ApiPagination, Pagination } from "../request/pagination"

export interface AdminNotificationItem {
  body: string
  clickAction: string
  createdAt: string
  eventType: number
  id: number
  isRead: boolean
  readAt: string | null
  resourceId: number
  resourceType: string
  title: string
  updatedAt: string
}

export interface ApiNotificationsResponse {
  data: AdminNotificationItem[]
  pagination: ApiPagination
}

export interface Notification {
  body: string
  clickAction: string
  date: string
  id: string
  isRead: boolean
  resourceId: number
  title: string
  type: string
}

export interface NotificationsResponse {
  data: Notification[]
  pagination: Pagination
}

export function notificationItemAdapter(item: AdminNotificationItem): Notification {
  return {
    body: item.body,
    clickAction: item.clickAction,
    date: extractISODate(item.createdAt) ?? '',
    id: String(item.id),
    isRead: item.isRead,
    resourceId: item.resourceId,
    title: item.title,
    type: item.resourceType,
  }
}

export function notificationsResponseAdapter(response: ApiNotificationsResponse): NotificationsResponse {
  return {
    data: response.data.map(notificationItemAdapter),
    pagination: response.pagination,
  }
}
