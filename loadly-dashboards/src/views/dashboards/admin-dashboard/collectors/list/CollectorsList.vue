<template>
  <div class="page-content">
    <UsersListHeader
      :breadcrumb="breadcrumbItems"
      :title="$t('title.collectors')"
      create-route-name="CreateCollector"
      :button-label="t('button.addCollector')"
      :has-create-permission
    />
    <UsersListFilters />

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
      edit-route-name="EditCollector"
      view-route-name="ViewCollector"
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
  import type { BreadcrumbItem } from '@/types/ui'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { Permission, Collector } from '@/types/response/permission-group'

  const UsersListActions = defineAsyncComponent(() => import('@/components/users/list/UsersListActions.vue'))

  const userStore = useUserStore()
  const { resourceUrl } = storeToRefs(userStore)
  resourceUrl.value = '/collectors'

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
  const { t } = useI18n()

  const { hasPermission: hasCreatePermission } = usePermission(Permission.Collector, Collector.Create)
  const { hasPermission: hasViewPermission } = usePermission(Permission.Collector, Collector.View)
  const { hasPermission: hasEditPermission } = usePermission(Permission.Collector, Collector.Edit)

  const breadcrumbItems = computed<BreadcrumbItem[]>(() => [
    { label: t('title.collectors'), to: '/collectors' },
    { label: t('title.collectorsList') },
  ])

  const selectedIds = ref<string[]>([])

  function onSelect(ids: Set<string>): void {
    selectedIds.value = Array.from(ids)
  }

  onMounted(fetchUsers)
  onBeforeUnmount(resetFilters)
</script>

<style scoped>

</style>
