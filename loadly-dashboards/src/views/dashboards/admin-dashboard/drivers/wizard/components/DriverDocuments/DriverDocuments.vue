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
      :disabled="nextButtonDisabled"
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
      :disabled="nextButtonDisabled"
      @update="onDocumentUpdate"
    />

    <p
      v-if="!requiredDocuments.length && !optionalDocuments.length"
      class="text-neutral-10"
    >
      {{ $t('content.noData') }}
    </p>

    <BaseActions
      container-class="flex items-center justify-end gap-3 sm:gap-6"
      :prev-button-options="prevButtonOptions"
      :next-button-options="nextButtonOptions"
      @prev="$emit('prev', documents)"
      @next="onNext"
    />
  </div>
</template>

<script setup lang="ts">
  import { useDriverDocuments } from '../../composables/useDriverDocuments'
  import type { DriverDocumentItem } from '../../composables/useDriverDocuments'
  import { useEnum } from '@/composables/useEnum'
  import type { SelectOption } from '@/types/ui'
  import type { DriverDocument } from '@/types/admin-dashboard/response/driver/driver-detail'
  import type { DocumentUploaderItem } from '@/types/documents.types'

  const props = defineProps<{
    nextButtonLabel: string
    nextButtonDisabled: boolean
    initialDocuments?: DriverDocument[] | DriverDocumentItem[]
  }>()

  const emit = defineEmits<{
    submit: [data: Record<string, unknown>]
    prev: [documents: DriverDocumentItem[]]
  }>()

  const { t } = useI18n()
  const { enums } = useEnum()
  const documentStatusOptions = computed<SelectOption[]>(() => enums.value?.DocumentVerificationStatus ?? [])
  const initialDocumentsComputed = computed(() => props.initialDocuments)

  const {
    documents,
    requiredDocuments,
    optionalDocuments,
    fetchDocuments,
    updateDocument,
    validateRequiredDocuments,
    updateFormData,
  } = useDriverDocuments(
    emit,
    initialDocumentsComputed
  )

  const validationError = ref<string | null>(null)

  const prevButtonOptions = computed(() => ({
    label: t('button.back'),
    class: 'flex h-13.5 w-1/2 items-center justify-center gap-2 rounded-2xl bg-black/15 px-4 py-4.25 font-medium text-content-secondary transition-colors hover:bg-black/20 sm:w-52.5',
    hasScrollTop: true,
  }))

  const nextButtonOptions = computed(() => ({
    label: props.nextButtonLabel,
    class: 'w-1/2 h-13.5 btn-primary py-3.75 sm:w-75',
    disabled: props.nextButtonDisabled,
  }))

  function onDocumentUpdate(index: number, doc: DocumentUploaderItem): void {
    updateDocument(index, doc)
  }

  function onNext(): void {
    if (props.nextButtonDisabled) {
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
    updateFormData()
  }

  watch(() => documents.value, () => {
    if (!validationError.value) return

    const validation = validateRequiredDocuments()
    if (!validation.isValid) return

    validationError.value = null
  }, { deep: true })

  watch(() => props.initialDocuments, async () => {
    await fetchDocuments()
  }, { deep: true, immediate: false })

  onMounted(async () => {
    await fetchDocuments()
  })
</script>
