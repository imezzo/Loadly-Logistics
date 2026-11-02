<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <ListHeader />
    <ListFilters />
    <ListTable />
  </div>
</template>

<script setup lang="ts">
  import ListHeader from './components/ListHeader.vue'
  import ListFilters from './components/ListFilters.vue'
  import ListTable from './components/ListTable.vue'
  import { useTripReportsList } from './composables/useTripReportsList'

  const { t } = useI18n()
  const route = useRoute()
  const { fetchTrips, emptyFilters, filters } = useTripReportsList()

  const breadcrumbItems = computed(() => ([
    { label: t('sidebar.financialReports') },
    { label: t('sidebar.tripReports') },
  ]))

  onMounted(() => {
    fetchTrips()
  })

  onUnmounted(() => {
    if (!route.path.includes('/financial') && route.name !== 'ViewTrip') {
      filters.value = emptyFilters()
    }
  })
</script>

<style scoped>

</style>
