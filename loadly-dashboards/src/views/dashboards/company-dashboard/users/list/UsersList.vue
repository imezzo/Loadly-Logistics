<template>
  <div class="page-content">
    <UsersListHeader :has-create-permission />
    <UsersListFilters />

    <UsersListTable
      :users
      :pagination
      :has-edit-permission="hasEditPermission"
      :has-view-permission="hasViewPermission"
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
  import { Permission, Users } from '@/types/response/permission-group'

  const userStore = useUserStore()
  const { resourceUrl } = storeToRefs(userStore)
  resourceUrl.value = '/users'

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

  const { hasPermission: hasCreatePermission } = usePermission(Permission.Users, Users.Create)
  const { hasPermission: hasEditPermission } = usePermission(Permission.Users, Users.Edit)
  const { hasPermission: hasViewPermission } = usePermission(Permission.Users, Users.View)

  onMounted(fetchUsers)
  onBeforeUnmount(resetFilters)
</script>

<style scoped>

</style>
