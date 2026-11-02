import type { Pagination } from '@/types/response/pagination'
import { getLocalizedValue } from '@/utils/i18n'

export interface Document {
  addedBy?: number | null
  createdAt?: string
  entityId?: number
  id: number
  label: string
  nameAr?: string
  nameEn?: string
  required: boolean
  status?: number
  updatedAt?: string
}

export interface DocumentInfo {
  id: number
  documentId?: number
  expirationDate?: string
  status?: string
  fileUrl?: string | null
  path?: string | null
}

export interface ApiDocumentInfo {
  label: string
  required: boolean
  file?: File
  id?: number
  expirationDate?: string
  status?: string
  fileUrl?: string | null
  documentId?: number
}

export interface DocumentsListResponse {
  documents: Document[]
  pagination?: Pagination
}

export interface ApiDocument {
  id: number
  entityId: number
  nameEn: string
  nameAr: string
  required: boolean
  status: number
  addedBy: number | null
  createdAt: string
  updatedAt: string
}

export interface ApiDocumentsListResponse {
  data: ApiDocument[]
  pagination: Pagination
}

export function documentAdapter(document: ApiDocument): Document {
  return {
    id: document.id,
    label: getLocalizedValue(document.nameEn, document.nameAr),
    required: document.required,
    nameEn: document.nameEn ?? '',
    nameAr: document.nameAr ?? '',
    entityId: document.entityId ?? 0,
    status: document.status ?? 1,
    addedBy: document.addedBy ?? null,
    createdAt: document.createdAt ?? '',
    updatedAt: document.updatedAt ?? '',
  }
}

export function documentsListResponseAdapter(response: ApiDocumentsListResponse): DocumentsListResponse {
  return {
    documents: response.data.map(document => documentAdapter(document)),
    pagination: response.pagination,
  }
}
