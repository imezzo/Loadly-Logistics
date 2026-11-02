<template>
  <div class="page-content">
    <UsersListHeader :has-create-permission />
    <UsersListFilters show-role-ids />

    <UsersListActions
      v-if="selectedIds.length && hasEditPermission"
      :selected-ids
      @reset="selectedIds = []"
      @fetch-users="fetchUsers"
    />

    <UsersListTable
      :has-activation-actions="hasEditPermission"
      :users
      :pagination
      :has-edit-permission="hasEditPermission"
      :has-view-permission="hasViewPermission"
      @select="onSelect"
    />

    <BasePagination
      :pagination
      @set-page="setPage"
      @next-page="nextPage"
      @prev-page="prevPage"
      @update-items-per-page="updateItemsPerPage"
    />
  </div>
</template>

<script setup lang="ts">
  import { useUsersList } from '@/composables/modules/users/useUsersList'
  import { useUserStore } from '@/stores/admin-dashboard/user'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { Permission, Admin } from '@/types/response/permission-group'

  const UsersListActions = defineAsyncComponent(() => import('@/components/users/list/UsersListActions.vue'))

  const userStore = useUserStore()
  const { resourceUrl } = storeToRefs(userStore)
  resourceUrl.value = '/admins'

  const {
    fetchUsers,
    nextPage,
    pagination,
    prevPage,
    resetFilters,
    setPage,
    updateItemsPerPage,
    users,
  } = useUsersList()
  const { hasPermission: hasCreatePermission } = usePermission(Permission.Admin, Admin.Create)
  const { hasPermission: hasEditPermission } = usePermission(Permission.Admin, Admin.Edit)
  const { hasPermission: hasViewPermission } = usePermission(Permission.Admin, Admin.View)

  const selectedIds = ref<string[]>([])

  function onSelect(ids: Set<string>): void {
    selectedIds.value = Array.from(ids)
  }

  onMounted(fetchUsers)
  onBeforeUnmount(resetFilters)
</script>

<style scoped>

</style>
