<template>
  <svg
    aria-hidden="true"
    width="16"
    height="16"
    viewBox="0 0 16 16"
    fill="none"
    xmlns="http://www.w3.org/2000/svg"
  >
    <path
      v-if="exit"
      d="M5.333 2v2.667H2.667M10.667 2v2.667h2.666M5.333 14v-2.667H2.667M10.667 14v-2.667h2.666"
      stroke="currentColor"
      stroke-width="1.5"
      stroke-linecap="round"
      stroke-linejoin="round"
    />

    <path
      v-else
      d="M2.667 5.333V2.667H5.333M10.667 2.667h2.666V5.333M13.333 10.667v2.666H10.667M5.333 13.333H2.667V10.667"
      stroke="currentColor"
      stroke-width="1.5"
      stroke-linecap="round"
      stroke-linejoin="round"
    />
  </svg>
</template>

<script setup lang="ts">
  defineProps<{
    exit?: boolean
  }>()
</script>

<style scoped>

</style>
