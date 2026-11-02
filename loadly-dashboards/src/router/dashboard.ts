import type { RouteRecordRaw } from 'vue-router'

export const dashboardRoutes: RouteRecordRaw[] = [
  {
    path: '/',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '/',
        name: 'Dashboard',
        component: () => import('@/views/global-modules/dashboard/DashboardPage.vue'),
        meta: {
          title: 'Home',
          requiresAuth: true,
        }
      },
    ]
  },
]

