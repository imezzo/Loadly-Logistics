import type { RouteRecordRaw } from 'vue-router'

export const profileRoutes: RouteRecordRaw[] = [
  {
    path: '/profile',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'Profile',
        component: () => import('@/views/global-modules/profile/Profile.vue'),
        meta: {
          title: 'Profile',
          requiresAuth: true,
        },
      },
    ],
  },
]
