import { groupedPermissionsResponseAdapter } from "@/types/response/permission"
import { api } from "@/utils/api"

export const usePermissionStore = defineStore('permission', () => {
  async function getGroupedPermissions(id: number) {
    return api({
      url: `/role-has-permissions/${id}/grouped`,
      method: 'GET',
      responseAdapter: groupedPermissionsResponseAdapter,
    })
  }

  async function updatePermissions(data: { roleId: number; permissionIds: number[] }) {
    const { roleId: id, permissionIds } = data

    return api({
      url: `/role-has-permissions/${id}/sync`,
      method: 'POST',
      data: { permissionIds },
    })
  }

  return {
    getGroupedPermissions,
    updatePermissions,
  }
})

