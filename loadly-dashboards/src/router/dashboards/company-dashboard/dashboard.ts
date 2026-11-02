import type { RouteRecordRaw } from 'vue-router'

export const dashboardRoutes: RouteRecordRaw[] = [
  {
    path: '/',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '/',
        name: 'Dashboard',
        component: () => import('@/views/dashboards/company-dashboard/dashboard/Dashboard.vue'),
        meta: {
          title: 'My Dashboard',
          requiresAuth: true,
        }
      },
    ]
  },
]

