<template>
  <span
    class="inline-flex items-center rounded-lg border px-3 py-1.75 text-sm"
    :class="variantClasses[variant ?? 'neutral']"
  >
    <slot>{{ label }}</slot>
  </span>
</template>

<script setup lang="ts">
  import type { ChipVariant } from '@/types/ui'

  defineProps<{
    label?: string
    variant?: ChipVariant
  }>()

  const variantClasses: Record<ChipVariant, string> = {
    active:   'border-success-500 bg-success-500 text-white',
    amber:    'border-amber-300 bg-amber-100 text-amber-600',
    disabled: 'border-neutral-500 bg-neutral-400 text-neutral-900 opacity-50',
    error:    'border-error-300 bg-error-100 text-error-500',
    info:     'border-info-300 bg-info-50 text-info-500',
    indigo:   'border-indigo-300 bg-indigo-100 text-indigo-600',
    lime:     'border-lime-600 bg-lime-50 text-lime-600',
    neutral:  'border-neutral-500 bg-neutral-400 text-neutral-900',
    purple:   'border-purple-300 bg-purple-100 text-purple-600',
    success:  'border-success-300 bg-success-100 text-success-600',
    warning:  'border-warning-500 bg-warning-50 text-warning-500',
  }
</script>

<style scoped>

</style>

