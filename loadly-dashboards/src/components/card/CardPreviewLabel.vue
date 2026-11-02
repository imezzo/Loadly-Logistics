<template>
  <span class="font-medium text-neutral-10">
    <slot name="label">
      {{ label }}
    </slot>
  </span>

  <p class="text-sm font-medium text-content-secondary">
    <slot name="content">
      {{ content }}
    </slot>
  </p>

  <span
    v-if="isEdited"
    class="ms-auto w-fit rounded-md bg-success-500 px-2 py-1 text-xs text-white"
  >
    {{ $t('content.edited') }}
  </span>
  <div v-else />
</template>

<script setup lang="ts">
  defineProps<{
    label: string
    content: string
    isEdited?: boolean
  }>()
</script>

<style scoped>

</style>
