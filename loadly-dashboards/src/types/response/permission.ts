import { Permission } from "./permission-group"

export type PermissionKey = (typeof Permission)[keyof typeof Permission]
export type ApiPermission = Record<PermissionKey, string[]>

export interface GroupedPermission {
  id: number
  name: string
  checked: boolean
}

export interface PermissionModule {
  id: string
  name: string
  permissions: GroupedPermission[]
}

export type GroupedPermissionsResponse = Record<string, GroupedPermission[]>

export interface ApiGroupedPermission {
  id: number
  name: string
  nameAr: string | null
  guardName: string
  status: string
  permissionGroupId: number
  isAssigned: boolean
  createdAt: string
  updatedAt: string
  deletedAt: string | null
}

export interface ApiGroupedPermissionsResponse {
  permissions: Record<string, ApiGroupedPermission[]>
}

function formatGroupName(key: string): string {
  return key
    .split('_')
    .join(' ')
    .split('.')
    .join(' ')
}

function groupedPermissionAdapter(permission: ApiGroupedPermission): GroupedPermission {
  return {
    id: permission.id,
    name: formatGroupName(permission.name),
    checked: permission.isAssigned,
  }
}

export function groupedPermissionsResponseAdapter(response: ApiGroupedPermissionsResponse): PermissionModule[] {
  return Object.entries(response.permissions).map(([key, permissions]) => ({
    id: key,
    name: formatGroupName(key),
    permissions: permissions.map(permission => groupedPermissionAdapter(permission)),
  }))
}
