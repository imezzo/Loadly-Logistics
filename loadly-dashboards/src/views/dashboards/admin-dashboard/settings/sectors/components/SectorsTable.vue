<template>
  <BaseTable
    :columns="tableColumns"
    :data="sectors"
  >
    <template #cell-action="{ row }">
      <SectorsTableActions
        @edit="$emit('edit', row)"
        @delete="$emit('delete', row)"
      />
    </template>
  </BaseTable>

  <BasePagination
    :pagination="pagination"
    @set-page="setPage"
    @next-page="nextPage"
    @prev-page="prevPage"
    @update-items-per-page="updateItemsPerPage"
  />
</template>

<script setup lang="ts">
  import type { Sector } from '@/types/admin-dashboard/response/sector/sector'
  import type { TableColumn } from '@/types/ui'
  import { useList } from '../composables/useList'
  import SectorsTableActions from './SectorsTableActions.vue'

  defineEmits<{
    delete: [sector: Sector]
    edit: [sector: Sector]
  }>()

  const { t } = useI18n()
  const {
    nextPage,
    pagination,
    prevPage,
    sectors,
    setPage,
    updateItemsPerPage,
  } = useList()

  const tableColumns = ref<TableColumn[]>([
    { key: 'name', label: t('table.name') },
    { key: 'action', label: t('table.action') },
  ])
</script>

<style scoped>

</style>
