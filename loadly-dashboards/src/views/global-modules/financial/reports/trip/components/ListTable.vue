<template>
  <BaseTable
    :columns="tableColumns"
    :data="rows"
  >
    <template #cell-id="{ row }">
      #{{ row.id }}
    </template>

    <template #cell-plateNumber="{ row }">
      {{ row.plateLetters }} {{ row.plateNumber }}
    </template>

    <template #cell-sector="{ row }">
      {{ row.sector?.name }}
    </template>

    <template #cell-gasRevenue="{ row }">
      {{ $t('table.gasRevenueWithRate', {
        amount: row.totalFuelAmount ?? '—',
        rate: row.consumptionRate ?? '0',
      }) }}
    </template>

    <template #cell-destinationName="{ row }">
      {{ getDestinationName(row) }}
    </template>

    <template #cell-durationTime="{ row }">
      {{ getDuration(row) }}
    </template>

    <template #cell-totalBillableAmount="{ row }">
      {{ formatValueWithCurrency(row.totalBillableAmount) }}
      <span
        v-if="row.fuelPricingCalculationOption"
        class="ml-1 text-sm font-medium"
        :class="row.fuelPricingCalculationOption === fuelPricingCalculationOption.IncludedInContractPrice
          ? 'text-success-600'
          : 'text-error-600'"
      >
        {{ row.fuelPricingCalculationOption === fuelPricingCalculationOption.IncludedInContractPrice
          ? $t('table.gasIncluded')
          : $t('table.gasExcluded')
        }}
      </span>
    </template>

    <template #cell-action="{ row }">
      <RouterLink
        :to="{ name: 'ViewTrip', params: { id: row.id } }"
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
  import { useTripReportsList } from '../composables/useTripReportsList'
  import { fuelPricingCalculationOption, type Trip } from '@/types/admin-dashboard/response/trip/trip'
  import { getDurationParts } from '@/utils/time'
  import { formatValueWithCurrency } from '@/utils/currency'
  import { getDestinationName } from '@/utils/trip'

  const { t } = useI18n()
  const {
    isExternalTrip,
    nextPage,
    pagination,
    prevPage,
    rows,
    setPage,
    updateItemsPerPage,
  } = useTripReportsList()

  const tableColumns = computed<TableColumn[]>(() => [
    { key: 'id', label: t('content.tripId') },
    ...(!isExternalTrip.value
      ? [
          { key: 'companyName', label: t('table.companyName'), cellClass: 'max-w-50' },
          { key: 'contractRef', label: t('table.contract') },
        ]
      : [
          { key: 'contactPersonName', label: t('table.leadName') },
          { key: 'contactPersonMobile', label: t('table.leadMobile') },
        ]),
    { key: 'driverName', label: t('table.driverName') },
    { key: 'plateNumber', label: t('table.vehicle') },
    { key: 'vehicleType', label: t('table.vehicleType') },
    { key: 'sector', label: t('title.sectors') },
    { key: 'financialDate', label: t('form.financialDate') },
    { key: 'destinationName', label: t('table.destination'), cellClass: 'max-w-32 break-all' },
    { key: 'contractDistanceKm', label: t('table.expectedKm') },
    { key: 'actualDistanceKm', label: t('table.kmActual') },
    { key: 'gasRevenue', label: t('table.gasRevenue') },
    { key: 'durationTime', label: t('table.durationTime') },
    { key: 'totalBillableAmount', label: t('table.tripRevenue') },
    { key: 'action', label: t('table.action') },
  ])

  function getDuration(row: Trip): string {
    const startedRecording = row.recordings?.find(r => r.eventType === 4)
    const completedRecording = row.recordings?.find(r => r.eventType === 5)

    if (!startedRecording || !completedRecording) return '—'

    const startedAt = new Date(startedRecording.createdAt)
    const completedAt = new Date(completedRecording.createdAt)


    if (Number.isNaN(startedAt.getTime()) || Number.isNaN(completedAt.getTime())) return '—'

    const { months, days, hours, minutes } = getDurationParts(
      completedAt.getTime() - startedAt.getTime(),
    )

    if (!months && !days && !hours && !minutes) return '—'

    const parts: string[] = []

    if (months) parts.push(t('form.monthsDuration', { count: months }))
    if (days) parts.push(t('form.daysDuration', { count: days }))
    if (hours) parts.push(t('form.hoursDuration', { count: hours }))
    if (minutes) parts.push(t('form.minutesDuration', { count: minutes }))

    return parts.join(', ')
  }
</script>

<style scoped>

</style>
