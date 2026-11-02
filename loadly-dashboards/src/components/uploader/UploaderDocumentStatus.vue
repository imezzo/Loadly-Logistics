<template>
  <div class="flex items-center gap-3 rounded-lg border border-neutral-200 bg-white p-4">
    <div
      v-if="uploadProgress !== undefined"
      class="flex size-12 items-center justify-center"
    >
      <IconUploadProgress :progress="uploadProgress" />
    </div>

    <UploaderFileExtension :file-url="fileName" />
    <div class="flex-1">
      <p class="text-sm font-medium text-neutral-10">
        {{ fileName }}
      </p>

      <p
        v-if="uploadProgress !== undefined"
        class="text-xs text-neutral-600"
      >
        {{ $t('content.uploading') }} ({{ Math.ceil((100 - (uploadProgress || 0)) / 10) }} {{ $t('content.secondsLeft') }})
      </p>

      <div
        v-else
        class="flex items-center gap-2"
      >
        <span
          v-if="status"
          class="text-xs text-neutral-600"
        >
          ({{ statusLabel }})
        </span>
        <span
          v-if="document.expirationDate"
          class="text-xs text-neutral-600"
        >
          - {{ formatDate(document.expirationDate) }}
        </span>
      </div>
    </div>

    <BaseLink
      v-if="canPreview"
      :to="previewHref"
      target="_blank"
      rel="noopener noreferrer"
      class="
        shrink-0 text-sm font-medium text-primary
        hover:text-primary/80
      "
      @click="preview"
    >
      {{ $t('content.preview') }}
    </BaseLink>

    <button
      v-if="showReplace"
      type="button"
      class="
        flex size-8 shrink-0 items-center justify-center rounded-full text-neutral-600
        transition-colors
        hover:bg-neutral-100
      "
      :disabled="!disabled"
      @click="$emit('replace')"
    >
      <IconRefresh />
    </button>

    <button
      type="button"
      class="
        flex size-8 shrink-0 items-center justify-center rounded-full text-neutral-600
        transition-colors
        hover:bg-neutral-100
      "
      :disabled="!disabled"
      @click="$emit('remove')"
    >
      <IconCross class="size-4" />
    </button>
  </div>
</template>

<script setup lang="ts">
  import { formatDate } from '@/utils/date'
  import type { SelectOption } from '@/types/ui'
  import type { DocumentUploaderItem } from '@/types/documents.types'

  const props = defineProps<{
    fileName: string
    uploadProgress?: number
    documentStatusOptions?: SelectOption[]
    showReplace?: boolean
    disabled?: boolean
    showPreview?: boolean
    document: DocumentUploaderItem
  }>()

  defineEmits<{
    remove: []
    replace: []
  }>()

  const status = computed(() => props.document.status)

  const canPreview = computed(() => {
    return props.showPreview
      && props.uploadProgress === undefined
      && (!!props.document.fileUrl || !!props.document.file)
  })

  const previewHref = computed(() => props.document.fileUrl || '#')

  function preview(event: MouseEvent): void {
    if (!props.document.file) return

    event.preventDefault()
    const url = URL.createObjectURL(props.document.file)
    window.open(url, '_blank')
  }

  const statusLabel = computed(() => {
    if (!status.value || !props.documentStatusOptions) return ''

    const option = props.documentStatusOptions.find(opt => opt.value === status.value)

    return option?.label || status.value
  })
</script>

<style scoped>

</style>

