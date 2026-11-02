<template>
  <div
    class="
      relative flex min-h-19.5 w-100 flex-col items-center justify-center gap-2 rounded-xl border
      bg-white px-6 py-4 transition-colors
    "
    :class="[
      hasError ? 'border-error-600' : 'border-neutral-500',
      disabled
        ? 'cursor-not-allowed'
        : `
          cursor-pointer
          hover:border-primary hover:bg-gray-100
        `,
    ]"
    @click="!disabled && !hasAttachment && fileInputRef?.click()"
    @drop.prevent="!disabled && drop($event)"
    @dragover.prevent
    @dragenter.prevent
  >
    <input
      ref="fileInputRef"
      type="file"
      class="hidden"
      :accept
      :multiple
      :disabled
      @change="onFileChange($event)"
    >

    <template v-if="hasAttachment">
      <div
        class="flex w-full items-center gap-3"
        @click.stop
      >
        <UploaderFileExtension
          :file-url="fileName"
          :class="{ 'opacity-50': disabled }"
        />

        <p
          class="min-w-0 flex-1 truncate text-sm font-medium text-neutral-800"
          :class="{ 'opacity-50': disabled }"
        >
          {{ multiple && fileNames.length ? fileNames.join(', ') : fileName }}
        </p>

        <button
          v-if="showPreview && !multiple"
          type="button"
          class="
            shrink-0 cursor-pointer text-sm font-medium text-primary
            hover:text-primary/80
          "
          @click="preview()"
        >
          {{ $t('content.preview') }}
        </button>


        <template v-if="!disabled">
          <button
            type="button"
            class="
              shrink-0 text-sm font-medium text-primary
              hover:text-primary/80
            "
            @click="fileInputRef?.click()"
          >
            {{ $t('button.replace') }}
          </button>

          <button
            type="button"
            class="
              shrink-0 text-sm text-error-600
              hover:text-error-500
            "
            @click="remove()"
          >
            {{ multiple ? $t('button.removeAll') : $t('button.remove') }}
          </button>
        </template>
      </div>
    </template>

    <p
      v-else
      class="text-center"
    >
      <span class="font-bold text-primary">{{ $t('content.clickToUpload') }}</span>

      <span class="text-gray-700">
        {{ $t('content.dragAndDropFormats') }}
      </span>
    </p>

    <BaseValidation
      v-if="sizeError"
      :message="$t('error.fileSizeExceeded')"
      custom-class="mt-2"
    />
  </div>
</template>

<script setup lang="ts">
  defineOptions({ name: 'FileUploader' })

  const props = withDefaults(defineProps<{
    accept?: string
    disabled?: boolean
    file?: File
    fileUrl?: string | null
    hasError?: boolean
    multiple?: boolean
    showPreview?: boolean
  }>(), {
    accept: '.pdf,.jpg,.jpeg,.png',
    file: undefined,
    fileUrl: null,
    multiple: false,
    showPreview: false,
  })

  const emit = defineEmits<{
    fileChange: [file: File]
    fileRemove: []
    filesChange: [files: FileList]
  }>()

  const MAX_FILE_SIZE = 10 * 1024 * 1024

  const fileInputRef = useTemplateRef<HTMLInputElement>('fileInputRef')
  const fileNames = ref<string[]>([])
  const sizeError = ref(false)

  const fileName = computed(() => {
    if (props.file?.name) return props.file.name
    if (!props.fileUrl) return ''

    const urlParts = props.fileUrl.split('/')

    return urlParts[urlParts.length - 1] || props.fileUrl
  })

  const hasAttachment = computed(() => !!props.file || !!props.fileUrl)

  function validateFileSize(files: FileList): boolean {
    sizeError.value = Object.values(files).some(file => file.size > MAX_FILE_SIZE)

    return !sizeError.value
  }

  function acceptFiles(files: FileList): boolean {
    if (!validateFileSize(files)) return false

    fileNames.value = Object.values(files).map(file => file.name)
    emit('fileChange', files[0] as File)
    emit('filesChange', files)

    return true
  }

  function onFileChange(event: Event): void {
    const files = (event.target as HTMLInputElement)?.files

    if (!files?.length) return
    if (acceptFiles(files)) return

    if (fileInputRef.value) {
      fileInputRef.value.value = ''
    }

    sizeError.value = false
  }

  function drop(event: DragEvent): void {
    const files = event.dataTransfer?.files
    if (!files?.length) return
    acceptFiles(files)
  }

  function preview(): void {
    if (props.file) {
      const url = URL.createObjectURL(props.file)
      window.open(url, '_blank', 'noopener')
      setTimeout(() => URL.revokeObjectURL(url), 60_000)

      return
    }

    if (props.fileUrl) {
      window.open(props.fileUrl, '_blank', 'noopener')
    }
  }

  function remove(): void {
    if (fileInputRef.value) {
      fileInputRef.value.value = ''
    }

    fileNames.value = []
    sizeError.value = false
    emit('fileRemove')
  }
</script>

<style scoped>

</style>
