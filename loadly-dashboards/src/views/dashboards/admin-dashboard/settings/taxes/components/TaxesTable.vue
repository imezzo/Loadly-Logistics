<template>
  <BaseTable
    :columns="tableColumns"
    :data="taxes"
  >
    <template #cell-statusName="{ row }">
      <BaseChip
        :label="row.statusName"
        :variant="taxStatusVariants[Number(row.status)]"
      />
    </template>

    <template #cell-action="{ row }">
      <TaxesTableActions
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
  import type { Tax } from '@/types/admin-dashboard/response/tax/tax'
  import type { TableColumn } from '@/types/ui'
  import { useList } from '../composables/useList'
  import TaxesTableActions from './TaxesTableActions.vue'
  import { taxStatusVariants } from '@/utils/statusChipVariants'

  defineEmits<{
    edit: [tax: Tax]
    delete: [tax: Tax]
  }>()

  const { t } = useI18n()
  const {
    nextPage,
    pagination,
    prevPage,
    setPage,
    taxes,
    updateItemsPerPage,
  } = useList()

  const tableColumns = ref<TableColumn[]>([
    { key: 'name', label: t('table.name') },
    { key: 'typeName', label: t('table.type') },
    { key: 'value', label: t('table.value') },
    { key: 'statusName', label: t('table.status') },
    { key: 'action', label: t('table.action') },
  ])
</script>

<style scoped>

</style>
