<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-4"
    />

    <h2 class="mb-4 text-lg font-bold text-neutral-10">
      {{ t('title.fleetManagement') }}
    </h2>

    <div
      class="
        grid gap-4
        sm:grid-cols-2
      "
    >
      <FleetWeeklyVehicleTripsComparisonCard />
    </div>

    <TopVehicles />
  </div>
</template>

<script setup lang="ts">
  import TopVehicles from './components/TopVehicles.vue'
  import FleetWeeklyVehicleTripsComparisonCard from './components/FleetWeeklyVehicleTripsComparisonCard.vue'

  const { t } = useI18n()
  const breadcrumbItems = computed(() => ([
    { label: t('sidebar.dashboardReports') },
    { label: t('sidebar.fleetUtilizationStatistics') },
  ]))
</script>

<style scoped>

</style>
