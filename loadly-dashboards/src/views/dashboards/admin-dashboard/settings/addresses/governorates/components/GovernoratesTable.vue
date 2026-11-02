<template>
  <BaseTable
    :columns="tableColumns"
    :data="governorates"
  >
    <template #cell-action="{ row }">
      <div class="flex items-center gap-2">
        <button
          v-if="hasEditPermission"
          type="button"
          class="btn-action-success"
          @click="$emit('edit', row)"
        >
          <IconEdit class="size-4" />
        </button>

        <button
          v-if="hasDeletePermission"
          type="button"
          class="btn-action-error"
          @click="$emit('delete', row)"
        >
          <IconTrash class="size-4" />
        </button>
      </div>
    </template>
  </BaseTable>

  <BasePagination
    :pagination="pagination"
    @set-page="$emit('setPage', $event)"
    @next-page="$emit('nextPage')"
    @prev-page="$emit('prevPage')"
    @update-items-per-page="$emit('updateItemsPerPage', $event)"
  />
</template>

<script setup lang="ts">
  import type { TableColumn } from '@/types/ui'
  import type { Governorate } from '@/types/admin-dashboard/response/governorate'
  import type { Pagination } from '@/types/response/pagination'
  import { Address, Permission } from '@/types/response/permission-group'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'

  defineProps<{
    governorates: Governorate[]
    pagination: Pagination
    isLoading: boolean
  }>()

  const {
    hasPermission: hasEditPermission,
  } = usePermission(Permission.Address, Address.EditGovernerate)

  const {
    hasPermission: hasDeletePermission,
  } = usePermission(Permission.Address, Address.DeleteGovernerate)

  defineEmits<{
    setPage: [page: number]
    nextPage: []
    prevPage: []
    updateItemsPerPage: [size: number]
    edit: [governorate: Governorate]
    delete: [governorate: Governorate]
  }>()

  const { t } = useI18n()

  const tableColumns = ref<TableColumn[]>([
    { key: 'name', label: t('table.name'), cellClass: 'font-medium text-neutral-20' },
    { key: 'action', label: t('table.action') },
  ])
</script>

<style scoped>

</style>

