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
  import { useTripAuditingList } from './composables/useTripAuditingList'

  const { t } = useI18n()
  const { fetchTrips } = useTripAuditingList()

  const breadcrumbItems = computed(() => ([
    { label: t('sidebar.tripAuditing') },
  ]))

  onMounted(fetchTrips)
</script>

<style scoped>

</style>
