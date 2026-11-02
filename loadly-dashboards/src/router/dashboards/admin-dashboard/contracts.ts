import { Contract, Permission } from '@/types/response/permission-group'
import type { RouteRecordRaw } from 'vue-router'

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
        path: 'create',
        name: 'CreateContract',
        component: () => import('@/views/global-modules/contracts/wizard/ContractWizard.vue'),
        meta: {
          title: 'Add new contract',
          requiresAuth: true,
          module: Permission.Contract,
          permission: Contract.Create,
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
      {
        path: 'update/:id',
        name: 'UpdateContract',
        component: () => import('@/views/global-modules/contracts/wizard/ContractWizard.vue'),
        meta: {
          title: 'Update contract',
          requiresAuth: true,
          module: Permission.Contract,
          permission: Contract.Edit,
        }
      },
      {
        path: 'renew/:id',
        name: 'RenewContract',
        component: () => import('@/views/global-modules/contracts/wizard/ContractWizard.vue'),
        meta: {
          title: 'Renew contract',
          requiresAuth: true,
          module: Permission.Contract,
          permission: Contract.Edit,
        }
      },
      {
        path: ':id/contract-slots-status',
        name: 'ContractSlotsStatus',
        component: () => import('@/views/global-modules/contracts/vehicle-by-days/VehicleByDays.vue'),
        meta: {
          title: 'Contract Slots Status',
          requiresAuth: true,
          module: Permission.Contract,
          permission: Contract.ViewSlotActivity,
        }
      },
    ]
  },
]
