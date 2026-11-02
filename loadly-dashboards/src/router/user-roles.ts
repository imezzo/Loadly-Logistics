import { Permission } from '@/types/response/permission-group'
import type { RouteRecordRaw } from 'vue-router'

export const userRolesRoutes: RouteRecordRaw[] = [
  {
    path: '/users-roles',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'UsersRoles',
        component: () => import('@/views/global-modules/users-roles/roles/UsersRoles.vue'),
        meta: {
          title: 'Users Roles',
          requiresAuth: true,
          module: Permission.PermissionModule,
        }
      },
      {
        path: ':id/permissions',
        name: 'RolePermissions',
        component: () => import('@/views/global-modules/users-roles/permissions/PermissionsList.vue'),
        meta: {
          title: 'Role Permissions',
          requiresAuth: true,
          module: Permission.PermissionModule,
        }
      },
    ]
  },
]
