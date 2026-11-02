<template>
  <button
    class="
      flex items-center gap-2 transition-colors
      hover:text-primary
    "
    type="button"
    :aria-label="label
      ? `Sort by ${label}`
      : 'Sort'"
    @click="$emit('sort', columnKey)"
  >
    <span v-if="label">{{ label }}</span>

    <div class="flex flex-col">
      <IconChevron
        filled
        class="-mb-0.5 size-3 transition-opacity"
        :class="sortKey !== columnKey
          ? 'opacity-30'
          : sortOrder === 'asc'
            ? 'opacity-100'
            : 'opacity-30'"
      />

      <IconChevron
        filled
        class="size-3 rotate-180 transition-opacity"
        :class="sortKey !== columnKey
          ? 'opacity-30'
          : sortOrder === 'desc'
            ? 'opacity-100'
            : 'opacity-30'"
      />
    </div>
  </button>
</template>

<script setup lang="ts">
  defineProps<{
    label?: string
    columnKey: string
    sortKey?: string | null
    sortOrder?: 'asc' | 'desc' | null
  }>()

  defineEmits<{
    sort: [key: string]
  }>()
</script>

<style scoped>

</style>

