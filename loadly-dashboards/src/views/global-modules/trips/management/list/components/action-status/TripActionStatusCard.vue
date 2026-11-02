<template>
  <button
    type="button"
    class="
      hover:border-primary-300 hover:bg-neutral-200
      flex min-h-28 min-w-36 flex-1 flex-col items-center justify-center gap-3 rounded-2xl border
      border-neutral-200 bg-white px-4 py-5 text-lg font-bold text-neutral-10 transition-colors
      disabled:pointer-events-none disabled:opacity-60
    "
    :disabled
    @click="$emit('select')"
  >
    <slot name="icon" />

    <span>
      {{ label }}
    </span>
  </button>
</template>

<script setup lang="ts">
  defineProps<{
    label: string
    disabled?: boolean
  }>()

  defineEmits<{
    select: []
  }>()
</script>

<style scoped>

</style>
