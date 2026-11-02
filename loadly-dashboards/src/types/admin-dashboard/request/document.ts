import { apiPaginationAdapter } from '@/types/request/pagination'
import type { ApiPagination, Pagination } from '@/types/request/pagination'

export interface DocumentsListRequest {
  entityId?: number
  modelType?: string
  pagination: Pagination
  required?: boolean
  search?: string
  status?: number
}

export interface DocumentCreateRequest {
  entityId: number
  modelType: string
  nameAr: string
  nameEn: string
  required: boolean
  status: number
}

export interface DocumentUpdateRequest {
  nameAr: string
  nameEn: string
  required: boolean
  status: number
}

export interface ApiDocumentsListRequest extends ApiPagination {
  entityId?: number
  modelId?: number
  modelType?: string
  required?: boolean
  search?: string
  status?: number
}

export function documentsListRequestAdapter(data: DocumentsListRequest): ApiDocumentsListRequest {
  return {
    ...apiPaginationAdapter(data.pagination),
    ...(data.search ? { search: data.search } : {}),
    ...(data.modelType ? { modelType: data.modelType } : {}),
    ...(data.entityId !== undefined ? { entityId: data.entityId } : {}),
    ...(data.required !== undefined ? { required: data.required } : {}),
    ...(data.status !== undefined ? { status: data.status } : {}),
  }
}
