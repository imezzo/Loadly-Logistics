import type {
  RouteLocationNormalized,
  RouteLocationNormalizedLoaded,
  RouterScrollBehavior
} from "vue-router"

export const scrollBehavior: RouterScrollBehavior = (
  to: RouteLocationNormalized,
  from: RouteLocationNormalizedLoaded,
  savedPosition
) => {
  if (savedPosition) {
    return savedPosition
  } else {
    return { top: 0 }
  }
}
