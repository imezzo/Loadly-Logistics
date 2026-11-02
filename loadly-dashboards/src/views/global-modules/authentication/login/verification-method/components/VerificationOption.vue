<template>
  <button
    type="button"
    role="radio"
    :aria-checked="isActive"
    :aria-label="`${label} - ${description}`"
    class="
      flex min-h-12.5 w-full cursor-pointer items-center justify-between gap-1.5 rounded-2xl p-2
      transition-all
      hover:bg-neutral-95
    "
    :class="{ 'border-primary bg-brand-98 text-primary ring-1 ring-primary': isActive }"
  >
    <div
      class="flex size-10 items-center justify-center rounded-full bg-neutral-95 text-neutral-20"
    >
      <slot name="icon" />
    </div>

    <div class="text-start">
      <p class="font-bold text-neutral-20">
        {{ label }}
      </p>

      <p class="text-xs text-content-secondary">
        {{ description }}
      </p>
    </div>

    <IconChevron class="ms-auto" />
  </button>
</template>

<script setup lang="ts">
  defineProps<{
    description: string
    isActive: boolean
    label: string
  }>()
</script>

<style scoped>

</style>
