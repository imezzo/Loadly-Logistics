<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-4"
    />

    <DashboardReportsHeader :title="$t('sidebar.overview')" />
    <DashboardTrips />
    <DashboardRevenue />
  </div>
</template>

<script setup lang="ts">
  import DashboardTrips from './components/DashboardTrips.vue'
  import DashboardRevenue from './components/DashboardRevenue.vue'

  const { t } = useI18n()
  const breadcrumbItems = computed(() => ([
    { label: t('sidebar.dashboardReports') },
    { label: t('sidebar.overview') },
  ]))
</script>

<style scoped>

</style>
