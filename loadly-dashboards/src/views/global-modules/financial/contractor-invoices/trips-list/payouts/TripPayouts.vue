<template>
  <div>
    <ListFilters />
    <PayoutsTable />
  </div>
</template>

<script setup lang="ts">
  import ListFilters from './components/ListFilters.vue'
  import PayoutsTable from './components/PayoutsTable.vue'
  import { useContractorPayoutsList } from './composables/useContractorPayoutsList'

  const { fetchPayouts } = useContractorPayoutsList()

  onMounted(fetchPayouts)
</script>

<style scoped>

</style>
