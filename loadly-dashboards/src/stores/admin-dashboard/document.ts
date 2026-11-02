import type {
  DocumentCreateRequest,
  DocumentsListRequest,
  DocumentUpdateRequest,
} from '@/types/admin-dashboard/request/document'
import { documentsListRequestAdapter } from '@/types/admin-dashboard/request/document'
import {
  documentAdapter,
  documentsListResponseAdapter,
} from '@/types/admin-dashboard/response/document'
import type {
  ApiDocument,
  DocumentsListResponse,
} from '@/types/admin-dashboard/response/document'
import { api } from '@/utils/api'

export const useDocumentStore = defineStore('document', () => {
  const documentsList = ref<DocumentsListResponse | null>(null)

  async function getList(data: DocumentsListRequest) {
    return api({
      url: '/documents',
      method: 'GET',
      params: documentsListRequestAdapter(data),
      responseAdapter: documentsListResponseAdapter,
    })
  }

  async function getById(id: number) {
    return api({
      url: `/documents/${id}`,
      method: 'GET',
      responseAdapter: (data: ApiDocument) => documentAdapter(data),
    })
  }

  async function create(data: DocumentCreateRequest) {
    return api({
      url: '/documents',
      method: 'POST',
      data,
    })
  }

  async function update(id: number, data: DocumentUpdateRequest) {
    return api({
      url: `/documents/${id}`,
      method: 'PUT',
      data,
    })
  }

  async function deleteDocument(id: number) {
    return api({
      url: `/documents/${id}`,
      method: 'DELETE',
    })
  }

  return {
    documentsList,
    getList,
    getById,
    create,
    update,
    deleteDocument,
  }
})
