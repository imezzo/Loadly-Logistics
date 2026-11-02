<template>
  <section class="mb-6 overflow-hidden rounded-xl border border-neutral-150 bg-neutral-150">
    <dl
      class="
        grid grid-cols-1 gap-px
        sm:grid-cols-2
        lg:grid-cols-3
      "
    >
      <div
        v-for="item in items"
        :key="item.key"
        class="bg-neutral-25 px-4 py-3"
      >
        <dt class="flex items-center gap-1 text-sm font-bold text-neutral-800">
          {{ item.label }}

          <BaseTooltip
            v-if="item.hint"
            :trigger-label="item.label"
          >
            <template #trigger>
              <IconInfo class="size-4 text-info-500" />
            </template>

            {{ item.hint }}
          </BaseTooltip>
        </dt>

        <div
          v-if="item.children"
          class="
            mt-3 grid grid-cols-1 gap-px
            sm:grid-cols-2
          "
        >
          <div
            v-for="child in item.children"
            :key="child.key"
          >
            <dt class="text-sm font-semibold text-content-secondary">
              {{ child.label }}
            </dt>

            <dd class="mt-1 text-sm text-content-secondary">
              {{ formatAmount(child.value ?? 0, child.withCurrency) }}
            </dd>
          </div>
        </div>

        <dd
          v-else
          class="mt-1 text-base font-bold text-neutral-800"
        >
          {{ formatAmount(item.value ?? 0) }}
        </dd>
      </div>
    </dl>
  </section>
</template>

<script setup lang="ts">
  import type { MonthlyContractKmPool } from '@/types/admin-dashboard/response/contract/monthly-contract/monthly-contract'
  import type { CalculateSlotChargesResponse } from '@/types/admin-dashboard/response/contract/included-slot-charges-calculate'
  import { ConsumptionType } from '@/types/request/contract/consumption-types.enums'

  interface SummaryItem {
    children?: SummaryItem[]
    hint?: string
    key: string
    label: string
    value?: number
    withCurrency?: boolean
  }

  const props = defineProps<{
    charges: CalculateSlotChargesResponse
    consumptionType?: number | null
    kmPool?: MonthlyContractKmPool | null
  }>()

  const { t } = useI18n()

  const items: ComputedRef<SummaryItem[]> = computed(() => [
    {
      key: 'totalAmount',
      hint: t('content.totalAmountHint'),
      label: t('form.total'),
      value: props.charges.totalAmount,
    },
    {
      key: 'totalWorkingDaysAmount',
      label: t('content.totalWorkingDays'),
      value: props.charges.totalWorkingDaysAmount,
      children: [
        {
          key: 'totalWorkingDaysAmount',
          label: t('content.amount'),
          value: (props.charges.totalWorkingDaysAmount),
          withCurrency: true,
        },
        {
          key: 'totalWorkingDaysCount',
          label: t('content.count'),
          value: props.charges.totalWorkingDaysCount,
        },
      ],
    },
    {
      key: 'totalExtraWorkingDaysAmount',
      label: t('content.totalExtraWorkingDays'),
      value: props.charges.totalExtraWorkingDaysAmount,
      children: [
        {
          key: 'totalExtraWorkingDaysAmount',
          label: t('content.amount'),
          value: props.charges.totalExtraWorkingDaysAmount,
          withCurrency: true,
        },
        {
          key: 'totalExtraWorkingDaysCount',
          label: t('content.count'),
          value: props.charges.totalExtraWorkingDaysCount,
        },
      ],
    },
    ...(Number(props.consumptionType) === ConsumptionType.Kilometers ? [
      {
        key: 'totalExtraKmsAmount',
        label: t('content.totalExtraKms'),
        value: props.charges.totalExtraKmsAmount,
        children: [
          {
            key: 'totalExtraKmsAmount',
            label: t('content.amount'),
            value: props.charges.totalExtraKmsAmount,
            withCurrency: true,
          },
          {
            key: 'totalExtraKmsCount',
            label: t('content.count'),
            value: props.charges.totalExtraKmsCount,
          },
        ],
      }] : []),
    ...(Number(props.consumptionType) === ConsumptionType.Trips ? [
      {
        key: 'totalExtraTripsAmount',
        label: t('content.totalExtraTrips'),
        value: props.charges.totalExtraTripsAmount,
        children: [
          {
            key: 'totalExtraTripsAmount',
            label: t('content.amount'),
            value: props.charges.totalExtraTripsAmount,
            withCurrency: true,
          },
          {
            key: 'totalExtraTripsCount',
            label: t('content.count'),
            value: props.charges.totalExtraTripsCount,
          },
        ],
      }] : []),
    {
      key: 'totalTripsCount',
      label: t('content.totalTrips'),
      value: props.charges.totalTripsCount,
      children: [
        {
          key: 'totalTripsAmount',
          label: t('content.totalDaysWithTripsCount'),
          value: props.charges.totalDaysWithTripsCount,
        },
        {
          key: 'totalTripsCount',
          label: t('content.totalTripsCount'),
          value: props.charges.totalTripsCount,
        },
      ],
    },
    ...(Number(props.consumptionType) === ConsumptionType.Trips
      ? [{
        key: 'totalFuelAmount',
        label: t('content.totalFuel'),
        value: props.charges.totalFuelAmount,
        children: [
          {
            key: 'totalFuelAmount',
            label: t('content.amount'),
            value: props.charges.totalFuelAmount,
            withCurrency: true,
          },
          {
            key: 'totalFuelKms',
            label: t('content.totalFuelKms'),
            value: props.charges.totalFuelKms,
          },
        ],
      }]
      : []),
    ...(Number(props.consumptionType) === ConsumptionType.Kilometers
      ? [{
        key: 'kmPool',
        label: t('content.kmPool'),
        value: props.kmPool?.totalConsumption,
        children: [
          {
            key: 'totalMaxKms',
            label: t('content.totalMaxKms'),
            value: props.kmPool?.totalMaxKms,
          },
          {
            key: 'totalConsumption',
            label: t('content.totalConsumption'),
            value: props.kmPool?.totalConsumption,
          },
        ],
      }]
      : []),
    {
      key: 'totalWaitingForLoadingDaysCount',
      label: t('content.totalWaitingForLoadingDays'),
      value: props.charges.totalWaitingForLoadingDaysCount,
      children: [
        {
          key: 'totalWaitingForLoadingWorkingDaysCount',
          label: t('content.count'),
          value: props.charges.totalWaitingForLoadingWorkingDaysCount,
        },
        {
          key: 'totalWaitingForLoadingAmount',
          label: t('content.amount'),
          value: props.charges.totalWaitingForLoadingAmount,
          withCurrency: true,
        },
      ],
    },
    {
      key: 'totalOvernightTripsCount',
      label: t('content.totalOvernightTrips'),
      value: props.charges.totalOvernightTripsCount,
      children: [
        {
          key: 'totalOvernightCustomerAmount',
          label: t('content.totalOvernightCustomerAmount'),
          value: props.charges.totalOvernightCustomerAmount,
          withCurrency: true,
        },
        {
          key: 'totalOvernightLoadlyAmount',
          label: t('content.totalOvernightLoadlyAmount'),
          value: props.charges.totalOvernightLoadlyAmount,
          withCurrency: true,
        },
      ],
    },
  ])

  function formatAmount(value: number, withCurrency = false): string {
    const formattedValue = Number(value ?? 0).toLocaleString('en-US')

    return withCurrency
      ? `${formattedValue} ${t('form.egp')}`
      : formattedValue
  }
</script>

<style scoped>

</style>
