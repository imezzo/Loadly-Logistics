<template>
  <DashboardReportsTrips
    :filters="filters"
    :rows="rows"
    @change-filters="filters = $event"
  />
</template>

<script setup lang="ts">
  import { useDashboardStore } from '@/stores/admin-dashboard/dashboard'
  import { useLoading } from '@/composables/useLoading'
  import type { DashboardTripsRequest } from '@/types/admin-dashboard/request/dashboard'
  import type { ChartProgressBarRow } from '@/types/chart'
  import DashboardReportsTrips from '@/components/dashboard-reports/DashboardReportsTrips.vue'

  const { t } = useI18n()
  const { setLoading } = useLoading()
  const dashboardStore = useDashboardStore()

  const rows = ref<ChartProgressBarRow[]>([])
  const filters = ref<DashboardTripsRequest>({
    isLastMonth: false,
    isLastWeek: false,
    year: new Date().getFullYear(),
  })

  async function fetchTrips(): Promise<void> {
    setLoading(true)
    const { data } = await dashboardStore.fetchDashboardTrips(filters.value)
    setLoading(false)

    if (!data) return
    rows.value = [
      { label: t('sidebar.allTrips'), activeCount: data.totalTrips, total: data.totalTrips },
      { label: t('content.inProgress'), activeCount: data.inProgressTrips, total: data.totalTrips },
      { label: t('content.scheduledTrips'), activeCount: data.scheduledTrips, total: data.totalTrips },
      { label: t('content.otherTrips'), activeCount: data.otherTrips, total: data.totalTrips },
      { label: t('content.completed'), activeCount: data.completedTrips, total: data.totalTrips },
      { label: t('button.cancel'), activeCount: data.canceledTrips, total: data.totalTrips },
    ]
  }

  watch([filters], fetchTrips)
  onMounted(fetchTrips)
</script>

<style scoped>

</style>
