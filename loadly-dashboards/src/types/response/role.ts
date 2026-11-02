import { getLocalizedValue } from '@/utils/i18n'
import type { Pagination } from './pagination'

export interface Permission {
  id: number
  name: string
  nameAr: string | null
  guardName: string
}

export interface Role {
  id: number
  name: string
  nameAr: string
  nameEn: string
  guardName: string
  description: string | null
  createdAt: string
  updatedAt: string
  permissions: Permission[]
}

export interface RolesListResponse {
  roles: Role[]
  pagination: Pagination
}

export interface ApiPermission {
  id: number
  name: string
  nameAr: string | null
  guardName: string
}

export interface ApiRole {
  id: number
  name: string
  nameAr: string | null
  nameEn?: string
  description: string | null
  guardName: string
  createdAt: string
  updatedAt: string
  permissions: ApiPermission[]
}

export interface ApiRolesListResponse {
  data: ApiRole[]
  pagination: Pagination
}

export function roleAdapter(role: ApiRole): Role {
  const nameEn = role.nameEn ?? role.name
  const nameAr = role.nameAr ?? ''

  return {
    id: role.id,
    name: getLocalizedValue(nameEn, nameAr),
    nameAr,
    nameEn,
    guardName: role.guardName,
    description: role.description,
    createdAt: role.createdAt,
    updatedAt: role.updatedAt,
    permissions: role.permissions.map(permission => ({
      id: permission.id,
      name: permission.name,
      nameAr: permission.nameAr,
      guardName: permission.guardName,
    })),
  }
}

export function rolesListResponseAdapter(response: ApiRolesListResponse): RolesListResponse {
  return {
    roles: response.data.map(roleAdapter),
    pagination: response.pagination,
  }
}

export function roleResponseAdapter(response: ApiRole): Role {
  return roleAdapter(response)
}
