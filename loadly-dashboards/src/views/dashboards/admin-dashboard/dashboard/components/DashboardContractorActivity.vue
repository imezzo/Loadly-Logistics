<template>
  <section class="mt-6 rounded-2xl border border-neutral-500 bg-white p-6">
    <header>
      <h2 class="text-base font-bold text-neutral-10">
        {{ $t('content.contractorActivity') }}
      </h2>
      <p class="mt-1 text-sm text-content-secondary">
        {{ $t('content.realTimeOperationsMonitor') }} • {{ $t('content.lastUpdatedAt', { time: lastUpdatedAt }) }}
      </p>
    </header>

    <div
      class="
        mt-6 grid grid-cols-1 gap-4
        md:grid-cols-3
      "
    >
      <DashboardContractorActivityCard
        v-for="metric in metrics"
        :key="metric.title"
        v-bind="metric"
      />
    </div>
  </section>
</template>

<script setup lang="ts">
  import DashboardContractorActivityCard from './DashboardContractorActivityCard.vue'
  import type { DashboardContractorActivityMetric } from '../composables/useDashboardOverview'

  defineProps<{
    metrics: DashboardContractorActivityMetric[]
    lastUpdatedAt: string
  }>()
</script>

<style scoped>

</style>
