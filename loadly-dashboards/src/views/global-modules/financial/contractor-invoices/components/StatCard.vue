<template>
  <article class="flex flex-1 flex-col gap-3 p-5">
    <div class="flex items-center gap-2 text-neutral-600">
      <slot name="icon" />

      <span class="text-sm font-medium">
        {{ label }}
      </span>
    </div>

    <div class="flex items-baseline gap-1.5">
      <span class="text-3xl font-bold text-neutral-800">
        {{ formattedValue }}
      </span>

      <span
        v-if="showCurrency"
        class="text-sm font-medium text-neutral-600"
      >
        {{ $t('form.egp') }}
      </span>
    </div>

    <div class="flex items-center gap-2">
      <span
        class="rounded-full px-2 py-0.5 text-xs font-semibold"
        :class="badgeClass"
      >
        {{ formattedPercent }}
      </span>

      <span class="text-sm text-neutral-600">
        {{ $t('content.vsLastMonth') }}
      </span>
    </div>
  </article>
</template>

<script setup lang="ts">
  import type { ComparisonDirection } from '@/types/company-dashboard/response/dashboard-home'

  const props = defineProps<{
    direction?: ComparisonDirection
    label?: string
    percentChange?: number
    showCurrency?: boolean
    value?: number
  }>()

  const isPositive = computed(() => props.direction === 'up')

  const formattedValue = computed(() => Number(props.value ?? 0).toLocaleString('en-US'))

  const formattedPercent = computed(() => {
    const sign = isPositive.value ? '+' : '-'

    return `${sign}${props.percentChange ?? 0}%`
  })

  const badgeClass = computed(() =>
    isPositive.value
      ? 'bg-success-50 text-success-500'
      : 'bg-error-50 text-error-600',
  )
</script>

<style scoped>

</style>
