<template>
  <BaseTable
    :columns="tableColumns"
    :data="vehicleInsurances"
  >
    <template #cell-startDate="{ row }">
      {{ formatDate(row.startDate) }}
    </template>

    <template #cell-endDate="{ row }">
      {{ formatDate(row.endDate) }}
    </template>

    <template #cell-amount="{ row }">
      {{ formatAmount(row.amount) }}
    </template>

    <template #cell-status="{ row }">
      {{ statusLabel(row.status) }}
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
  import type { VehicleInsurance } from '@/types/admin-dashboard/response/vehicle/vehicle-insurance'
  import type { Pagination } from '@/types/response/pagination'
  import {
    VehicleInsurance as VehicleInsurancePermission,
    Permission,
  } from '@/types/response/permission-group'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { useEnum } from '@/composables/useEnum'

  defineProps<{
    vehicleInsurances: VehicleInsurance[]
    pagination: Pagination
    isLoading: boolean
  }>()

  defineEmits<{
    setPage: [page: number]
    nextPage: []
    prevPage: []
    updateItemsPerPage: [size: number]
    edit: [vehicleInsurance: VehicleInsurance]
    delete: [vehicleInsurance: VehicleInsurance]
  }>()

  const { t } = useI18n()
  const { getDisplayName } = useEnum()
  const {
    hasPermission: hasEditPermission,
  } = usePermission(
    Permission.VehicleInsurance,
    VehicleInsurancePermission.Edit
  )

  const {
    hasPermission: hasDeletePermission,
  } = usePermission(
    Permission.VehicleInsurance,
    VehicleInsurancePermission.Delete
  )

  function statusLabel(value?: number): string {
    if (value === undefined) return ''

    return getDisplayName('AddressStatus', value)
  }

  function formatDate(value: string): string {
    if (!value) return ''

    try {
      const date = new Date(value)

      return date.toLocaleDateString()
    } catch {
      return value
    }
  }

  function formatAmount(value: number): string {
    return new Intl.NumberFormat(undefined, {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    }).format(value)
  }

  const tableColumns = ref<TableColumn[]>([
    { key: 'policyNo', label: t('form.policyNo'), cellClass: 'font-medium text-neutral-20' },
    { key: 'insuranceProvider', label: t('form.insuranceProvider') },
    { key: 'startDate', label: t('form.startDate') },
    { key: 'endDate', label: t('form.endDate') },
    { key: 'amount', label: t('form.amount') },
    { key: 'status', label: t('table.status') },
    { key: 'action', label: t('table.action') },
  ])
</script>

<style scoped>

</style>
