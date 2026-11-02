import { Contract, Permission, Trip, Users, PermissionModule, Dashboard } from '../response/permission-group'
import type { SidebarItem } from '../sidebarNavigation'

export const mainNavigation: SidebarItem[] = [
  {
    id: 'dashboard',
    label: 'sidebar.dashboard',
    icon: 'IconDashboard',
    href: '/',
    routeName: 'AdminDashboard',
  },
  {
    id: 'contracts',
    label: 'sidebar.contracts',
    icon: 'IconContracts',
    href: '/contracts',
    routeName: 'Contracts',
    module: Permission.Contract,
    permission: Contract.View,
  },
  {
    id: 'trip-management',
    label: 'sidebar.tripManagement',
    icon: 'IconTrip',
    isExpandable: true,
    routeName: 'AllTrips',
    module: Permission.Trip,
    permission: Trip.View,
    children: [
      {
        id: 'company-trips',
        label: 'sidebar.tripsOnMap',
        href: '/trips/map',
        routeName: 'TripsOnMap',
      },
      {
        id: 'all-trips',
        label: 'sidebar.contractTrips',
        href: '/trips',
        routeName: 'AllTrips',
      },
    ],
  },
  // {
  //   id: 'financial-management',
  //   label: 'sidebar.financialManagement',
  //   icon: 'IconMoneyBagBroken',
  //   isExpandable: true,
  //   routeName: 'FinancialManagement',
  //   module: Permission.Invoice,
  //   permission: Invoice.View,
  //   children: [
  //     {
  //       id: 'payment-requests',
  //       label: 'sidebar.paymentRequests',
  //       href: '/financial/payment-requests',
  //       routeName: 'FinancialPaymentRequests',
  //     }
  //   ],
  // },
  {
    id: 'dashboard-reports',
    label: 'sidebar.dashboardAndReports',
    icon: 'IconDashboard',
    routeName: 'DashboardReports',
    isExpandable: true,
    module: Permission.Dashboard,
    permission: Dashboard.View,
    children: [
      {
        id: 'dashboard-reports-kilometer-usage',
        label: 'sidebar.kilometerUsageVsPackageLimits',
        href: '/dashboard-reports',
        routeName: 'DashboardReportsOverview',
      },
      {
        id: 'dashboard-reports-cost-analysis',
        label: 'sidebar.costAnalysisReports',
        href: '/dashboard-reports/cost-analysis-reports',
        routeName: 'DashboardReportsCostAnalysisReports',
      },
      {
        id: 'dashboard-reports-invoice-billing',
        label: 'sidebar.invoiceAndBillingInformation',
        href: '/dashboard-reports/invoice-billing',
        routeName: 'DashboardReportsInvoiceBilling',
      },
    ],
  },
]

export const settingsNavigation: SidebarItem[] = [
  {
    id: 'profile',
    label: 'sidebar.profile',
    icon: 'IconProfile',
    href: '/profile',
    routeName: 'Profile',
  },
  {
    id: 'users',
    label: 'sidebar.users',
    icon: 'IconUsers',
    href: '/users',
    routeName: 'UsersList',
    module: Permission.Users,
    permission: Users.View,
  },
  {
    id: 'users-role',
    label: 'sidebar.usersRole',
    icon: 'IconTasks',
    href: '/users-roles',
    routeName: 'UsersRoles',
    module: Permission.PermissionModule,
    permission: PermissionModule.Manage,
  },
  {
    id: 'logout',
    label: 'sidebar.logout',
    icon: 'IconExit',
    routeName: 'Logout',
  },
]
