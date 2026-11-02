import { Collector, Permission } from '@/types/response/permission-group'
import type { RouteRecordRaw } from 'vue-router'

export const collectorsRoutes: RouteRecordRaw[] = [
  {
    path: '/collectors',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'CollectorsList',
        component: () => import('@/views/dashboards/admin-dashboard/collectors/list/CollectorsList.vue'),
        meta: {
          title: 'Collectors',
          requiresAuth: true,
          module: Permission.Collector,
          permission: Collector.View,
        }
      },
      {
        path: 'wizard',
        name: 'CreateCollector',
        component: () => import('@/views/dashboards/admin-dashboard/collectors/wizard/CollectorsWizard.vue'),
        meta: {
          title: 'Add Collector',
          requiresAuth: true,
          module: Permission.Collector,
          permission: Collector.Create,
        }
      },
      {
        path: 'wizard/:id',
        name: 'EditCollector',
        component: () => import('@/views/dashboards/admin-dashboard/collectors/wizard/CollectorsWizard.vue'),
        meta: {
          title: 'Edit Collector',
          requiresAuth: true,
          module: Permission.Collector,
          permission: Collector.Edit,
        }
      },
      {
        path: 'view/:id',
        name: 'ViewCollector',
        component: () => import('@/views/dashboards/admin-dashboard/collectors/wizard/CollectorsWizard.vue'),
        meta: {
          title: 'View Collector',
          requiresAuth: true,
          module: Permission.Collector,
          permission: Collector.View,
        }
      },
    ]
  },
]
