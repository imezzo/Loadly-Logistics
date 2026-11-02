import { Invoice, Permission } from '@/types/response/permission-group'
import type { RouteRecordRaw } from 'vue-router'

export const financialRoutes: RouteRecordRaw[] = [
  {
    path: '/financial',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: 'invoices/:id',
        name: 'FinancialInvoiceView',
        component: () => import('@/views/global-modules/financial/invoice/view/InvoiceView.vue'),
        meta: {
          title: 'Invoice View',
          requiresAuth: true,
          module: Permission.Invoice,
          permission: Invoice.View,
        }
      },
      {
        path: 'payment-requests',
        name: 'FinancialPaymentRequests',
        component: () => import('@/views/global-modules/financial/payment/list/PaymentList.vue'),
        meta: {
          title: 'Payment Tracking',
          requiresAuth: true,
          module: Permission.Invoice,
          permission: Invoice.View,
        }
      },
    ]
  },
]
