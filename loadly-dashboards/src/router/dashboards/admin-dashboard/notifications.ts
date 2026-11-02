import type { RouteRecordRaw } from 'vue-router'

export const notificationsRoutes: RouteRecordRaw[] = [
  {
    path: '/notifications',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'NotificationsList',
        component: () => import('@/views/dashboards/admin-dashboard/notifications/list/NotificationsList.vue'),
        meta: {
          title: 'Notifications Center',
          requiresAuth: true,
        }
      },
      {
        path: 'create',
        name: 'CreateBroadcastNotification',
        component: () => import('@/views/dashboards/admin-dashboard/notifications/wizard/BroadcastNotificationWizard.vue'),
        meta: {
          title: 'Send Notification',
          requiresAuth: true,
        }
      },
      {
        path: 'view/:id',
        name: 'ViewBroadcastNotification',
        component: () => import('@/views/dashboards/admin-dashboard/notifications/wizard/BroadcastNotificationWizard.vue'),
        meta: {
          title: 'View Notification',
          requiresAuth: true,
        }
      },
    ]
  },
  {
    path: '/brdcst/:code',
    name: 'PublicBroadcastNotification',
    component: () => import('@/views/dashboards/admin-dashboard/notifications/public/PublicBroadcastNotification.vue'),
    meta: {
      title: 'Notification',
      requiresAuth: false,
      isPublic: true,
    },
  },
]
