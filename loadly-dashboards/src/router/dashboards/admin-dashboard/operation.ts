import type { RouteRecordRaw } from 'vue-router'

export const operationRoutes: RouteRecordRaw[] = [
  {
    path: '/operation',
    name: 'Operation',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: 'sector-analytics',
        name: 'SectorAnalytics',
        component: () =>
          import('@/views/dashboards/admin-dashboard/operations/sector-analytics/SectorAnalytics.vue'),
        meta: {
          title: 'Sector Analytics',
          requiresAuth: true,
        },
      },
    ],
  },
]
