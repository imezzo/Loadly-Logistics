<template>
  <div class="-mt-2 flex flex-wrap gap-2">
    <button
      v-for="durationMonths in options"
      :key="durationMonths"
      type="button"
      :class="[
        'rounded-lg border px-3 py-1.5 text-sm font-medium transition-colors',
        activeMonths === durationMonths
          ? `
            border-primary bg-primary text-white
            hover:bg-primary/90
          `
          : `
            border-neutral-200 bg-white text-neutral-700
            hover:bg-neutral-200
          `,
      ]"
      :disabled="disabled"
      @click="$emit('select', durationMonths)"
    >
      {{ $t('form.monthsDuration', { count: durationMonths }) }}
    </button>
  </div>
</template>

<script setup lang="ts">
  defineProps<{
    options: readonly number[] | number[]
    activeMonths: number | null
    disabled?: boolean
  }>()

  defineEmits<{
    select: [durationMonths: number]
  }>()
</script>

<style scoped>

</style>\
