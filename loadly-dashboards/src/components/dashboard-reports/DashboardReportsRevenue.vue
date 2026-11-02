<template>
  <section class="mt-4 rounded-2xl border border-gray-50 px-4 py-6">
    <div class="mb-6 flex flex-wrap items-center gap-3">
      <h3 class="text-lg font-bold text-neutral-10">
        {{ t('title.revenueMetrics') }}
      </h3>

      <BaseSelect
        :model-value="filters.month"
        :options="monthLookup"
        wrapper-class="ms-auto min-w-44"
        class="rounded-full"
        @update:model-value="filters.month = $event"
      />

      <BaseSelect
        :model-value="filters.year"
        :options="yearLookup"
        wrapper-class="min-w-32"
        class="rounded-full"
        @update:model-value="filters.year = $event"
      />
    </div>

    <div class="mb-3 flex gap-5">
      <span class="flex items-center gap-1.5 text-sm text-neutral-10">
        <span class="size-3 shrink-0 rounded-sm bg-primary" />
        {{ tripsDataset?.label || t('title.trips') }}
      </span>

      <span class="flex items-center gap-1.5 text-sm text-neutral-10">
        <span class="size-3 shrink-0 rounded-sm bg-blue-200" />
        {{ costsDataset?.label || t('title.cost') }}
      </span>
    </div>

    <ChartBar
      :months
      :trips
      :costs
      :trips-dataset
      :costs-dataset
      :tooltip-label
    />
  </section>
</template>

<script setup lang="ts">
  import { useDateLookups } from '@/composables/admin-dashboard/useDateLookups'

  defineProps<{
    months: string[]
    trips: number[]
    costs: number[]
    tripsDataset?: {
      label?: string
      data?: number[]
      backgroundColor?: string
    }
    costsDataset?: {
      label?: string
      data?: number[]
      backgroundColor?: string
    }
    tooltipLabel?: {
      first?: string
      second?: string
    }
  }>()

  const emit = defineEmits<{
    fetch: [{ month?: string, year?: string }]
  }>()

  const { t } = useI18n()
  const { yearLookup, monthLookup } = useDateLookups()

  const filters = ref<{ month?: string, year?: string }>({
    month: '',
    year: new Date().getFullYear().toString(),
  })

  watch(
    [() => filters.value.year, () => filters.value.month],
    () => emit('fetch', filters.value),
    { immediate: true }
  )
</script>

<style scoped>

</style>
