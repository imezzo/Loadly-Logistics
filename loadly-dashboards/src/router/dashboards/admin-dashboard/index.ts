import type { RouteRecordRaw } from 'vue-router'
import { authRoutes } from '@/router/auth'
import { collectorsRoutes } from './collectors'
import { companiesRoutes } from './companies'
import { contractorsRoutes } from './contractors'
import { contractsRoutes } from './contracts'
import { dashboardReportsRoutes } from './dashboard-reports'
import { dashboardRoutes } from './dashboard'
import { driversRoutes } from './drivers'
import { errorsRoutes } from '@/router/errors'
import { financialRoutes } from './financial'
import { fleetRoutes } from './fleet'
import { notificationsRoutes } from './notifications'
import { profileRoutes } from '@/router/profile'
import { operationRoutes } from './operation'
import { settingsRoutes } from './settings'
import { staticPagesRoutes } from './static-pages'
import { tripsRoutes } from './trips'
import { userRolesRoutes } from '@/router/user-roles'
import { usersRoutes } from './users'
import { vehicleFacilitiesRoutes } from './vehicle-facilities'

export const routes: RouteRecordRaw[] = [
  ...authRoutes,
  ...collectorsRoutes,
  ...companiesRoutes,
  ...contractorsRoutes,
  ...contractsRoutes,
  ...dashboardReportsRoutes,
  ...dashboardRoutes,
  ...driversRoutes,
  ...errorsRoutes,
  ...financialRoutes,
  ...fleetRoutes,
  ...notificationsRoutes,
  ...operationRoutes,
  ...profileRoutes,
  ...settingsRoutes,
  ...staticPagesRoutes,
  ...tripsRoutes,
  ...userRolesRoutes,
  ...usersRoutes,
  ...vehicleFacilitiesRoutes,
]
