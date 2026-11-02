<template>
  <BaseTable
    :columns="tableColumns"
    :data="drivers"
  >
    <template #cell-contractor="{ row }">
      {{ row.contractor || row.contractorId
        ? $t('content.contractor')
        : $t('form.loadly')
      }}
    </template>

    <template #cell-licenseCategory="{ row }">
      {{
        row.licenseCategory
          ? getDisplayName('LicenseCategory', Number(row.licenseCategory))
          : $t('content.noData')
      }}
    </template>

    <template #cell-rate="{ row }">
      <div class="flex items-center gap-1">
        <IconStar class="size-4 fill-warning-300 text-warning-300" />
        <span>{{ row.rate?.toFixed(1) || 'N/A' }}</span>
      </div>
    </template>

    <template #cell-status="{ row }">
      <BaseChip
        :label="getDisplayName('DriverWorkingStatus', Number(row.workingStatus))"
        :variant="driverStatusVariants[Number(row.workingStatus)]"
      />
    </template>

    <template #cell-action="{ row }">
      <div class="flex items-center gap-2">
        <button
          type="button"
          class="btn-info-outline"
        >
          <IconEye
            visible
            class="size-4"
          />
        </button>

        <RouterLink
          v-if="hasEditPermission"
          :to="{ name: 'DriverEdit', params: { id: row.id } }"
          class="btn-action-success"
        >
          <IconEdit class="size-4" />
        </RouterLink>
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
  import { useEnum } from '@/composables/useEnum'
  import { useList } from '../composables/useList'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { Permission, Driver } from '@/types/response/permission-group'
  import { driverStatusVariants } from '@/utils/statusChipVariants'

  const { t } = useI18n()
  const { getDisplayName } = useEnum()
  const {
    drivers,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = useList()

  const {
    hasPermission: hasEditPermission,
  } = usePermission(Permission.Driver, Driver.Edit)

  const tableColumns = computed<TableColumn[]>(() => [
    { key: 'contractor', label: t('table.owner') },
    { key: 'fullName', label: t('table.driverName'), cellClass: 'font-medium text-neutral-20' },
    { key: 'phone', label: t('table.phoneNumber') },
    { key: 'licenseCategory', label: t('table.licenseCategory') },
    { key: 'rate', label: t('table.rate') },
    { key: 'status', label: t('table.status') },
    { key: 'action', label: t('table.action') },
  ])
</script>

<style scoped>

</style>
