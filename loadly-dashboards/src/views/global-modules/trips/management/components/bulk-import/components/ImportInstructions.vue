<template>
  <ol
    class="
      mb-4 list-inside list-decimal rounded-lg bg-info-50 p-2 text-xs font-medium text-neutral-800
    "
  >
    <li>{{ $t('title.importTripsRequiredHeaders', { headers: requiredHeaders.join(', ') }) }}</li>

    <li>{{ $t('content.excelFilesOnly') }}</li>
  </ol>
</template>

<script setup lang="ts">
  defineProps<{
    requiredHeaders: string[]
  }>()
</script>

<style scoped>

</style>
