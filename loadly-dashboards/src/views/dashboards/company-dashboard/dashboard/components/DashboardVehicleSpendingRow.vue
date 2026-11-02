<template>
  <div
    class="py-4"
    :class="{ 'border-b border-neutral-200': showDivider }"
  >
    <div
      class="
        flex flex-col gap-3
        sm:flex-row sm:items-center
      "
    >
      <span
        class="
          w-full shrink-0 text-sm font-medium text-neutral-10
          sm:w-36
        "
      >
        {{ label }}
      </span>

      <div class="h-2 flex-1 overflow-hidden rounded-full bg-neutral-200">
        <div
          class="h-full rounded-full transition-all duration-300"
          :class="barColorClass"
          :style="{ width: `${Math.min(progress, 100)}%` }"
        />
      </div>

      <div
        class="
          w-full shrink-0 text-start
          sm:w-28 sm:text-end
        "
      >
        <p class="font-bold text-neutral-10">
          {{ value }}
        </p>
        <p class="text-xs text-content-secondary">
          {{ subtext }}
        </p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
  const props = defineProps<{
    label: string
    value: string
    subtext: string
    progress: number
    showDivider?: boolean
  }>()

  const barColorClass = computed(() => {
    const progress = Math.min(props.progress, 100)
    if (progress >= 60) return 'bg-success-500'
    if (progress > 20) return 'bg-warning-300'
    if (progress > 0) return 'bg-error-600'

    return 'bg-neutral-200'
  })
</script>

<style scoped>

</style>
