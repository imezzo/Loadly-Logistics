<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-4"
    />

    <DashboardReportsHeader :title="$t('sidebar.costAnalysisReports')" />
    <TripsCostAnalysis />
    <TripsList />
  </div>
</template>

<script setup lang="ts">
  import TripsCostAnalysis from './components/TripsCostAnalysis.vue'
  import TripsList from './components/TripsList.vue'

  const { t } = useI18n()
  const breadcrumbItems = computed(() => ([
    { label: t('sidebar.dashboardReports') },
    { label: t('sidebar.costAnalysisReports') },
  ]))
</script>

<style scoped>

</style>
