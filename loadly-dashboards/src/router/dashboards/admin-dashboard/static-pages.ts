import type { RouteRecordRaw } from 'vue-router'

export const staticPagesRoutes: RouteRecordRaw[] = [
  {
    path: '/info/privacy-policy',
    name: 'PrivacyPolicy',
    component: () => import('@/views/dashboards/admin-dashboard/static/privacy-policy.vue'),
    meta: {
      title: 'Privacy Policy',
      requiresAuth: false,
    },
  },
]
