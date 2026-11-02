<template>
  <article class="rounded-2xl border border-neutral-500 bg-white p-6">
    <header>
      <h2 class="text-base font-bold text-neutral-10">
        {{ $t('content.driverFleetOverview') }}
      </h2>

      <p class="mt-1 text-sm text-content-secondary">
        {{ $t('content.currentFleetUtilization') }}
      </p>
    </header>

    <div class="mt-2">
      <DashboardMetricProgressRow
        v-for="metric in metrics"
        :key="metric.label"
        :label="metric.label"
        :value="metric.value"
        :subtext="metric.subtext"
        :progress="metric.progress"
        :variant="metric.variant"
        :subtext-variant="metric.subtextVariant"
      />
    </div>
  </article>
</template>

<script setup lang="ts">
  import DashboardMetricProgressRow from './DashboardMetricProgressRow.vue'
  import type { DashboardFleetMetric } from '../composables/useDashboardOverview'

  defineProps<{
    metrics: DashboardFleetMetric[]
  }>()
</script>

<style scoped>

</style>
