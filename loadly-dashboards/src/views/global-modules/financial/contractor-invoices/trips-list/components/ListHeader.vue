<template>
  <div
    class="
      mt-3 mb-6 flex flex-col gap-4
      sm:flex-row sm:items-center sm:justify-between
    "
  >
    <h1 class="text-xl font-bold text-neutral-800">
      {{ $t('title.contractorWithName', { name: contractorName }) }}
    </h1>

    <!-- <ButtonExport /> -->
  </div>
</template>

<script setup lang="ts">
  defineProps<{
    contractorName: string
  }>()
</script>

<style scoped>

</style>
