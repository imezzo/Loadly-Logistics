<template>
  <ul>
    <li
      v-for="(error, key) in errors"
      :key="key"
      class="mb-1.5 list-inside list-disc text-xs text-red-600"
    >
      {{ key }}: {{ error }}
    </li>
  </ul>
</template>

<script setup lang="ts">
  defineProps<{
    errors: Record<string, string>
  }>()
</script>

<style scoped>

</style>
