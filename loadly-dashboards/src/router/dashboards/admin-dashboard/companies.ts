import { Company, Permission } from '@/types/response/permission-group'
import type { RouteRecordRaw } from 'vue-router'

export const companiesRoutes: RouteRecordRaw[] = [
  {
    path: '/companies',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'CompaniesList',
        component: () => import('@/views/dashboards/admin-dashboard/companies/list/CompaniesList.vue'),
        meta: {
          title: 'Companies',
          requiresAuth: true,
          module: Permission.Company,
          permission: Company.View,
        }
      },
      {
        path: 'create',
        name: 'CreateCompany',
        component: () => import('@/views/dashboards/admin-dashboard/companies/create/CreateCompany.vue'),
        meta: {
          title: 'Add new companies',
          requiresAuth: true,
          module: Permission.Company,
          permission: Company.Create,
        }
      },
      {
        path: 'edit/:id',
        name: 'UpdateCompany',
        component: () => import('@/views/dashboards/admin-dashboard/companies/update/UpdateCompany.vue'),
        meta: {
          title: 'Edit Company',
          requiresAuth: true,
          module: Permission.Company,
          permission: Company.Edit,
        }
      },
    ]
  },
]

