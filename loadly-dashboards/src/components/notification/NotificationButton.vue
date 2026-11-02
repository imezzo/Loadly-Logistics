<template>
  <button
    class="
      relative flex size-14 cursor-pointer items-center justify-center rounded-full border-none
      bg-neutral-30 p-4
    "
    :aria-label="$t('a11y.notifications')"
    v-bind="$attrs"
  >
    <IconBell />
    <span
      v-if="hasNotification"
      class="absolute end-2.75 top-2.75 size-4 rounded-full bg-error-600 text-xs text-white"
      v-text="notificationCount?.toString()"
    />
  </button>
</template>

<script setup lang="ts">
  defineProps<{
    hasNotification?: boolean
    notificationCount?: number
  }>()
</script>

<style scoped>

</style>
