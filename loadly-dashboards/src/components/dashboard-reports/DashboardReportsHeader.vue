<template>
  <div
    class="
      mb-6 flex flex-col gap-3
      sm:flex-row sm:items-center sm:justify-between
    "
  >
    <h1 class="text-xl font-bold text-neutral-10">
      {{ title }}
    </h1>

    <slot name="actions">
      <ButtonExport />
    </slot>
  </div>
</template>

<script setup lang="ts">
  defineProps<{
    title: string
  }>()
</script>

<style scoped>

</style>
