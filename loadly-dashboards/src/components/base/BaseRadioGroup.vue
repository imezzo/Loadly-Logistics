<template>
  <div class="flex flex-col gap-2">
    <div
      class="flex flex-wrap gap-6"
      :class="wrapperClass"
    >
      <div
        v-for="option in options"
        :key="option.value"
        class="flex flex-col gap-0.5"
      >
        <BaseRadio
          :model-value="modelValue?.toString() === option.value.toString()"
          :label="option.label"
          :description="option.description ?? undefined"
          :disabled="disabled"
          @change="onChange(option.value)"
        />
      </div>
    </div>

    <p
      v-if="error"
      :class="errorClass"
    >
      {{ error }}
    </p>
  </div>
</template>

<script setup lang="ts">
  import BaseRadio from './BaseRadio.vue'

  defineOptions({
    inheritAttrs: false,
  })

  defineProps<{
    modelValue?: string | number
    name?: string
    label?: string
    labelClass?: string
    required?: boolean
    info?: boolean
    options: { value: string | number; label: string; description?: string | null }[]
    wrapperClass?: string
    error?: string
    errorClass?: string
    disabled?: boolean
  }>()

  const emit = defineEmits<{
    'update:modelValue': [value: string | number]
    change: [value: string | number]
  }>()

  function onChange(value: string | number): void {
    emit('update:modelValue', value)
    emit('change', value)
  }
</script>

<style scoped>
</style>

