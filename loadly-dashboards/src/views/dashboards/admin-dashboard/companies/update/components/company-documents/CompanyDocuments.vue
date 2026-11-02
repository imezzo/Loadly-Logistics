<template>
  <div class="space-y-6">
    <h4 class="mb-4 font-bold text-neutral-10">
      {{ $t('content.filesAdded') }}
    </h4>

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
  import type { DocumentInfo } from '@/types/admin-dashboard/response/document'

  interface Document {
    file?: File
    expirationDate?: string
    status?: string
    id?: number
  }

  const props = defineProps<{
    formValues?: Record<string, unknown>
  }>()

  const emit = defineEmits<{
    update: [data: Record<string, unknown>]
    prev: []
    next: []
    submit: []
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

  const isMerging = ref(false)

  function mergeDocuments() {
    if (isMerging.value) return
    const companyDocs = props.formValues?.documents as DocumentInfo[] | undefined
    if (!Array.isArray(companyDocs) || documents.value.length === 0) return

    isMerging.value = true

    const companyDocsMap = new Map<number, DocumentInfo>()
    companyDocs.forEach((doc: DocumentInfo) => {
      companyDocsMap.set(doc.documentId || 0, doc)
    })

    documents.value = documents.value.map((doc) => {
      const companyDoc = companyDocsMap.get(doc.id || 0)
      if (companyDoc) {
        return {
          ...doc,
          id: companyDoc.id,
          expirationDate: companyDoc.expirationDate || undefined,
          status: companyDoc.status || undefined,
          fileUrl: companyDoc.fileUrl || undefined,
        }
      }

      return doc
    })

    isMerging.value = false
  }

  watch(() => props.formValues?.documents, () => {
    if (documents.value.length > 0) {
      mergeDocuments()
    }
  }, { immediate: true, deep: true })

  watch(() => props.formValues, (formValues) => {
    if (documents.value.length > 0 && formValues) {
      getSavedFiles(formValues)
    }
  }, { immediate: true, deep: true })

  onMounted(async () => {
    await fetchDocuments()
    mergeDocuments()
    if (props.formValues) {
      getSavedFiles(props.formValues)
    }
  })
</script>

<style scoped>

</style>

