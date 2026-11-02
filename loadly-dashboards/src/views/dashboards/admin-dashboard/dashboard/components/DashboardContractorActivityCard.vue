<template>
  <article class="rounded-2xl border border-neutral-500 bg-white p-5">
    <p class="text-sm font-medium text-content-secondary">
      {{ title }}
    </p>

    <div class="mt-3 flex flex-wrap items-end gap-2">
      <span class="text-4xl font-bold text-neutral-10">
        {{ value }}
      </span>
      <span class="mb-1 text-sm text-content-secondary">
        {{ secondaryInfo }}
      </span>
    </div>

    <div
      class="mt-2 flex items-center gap-1.5"
      :class="trendColorClass"
    >
      <IconArrow :class="trendArrowClass" />
      <span class="text-sm font-medium">
        {{ trendText }}
      </span>
    </div>

    <DashboardSparkline
      class="mt-4"
      :points="sparklinePoints"
      :variant="variant"
    />
  </article>
</template>

<script setup lang="ts">
  import DashboardSparkline from './DashboardSparkline.vue'
  import type { DashboardContractorActivityMetric } from '../composables/useDashboardOverview'

  const props = defineProps<DashboardContractorActivityMetric>()

  const trendColorClass = computed(() => {
    switch (props.variant) {
      case 'success':
        return 'text-success-500'
      case 'warning':
        return 'text-warning-300'
      case 'primary':
      default:
        return 'text-primary'
    }
  })

  const trendArrowClass = computed(() => {
    const base = 'size-3.5 shrink-0'

    if (props.trendDirection === 'down') {
      return `${base} rotate-120 fill-warning-300!`
    }

    return `${base} -rotate-60 ${props.variant === 'success' ? 'fill-success-500!' : 'fill-primary!'}`
  })
</script>

<style scoped>

</style>
