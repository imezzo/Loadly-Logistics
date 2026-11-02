<template>
  <button
    type="button"
    class="
      absolute end-2 top-2 flex size-10 items-center justify-center rounded-lg border
      border-neutral-500 bg-white text-neutral-10 transition-colors
      4xl:size-30
    "
    :aria-label="
      isFullscreen
        ? $t('a11y.exitFullscreen')
        : $t('a11y.enterFullscreen')
    "
    @click="$emit('toggle')"
  >
    <IconFullscreen
      :exit="isFullscreen"
      class="
        size-4
        4xl:size-10
      "
    />
  </button>
</template>

<script setup lang="ts">
  defineProps<{
    isFullscreen: boolean
  }>()

  defineEmits<{
    toggle: []
  }>()
</script>

<style scoped>

</style>
