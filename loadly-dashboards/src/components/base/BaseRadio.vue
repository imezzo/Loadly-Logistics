<template>
  <div class="flex flex-col gap-0.5">
    <button
      type="button"
      role="radio"
      :aria-checked="isChecked"
      :aria-label="label"
      class="flex items-center gap-2"
      :class="{ 'cursor-pointer': !disabled }"
      :disabled="disabled"
      @click="onClick"
    >
      <div
        class="
          flex size-4 shrink-0 items-center justify-center rounded-full border transition-colors
        "
        :class="radioClasses"
      >
        <IconCheck
          v-if="isChecked"
          class="size-3 stroke-white!"
        />
      </div>

      <span
        v-if="label || $slots.default"
        class="text-sm transition-colors"
      >
        <slot>{{ label }}</slot>
      </span>
    </button>

    <p
      v-if="description"
      class="mt-6 font-medium text-neutral-10"
    >
      {{ description }}
    </p>
  </div>
</template>

<script setup lang="ts">
  const props = defineProps<{
    modelValue?: boolean
    checked?: boolean
    label?: string
    description?: string | null
    disabled?: boolean
    variant?: 'default' | 'success' | 'error'
  }>()

  const emit = defineEmits<{
    'update:modelValue': [value: boolean]
    change: [value: boolean]
  }>()

  const isChecked = computed(() => props.modelValue ?? props.checked ?? false)

  const radioClasses = computed(() => {
    if (!isChecked.value) {
      return 'bg-white border-black/40'
    }

    switch (props.variant) {
      case 'success':
        return 'bg-success-700 border-success-700'
      case 'error':
        return 'bg-error-600 border-error-600'
      default:
        return 'bg-primary border-primary'
    }
  })

  function onClick(): void {
    const newValue = !isChecked.value
    emit('update:modelValue', newValue)
    emit('change', newValue)
  }
</script>

<style scoped>

</style>

