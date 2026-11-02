<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <LocationsHeader />

    <VehicleFacilityGrid />
  </div>
</template>

<script setup lang="ts">
  import LocationsHeader from './components/LocationsHeader.vue'
  import VehicleFacilityGrid from './components/VehicleFacilityGrid.vue'

  const { t } = useI18n()

  const breadcrumbItems = [
    { label: t('title.settings'), to: '/settings' },
    { label: t('title.garages') },
  ]
</script>

<style scoped>

</style>
