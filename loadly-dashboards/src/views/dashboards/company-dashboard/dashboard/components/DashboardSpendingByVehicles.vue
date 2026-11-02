<template>
  <article class="rounded-2xl border border-neutral-500 bg-white p-6">
    <header>
      <h2 class="text-base font-bold text-neutral-10">
        {{ $t('content.spendingByVehicles') }}
      </h2>
      <p class="mt-1 text-sm text-content-secondary">
        {{ $t('content.spendingByVehiclesSubtitle') }}
      </p>
    </header>

    <div class="mt-2">
      <DashboardVehicleSpendingRow
        v-for="(item, index) in items"
        :key="item.vehicleTypeId"
        :label="item.name"
        :progress="item.sharePercent"
        :value="`${item.sharePercent}%`"
        :subtext="$t('content.vsLastWeekDelta', { value: formatWeekDelta(item.vsLastWeek) })"
        :show-divider="index < items.length - 1"
      />
    </div>
  </article>
</template>

<script setup lang="ts">
  import DashboardVehicleSpendingRow from './DashboardVehicleSpendingRow.vue'
  import type { ComparisonMetric, SpendingByVehicleType } from '@/types/company-dashboard/response/dashboard-home'

  defineProps<{
    items: SpendingByVehicleType[]
  }>()

  function formatWeekDelta(metric: ComparisonMetric): string {
    if (metric.direction === 'flat') return '0'

    const sign = metric.direction === 'up' ? '+' : '-'

    return `${sign}${Math.abs(metric.percentChange)}`
  }
</script>

<style scoped>

</style>
