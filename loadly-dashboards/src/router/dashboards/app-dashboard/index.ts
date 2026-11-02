import type { RouteRecordRaw } from 'vue-router'
import { authRoutes } from '@/router/auth'
import { dashboardRoutes } from '@/router/dashboard'
import { errorsRoutes } from '@/router/errors'

export const routes: RouteRecordRaw[] = [
  ...authRoutes,
  ...dashboardRoutes,
  ...errorsRoutes,
]
