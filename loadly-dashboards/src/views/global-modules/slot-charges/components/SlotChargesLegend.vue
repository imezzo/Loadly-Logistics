<template>
  <div class="flex flex-wrap items-center gap-x-6 gap-y-2">
    <span
      v-for="item in legendItems"
      :key="item.label"
      class="flex items-center gap-2 text-sm"
    >
      <span
        class="size-3 shrink-0 rounded-sm border"
        :class="item.indicatorClass"
      />

      <span :class="item.labelClass">
        {{ item.label }}
      </span>
    </span>
  </div>
</template>

<script setup lang="ts">
  const { t } = useI18n()

  interface LegendItem {
    label: string
    indicatorClass: string
    labelClass: string
  }

  const legendItems = computed<LegendItem[]>(() => [
    {
      label: t('content.contractSlotsStatusLegendExtra'),
      indicatorClass: 'border-info-300 bg-info-50',
      labelClass: 'text-info-500',
    },
    {
      label: t('content.contractSlotsStatusLegendWorking'),
      indicatorClass: 'border-success-300 bg-success-50',
      labelClass: 'text-success-600',
    },
    {
      label: t('content.contractSlotsStatusLegendSkipped'),
      indicatorClass: 'border-error-300 bg-error-100',
      labelClass: 'text-error-600',
    },
    {
      label: t('content.contractSlotsStatusLegendNotActive'),
      indicatorClass: 'border-neutral-400 bg-neutral-200',
      labelClass: 'text-neutral-700',
    },
    {
      label: t('content.contractSlotsStatusLegendUnknown'),
      indicatorClass: 'border-warning-300 bg-warning-50',
      labelClass: 'text-warning-600',
    },
  ])
</script>

<style scoped>

</style>
