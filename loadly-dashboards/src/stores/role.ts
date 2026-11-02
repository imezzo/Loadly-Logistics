import type { RolesListResponse } from "@/types/response/role"
import { roleResponseAdapter, rolesListResponseAdapter } from "@/types/response/role"
import { api } from "@/utils/api"

export const useRoleStore = defineStore('role', () => {
  const rolesList = ref<RolesListResponse | null>(null)
  const search = ref<string>('')

  async function getList(params?: { page?: number; pageSize?: number; search?: string }) {
    return api({
      url: '/roles',
      method: 'GET',
      params: {
        page: params?.page || 1,
        pageSize: params?.pageSize || 10,
        search: params?.search || '',
      },
      responseAdapter: rolesListResponseAdapter,
    })
  }

  async function create(data: {
    description: string
    guardName: string
    nameAr: string
    nameEn: string
  }) {
    return api({
      url: '/roles',
      method: 'POST',
      data,
    })
  }

  async function update(id: number, data: {
    description: string
    guardName: string
    nameAr: string
    nameEn: string
  }) {
    return api({
      url: `/roles/${id}`,
      method: 'PUT',
      data,
    })
  }

  async function deleteRole(id: number) {
    return api({
      url: `/roles/${id}`,
      method: 'DELETE',
    })
  }

  async function getById(id: number) {
    return api({
      url: `/roles/${id}`,
      method: 'GET',
      responseAdapter: roleResponseAdapter,
    })
  }

  return {
    create,
    deleteRole,
    getById,
    getList,
    rolesList,
    search,
    update,
  }
})

