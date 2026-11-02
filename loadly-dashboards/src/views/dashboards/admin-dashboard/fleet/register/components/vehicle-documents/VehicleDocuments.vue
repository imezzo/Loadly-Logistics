<template>
  <div class="flex w-full flex-col">
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
      @update="onDocumentUpdate"
    />

    <p
      v-if="!requiredDocuments.length && !optionalDocuments.length"
      class="text-neutral-10"
    >
      {{ $t('content.noData') }}
    </p>

    <BaseActions
      :has-next-button="!isFormDisabled"
      :next-button-options="isFormDisabled ? undefined : {}"
      :prev-button-options="{ hasScrollTop: true }"
      @prev="emit('prev')"
      @next="onNext"
    />
  </div>
</template>

<script setup lang="ts">
  import UploaderItem from '@/components/uploader/UploaderItem.vue'
  import { useVehicleDocuments } from '../../composables/useVehicleDocuments'
  import { useEnum } from '@/composables/useEnum'
  import type { SelectOption } from '@/types/ui'
  import { useVehicleWizard } from '../../composables/useVehicleWizard'
  import type { ApiDocumentInfo } from '@/types/admin-dashboard/response/document'
  import type { DocumentUploaderItem } from '@/types/documents.types'

  const props = defineProps<{
    formValues?: Record<string, unknown>
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
  const { isFormDisabled } = useVehicleWizard()

  const initialDocuments = computed<ApiDocumentInfo[] | undefined>(
    () => (props.formValues?._documents as ApiDocumentInfo[] | undefined)
  )

  const {
    documents,
    requiredDocuments,
    optionalDocuments,
    fetchDocuments,
    updateDocument,
    validateRequiredDocuments,
  } = useVehicleDocuments(emit, initialDocuments)

  const validationError = ref<string | null>(null)

  function onDocumentUpdate(index: number, doc: DocumentUploaderItem): void {
    updateDocument(index, doc)
  }

  function onNext(): void {
    if (isFormDisabled.value) {
      emit('next')

      return
    }

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

  watch(() => documents.value, () => {
    if (!validationError.value) return

    const validation = validateRequiredDocuments()
    if (!validation.isValid) return

    validationError.value = null
  }, { deep: true })

  onMounted(async () => {
    await fetchDocuments()
  })
</script>

<style scoped>

</style>
