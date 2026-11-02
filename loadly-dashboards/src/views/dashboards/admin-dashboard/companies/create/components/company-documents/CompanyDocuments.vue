<template>
  <div class="space-y-6">
    <UploaderItem
      v-for="(doc, index) in documents"
      :key="index"
      :doc="doc"
      :index="index"
      :document-status-options="documentStatusOptions"
      :show-validation-error="!!validationError"
      @update="onDocumentUpdate"
    />

    <BaseActions
      :prev-button-options="{ hasScrollTop: true }"
      :next-button-options="{ hasScrollTop: true }"
      @prev="emit('prev')"
      @next="next"
      @submit="onSubmit"
    />
  </div>
</template>

<script setup lang="ts">
  import UploaderItem from '@/components/uploader/UploaderItem.vue'
  import { useDocuments } from '../../composables/useDocuments'
  import type { SelectOption } from '@/types/ui'
  import { useEnum } from '@/composables/useEnum'

  interface Document {
    file?: File
    expirationDate?: string
    status?: string
    id?: number
  }

  const emit = defineEmits<{
    update: [data: Record<string, unknown>]
    prev: []
    next: []
    submit: []
  }>()

  const props = defineProps<{
    formValues?: Record<string, unknown>
  }>()

  const { t } = useI18n()
  const { enums } = useEnum()
  const {
    documents,
    fetchDocuments,
    validateRequiredDocuments,
    updateDocument,
    getSavedFiles,
  } = useDocuments(emit)

  const documentStatusOptions = computed<SelectOption[]>(() => enums.value?.DocumentVerificationStatus ?? [])

  const validationError = ref<string | null>(null)

  function onDocumentUpdate(index: number, doc: Document): void {
    updateDocument(index, doc)
  }

  function next(): void {
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

  function onSubmit(): void {
    const validation = validateRequiredDocuments()

    if (!validation.isValid) {
      validationError.value = t('error.missingRequiredDocuments', {
        documents: validation.missingDocuments.join(', '),
      })

      return
    }

    validationError.value = null
    emit('submit')
  }

  watch(() => documents.value, () => {
    if (validationError.value) {
      const validation = validateRequiredDocuments()
      if (validation.isValid) {
        validationError.value = null
      }
    }
  }, { deep: true })


  onMounted(async () => {
    await fetchDocuments()
    if (props.formValues) {
      getSavedFiles(props.formValues)
    }
  })

  watch(() => props.formValues, (newFormValues) => {
    if (newFormValues && documents.value.length > 0) {
      getSavedFiles(newFormValues)
    }
  }, { deep: true })
</script>

<style scoped>

</style>

