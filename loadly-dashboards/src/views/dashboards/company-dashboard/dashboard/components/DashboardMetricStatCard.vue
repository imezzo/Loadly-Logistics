<template>
  <article class="flex flex-col rounded-2xl border border-neutral-500 bg-white p-5">
    <span class="mb-3 text-3xl font-bold text-neutral-800">
      {{ value }}
    </span>

    <div class="mb-2 flex items-center gap-2">
      <slot name="icon" />
      <span class="text-sm font-medium text-content-secondary">
        {{ label }}
      </span>
    </div>

    <p
      class="text-sm font-bold"
      :class="comparison.variant === 'success'
        ? 'text-success-500'
        : comparison.variant === 'error'
          ? 'text-error-600'
          : 'text-content-secondary'"
    >
      {{
        comparison.variant === 'success'
          ? '↑'
          : comparison.variant === 'error'
            ? '↓'
            : ''
      }}
      {{ comparison.text }}
    </p>
  </article>
</template>

<script setup lang="ts">
  import type { ComparisonDisplay } from '../composables/useCompanyDashboard'

  defineProps<{
    value: string
    label: string
    comparison: ComparisonDisplay
  }>()
</script>

<style scoped>

</style>
