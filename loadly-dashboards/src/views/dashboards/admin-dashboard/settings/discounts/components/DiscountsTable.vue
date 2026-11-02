<template>
  <BaseTable
    :columns="tableColumns"
    :data="discounts"
  >
    <template #cell-statusName="{ row }">
      <BaseChip
        :label="row.statusName"
        :variant="discountStatusVariants[Number(row.status)]"
      />
    </template>

    <template #cell-action="{ row }">
      <DiscountsTableActions
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
  import type { Discount } from '@/types/admin-dashboard/response/discount/discount'
  import type { TableColumn } from '@/types/ui'
  import { useList } from '../composables/useList'
  import DiscountsTableActions from './DiscountsTableActions.vue'
  import { discountStatusVariants } from '@/utils/statusChipVariants'

  defineEmits<{
    delete: [discount: Discount]
    edit: [discount: Discount]
  }>()

  const { t } = useI18n()
  const {
    discounts,
    nextPage,
    pagination,
    prevPage,
    setPage,
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
