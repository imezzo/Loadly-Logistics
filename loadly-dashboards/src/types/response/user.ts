import type { Pagination } from './pagination'
import type { MfaChannel } from '../request/mfa'
import { ApiUserType, ApiUserStatus } from '../user'
import type { Role } from './role'

export interface UserRole {
  id: string
  name: string
}

export interface User {
  id: string
  fullName: string
  userName: string
  email: string
  phone: string
  mfaChannel: MfaChannel
  emailVerified: boolean
  phoneVerified: boolean
  status: number
  type: ApiUserType
  roles: UserRole[]
  lastLogin: string | null
}

export interface UsersListResponse {
  users: User[]
  pagination: Pagination
}



export interface ApiUser {
  id: number
  fullName: string
  userName: string
  email: string
  phone: string
  mfaChannel: MfaChannel
  emailVerified: boolean
  phoneVerified: boolean
  status: ApiUserStatus
  roles: Role[]
  type: ApiUserType
  createdAt: string
  updatedAt: string
}

export interface ApiUsersListResponse {
  data: ApiUser[]
  pagination: Pagination
}


export function userAdapter(user: ApiUser): User {
  return {
    id: user.id.toString(),
    fullName: user.fullName,
    userName: user.userName,
    email: user.email,
    phone: user.phone,
    mfaChannel: user.mfaChannel,
    emailVerified: user.emailVerified,
    phoneVerified: user.phoneVerified,
    status: user.status,
    type: user.type,
    roles: user.roles.map(role => ({ id: role.id.toString(), name: role.name })),
    lastLogin: null, // TODO add after API provides it
  }
}

export function usersListResponseAdapter(response: ApiUsersListResponse): UsersListResponse {
  return {
    users: response.data.map(user => userAdapter(user)),
    pagination: response.pagination,
  }
}
