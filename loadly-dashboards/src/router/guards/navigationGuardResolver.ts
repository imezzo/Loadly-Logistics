import type { RouteLocationNormalized } from 'vue-router'
import { useAuth } from '@/composables/admin-dashboard/useAuth'
import { checkPermissionGuard } from './permissionGuard'
import { useProfile } from '@/composables/admin-dashboard/useProfile'
import { useEnum } from '@/composables/useEnum'

const publicPaths = ['/errors/unauthorized', '/auth/reset-password', '/info/privacy-policy']

function setPageTitle(title?: string | unknown): void {
  if (title) {
    document.title = `${import.meta.env.VITE_BRAND_NAME} | ${title}`
  }
}

export async function navigationGuard(
  to: RouteLocationNormalized,
) {
  const { initProfile, profile } = useProfile()
  const { isLoggedIn, isTempLoggedIn } = useAuth()
  const { initEnums } = useEnum()
  setPageTitle(to.meta.title)

  const requiredPermission = (to.meta.permission as string) ?? ''
  const requiredModule = (to.meta.module as string) ?? ''
  const isPublicPage = !!to.meta.isPublic || publicPaths.includes(to.path)

  if (!isLoggedIn.value && to.meta.requiresAuth) {
    return { path: '/auth/login' }
  }

  if (isLoggedIn.value
    && !isTempLoggedIn.value
    && !to.meta.requiresAuth
    && !to.meta.isMfaStep
    && !isPublicPage) {

    return { path: '/' }
  }

  if (!profile.value
    && !to.path.includes('/auth/')
    && !isPublicPage) {

    await initProfile()
    await initEnums()
  }

  if (requiredPermission || requiredModule) {
    return await checkPermissionGuard(requiredPermission, requiredModule)
  }

  return true
}

