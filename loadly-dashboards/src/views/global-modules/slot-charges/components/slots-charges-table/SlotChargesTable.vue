<template>
  <div class="max-h-150 overflow-x-auto bg-white">
    <table class="relative w-full min-w-max">
      <thead class="sticky top-0 z-10 bg-slate-100">
        <tr>
          <th
            v-for="col in columns"
            :key="col.key"
            :class="col.class"
            class="border-x border-neutral-500"
          >
            {{ col.label }}
          </th>
        </tr>
      </thead>

      <tbody>
        <tr
          v-for="(row, rowIndex) in data"
          :key="rowIndex"
          class="
            border-b border-neutral-500 transition-colors
            last:border-b-0
            hover:bg-neutral-30/50
          "
        >
          <td
            v-for="col in columns"
            :key="col.key"
            class="
              border border-neutral-500 p-1 align-top text-sm
              hover:bg-info-500/10
            "
            :class="[
              col.cellClass,
              row[(col.key as `day_${string}`)]?.disposition === ContractSlotDayDisposition.NotActive
                ? 'bg-neutral-20/25'
                : isDayClickable(row[(col.key as `day_${string}`)])
                  ? 'cursor-pointer'
                  : ''
            ]"
            @click="onCellClick(row, col.key)"
          >
            <template v-if="col.key === 'vehicleLabel'">
              {{ row.vehicleLabel }}
            </template>

            <SlotChargesCell
              v-else
              :day="row[(col.key as `day_${string}`)]"
            />
          </td>
        </tr>

        <tr v-if="!data.length">
          <td
            class="px-4 py-6 text-center text-sm text-content-secondary"
            :colspan="columns.length || 1"
          >
            {{ $t('content.noData') }}
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup lang="ts">
  import SlotChargesCell from './SlotChargesCell.vue'
  import type { ContractSlotsTableRow } from '../../composables/useSlotsCharges.ts'
  import type { TableColumn } from '@/types/ui'
  import type { SlotCalendarDay } from '@/types/admin-dashboard/response/contract/included-slot-charges-calculate.ts'
  import { ContractSlotDayDisposition } from '@/types/admin-dashboard/response/contract/monthly-contract/monthly-contract.enums'

  const props = defineProps<{
    columns: TableColumn[]
    data: ContractSlotsTableRow[]
    hasClickableDays?: boolean
  }>()

  const emit = defineEmits<{
    rowAction: [payload: {
      day: SlotCalendarDay
      vehicleLabel: string
    }]
  }>()

  function isDayClickable(day: SlotCalendarDay | null | undefined): day is SlotCalendarDay {
    return Boolean(
      props.hasClickableDays
      && day
      && !day.tripCount
      && day.disposition !== ContractSlotDayDisposition.NotActive,
    )
  }

  function onCellClick(row: ContractSlotsTableRow, key: string): void {
    const day = row[key as `day_${string}`]

    if (!isDayClickable(day)) return

    emit('rowAction', {
      day,
      vehicleLabel: row.vehicleLabel,
    })
  }
</script>

<style scoped>

</style>
