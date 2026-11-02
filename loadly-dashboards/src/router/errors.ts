import type { RouteRecordRaw } from 'vue-router'

export const errorsRoutes: RouteRecordRaw[] = [
  {
    path: '/errors/unauthorized',
    name: 'Unauthorized',
    component: () => import('@/views/errors/Unauthorized.vue'),
    meta: {
      title: 'Unauthorized',
      requiresAuth: false,
    },
  },
  {
    path: '/errors/not-found',
    name: 'NotFound',
    component: () => import('@/views/errors/NotFound.vue'),
    meta: {
      title: 'Not Found',
      requiresAuth: false,
    },
  },
]

