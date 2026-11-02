<template>
  <div class="flex flex-wrap items-end gap-3">
    <div class="min-w-60 flex-1">
      <label class="mb-1 block text-sm font-medium text-neutral-800">
        {{ $t('content.chooseVehicleType') }}
      </label>

      <BaseMultiSelect
        :model-value="modelValue"
        :options="options"
        :placeholder="placeholder"
        wrapper-class="[&>.select-button]:h-10 [&>.select-button]:rounded-lg min-h-10"
        :on-scroll-to-end="() => emit('scrollToEnd')"
        @update:model-value="emit('update:modelValue', $event)"
      />
    </div>

    <button
      type="button"
      class="btn-primary shrink-0 px-4"
      :disabled="!modelValue.length"
      @click="emit('addTypes')"
    >
      {{ $t('button.addTypes') }}
    </button>
  </div>
</template>

<script setup lang="ts">
  import type { SelectOption } from '@/types/ui'

  defineProps<{
    modelValue: string[]
    options: SelectOption[]
    placeholder?: string
  }>()

  const emit = defineEmits<{
    'update:modelValue': [value: string[]]
    addTypes: []
    scrollToEnd: []
  }>()
</script>

<style scoped>

</style>
