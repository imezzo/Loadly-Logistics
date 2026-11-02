<template>
  <div
    v-if="day"
    class="
      relative mb-auto flex h-full! min-w-28 flex-col items-start justify-start gap-0.75 rounded-lg
      text-xs
    "
  >
    <span
      v-if="isEdited"
      class="
        absolute -end-1 -top-1 z-1 flex size-6 items-center justify-center rounded-full
        bg-warning-500 text-white
      "
      :title="$t('content.edited')"
    >
      <IconPencil class="size-3.5" />
    </span>

    <span
      v-if="day.tripCount"
      :class="cellClass + ' items-center gap-1 bg-info-50 text-info-500'"
    >
      {{ $t('content.tripCountLabel', { count: day.tripCount }) }}

      <SlotChargesTripsTooltip :trips="day.trips" />
    </span>

    <span
      v-if="day.workingDayIndex"
      :class="cellClass + ' bg-success-100 text-success-500'"
    >
      {{ $t('content.workingDayIndexLabel', { index: day.workingDayIndex }) }}
    </span>

    <span
      v-if="day.amount"
      :class="cellClass + ' bg-warning-500/10 text-warning-700'"
    >
      {{ day.amount.toLocaleString() }}
    </span>

    <span
      v-if="facilityLabel"
      class="flex-wrap border border-dashed"
      :class="[
        cellClass,
        facilityVariants[day.facilityType ?? VehicleFacilityType.Garage]
      ]"
    >
      {{ facilityLabel }}
    </span>
  </div>

  <span
    v-else
    class="text-content-secondary"
  >
    -
  </span>
</template>

<script setup lang="ts">
  import SlotChargesTripsTooltip from './SlotChargesTripsTooltip.vue'
  import { useEnum } from '@/composables/useEnum'
  import { ContractSlotDayDispositionSource } from '@/types/admin-dashboard/response/contract/monthly-contract/monthly-contract.enums'
  import type { SlotCalendarDay } from '@/types/admin-dashboard/response/contract/included-slot-charges-calculate'
  import { VehicleFacilityType } from '@/types/admin-dashboard/response/vehicle-facility.enums'

  const props = defineProps<{
    day?: SlotCalendarDay | null
  }>()

  const { getDisplayName } = useEnum()

  const isEdited = computed(() =>
    props.day?.dispositionSource === ContractSlotDayDispositionSource.Manual,
  )

  const facilityLabel = computed(() => {
    const typeName = props.day?.facilityType
      ? getDisplayName('VehicleFacilityType', props.day.facilityType)
      : ''

    return [typeName, props.day?.facilityName].filter(Boolean).join(' - ')
  })

  const cellClass = 'flex! w-full justify-center rounded-md px-1.25! py-0.75! text-xs font-semibold'

  const facilityVariants: Record<VehicleFacilityType, string> = {
    [VehicleFacilityType.Garage]: 'bg-rose-50 text-rose-500',
    [VehicleFacilityType.Agency]: 'bg-cyan-50 text-cyan-500',
    [VehicleFacilityType.Workshop]: 'bg-emerald-50 text-emerald-500',
    [VehicleFacilityType.Parking]: 'bg-indigo-50 text-indigo-500',
  }
</script>

<style scoped>

</style>
