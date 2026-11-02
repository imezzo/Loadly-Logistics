<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-4"
    />

    <h2 class="mb-4 text-lg font-bold text-neutral-10">
      {{ t('sidebar.driverPerformanceMetrics') }}
    </h2>

    <TopDrivers />
  </div>
</template>

<script setup lang="ts">
  import TopDrivers from './components/TopDrivers.vue'

  const { t } = useI18n()

  const breadcrumbItems = computed(() => ([
    { label: t('sidebar.dashboardReports') },
    { label: t('sidebar.driverPerformanceMetrics') },
  ]))
</script>

<style scoped>

</style>
