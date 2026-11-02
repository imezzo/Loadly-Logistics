<template>
  <div
    class="flex items-center gap-2"
    :class="wrapperClass"
  >
    <input
      :id="id"
      v-model="model"
      type="checkbox"
      :disabled="disabled"
      :indeterminate="indeterminate"
      class="checkbox"
      v-bind="$attrs"
      @change="onCheckedChange?.(($event.target as HTMLInputElement).checked)"
    >

    <label
      v-if="checkboxLabel || label"
      :for="id"
      class="inline-flex cursor-pointer items-center gap-2 text-sm capitalize select-none"
      :class="{ 'opacity-50': disabled }"
    >
      {{ checkboxLabel || label }}
    </label>
  </div>
</template>

<script setup lang="ts">
  defineOptions({
    inheritAttrs: false,
  })

  defineProps<{
    checkboxLabel?: string
    disabled?: boolean
    indeterminate?: boolean
    label?: string
    onCheckedChange?: (value: boolean) => void
    wrapperClass?: string
  }>()

  const id = useId()

  const model = defineModel<boolean>()
</script>

<style scoped>

</style>
