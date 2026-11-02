<template>
  <svg
    v-if="filled"
    aria-hidden="true"
    width="10"
    height="6"
    viewBox="0 0 10 6"
    fill="none"
    xmlns="http://www.w3.org/2000/svg"
  >
    <path
      d="M0.606106 5.45449H9.09082C9.25497 5.45449 9.39706 5.39455 9.51696 5.27459C9.6368 5.15465 9.69696 5.01263 9.69696 4.84848C9.69696 4.68434 9.63683 4.54238 9.51696 4.42228L5.27459 0.179901C5.15472 0.0600998 5.01273 0 4.84848 0C4.68424 0 4.54224 0.0600998 4.42228 0.179901L0.179901 4.42228C0.059934 4.54225 0 4.68434 0 4.84848C0 5.0126 0.059934 5.15465 0.179901 5.27459C0.300001 5.39455 0.441992 5.45449 0.606106 5.45449Z"
      fill="#98A2B3"
    />
  </svg>

  <svg
    v-else
    aria-hidden="true"
    width="20"
    height="20"
    viewBox="0 0 20 20"
    fill="none"
    xmlns="http://www.w3.org/2000/svg"
  >
    <path
      d="M8.33337 13.3333L11.6667 9.99996L8.33337 6.66663"
      stroke="#1B1B1B"
      stroke-width="1.5"
      stroke-linecap="round"
      stroke-linejoin="round"
    />
  </svg>
</template>

<script setup lang="ts">
  defineProps<{
    filled?: boolean
  }>()
</script>

<style scoped>

</style>
