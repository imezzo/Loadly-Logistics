<template>
  <div class="flex flex-col">
    <h1 class="mb-6 font-bold text-neutral-10">
      {{ $t('content.documentUpload') }}
    </h1>

    <h2
      v-if="requiredDocuments.length"
      class="mb-4 text-lg font-bold text-neutral-10"
    >
      {{ $t('content.requiredDocumentsSection') }}
    </h2>

    <UploaderItem
      v-for="(doc, index) in requiredDocuments"
      :key="`required-${index}`"
      :doc="doc"
      :index="documents.indexOf(doc)"
      :document-status-options="documentStatusOptions"
      :show-validation-error="!!validationError"
      :disabled="isFormDisabled"
      show-preview
      @update="onDocumentUpdate"
    />

    <hr
      v-if="requiredDocuments.length && optionalDocuments.length"
      class="mb-4 w-full shrink-0 border-neutral-300"
    >

    <h2
      v-if="optionalDocuments.length"
      class="mb-4 text-lg font-bold text-neutral-10"
    >
      {{ $t('content.optionalDocumentsSection') }}
    </h2>

    <UploaderItem
      v-for="(doc, index) in optionalDocuments"
      :key="`optional-${index}`"
      :doc="doc"
      :index="documents.indexOf(doc)"
      :document-status-options="documentStatusOptions"
      :show-validation-error="!!validationError"
      :disabled="isFormDisabled"
      show-preview
      @update="onDocumentUpdate"
    />

    <p
      v-if="!requiredDocuments.length && !optionalDocuments.length"
      class="text-neutral-10"
    >
      {{ $t('content.noData') }}
    </p>

    <BaseActions
      :prev-button-options="{ hasScrollTop: true }"
      :next-button-options="{ label: saveButtonTitle, hasScrollTop: true, disabled: hasUploadingDocuments }"
      @prev="emit('prev')"
      @next="onNext"
    />
  </div>
</template>

<script setup lang="ts">
  import { useContractDocuments } from '../../composables/useContractDocuments'
  import { useEnum } from '@/composables/useEnum'
  import type { SelectOption } from '@/types/ui'
  import type { DocumentUploaderItem } from '@/types/documents.types'
  import { useContractWizard } from '../../composables/useContractWizard'

  defineProps<{
    errors?: Record<string, string[]>
  }>()

  const emit = defineEmits<{
    update: [data: Record<string, unknown>]
    next: []
    prev: []
    discard: []
  }>()

  const { t } = useI18n()
  const { enums } = useEnum()
  const documentStatusOptions = computed<SelectOption[]>(() => enums.value?.DocumentVerificationStatus ?? [])
  const { formData, isFormDisabled, saveButtonTitle } = useContractWizard()

  const {
    documents,
    requiredDocuments,
    optionalDocuments,
    fetchDocuments,
    restoreFormValues,
    updateDocument,
    validateRequiredDocuments,
  } = useContractDocuments(emit)

  const validationError = ref<string | null>(null)
  const hasUploadingDocuments = computed(() =>
    documents.value.some(doc => doc.uploadState === 'uploading')
  )

  function onDocumentUpdate(index: number, doc: DocumentUploaderItem): void {
    updateDocument(index, doc)
  }

  function onNext(): void {
    if (isFormDisabled.value) emit('next')
    if (hasUploadingDocuments.value || isFormDisabled.value) return

    const validation = validateRequiredDocuments()

    if (!validation.isValid) {
      validationError.value = t('error.missingRequiredDocuments', {
        documents: validation.missingDocuments.join(', '),
      })

      return
    }

    validationError.value = null
    emit('next')
  }

  defineExpose({ submitStep: onNext })

  watch(() => documents.value, () => {
    if (!validationError.value) return

    const validation = validateRequiredDocuments()
    if (!validation.isValid) return

    validationError.value = null
  }, { deep: true })

  onMounted(async () => {
    await fetchDocuments()

    if (formData.value?._documents && Array.isArray(formData.value._documents)) {
      restoreFormValues(formData.value)
    }
  })
</script>

<style scoped>

</style>
