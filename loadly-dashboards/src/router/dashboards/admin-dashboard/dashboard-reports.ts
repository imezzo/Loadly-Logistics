import { Dashboard, Permission } from '@/types/response/permission-group'
import type { RouteRecordRaw } from 'vue-router'

export const dashboardReportsRoutes: RouteRecordRaw[] = [
  {
    path: '/dashboard-reports',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'DashboardReportsOverview',
        component: () => import('@/views/dashboards/admin-dashboard/dashboard-reports/overview/DashboardReports.vue'),
        meta: {
          title: 'Dashboard Reports',
          requiresAuth: true,
          module: Permission.Dashboard,
          permission: Dashboard.View,
        }
      },
      {
        path: 'fleet-utilization-statistics',
        name: 'DashboardReportsFleetUtilizationStatistics',
        component: () => import('@/views/dashboards/admin-dashboard/dashboard-reports/fleet-utilization-statistics/DashboardReportsFleet.vue'),
        meta: {
          title: 'Fleet utilization statistics',
          requiresAuth: true,
          module: Permission.Dashboard,
          permission: Dashboard.View,
        },
      },
      {
        path: 'driver-performance-metrics',
        name: 'DashboardReportsDriverPerformanceMetrics',
        component: () => import('@/views/dashboards/admin-dashboard/dashboard-reports/driver-performance-metrics/DashboardReportsDriverPerformance.vue'),
        meta: {
          title: 'Driver performance metrics',
          requiresAuth: true,
          module: Permission.Dashboard,
          permission: Dashboard.View,
        },
      },
      {
        path: 'driver-behavior',
        name: 'DashboardReportsDriverBehavior',
        component: () => import('@/views/dashboards/admin-dashboard/dashboard-reports/driver-behavior/DriverBehavior.vue'),
        meta: {
          title: 'Driver behavior',
          requiresAuth: true,
          module: Permission.Dashboard,
          permission: Dashboard.View,
        },
      },
    ]
  },
]
