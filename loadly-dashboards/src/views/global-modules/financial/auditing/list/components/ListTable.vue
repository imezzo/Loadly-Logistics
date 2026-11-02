<template>
  <BaseTable
    :columns="tableColumns"
    :data="rows"
  >
    <template #cell-plateNumber="{ row }">
      {{ row.plateLetters }} {{ row.plateNumber }}
    </template>

    <template #cell-sector="{ row }">
      {{ row.sector?.name }}
    </template>

    <template #cell-tollGatesCompanyShare="{ row }">
      <span class="text-success-600">
        {{ formatValueWithCurrency(getTollGatesCompanyShare(row)) }}
      </span>
    </template>

    <template #cell-tollGatesCustomerShare="{ row }">
      <span class="text-success-600">
        {{ formatValueWithCurrency(getTollGatesCustomerShare(row)) }}
      </span>
    </template>

    <template #cell-tripFrom="{ row }">
      {{ getTripPickupAddress(row) }}
    </template>

    <template #cell-tripTo="{ row }">
      {{ getTripDropOffAddress(row) }}
    </template>

    <template #cell-tripDocumentStatus="{ row }">
      <BaseChip
        v-if="row.tripDocumentStatusName"
        :label="row.tripDocumentStatusName"
        :variant="tripDocumentStatusVariants[Number(row.tripDocumentStatus)] ?? 'neutral'"
      />
      <span v-else>—</span>
    </template>

    <template #cell-action="{ row }">
      <RouterLink
        :to="{ name: 'FinancialTripAuditingForm', params: { id: row.id } }"
        class="
          flex items-center justify-center gap-2 rounded-lg border border-neutral-300 bg-white p-2
        "
      >
        <IconEye
          visible
          class="size-4 text-neutral-800"
        />
        <span class="btn-link">
          {{ t('content.tripDetailsLink') }}
        </span>
      </RouterLink>
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
  import { useTripAuditingList } from '../composables/useTripAuditingList'
  import type { Trip } from '@/types/admin-dashboard/response/trip/trip'
  import { tripDocumentStatusVariants } from '@/utils/statusChipVariants'
  import { formatValueWithCurrency } from '@/utils/currency'
  import { getTripDropOffAddress, getTripPickupAddress } from '@/utils/trip'

  const { t } = useI18n()
  const {
    isExternalTrip,
    nextPage,
    pagination,
    prevPage,
    rows,
    setPage,
    updateItemsPerPage,
  } = useTripAuditingList()

  const tableColumns = computed<TableColumn[]>(() => [
    { key: 'referenceNumber', label: t('content.tripId') },
    ...(!isExternalTrip.value
      ? [{ key: 'companyName', label: t('content.company'), cellClass: 'max-w-50' }]
      : [{ key: 'contactPersonName', label: t('table.leadName'), cellClass: 'max-w-50' }]),
    { key: 'financialDate', label: t('form.financialDate'), cellClass: 'flex flex-col items-start gap-1' },
    { key: 'driverName', label: t('table.driverName') },
    { key: 'plateNumber', label: t('table.plateNumber') },
    { key: 'vehicleType', label: t('table.vehicleType') },
    { key: 'sector', label: t('title.sectors') },
    { key: 'proofOfDelivery', label: t('table.pod') },
    { key: 'tollGatesCompanyShare', label: t('table.tollGatesCompany') },
    { key: 'tollGatesCustomerShare', label: t('table.tollGatesCustomer') },
    { key: 'tripFrom', label: t('table.addressFrom'), cellClass: 'max-w-50 wrap-text break-words' },
    { key: 'tripTo', label: t('table.addressTo'), cellClass: 'max-w-50 wrap-text break-words' },
    { key: 'tripDocumentStatus', label: t('content.tripDocumentStatus') },
    { key: 'action', label: t('table.action') },
  ])

  function getTollGatesCompanyShare(row: Trip): number {
    const tollGatesCharge = row.tripCharges?.find(charge => charge.chargeType === 10)

    return tollGatesCharge?.companyShare ?? 0
  }

  function getTollGatesCustomerShare(row: Trip): number {
    const tollGatesCharge = row.tripCharges?.find(charge => charge.chargeType === 10)

    return (tollGatesCharge?.amount ?? 0) - (tollGatesCharge?.companyShare ?? 0)
  }
</script>

<style scoped>

</style>
