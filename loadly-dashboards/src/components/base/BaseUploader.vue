<!-- TODO: Refactor this component to remove Uploader component and use it here and update its dependencies -->
<template>
  <Uploader
    :file="modelValue"
    :file-url="fileUrl"
    :accept="accept"
    :disabled="disabled"
    :show-preview="showPreview"
    :class="wrapperClass"
    @file-change="$emit('update:model-value', $event)"
    @file-remove="onRemove"
  />
</template>

<script setup lang="ts">
  const props = withDefaults(
    defineProps<{
      modelValue?: File
      accept?: string
      disabled?: boolean
      fileUrl?: string | null
      clearFileUrl?: () => void
      wrapperClass?: string
      fieldId?: string
      showPreview?: boolean
    }>(),
    {
      modelValue: undefined,
      accept: '.pdf,.jpg,.jpeg,.png',
      disabled: false,
      fileUrl: null,
      clearFileUrl: undefined,
      wrapperClass: '',
      fieldId: undefined,
      showPreview: false,
    }
  )

  const emit = defineEmits<{
    'update:model-value': [value: File | null]
  }>()

  function onRemove(): void {
    if (!props.modelValue) {
      props.clearFileUrl?.()
    }

    emit('update:model-value', null)
  }
</script>

<style scoped>

</style>
