<template>
  <DashboardReportsRevenue
    :months
    :trips
    :costs
    class="mb-6"
    @fetch="fetchMonthlyTripsCost"
  />
</template>

<script setup lang="ts">
  import { useDashboardStore } from '@/stores/admin-dashboard/dashboard'
  import { useLoading } from '@/composables/useLoading'

  interface MonthlyTripsCostRow {
    month: number
    monthName: string
    tripsCount: number
    totalCost: number
  }

  const { setLoading } = useLoading()

  const dashboardStore = useDashboardStore()

  const trips = ref<number[]>([])
  const costs = ref<number[]>([])
  const months = ref<string[]>([])

  async function fetchMonthlyTripsCost(filters: { month?: string, year?: string }): Promise<void> {
    setLoading(true)
    const { data } = await dashboardStore.fetchDashboardMonthlyTripsCost(filters)
    setLoading(false)

    months.value = data.map((item: MonthlyTripsCostRow) => item.monthName)
    trips.value = data.map((item: MonthlyTripsCostRow) => item.tripsCount)
    costs.value = data.map((item: MonthlyTripsCostRow) => item.totalCost)
  }
</script>

<style scoped>

</style>
