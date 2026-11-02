export interface User {
  id: string
  fullName: string
  email: string
  role: UserRole
  status: UserStatus
  lastLogin: string | null
}

export enum UserRole {
  ADMIN = 'Admin',
  SUPERADMIN = 'Superadmin',
}

export enum UserStatus {
  DISABLED = 0,
  ACTIVE = 1,
  SUSPENDED = 2,
}

export enum ApiUserType {
  SUPERADMIN = 1,
  ADMIN = 2,
}

export enum ApiUserStatus {
  ACTIVE = 1,
  SUSPENDED = 2,
  DISABLED = 3,
}

export interface UsersFilter {
  roleIds: string[]
  search: string
  status: UserStatus | ''
}
