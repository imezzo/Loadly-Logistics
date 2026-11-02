<template>
  <BaseTable
    :columns="tableColumns"
    :data="vehicleTypes"
  >
    <template #cell-category="{ row }">
      {{ getDisplayName('VehicleTypeCategory', row.category) }}
    </template>

    <template #cell-tags="{ row }">
      {{ row.tags.length
        ? row.tags.map((tag: VehicleTag) => tag.name).join(', ')
        : '-' }}
    </template>

    <template #cell-status="{ row }">
      {{ getDisplayName('AddressStatus', row.status) }}
    </template>

    <template #cell-action="{ row }">
      <VehicleTypesTableActions
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
  import type { VehicleTag } from '@/types/admin-dashboard/response/vehicle/vehicle-tag'
  import type { VehicleType } from '@/types/admin-dashboard/response/vehicle/vehicle-type'
  import type { TableColumn } from '@/types/ui'
  import { useEnum } from '@/composables/useEnum'
  import { useList } from '../composables/useList'
  import VehicleTypesTableActions from './VehicleTypesTableActions.vue'

  defineEmits<{
    delete: [vehicleType: VehicleType]
    edit: [vehicleType: VehicleType]
  }>()

  const { t } = useI18n()
  const { getDisplayName } = useEnum()
  const {
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
    vehicleTypes,
  } = useList()

  const tableColumns = ref<TableColumn[]>([
    { key: 'name', label: t('table.name') },
    { key: 'category', label: t('form.chooseVehicleCategory') },
    { key: 'tags', label: t('form.tags') },
    { key: 'status', label: t('table.status') },
    { key: 'action', label: t('table.action') },
  ])
</script>

<style scoped>

</style>
