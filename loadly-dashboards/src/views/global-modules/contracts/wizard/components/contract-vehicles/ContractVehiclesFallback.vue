<template>
  <div
    class="
      flex flex-col items-center justify-center rounded-lg border border-neutral-200 bg-white p-12
    "
  >
    <IconVehicle class="mb-4 size-12 text-neutral-400" />

    <p class="text-sm font-medium text-neutral-600">
      {{ $t('content.noVehicleTypesAdded') }}
    </p>

    <p class="mt-2 text-xs text-content-secondary">
      {{ $t('content.clickAddVehicleTypesToStart') }}
    </p>
  </div>
</template>

<script setup lang="ts">

</script>

<style scoped>

</style>
