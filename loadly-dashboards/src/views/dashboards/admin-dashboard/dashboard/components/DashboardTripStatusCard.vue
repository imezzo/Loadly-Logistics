<template>
  <article
    class="flex flex-col rounded-2xl border border-neutral-500 bg-white p-5"
  >
    <span class="mb-4 text-3xl font-bold text-neutral-800">
      {{ count }}
    </span>

    <span
      class="flex items-center gap-2 font-medium"
      :class="labelColorClass"
    >
      <span
        class="size-2 shrink-0 rounded-full"
        :class="dotColorClass"
      />
      {{ label }}
    </span>
  </article>
</template>

<script setup lang="ts">
  const props = defineProps<{
    count: number
    label: string
    variant: 'success' | 'warning' | 'default' | 'error'
  }>()

  const labelColorClass = computed(() => {
    switch (props.variant) {
      case 'success':
        return 'text-success-500'
      case 'warning':
        return 'text-warning-300'
      case 'error':
        return 'text-error-600'
      case 'default':
        return 'text-neutral-600'
      default:
        return 'text-neutral-800'
    }
  })

  const dotColorClass = computed(() => {
    switch (props.variant) {
      case 'success':
        return 'bg-success-500'
      case 'warning':
        return 'bg-warning-300'
      case 'error':
        return 'bg-error-600'
      case 'default':
        return 'bg-neutral-600'
      default:
        return 'bg-neutral-800'
    }
  })
</script>

<style scoped>

</style>
