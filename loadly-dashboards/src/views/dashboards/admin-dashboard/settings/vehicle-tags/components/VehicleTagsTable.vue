<template>
  <div :aria-busy="isLoading">
    <BaseTable
      :columns="tableColumns"
      :data="vehicleTags"
    >
      <template #cell-isPricing="{ row }">
        <IconCheck v-if="row.isPricing" />
        <span v-else>-</span>
      </template>

      <template #cell-action="{ row }">
        <button
          type="button"
          class="btn-action-success"
          :aria-label="$t('a11y.edit')"
          @click="$emit('edit', row)"
        >
          <IconEdit class="size-4" />
        </button>
      </template>
    </BaseTable>

    <BasePagination
      :pagination="pagination"
      @set-page="$emit('setPage', $event)"
      @next-page="$emit('nextPage')"
      @prev-page="$emit('prevPage')"
      @update-items-per-page="$emit('updateItemsPerPage', $event)"
    />
  </div>
</template>

<script setup lang="ts">
  import type { TableColumn } from '@/types/ui'
  import type { VehicleTag } from '@/types/admin-dashboard/response/vehicle/vehicle-tag'
  import type { Pagination } from '@/types/response/pagination'

  defineProps<{
    vehicleTags: VehicleTag[]
    pagination: Pagination
    isLoading: boolean
  }>()

  defineEmits<{
    setPage: [page: number]
    nextPage: []
    prevPage: []
    updateItemsPerPage: [size: number]
    edit: [vehicleTag: VehicleTag]
  }>()

  const { t } = useI18n()

  const tableColumns = computed<TableColumn[]>(() => [
    { key: 'name', label: t('table.vehicleTagName'), cellClass: 'font-medium text-neutral-20' },
    { key: 'isPricing', label: t('table.pricing') },
    { key: 'action', label: t('table.action') },
  ])
</script>

<style scoped>

</style>
