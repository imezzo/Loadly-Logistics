<template>
  <BaseTable
    :columns="tableColumns"
    :data="industryTypes"
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
  import type { IndustryType } from '@/types/admin-dashboard/response/industry-type'
  import type { Pagination } from '@/types/response/pagination'
  import { IndustryType as IndustryTypePermission, Permission } from '@/types/response/permission-group'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'

  defineProps<{
    industryTypes: IndustryType[]
    pagination: Pagination
    isLoading: boolean
  }>()

  defineEmits<{
    setPage: [page: number]
    nextPage: []
    prevPage: []
    updateItemsPerPage: [size: number]
    edit: [industryType: IndustryType]
    delete: [industryType: IndustryType]
  }>()

  const { t } = useI18n()
  const {
    hasPermission: hasEditPermission,
  } = usePermission(Permission.IndustryType, IndustryTypePermission.Edit)

  const {
    hasPermission: hasDeletePermission,
  } = usePermission(Permission.IndustryType, IndustryTypePermission.Delete)

  const tableColumns = ref<TableColumn[]>([
    { key: 'name', label: t('table.name'), cellClass: 'font-medium text-neutral-20' },
    { key: 'action', label: t('table.action') },
  ])
</script>

<style scoped>

</style>
