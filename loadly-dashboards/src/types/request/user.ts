import type { ApiPagination, Pagination } from './pagination'
import { apiPaginationAdapter } from './pagination'
import { ApiUserType, UserRole } from "../user"

export interface UsersListRequest {
  pagination: Pagination
  roleIds?: string[]
  search: string
  status?: string
  type?: UserRole
}

export interface UserCreateRequest {
  email: string
  fullName: string
  mfaChannel?: string
  phone: string
  roleId?: number
  roleIds?: number[]
  status: number
  type: UserRole
  userName: string
}

export interface UserUpdateRequest {
  email: string
  fullName: string
  id?: string
  mfaChannel?: string
  phone: string
  roleId?: number
  roleIds?: number[]
  status: number
  type: UserRole
  userName: string
}


export interface ApiUsersListRequest extends ApiPagination {
  search: string
  status?: string
  type?: ApiUserType
}

export interface ApiUserCreateRequest {
  email: string
  fullName: string
  mfaChannel?: number
  phone: string
  roleId?: number
  roleIds?: number[]
  status: number
  type: ApiUserType
  userName: string
}


export function userTypeAdapter(type: UserRole): ApiUserType {
  switch (type) {
    case UserRole.ADMIN:
      return ApiUserType.ADMIN
    case UserRole.SUPERADMIN:
      return ApiUserType.SUPERADMIN
  }
}

export function usersListRequestAdapter(data: UsersListRequest): ApiUsersListRequest {
  const roleIds: Record<string, string> = {}
  data.roleIds?.forEach((id, index) => {
    roleIds[`roleIds[${index}]`] = id
  })

  return {
    search: data.search,
    ...(data.type ? { type: userTypeAdapter(data.type) } : {}),
    ...(data.status ? { status: data.status } : {}),
    ...(data.roleIds?.length ? roleIds : {}),
    ...apiPaginationAdapter(data.pagination),
  }
}

export function userCreateRequestAdapter(data: UserCreateRequest): ApiUserCreateRequest {
  return {
    email: data.email,
    fullName: data.fullName,
    mfaChannel: Number(data.mfaChannel),
    ...(data.roleId && { roleId: Number(data.roleId) }),
    ...(data.roleIds?.length && { roleIds: data.roleIds }),
    phone: data.phone,
    status: data.status,
    type: userTypeAdapter(data.type),
    userName: data.userName,
  }
}
