<template>
  <BaseTable
    :columns="tableColumns"
    :data="colors"
  >
    <template #cell-colorHash="{ row }">
      <div class="flex items-center gap-2">
        <span
          class="inline-block size-8 shrink-0 rounded-sm border border-neutral-500"
          :style="{ backgroundColor: row.colorHash }"
        />
        <span class="font-medium text-neutral-20">{{ row.colorHash }}</span>
      </div>
    </template>

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
  import type { Color } from '@/types/admin-dashboard/response/color'
  import type { Pagination } from '@/types/response/pagination'
  import { Color as ColorPermission, Permission } from '@/types/response/permission-group'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'

  defineProps<{
    colors: Color[]
    pagination: Pagination
    isLoading: boolean
  }>()

  defineEmits<{
    setPage: [page: number]
    nextPage: []
    prevPage: []
    updateItemsPerPage: [size: number]
    edit: [color: Color]
    delete: [color: Color]
  }>()

  const { t } = useI18n()
  const {
    hasPermission: hasEditPermission,
  } = usePermission(Permission.Color, ColorPermission.Edit)

  const {
    hasPermission: hasDeletePermission,
  } = usePermission(Permission.Color, ColorPermission.Delete)

  const tableColumns = ref<TableColumn[]>([
    { key: 'name', label: t('table.name'), cellClass: 'font-medium text-neutral-20' },
    { key: 'colorHash', label: t('table.colorHash') },
    { key: 'action', label: t('table.action') },
  ])
</script>

<style scoped>

</style>
