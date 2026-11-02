<template>
  <div>
    <ListActions />
    <ListFilters />
    <TripsTable />
  </div>
</template>

<script setup lang="ts">
  import ListActions from './components/ListActions.vue'
  import ListFilters from './components/ListFilters.vue'
  import TripsTable from './components/TripsTable.vue'
  import { useContractorTripsList } from './composables/useContractorTripsList.ts'

  const { fetchTrips } = useContractorTripsList()

  onMounted(fetchTrips)
</script>

<style scoped>

</style>
