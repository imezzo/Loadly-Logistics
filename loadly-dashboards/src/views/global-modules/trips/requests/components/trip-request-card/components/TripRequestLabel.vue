<template>
  <div class="flex items-center gap-2 text-sm text-neutral-700">
    <slot name="icon" />

    <span class="font-medium">{{ label }}:</span>

    <span class="truncate">{{ value }}</span>
  </div>
</template>

<script setup lang="ts">
  defineProps<{
    label: string
    value?: string
  }>()
</script>

<style scoped>

</style>
