import { createRouter, createWebHistory } from 'vue-router'
import {
  clearChunkReloadFlag,
  clearPendingNavigationUrl,
  isChunkLoadError,
  reloadAppOnce,
  setPendingNavigationUrl,
} from '@/utils/chunkReload'
import { routes } from './routesResolver'
import { scrollBehavior } from './scrollBehaviorResolver'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
  scrollBehavior,
})

router.beforeEach((to) => {
  setPendingNavigationUrl(router.resolve(to).href)

  return import('./guards/navigationGuardResolver').then((module) => module.navigationGuard(to))
})

router.onError((error, to) => {
  if (!isChunkLoadError(error)) {
    clearPendingNavigationUrl()

    return
  }

  reloadAppOnce(router.resolve(to).href)
})

router.afterEach(() => {
  clearPendingNavigationUrl()
  clearChunkReloadFlag()
})

export default router
