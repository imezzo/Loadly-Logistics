import { usePermission } from '@/composables/admin-dashboard/usePermission'

export async function checkPermissionGuard(
  requiredPermission: string,
  requiredModule: string,
): Promise<{ path: string } | true> {
  const {
    hasPermission,
    hasModule
  } = usePermission(requiredModule, requiredPermission)

  if (!hasModule.value
    || (!hasPermission.value && requiredPermission)
  ) {
    return { path: '/errors/unauthorized' }
  }

  return true
}

