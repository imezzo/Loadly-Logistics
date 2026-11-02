<template>
  <BaseTable
    :columns="tableColumns"
    :data="contracts"
  >
    <template #cell-referenceNumber="{ row }">
      <span class="flex max-w-45 flex-wrap">
        {{ row.referenceNumber }} - {{ row.name }}
      </span>
    </template>

    <template #cell-status="{ row }">
      <BaseChip
        :label="getDisplayName('ContractStatus', Number(row.status))"
        :variant="contractStatusVariants[Number(row.status)]"
      />
    </template>

    <template #cell-action="{ row }">
      <div class="flex items-center gap-2">
        <BaseLink
          v-if="hasViewPermission || isCompanyDashboard"
          :to="{ name: 'ViewContract', params: { id: row.id } }"
          class="btn-info-outline"
        >
          <IconInfo class="size-4" />
        </BaseLink>

        <BaseLink
          v-if="canEditContract(row.status)"
          :to="{ name: 'UpdateContract', params: { id: row.id } }"
          class="btn-action-success"
        >
          <IconEdit class="size-4" />
        </BaseLink>
      </div>
    </template>
  </BaseTable>

  <BasePagination
    v-if="contractsList?.pagination"
    :pagination="contractsList.pagination"
    @set-page="setPage"
    @next-page="nextPage"
    @prev-page="prevPage"
    @update-items-per-page="updateItemsPerPage"
  />
</template>

<script setup lang="ts">
  import type { TableColumn } from '@/types/ui'
  import { useContractList } from '../composables/useContractList'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { Permission, Contract } from '@/types/response/permission-group'
  import { useApp } from '@/composables/useApp'
  import { contractStatusVariants } from '@/utils/statusChipVariants'
  import { useEnum } from '@/composables/useEnum'

  const { t } = useI18n()
  const { getDisplayName } = useEnum()
  const { isCompanyDashboard, isAdminDashboard } = useApp()
  const {
    contracts,
    contractsList,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = useContractList()

  const { hasPermission: hasViewPermission } = usePermission(Permission.Contract, Contract.View)
  const { hasPermission: hasEditPermission } = usePermission(Permission.Contract, Contract.Edit)
  const { hasPermission: hasEditActivePermission } = usePermission(Permission.Contract, Contract.EditActive)

  function canEditContract(status: string): boolean {
    if ((!hasEditPermission.value && !hasEditActivePermission.value)
      || isCompanyDashboard
    ) return false

    if (hasEditActivePermission.value && status.toLowerCase() === 'active') return true

    return hasEditPermission.value
      && !['pending approval', 'active'].includes(status.toLowerCase())
  }

  const tableColumns = ref<TableColumn[]>([
    { key: 'referenceNumber', label: t('table.contractNumber') },
    ...(isAdminDashboard
      ? [{ key: 'companyName', label: t('table.companyName'), cellClass: 'font-medium text-neutral-20' }]
      : []),
    { key: 'contractType', label: t('table.contractType') },
    { key: 'date', label: t('table.startEndDate') },
    { key: 'status', label: t('table.status') },
    { key: 'action', label: t('table.action') },
  ])
</script>

<style scoped>

</style>

