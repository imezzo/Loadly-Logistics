<template>
  <DashboardReportsRevenue
    :months
    :trips
    :costs
    :trips-dataset="{ label: 'Total Trips' }"
    :costs-dataset="{ label: 'Total Kilometers' }"
    :tooltip-label="{ first: $t('title.trips'), second: $t('table.km') }"
    class="mb-6"
    @fetch="fetchMonthlyTripsKilometers"
  />
</template>

<script setup lang="ts">
  import { useDashboardStore } from '@/stores/company-dashboard/dashboard'
  import { useLoading } from '@/composables/useLoading'
  import type { MonthlyTripsKilometersRequest } from '@/types/company-dashboard/request/dashboard'
  import type { MonthlyTripsKilometersResponse } from '@/types/company-dashboard/response/dashboard'

  const { setLoading } = useLoading()

  const dashboardStore = useDashboardStore()

  const trips = ref<number[]>([])
  const costs = ref<number[]>([])
  const months = ref<string[]>([])

  async function fetchMonthlyTripsKilometers(filters: {month?: string, year?: string}): Promise<void> {
    setLoading(true)
    const { data } = await dashboardStore.fetchDashboardMonthlyTripsKilometers(filters as MonthlyTripsKilometersRequest)
    setLoading(false)

    months.value = data.map((item: MonthlyTripsKilometersResponse) => item.monthName)
    trips.value = data.map((item: MonthlyTripsKilometersResponse) => item.totalTrips)
    costs.value = data.map((item: MonthlyTripsKilometersResponse) => item.totalKilometers)
  }
</script>

<style scoped>

</style>
