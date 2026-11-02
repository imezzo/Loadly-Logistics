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
        component: () => import('@/views/dashboards/company-dashboard/dashboard-reports/trips-reports/TripsReports.vue'),
        meta: {
          title: 'Dashboard Reports',
          requiresAuth: true,
          module: Permission.Dashboard,
          permission: Dashboard.View,
        }
      },
      {
        path: 'cost-analysis-reports',
        name: 'DashboardReportsCostAnalysisReports',
        component: () => import('@/views/dashboards/company-dashboard/dashboard-reports/trips-cost-reports/TripsCostReports.vue'),
        meta: {
          title: 'Cost analysis reports',
          requiresAuth: true,
          module: Permission.Dashboard,
          permission: Dashboard.View,
        },
      },
      {
        path: 'invoice-billing',
        name: 'DashboardReportsInvoiceBilling',
        component: () => import('@/views/dashboards/company-dashboard/dashboard-reports/invoice-reports/InvoiceReports.vue'),
        meta: {
          title: 'Invoice and billing information',
          requiresAuth: true,
          module: Permission.Dashboard,
          permission: Dashboard.View,
        },
      },
    ]
  },
]
