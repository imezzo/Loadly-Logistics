<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-4"
    />

    <DashboardReportsHeader :title="$t('sidebar.driverBehavior')" />

    <ListFilters />
    <ListTable />
  </div>
</template>

<script setup lang="ts">
  import ListFilters from './components/ListFilters.vue'
  import ListTable from './components/ListTable.vue'

  const { t } = useI18n()

  const breadcrumbItems = computed(() => ([
    { label: t('sidebar.dashboardAndReports') },
    { label: t('sidebar.driverBehavior') },
  ]))
</script>

<style scoped>

</style>
