import { useProfileStore } from '@/stores/admin-dashboard/profile'

export function usePermission(module: string, permission: string) {
  const profileStore = useProfileStore()

  function hasPermission(): boolean {
    if (!profileStore.profile) {
      return false
    }

    const modulePermissions = profileStore.profile.permissions[module]
    if (!modulePermissions) {
      return false
    }

    return modulePermissions.includes(permission)
  }

  function hasModule(): boolean {
    if (!profileStore.profile) {
      return false
    }

    const modulePermissions = profileStore.profile.permissions[module]

    return !!(modulePermissions?.length ?? false)
  }

  return {
    hasPermission: computed(hasPermission),
    hasModule: computed(hasModule),
  }
}

