<template>
  <div
    class="
      flex flex-col rounded-2xl bg-neutral-100 p-5 transition-shadow
      hover:shadow-md
    "
  >
    <span
      class="mb-4 text-3xl font-bold text-neutral-800"
    >
      {{ count }}
    </span>

    <span
      class="font-medium"
      :class="labelColorClass"
    >
      {{ label }}
    </span>
  </div>
</template>

<script setup lang="ts">
  const props = defineProps<{
    count: number
    label: string
    variant: 'success' | 'info' | 'warning' | 'default' | 'error'
  }>()

  const labelColorClass = computed(() => {
    switch (props.variant) {
      case 'success':
        return 'text-success-500'
      case 'info':
        return 'text-info-500'
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
</script>

<style scoped>

</style>
