<template>
  <div :class="['flex items-center', containerClass]">
    <button
      v-if="prevButtonOptions"
      type="button"
      :class="prevButtonClass"
      :disabled="prevButtonOptions.disabled"
      @click="prev"
    >
      <IconArrow
        v-if="prevButtonOptions.hasIcon"
        class="size-5 rotate-180 fill-neutral-800!"
      />
      {{ prevButtonOptions.label || $t('button.back') }}
    </button>

    <slot />

    <button
      v-if="secondaryButtonOptions"
      type="button"
      :class="secondaryButtonClass"
      :disabled="secondaryButtonOptions.disabled"
      @click="handleSecondaryClick"
    >
      {{ secondaryButtonOptions.label }}
    </button>

    <button
      v-if="nextButtonOptions"
      :type="nextButtonOptions.asSubmit ? 'submit' : 'button'"
      :class="nextButtonClass"
      :disabled="nextButtonOptions.disabled"
      @click="next"
    >
      {{ nextButtonOptions.label || $t('button.next') }}
      <IconArrow
        v-if="nextButtonOptions.hasIcon"
        class="size-5"
      />
    </button>
  </div>
</template>

<script setup lang="ts">
  interface BaseActionOptions {
    class?: string
    disabled?: boolean
    hasScrollTop?: boolean
    hasIcon?: boolean
    label?: string
  }

  const props = withDefaults(defineProps<{
    containerClass?: string
    nextButtonOptions?: BaseActionOptions & { asSubmit?: boolean }
    prevButtonOptions?: BaseActionOptions
    secondaryButtonOptions?: BaseActionOptions
  }>(), {
    containerClass: 'justify-end gap-3 mt-8',
    nextButtonOptions: undefined,
    prevButtonOptions: undefined,
    secondaryButtonOptions: undefined,
  })

  const emit = defineEmits<{
    next: []
    prev: []
    secondary: []
  }>()

  const DEFAULT_CLASSES = {
    prev: 'btn-secondary h-13.5 gap-2 md:w-52.5',
    next: 'btn-primary h-13.5 gap-2 md:w-75',
    secondary: `flex h-13.5 items-center justify-center gap-2 rounded-2xl
    bg-gray-200 px-6 py-3 text-sm font-semibold text-neutral-800
    transition-colors hover:bg-gray-300 md:w-52.5`,
  } as const

  function joinClasses(classes: string): string {
    return classes.split(/\s+/).join(' ').trim()
  }

  const prevButtonClass = computed(() =>
    (joinClasses(props.prevButtonOptions?.class ?? DEFAULT_CLASSES.prev)))

  const nextButtonClass = computed(() =>
    (joinClasses(props.nextButtonOptions?.class ?? DEFAULT_CLASSES.next)))

  const secondaryButtonClass = computed(() =>
    (joinClasses(props.secondaryButtonOptions?.class ?? joinClasses(DEFAULT_CLASSES.secondary))))

  function scrollToTop(): void {
    scrollTo({ top: 0, behavior: 'smooth' })
  }

  function prev(): void {
    if (props.prevButtonOptions?.hasScrollTop) {
      scrollToTop()
    }
    emit('prev')
  }

  function handleSecondaryClick(): void {
    if (props.secondaryButtonOptions?.hasScrollTop) {
      scrollToTop()
    }
    emit('secondary')
  }

  function next(): void {
    if (props.nextButtonOptions?.hasScrollTop) {
      scrollToTop()
    }
    if (!props.nextButtonOptions?.asSubmit) emit('next')
  }
</script>

<style scoped>

</style>
