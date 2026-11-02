import type { PermissionModule } from '@/types/response/permission'
import { useRoleStore } from '@/stores/role'
import { usePermissionStore } from '@/stores/permission'
import { useLoading } from '@/composables/useLoading'

export function usePermissionsList(roleId: Ref<number>) {
  const roleStore = useRoleStore()
  const permissionStore = usePermissionStore()
  const { setLoading } = useLoading()

  const roleName = ref<string>('')
  const isErrorPopupOpen = ref<boolean>(false)
  const isPermissionsSaved = ref<boolean>(false)
  const errorMessage = ref<string>('')
  const permissions = ref<PermissionModule[]>([])

  const { t } = useI18n()

  async function fetchRoleName(): Promise<void> {
    if (!roleId.value) return

    const { success: roleSuccess, data: roleData } = await roleStore.getById(roleId.value)
    if (roleSuccess && roleData) {
      roleName.value = roleData.name
    } else {
      roleName.value = `Role ${roleId.value}`
    }
  }

  async function fetchRoleDetails(): Promise<void> {
    if (!roleId.value) return

    setLoading(true)
    const { success, data, message } = await permissionStore.getGroupedPermissions(roleId.value)
    setLoading(false)

    if (!success) {
      errorMessage.value = message || t('error.systemError')
      isErrorPopupOpen.value = true

      return
    }

    permissions.value = data
    await fetchRoleName()
  }

  async function savePermissions(): Promise<void> {
    if (!roleId.value) return

    setLoading(true)

    const checkedPermissionIds = permissions.value
      .flatMap(module => module.permissions.filter(p => p.checked))
      .map(p => p.id)

    const { success, message } = await permissionStore.updatePermissions({
      roleId: roleId.value,
      permissionIds: checkedPermissionIds,
    })

    setLoading(false)

    if (success) {
      isPermissionsSaved.value = true

      return
    }
    errorMessage.value = message || t('error.systemError')
    isErrorPopupOpen.value = true
  }

  return {
    roleName,
    isErrorPopupOpen,
    isPermissionsSaved,
    errorMessage,
    permissions,
    fetchRoleDetails,
    savePermissions,
  }
}

