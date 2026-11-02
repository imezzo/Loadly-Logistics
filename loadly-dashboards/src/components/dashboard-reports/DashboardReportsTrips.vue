<template>
  <section class="rounded-2xl border border-gray-50 px-4 py-6">
    <div class="mb-6 flex flex-wrap items-center gap-3">
      <h3 class="text-lg font-bold text-neutral-10">
        {{ $t('title.trips') }}
      </h3>

      <button
        type="button"
        class="ms-auto rounded-full border px-4 py-2.5 text-sm transition-colors"
        :class="filters.isLastWeek
          ? 'border-primary bg-primary/10 text-primary'
          : `
            border-gray-50 text-content-secondary
            hover:bg-gray-50
          `"
        @click="onLastWeekChange"
      >
        {{ $t('content.lastWeek') }}
      </button>

      <button
        type="button"
        class="rounded-full border px-4 py-2.5 text-sm transition-colors"
        :class="filters.isLastMonth
          ? 'border-primary bg-primary/10 text-primary'
          : `
            border-gray-50 text-content-secondary
            hover:bg-gray-50
          `"
        @click="onLastMonthChange"
      >
        {{ $t('content.lastMonth') }}
      </button>

      <BaseSelect
        :model-value="filters.year"
        :options="yearLookup"
        wrapper-class="w-36"
        class="rounded-full border-0 bg-neutral-100"
        @update:model-value="onYearChange"
      />
    </div>

    <ChartProgressBar
      v-if="rows.length"
      :rows
    />
  </section>
</template>

<script setup lang="ts">
  import { useDateLookups } from '@/composables/admin-dashboard/useDateLookups'
  import type { DashboardTripsRequest } from '@/types/admin-dashboard/request/dashboard'
  import type { ChartProgressBarRow } from '@/types/chart'

  const props = defineProps<{
    filters: DashboardTripsRequest
    rows: ChartProgressBarRow[]
  }>()

  const emit = defineEmits<{
    changeFilters: [{ isLastMonth?: boolean, isLastWeek?: boolean, year?: number }]
  }>()

  const { yearLookup } = useDateLookups()

  function onYearChange(year: string): void {
    emit('changeFilters', {
      isLastMonth: false,
      isLastWeek: false,
      year: +year,
    })
  }

  function onLastMonthChange(): void {
    emit('changeFilters', {
      isLastMonth: !props.filters.isLastMonth,
      isLastWeek: false,
      year: undefined,
    })
  }

  function onLastWeekChange(): void {
    emit('changeFilters', {
      isLastMonth: false,
      isLastWeek: !props.filters.isLastWeek,
      year: undefined,
    })
  }
</script>

<style scoped>

</style>
