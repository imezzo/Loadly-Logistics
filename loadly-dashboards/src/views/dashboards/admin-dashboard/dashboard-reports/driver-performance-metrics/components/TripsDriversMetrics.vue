<template>
  <section class="mt-8 rounded-2xl border border-gray-50 px-4 py-6">
    <div class="mb-6 flex flex-wrap items-center gap-3">
      <h3 class="text-lg font-bold text-neutral-10">
        {{ t('title.tripsAndDriversMetrics') }}
      </h3>

      <div
        class="
          ms-auto flex flex-wrap items-center gap-2
          sm:gap-3
        "
      >
        <BaseSelect
          v-model="year"
          :options="yearLookup"
          wrapper-class="min-w-28"
          class="rounded-full"
        />
      </div>
    </div>

    <div class="mb-4 flex flex-wrap gap-5">
      <span class="flex items-center gap-1.5 text-sm text-neutral-10">
        <span class="size-3 shrink-0 rounded-sm bg-primary" />
        {{ t('title.trips') }}
      </span>

      <span class="flex items-center gap-1.5 text-sm text-neutral-10">
        <span class="size-3 shrink-0 rounded-sm bg-green-600" />
        {{ t('sidebar.drivers') }}
      </span>
    </div>

    <ChartBar
      v-if="labels.length"
      :months="labels"
      :trips="trips"
      :costs="drivers"
      :trips-dataset="{ label: t('title.trips'), backgroundColor: '#0F766E' }"
      :costs-dataset="{ label: t('sidebar.drivers'), backgroundColor: '#16a34a' }"
    />
  </section>
</template>

<script setup lang="ts">
  import ChartBar from '@/components/chart/ChartBar.vue'
  import { useDateLookups } from '@/composables/admin-dashboard/useDateLookups'
  import { useDashboardStore } from '@/stores/admin-dashboard/dashboard'
  import { useLoading } from '@/composables/useLoading'

  interface MonthlyTripsDriversRow {
    availableDriversCount: number
    month: number
    tripsCount: number
  }

  const { t } = useI18n()
  const { yearLookup, months } = useDateLookups()
  const dashboardStore = useDashboardStore()
  const { setLoading } = useLoading()

  const year = ref(String(new Date().getFullYear()))

  const labels = ref<string[]>([])
  const trips = ref<number[]>([])
  const drivers = ref<number[]>([])

  async function fetchMetrics(): Promise<void> {
    setLoading(true)
    const { success, data } = await dashboardStore.fetchMonthlyTripsAvailableDrivers({ year: Number(year.value) })
    setLoading(false)

    const rows = Array.isArray(data) ? data as MonthlyTripsDriversRow[] : []

    if (!success || !rows.length) {
      labels.value = []
      trips.value = []
      drivers.value = []

      return
    }

    labels.value = rows.map((row) => {
      const name = months[row.month - 1]

      return name ? name.slice(0, 3) : String(row.month)
    })
    trips.value = rows.map(row => row.tripsCount)
    drivers.value = rows.map(row => row.availableDriversCount)
  }

  watch([year], () => void fetchMetrics(), { immediate: true })
</script>

<style scoped>

</style>
