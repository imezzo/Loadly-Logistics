import { ContractBillingSnapshot, Invoice, Permission, Trip } from '@/types/response/permission-group'
import type { RouteRecordRaw } from 'vue-router'

export const financialRoutes: RouteRecordRaw[] = [
  {
    path: '/financial',
    component: () => import('@/layouts/defaultLayout.vue'),
    children: [
      {
        path: '',
        name: 'FinancialList',
        component: () => import('@/views/global-modules/financial/invoice/list/InvoiceList.vue'),
        meta: {
          title: 'Financial',
          requiresAuth: true,
          module: Permission.Invoice,
          permission: Invoice.View,
        }
      },
      {
        path: 'company/:id',
        name: 'FinancialCompanyDetails',
        component: () => import('@/views/global-modules/financial/invoice/company/CompanyDetails.vue'),
        meta: {
          title: 'Financial Company Details',
          requiresAuth: true,
          module: Permission.Invoice,
          permission: Invoice.View,
        }
      },
      {
        path: 'company/:id/vehicle-by-days',
        name: 'FinancialVehicleByDays',
        component: () => import('@/views/global-modules/financial/invoice/vehicle-by-days/VehicleByDays.vue'),
        meta: {
          title: 'Vehicle By Days',
          requiresAuth: true,
          module: Permission.Invoice,
          permission: Invoice.View,
        }
      },
      {
        path: 'company/:id/:invoiceType/:invoiceId?',
        name: 'FinancialInvoiceWizard',
        component: () => import('@/views/global-modules/financial/invoice/wizard/InvoiceWizard.vue'),
        meta: {
          title: 'Invoice Wizard',
          requiresAuth: true,
          module: Permission.Invoice,
          permission: Invoice.Create,
        }
      },
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
        path: 'invoices/:id/edit',
        name: 'FinancialInvoiceEdit',
        component: () => import('@/views/global-modules/financial/invoice/wizard/InvoiceWizard.vue'),
        meta: {
          title: 'Edit Invoice',
          requiresAuth: true,
          module: Permission.Invoice,
          permission: Invoice.Edit,
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
      {
        path: 'contractor-invoices',
        name: 'FinancialContractorInvoices',
        component: () => import('@/views/global-modules/financial/contractor-invoices/contractors-list/ContractorInvoices.vue'),
        meta: {
          title: 'Contractor Invoices',
          requiresAuth: true,
          module: Permission.Invoice,
          permission: Invoice.View,
        }
      },
      {
        path: 'contractor-invoices/:contractorId',
        component: () => import('@/views/global-modules/financial/contractor-invoices/trips-list/TripsList.vue'),
        children: [
          {
            path: '',
            redirect: { name: 'FinancialContractorTrips' },
          },
          {
            path: 'trips',
            name: 'FinancialContractorTrips',
            component: () => import('@/views/global-modules/financial/contractor-invoices/trips-list/all-trips/AllTrips.vue'),
            meta: {
              title: 'Contractor Trips',
              requiresAuth: true,
              module: Permission.Invoice,
              permission: Invoice.View,
            }
          },
          {
            path: 'payouts',
            name: 'FinancialContractorPayouts',
            component: () => import('@/views/global-modules/financial/contractor-invoices/trips-list/payouts/TripPayouts.vue'),
            meta: {
              title: 'Contractor Payouts',
              requiresAuth: true,
              module: Permission.Invoice,
              permission: Invoice.View,
            }
          },
        ]
      },
      {
        path: 'contractor-invoices/:contractorId/invoices/:invoiceId',
        name: 'FinancialContractorInvoiceView',
        component: () => import('@/views/global-modules/financial/contractor-invoices/view-invoice/ViewInvoice.vue'),
        meta: {
          title: 'Contractor Invoice',
          requiresAuth: true,
          module: Permission.Invoice,
          permission: Invoice.View,
        }
      },
      {
        path: 'transfers',
        name: 'FinancialTransfers',
        component: () => import('@/views/global-modules/financial/transfer/list/TransferList.vue'),
        meta: {
          title: 'Financial Transfers',
          requiresAuth: true,
          module: Permission.Invoice,
          permission: Invoice.View,
        }
      },
      {
        path: 'transfers/:id',
        name: 'FinancialTransferDetails',
        component: () => import('@/views/global-modules/financial/transfer/view/TransferDetails.vue'),
        meta: {
          title: 'Financial Transfer Details',
          requiresAuth: true,
          module: Permission.Invoice,
          permission: Invoice.View,
        }
      },
      {
        path: 'reports/monthly-contracts',
        name: 'FinancialMonthlyContracts',
        component: () => import('@/views/global-modules/financial/monthly-contract/list/MonthlyContractList.vue'),
        meta: {
          title: 'Review monthly contracts',
          requiresAuth: true,
          module: Permission.ContractBillingSnapshot,
          permission: ContractBillingSnapshot.View,
        }
      },
      {
        path: 'reports/monthly-contracts/:id',
        name: 'FinancialMonthlyContractSlotsCharges',
        component: () => import('@/views/global-modules/financial/monthly-contract/slots-charges/SlotsCharges.vue'),
        meta: {
          title: 'Review monthly contracts',
          requiresAuth: true,
          module: Permission.ContractBillingSnapshot,
          permission: ContractBillingSnapshot.View,
        }
      },
      {
        path: 'reports/trip/company',
        name: 'FinancialTripCompanyReports',
        component: () => import('@/views/global-modules/financial/reports/trip/TripReports.vue'),
        meta: {
          title: 'Trip Reports',
          requiresAuth: true,
          module: Permission.Invoice,
          permission: Invoice.View,
        }
      },
      {
        path: 'reports/trip/external',
        name: 'FinancialTripExternalReports',
        component: () => import('@/views/global-modules/financial/reports/trip/TripReports.vue'),
        meta: {
          title: 'Trip Reports',
          requiresAuth: true,
          module: Permission.Invoice,
          permission: Invoice.View,
        }
      },
      {
        path: 'auditing/company',
        name: 'FinancialTripAuditingCompany',
        component: () => import('@/views/global-modules/financial/auditing/list/TripAuditing.vue'),
        meta: {
          title: 'Trip Auditing',
          requiresAuth: true,
          module: Permission.Trip,
          permission: Trip.AuditTrip,
        }
      },
      {
        path: 'auditing/clients',
        name: 'FinancialTripAuditingClients',
        component: () => import('@/views/global-modules/financial/auditing/list/TripAuditing.vue'),
        meta: {
          title: 'Trip Auditing',
          requiresAuth: true,
          module: Permission.Trip,
          permission: Trip.AuditTrip,
        }
      },
      {
        path: 'auditing/:id',
        name: 'FinancialTripAuditingForm',
        component: () => import('@/views/global-modules/financial/auditing/wizard/TripAuditingForm.vue'),
        meta: {
          title: 'Trip Auditing',
          requiresAuth: true,
          module: Permission.Trip,
          permission: Trip.AuditTrip,
        }
      },
    ]
  },
]
