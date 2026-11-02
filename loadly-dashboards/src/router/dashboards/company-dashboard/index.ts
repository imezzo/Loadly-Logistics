import type { RouteRecordRaw } from 'vue-router'
import { authRoutes } from '@/router/auth'
import { contractsRoutes } from './contracts'
import { dashboardReportsRoutes } from './dashboard-reports'
import { dashboardRoutes } from './dashboard'
import { errorsRoutes } from '@/router/errors'
import { tripsRoutes } from './trips'
import { profileRoutes } from '@/router/profile'
import { usersRoutes } from './users'
import { userRolesRoutes } from '@/router/user-roles'

export const routes: RouteRecordRaw[] = [
  ...authRoutes,
  ...contractsRoutes,
  ...dashboardReportsRoutes,
  ...dashboardRoutes,
  ...errorsRoutes,
  ...profileRoutes,
  ...tripsRoutes,
  ...userRolesRoutes,
  ...usersRoutes,
]
