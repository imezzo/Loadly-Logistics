import type { RouteRecordRaw } from 'vue-router'
import { Permission, Contract } from '@/types/response/permission-group'

export const contractsRoutes: RouteRecordRaw[] = [
  {
    path: '/contracts',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'ContractsList',
        component: () => import('@/views/global-modules/contracts/list/ContractsList.vue'),
        meta: {
          title: 'Contracts',
          requiresAuth: true,
          module: Permission.Contract,
          permission: Contract.View,
        }
      },
      {
        path: 'view/:id',
        name: 'ViewContract',
        component: () => import('@/views/global-modules/contracts/wizard/ContractWizard.vue'),
        meta: {
          title: 'View contract',
          requiresAuth: true,
          module: Permission.Contract,
          permission: Contract.View,
        }
      },
    ]
  },
]
