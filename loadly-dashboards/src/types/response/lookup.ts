import type { Pagination } from '@/types/response/pagination'

export interface LookupItem {
  label: string
  value: string
}

export interface LookupResponse {
  items: LookupItem[]
  pagination: Pagination
}

export interface ApiLookupItem {
  id: number
  value: string
}

export interface ApiLookupResponse {
  data?: ApiLookupItem[]
  pagination: Pagination
}

export function lookupItemResponseAdapter(response: ApiLookupItem[]): LookupItem[] {
  return response.map((item) => ({
    label: item.value,
    value: item.id.toString(),
  }))
}

export function lookupResponseAdapter(response: ApiLookupResponse): LookupResponse {
  return {
    items: lookupItemResponseAdapter(response.data ?? []),
    pagination: response.pagination,
  }
}
