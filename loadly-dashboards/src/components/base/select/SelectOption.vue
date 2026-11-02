<!-- eslint-disable vue/no-v-html -->
<template>
  <li
    class="select-option"
    :class="{ 'is-selected': isSelected }"
    tabindex="0"
    @keydown.enter="$emit('select', option)"
    @keydown.space="$emit('select', option)"
    @click="$emit('select', selectedOption)"
  >
    <span
      v-if="option.content"
      class="min-w-0 flex-1"
    >
      <span v-html="option.content?.()" />
    </span>
    <span v-else>{{ option.label }}</span>

    <IconCheck v-if="isSelected" />
  </li>
</template>

<script setup lang="ts">
  import type { SelectOption } from '@/types/ui'

  const props = defineProps<{
    option: SelectOption
    isSelected: boolean
    isMultiple?: boolean
  }>()

  defineEmits<{
    select: [option: SelectOption]
  }>()

  const selectedOption = computed(() => (props.isSelected && !props.isMultiple
    ? { label: '', value: '' }
    : props.option))
</script>
