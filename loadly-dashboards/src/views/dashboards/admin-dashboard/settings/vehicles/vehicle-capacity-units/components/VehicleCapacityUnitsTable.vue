<template>
  <BaseTable
    :columns="tableColumns"
    :data="vehicleCapacityUnits"
  >
    <template #cell-status="{ row }">
      <span class="font-medium text-neutral-20">{{ statusLabel(row.status) }}</span>
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
  import type { VehicleCapacityUnit } from '@/types/admin-dashboard/response/vehicle/vehicle-capacity-unit'
  import type { Pagination } from '@/types/response/pagination'
  import {
    VehicleCapacityUnit as VehicleCapacityUnitPermission,
    Permission,
  } from '@/types/response/permission-group'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { useEnum } from '@/composables/useEnum'

  defineProps<{
    vehicleCapacityUnits: VehicleCapacityUnit[]
    pagination: Pagination
    isLoading: boolean
  }>()

  defineEmits<{
    setPage: [page: number]
    nextPage: []
    prevPage: []
    updateItemsPerPage: [size: number]
    edit: [vehicleCapacityUnit: VehicleCapacityUnit]
    delete: [vehicleCapacityUnit: VehicleCapacityUnit]
  }>()

  const { t } = useI18n()
  const { getDisplayName } = useEnum()
  const {
    hasPermission: hasEditPermission,
  } = usePermission(
    Permission.VehicleCapacityUnit,
    VehicleCapacityUnitPermission.Edit
  )

  const {
    hasPermission: hasDeletePermission,
  } = usePermission(
    Permission.VehicleCapacityUnit,
    VehicleCapacityUnitPermission.Delete
  )

  function statusLabel(value?: number): string {
    if (value === undefined) return ''

    return getDisplayName('AddressStatus', value)
  }

  const tableColumns = ref<TableColumn[]>([
    { key: 'name', label: t('table.name'), cellClass: 'font-medium text-neutral-20' },
    { key: 'status', label: t('table.status') },
    { key: 'action', label: t('table.action') },
  ])
</script>

<style scoped>

</style>
