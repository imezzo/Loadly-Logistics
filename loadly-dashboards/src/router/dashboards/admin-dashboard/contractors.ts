import { Contractor, Permission } from '@/types/response/permission-group'
import type { RouteRecordRaw } from 'vue-router'

export const contractorsRoutes: RouteRecordRaw[] = [
  {
    path: '/contractors',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'ContractorsList',
        component: () => import('@/views/dashboards/admin-dashboard/contractors/list/ContractorsList.vue'),
        meta: {
          title: 'Contractors',
          requiresAuth: true,
          module: Permission.Contractor,
          permission: Contractor.View,
        }
      },
      {
        path: 'create',
        name: 'CreateContractor',
        component: () => import('@/views/dashboards/admin-dashboard/contractors/wizard/ContractorWizard.vue'),
        meta: {
          title: 'Add Contractor',
          requiresAuth: true,
          module: Permission.Contractor,
          permission: Contractor.Create,
        }
      },
      {
        path: 'edit/:id',
        name: 'EditContractor',
        component: () => import('@/views/dashboards/admin-dashboard/contractors/wizard/ContractorWizard.vue'),
        meta: {
          title: 'Edit Contractor',
          requiresAuth: true,
          module: Permission.Contractor,
          permission: Contractor.Edit,
        }
      },
      {
        path: 'view/:id',
        name: 'ViewContractor',
        component: () => import('@/views/dashboards/admin-dashboard/contractors/wizard/ContractorWizard.vue'),
        meta: {
          title: 'View Contractor',
          requiresAuth: true,
          module: Permission.Contractor,
          permission: Contractor.View,
        }
      },
    ]
  },
]
