<template>
  <article
    class="
      rounded-2xl border border-neutral-500 bg-white p-6
      xl:col-span-2
    "
  >
    <div class="mb-6 flex flex-wrap items-center gap-3">
      <h2 class="text-base font-bold text-neutral-10">
        {{ $t('content.monthlySpendingTrendEgp') }}
      </h2>

      <BaseSelect
        :model-value="year"
        :options="yearOptions"
        wrapper-class="ms-auto w-36"
        class="rounded-full border-0 bg-neutral-100"
        @update:model-value="year = $event"
      />
    </div>

    <ChartSingleBar
      v-if="months.length"
      :labels="months"
      :values="values"
    />

    <p
      v-else
      class="py-16 text-center text-sm text-content-secondary"
    >
      {{ $t('content.noData') }}
    </p>
  </article>
</template>

<script setup lang="ts">
  import ChartSingleBar from '@/components/chart/ChartSingleBar.vue'
  import { useDashboardStore } from '@/stores/company-dashboard/dashboard'
  import type { MonthlyTripsKilometersResponse } from '@/types/company-dashboard/response/dashboard'
  import type { SelectOption } from '@/types/ui'

  const props = defineProps<{
    yearOptions: SelectOption[]
    initialYear?: string
  }>()

  const dashboardStore = useDashboardStore()

  const year = ref(props.initialYear ?? String(new Date().getFullYear()))
  const months = ref<string[]>([])
  const values = ref<number[]>([])

  async function fetchMonthlyTrend(): Promise<void> {
    const { data } = await dashboardStore.fetchDashboardMonthlyTripsKilometers({
      year: Number(year.value),
    })

    const rows = Array.isArray(data) ? data as MonthlyTripsKilometersResponse[] : []

    months.value = rows.map((item) => item.monthName)
    values.value = rows.map((item) => item.totalSpending ?? item.totalKilometers)
  }

  watch(year, fetchMonthlyTrend, { immediate: true })
</script>

<style scoped>

</style>
