<template>
  <aside
    class="
      relative flex shrink-0 flex-col px-4 py-6
      lg:w-64
    "
  >
    <NavigationItem
      v-for="(step, index) in steps"
      :key="index"
      :step="step"
      :index="step.id"
      :current-step="props.currentStep"
      :is-last="index === steps.length - 1"
      :clickable="props.clickable"
      @select="emit('select-step', $event)"
    />
  </aside>
</template>

<script setup lang="ts">
  const props = withDefaults(
    defineProps<{
      steps: {
        id: number
        title: string
        icon: Component
        disabled?: boolean
      }[]
      currentStep: number
      clickable?: boolean
    }>(),
    { clickable: false }
  )

  const emit = defineEmits<{
    'select-step': [index: number]
  }>()
</script>

<style scoped>

</style>
