<template>
  <section
    class="
      grid grid-cols-1 gap-4
      md:grid-cols-2
      lg:grid-cols-3
    "
  >
    <div
      v-for="total in tripsCostTotals"
      :key="total.label"
      class="rounded-2xl border border-neutral-500 p-4 text-center"
    >
      <span class="text-2xl font-extrabold text-neutral-800">{{ total.value }}</span>
      <p class="mt-1.5 font-medium text-neutral-10">
        {{ total.label }}
      </p>
    </div>
  </section>
</template>

<script setup lang="ts">
  import { useDashboardStore } from '@/stores/company-dashboard/dashboard'

  const dashboardStore = useDashboardStore()

  const tripsCostTotals = ref({
    totalCostLastSevenDays: {
      label: 'Cost last 7 days',
      value: 0,
    },
    totalCostLastThirtyDays: {
      label: 'Cost last 30 days',
      value: 0,
    },
    totalCostLastThreeMonths: {
      label: 'Cost last 3 months',
      value: 0,
    },
  })

  onMounted(async () => {
    const { data } = await dashboardStore.fetchDashboardTripsCostTotals()
    tripsCostTotals.value.totalCostLastSevenDays.value = data.totalCostLastSevenDays
    tripsCostTotals.value.totalCostLastThirtyDays.value = data.totalCostLastThirtyDays
    tripsCostTotals.value.totalCostLastThreeMonths.value = data.totalCostLastThreeMonths
  })
</script>

<style scoped>

</style>
