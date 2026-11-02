import { Permission, Users } from '@/types/response/permission-group'
import type { RouteRecordRaw } from 'vue-router'

export const usersRoutes: RouteRecordRaw[] = [
  {
    path: '/users',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'UsersList',
        component: () => import('@/views/dashboards/company-dashboard/users/list/UsersList.vue'),
        meta: {
          title: 'Users',
          requiresAuth: true,
          module: Permission.Users,
          permission: Users.View,
        }
      },
      {
        path: 'wizard',
        name: 'CreateUser',
        component: () => import('@/views/dashboards/company-dashboard/users/wizard/UserWizard.vue'),
        meta: {
          title: 'Add User',
          requiresAuth: true,
          module: Permission.Users,
          permission: Users.Create,
        }
      },
      {
        path: 'wizard/:id',
        name: 'EditUser',
        component: () => import('@/views/dashboards/company-dashboard/users/wizard/UserWizard.vue'),
        meta: {
          title: 'Edit User',
          requiresAuth: true,
          module: Permission.Users,
          permission: Users.Edit,
        }
      },
      {
        path: 'view/:id',
        name: 'ViewUser',
        component: () => import('@/views/dashboards/company-dashboard/users/wizard/UserWizard.vue'),
        meta: {
          title: 'View User',
          requiresAuth: true,
          module: Permission.Users,
          permission: Users.View,
        }
      },
    ],
  },
]
