import type { Pagination } from '@/types/response/pagination'

export interface Lead {
  email: string
  fullName: string
  id: number
  phone: string
  totalCompletedTrips: number
}

export interface LeadsListResponse {
  leads: Lead[]
  pagination: Pagination
}

export interface ApiLead {
  addedBy: number
  createdAt: string
  email: string
  fullName: string
  id: number
  phone: string
  totalCompletedTrips: number
  updatedAt: string
}

export interface ApiLeadsListResponse {
  data: ApiLead[]
  pagination: Pagination
}

export function leadAdapter(data: ApiLead): Lead {
  return {
    email: data.email,
    fullName: data.fullName,
    id: data.id,
    phone: data.phone,
    totalCompletedTrips: data.totalCompletedTrips,
  }
}

export function leadsListResponseAdapter(
  response: ApiLeadsListResponse
): LeadsListResponse {
  return {
    leads: response.data.map(leadAdapter),
    pagination: response.pagination,
  }
}
