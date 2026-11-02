<template>
  <DefaultLayout>
    <div class="page-content">
      <BaseBreadcrumb
        :items="breadcrumbItems"
        class="mb-2"
      />

      <TripListHeader />

      <RouterView />
    </div>
  </DefaultLayout>
</template>

<script setup lang="ts">
  import DefaultLayout from '@/layouts/defaultLayout.vue'
  import TripListHeader from './components/TripsHeader.vue'

  const { t } = useI18n()
  const route = useRoute()

  const pageTitle = computed(() => {
    if (route.path.includes('external')) {
      return t('sidebar.externalTrips')
    }
    if (route.path.includes('company')) {
      return t('sidebar.tripsOnMap')
    }

    return t('sidebar.contractTrips')
  })

  const mainTripsPath = computed(() => {
    if (route.path.includes('external')) return '/trips/external'
    if (route.path.includes('map')) return '/trips/map'

    return '/trips'
  })

  const breadcrumbItems = computed(() => [
    { label: t('sidebar.tripManagement'), to: mainTripsPath.value },
    { label: pageTitle.value },
  ])
</script>

<style scoped>

</style>
