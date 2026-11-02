<template>
  <div class="mb-2 flex flex-wrap items-center gap-2">
    <label class="me-auto block font-medium text-neutral-10">
      {{ doc.label }}
      <span
        v-if="doc.required"
        class="text-error-600"
      >*</span>
    </label>

    <UploaderDocumentStatus
      v-if="localDoc.file
        || localDoc.fileUrl
        || localDoc.uploadState === 'uploading'"
      class="mb-3 w-full shrink-0"
      :file-name="fileName"
      :upload-progress="localDoc.uploadState === 'uploading'
        ? localDoc.uploadProgress
        : undefined"
      :document-status-options="documentStatusOptions"
      :show-replace="localDoc.uploadState !== 'uploading'"
      :show-preview
      :document="localDoc"
      :disabled="!disabled"
      @remove="isDeletionPopupOpen = true"
      @replace="isReplacePopupOpen = true"
    />

    <button
      v-else
      type="button"
      class="
        btn-primary-outline flex h-11 min-w-1/3 items-center justify-center rounded-lg px-4 text-sm
        font-medium
      "
      :class="{'border-error-600 text-error-600': showValidationError && hasError}"
      :disabled="disabled"
      @click="isUploadPopupOpen = true"
    >
      {{ $t('button.upload') }}
    </button>

    <BaseValidation
      v-if="showValidationError && hasError"
      :message="$t('error.missingRequiredDocument', { document: doc.label })"
      class="w-full"
    />

    <PopupDocumentUpload
      v-if="!disabled"
      v-model="isUploadPopupOpen"
      :document-status-options="documentStatusOptions"
      @upload="onUpload"
    />

    <PopupDocumentUpload
      v-if="!disabled"
      v-model="isReplacePopupOpen"
      :document-status-options="documentStatusOptions"
      @upload="onReplace"
    />

    <PopupDocumentDeletion
      v-if="!disabled"
      v-model="isDeletionPopupOpen"
      :document-status-options="documentStatusOptions"
      @delete="onDeleteDocument"
    />
  </div>
</template>

<script setup lang="ts">
  import type { SelectOption } from '@/types/ui'
  import { useUploadSimulator } from '@/composables/useUploadSimulator'
  import PopupDocumentDeletion from '@/components/popup/PopupDocumentDeletion.vue'
  import type { DocumentUploaderItem } from '@/types/documents.types'
  import { extractFileName } from '@/utils/file'

  const props = defineProps<{
    disabled?: boolean
    doc: DocumentUploaderItem
    documentStatusOptions: SelectOption[]
    index: number
    showPreview?: boolean
    showValidationError?: boolean
  }>()

  const emit = defineEmits<{
    update: [index: number, doc: DocumentUploaderItem]
  }>()

  const localDoc = ref<DocumentUploaderItem>({ ...props.doc })
  const isUploadPopupOpen = ref(false)
  const isReplacePopupOpen = ref(false)
  const isDeletionPopupOpen = ref(false)
  const MAX_FILE_SIZE = 10 * 1024 * 1024

  watch(() => props.doc, (newDoc) => {
    localDoc.value = { ...newDoc }
  }, { deep: true })

  const fileName = computed(() => {
    if (localDoc.value.file) return localDoc.value.file.name

    return extractFileName(localDoc.value.fileUrl || '')
  })

  const hasError = computed(() => {
    return props.doc.required && !localDoc.value.file && !localDoc.value.fileUrl
  })

  function emitUpdate(): void {
    emit('update', props.index, { ...localDoc.value })
  }

  const uploadProgress = computed({
    get: () => localDoc.value.uploadProgress,
    set: (value) => {
      localDoc.value.uploadProgress = value
    },
  })

  const uploadState = computed({
    get: () => localDoc.value.uploadState,
    set: (value) => {
      localDoc.value.uploadState = value
    },
  })

  const { startSimulation } = useUploadSimulator(
    uploadProgress,
    uploadState,
    emitUpdate
  )

  function startUploadFlow(): void {
    localDoc.value.uploadState = 'uploading'
    localDoc.value.uploadProgress = 0
    emitUpdate()
    startSimulation()
  }

  function onUpload(data: { file: File; status?: string; expirationDate?: string }): void {
    if (data.file.size > MAX_FILE_SIZE) {
      return
    }

    localDoc.value.file = data.file
    localDoc.value.status = data.status
    localDoc.value.expirationDate = data.expirationDate
    startUploadFlow()
  }

  function onReplace(data: { file: File; status?: string; expirationDate?: string }): void {
    if (data.file.size > MAX_FILE_SIZE) {
      return
    }

    localDoc.value.file = data.file
    if (data.status) {
      localDoc.value.status = data.status
    }
    if (data.expirationDate) {
      localDoc.value.expirationDate = data.expirationDate
    }
    startUploadFlow()
  }

  function onDeleteDocument(): void {
    localDoc.value.file = undefined
    localDoc.value.fileUrl = null
    localDoc.value.id = undefined
    localDoc.value.status = undefined
    localDoc.value.expirationDate = undefined
    localDoc.value.uploadState = undefined
    localDoc.value.uploadProgress = undefined
    emitUpdate()
  }
</script>

<style scoped>

</style>
