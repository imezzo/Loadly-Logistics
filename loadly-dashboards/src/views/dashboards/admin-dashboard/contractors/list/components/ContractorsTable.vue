<template>
  <BaseTable
    :columns="tableColumns"
    :data="contractors"
  >
    <template #cell-contractorStatus="{ row }">
      <BaseChip
        :label="getDisplayName('ContractorStatus', Number(row.contractorStatus))"
        :variant="contractorStatusVariants[Number(row.contractorStatus)]"
      />
    </template>

    <template #cell-status="{ row }">
      <BaseChip
        :label="getDisplayName('UserStatus', Number(row.status))"
        :variant="userStatusVariants[Number(row.status)]"
      />
    </template>

    <template #cell-action="{ row }">
      <div class="flex items-center gap-2">
        <BaseLink
          v-if="hasViewPermission"
          :to="{ name: 'ViewContractor', params: { id: row.id } }"
          class="btn-info-outline"
        >
          <IconEye
            visible
            class="size-4"
          />
        </BaseLink>

        <BaseLink
          v-if="hasEditPermission"
          :to="{ name: 'EditContractor', params: { id: row.id } }"
          class="btn-action-success"
        >
          <IconEdit class="size-4" />
        </BaseLink>
      </div>
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
  import type { TableColumn } from '@/types/ui'
  import { useList } from '../composables/useList'
  import { useEnum } from '@/composables/useEnum'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { Contractor, Permission } from '@/types/response/permission-group'
  import { contractorStatusVariants, userStatusVariants } from '@/utils/statusChipVariants'

  const { t } = useI18n()
  const { getDisplayName } = useEnum()
  const {
    contractors,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = useList()
  const { hasPermission: hasViewPermission } = usePermission(Permission.Contractor, Contractor.View)
  const { hasPermission: hasEditPermission } = usePermission(Permission.Contractor, Contractor.Edit)

  const tableColumns = computed<TableColumn[]>(() => [
    { key: 'fullName', label: t('table.name'), cellClass: 'font-medium text-neutral-20' },
    { key: 'phone', label: t('table.phoneNumber') },
    { key: 'totalVehicles', label: t('table.vehicles') },
    { key: 'totalDrivers', label: t('table.drivers') },
    { key: 'totalTrips', label: t('table.totalTrips') },
    { key: 'contractorStatus', label: t('table.contractorStatus') },
    { key: 'status', label: t('table.status') },
    { key: 'action', label: t('table.action') },
  ])
</script>

<style scoped>

</style>
