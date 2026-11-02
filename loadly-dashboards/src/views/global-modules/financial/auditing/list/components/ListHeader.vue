<template>
  <div
    class="
      mb-6 flex flex-col gap-4
      sm:flex-row sm:items-center sm:justify-between
    "
  >
    <h1 class="text-xl font-bold text-neutral-800">
      {{ $t('sidebar.tripAuditing') }}
    </h1>

    <TripAuditingExport />
  </div>
</template>

<script setup lang="ts">
  import TripAuditingExport from './TripAuditingExport.vue'
</script>

<style scoped>

</style>
