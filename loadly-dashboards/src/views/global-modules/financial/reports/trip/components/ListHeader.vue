<template>
  <div
    class="
      mb-6 flex flex-col gap-4
      sm:flex-row sm:items-center sm:justify-between
    "
  >
    <h1 class="text-xl font-bold text-neutral-800">
      {{ $t('sidebar.tripReports') }}
    </h1>

    <TripReportsExport />
  </div>
</template>

<script setup lang="ts">
  import TripReportsExport from './TripReportsExport.vue'
</script>

<style scoped>

</style>
