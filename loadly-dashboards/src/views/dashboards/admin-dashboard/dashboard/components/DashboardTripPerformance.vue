<template>
  <DashboardMetricsCard
    :title="$t('content.tripPerformance')"
    :subtitle="$t('content.operationalEfficiencyMetrics')"
  >
    <DashboardMetricProgressRow
      v-for="(metric, index) in metrics"
      :key="metric.label"
      :label="metric.label"
      :value="metric.value"
      :subtext="metric.subtext"
      :progress="metric.progress"
      :subtext-variant="metric.subtextVariant"
      :show-divider="index < metrics.length - 1"
    />
  </DashboardMetricsCard>
</template>

<script setup lang="ts">
  import DashboardMetricsCard from './DashboardMetricsCard.vue'
  import DashboardMetricProgressRow from './DashboardMetricProgressRow.vue'
  import type { DashboardPerformanceMetric } from '../composables/useDashboardOverview'

  defineProps<{
    metrics: DashboardPerformanceMetric[]
  }>()
</script>

<style scoped>

</style>
