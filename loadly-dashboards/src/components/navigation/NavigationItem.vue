<template>
  <component
    :is="clickable ? 'button' : 'div'"
    :type="clickable ? 'button' : undefined"
    class="relative flex max-w-53 items-center gap-3 pb-8 text-start"
    :class="clickable && `
      cursor-pointer transition-opacity
      hover:opacity-80
    `"
    :disabled="step.disabled"
    @click="clickable && !step.disabled && emit('select', index)"
  >
    <div
      class="
        relative flex size-12 shrink-0 items-center justify-center rounded-full border-2
        transition-colors
      "
      :class="isActive
        ? 'border-primary/5 bg-primary/10 text-black'
        : 'border-neutral-30 bg-white text-black/40'"
    >
      <component
        :is="step.icon"
        :color="isActive
          ? 'var(--color-black)'
          : 'rgba(0, 0, 0, 0.4)'"
        class="size-5"
      />
    </div>

    <span
      class="transition-font-weight text-sm"
      :class="isActive
        ? 'font-medium text-neutral-10'
        : 'font-normal text-neutral-800'"
    >
      {{ step.title }}
    </span>

    <span
      v-if="!isLast"
      class="absolute start-5.5 top-12 h-8 w-1 bg-neutral-30 transition-colors"
    />
  </component>
</template>

<script setup lang="ts">
  const props = defineProps<{
    step: {
      title: string
      icon: Component
      disabled?: boolean
    }
    index: number
    currentStep: number
    isLast: boolean
    clickable?: boolean
  }>()

  const clickable = computed(() => props.clickable ?? true)

  const emit = defineEmits<{
    select: [index: number]
  }>()

  const isActive = computed(() => props.index <= props.currentStep)
</script>

<style scoped>

</style>
