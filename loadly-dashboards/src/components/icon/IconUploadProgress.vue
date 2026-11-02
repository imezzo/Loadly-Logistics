<template>
  <svg
    aria-hidden="true"
    class="size-8 -rotate-90 transform transition-all duration-300"
    viewBox="0 0 32 32"
    fill="none"
    xmlns="http://www.w3.org/2000/svg"
  >
    <circle
      cx="16"
      cy="16"
      r="14"
      fill="none"
      stroke="#e5e7eb"
      stroke-width="2"
    />
    <circle
      cx="16"
      cy="16"
      r="14"
      fill="none"
      stroke="#6366f1"
      stroke-width="2"
      stroke-dasharray="88"
      :stroke-dashoffset="88 - (88 * (progress || 0)) / 100"
      class="transition-all duration-300"
    />
  </svg>
</template>

<script setup lang="ts">
  withDefaults(defineProps<{
    progress?: number
  }>(), {
    progress: 0,
  })
</script>

<style scoped>

</style>
