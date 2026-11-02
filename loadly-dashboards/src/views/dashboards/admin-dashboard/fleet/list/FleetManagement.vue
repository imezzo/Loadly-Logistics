<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <FleetListHeader />

    <FleetFilters />

    <VehiclesTable
      :vehicles="vehicles"
      :pagination="pagination"
      :is-loading="isLoading"
      @page-change="setPage"
      @items-per-page-change="updateItemsPerPage"
    />
  </div>
</template>

<script setup lang="ts">
  import { useList } from './composables/useList'
  import FleetListHeader from './components/FleetListHeader.vue'
  import FleetFilters from './components/FleetFilters.vue'
  import VehiclesTable from './components/VehiclesTable.vue'

  const { t } = useI18n()
  const route = useRoute()

  const breadcrumbItems = [
    { label: t('title.fleetVehicle'), to: '/fleet' },
    { label: t('title.fleetManagement') },
  ]

  const {
    fetchVehicles,
    isLoading,
    pagination,
    resetFilters,
    setPage,
    updateItemsPerPage,
    vehicles,
  } = useList()

  onMounted(fetchVehicles)

  onUnmounted(() => {
    if (!route.path.includes('/fleet')) {
      resetFilters()
    }
  })
</script>

<style scoped>

</style>
