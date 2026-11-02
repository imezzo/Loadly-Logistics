import { Driver, Permission } from '@/types/response/permission-group'
import type { RouteRecordRaw } from 'vue-router'

export const driversRoutes: RouteRecordRaw[] = [
  {
    path: '/drivers',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'Drivers',
        component: () => import('@/views/dashboards/admin-dashboard/drivers/list/DriversList.vue'),
        meta: {
          title: 'Drivers',
          requiresAuth: true,
          module: Permission.Driver,
          permission: Driver.View,
        }
      },
      {
        path: 'create',
        name: 'DriverCreate',
        component: () => import('@/views/dashboards/admin-dashboard/drivers/wizard/DriverWizard.vue'),
        meta: {
          title: 'Add New Driver',
          requiresAuth: true,
          module: Permission.Driver,
          permission: Driver.Create,
        }
      },
      {
        path: 'edit/:id',
        name: 'DriverEdit',
        component: () => import('@/views/dashboards/admin-dashboard/drivers/wizard/DriverWizard.vue'),
        meta: {
          title: 'Edit Driver',
          requiresAuth: true,
          module: Permission.Driver,
          permission: Driver.Edit,
        }
      },
    ]
  },
]
