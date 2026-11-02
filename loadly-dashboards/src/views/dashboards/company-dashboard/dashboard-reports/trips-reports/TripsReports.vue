<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-4"
    />

    <DashboardReportsHeader :title="t('sidebar.kilometerUsageVsPackageLimits')" />
    <DashboardRevenue />
    <DashboardTrips />
  </div>
</template>

<script setup lang="ts">
  import DashboardRevenue from './components/DashboardRevenue.vue'
  import DashboardTrips from './components/DashboardTrips.vue'

  const { t } = useI18n()
  const breadcrumbItems = computed(() => ([
    { label: t('sidebar.dashboardReports') },
    { label: t('sidebar.kilometerUsageVsPackageLimits') },
  ]))
</script>

<style scoped>

</style>
