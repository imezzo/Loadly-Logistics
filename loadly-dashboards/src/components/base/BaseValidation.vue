<template>
  <p
    v-if="message || $slots.default"
    :class="['mt-1 text-sm text-error-600', customClass]"
  >
    <slot>
      {{ message }}
    </slot>
  </p>
</template>

<script setup lang="ts">
  withDefaults(defineProps<{
    message?: string | null
    customClass?: string
  }>(), {
    message: null,
    customClass: '',
  })
</script>

<style scoped>

</style>
