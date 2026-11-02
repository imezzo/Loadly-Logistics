<template>
  <LazyPopupSuccess
    v-if="isSuccess"
    :model-value="isSuccess"
    @update:model-value="onSuccess"
  >
    <template #message>
      <p class="text-center text-2xl font-bold text-neutral-10">
        {{ isEditMode ? $t('success.contractorUpdated') : $t('success.contractorCreated') }}
      </p>
    </template>
  </LazyPopupSuccess>

  <BaseAlert
    v-if="errorMessage"
    :model-value="!!errorMessage"
    :message="errorMessage"
    variant="error"
    class="mb-4 w-full"
  />

  <BaseForm
    ref="form"
    :validation-schema
    :initial-values
    :groups="fieldsGroup"
    :errors="formErrors"
    @submit="onSubmit"
  >
    <template
      v-if="contractor?.documents"
      #field-nationalId
    >
      <UploaderItem
        v-for="(doc, index) in contractor?.documents"
        :key="index"
        :doc="doc as DocumentUploaderItem"
        :index
        :disabled="isFormDisabled"
        :document-status-options
        :show-validation-error="!!validationError"
        show-preview
        class="mt-5"
        @update="onDocumentUpdate"
      />
    </template>

    <template #submit="{ meta }">
      <BaseActions
        container-class="flex items-center justify-end gap-3 sm:gap-6"
        :prev-button-options
        :next-button-options="isViewMode
          ? undefined
          : {
            ...nextButtonOptions,
            disabled: isLoading || (isEditMode && !meta.dirty)
          }"
        @prev="router.push({ name: 'ContractorsList' })"
      />
    </template>
  </BaseForm>
</template>

<script setup lang="ts">
  import { useContractorForm } from '../composables/useContractorForm'
  import { useContractorStore } from '@/stores/admin-dashboard/contractor'
  import type { DocumentUploaderItem } from '@/types/documents.types'
  import { useDocumentsUploader } from '@/composables/useDocumentsUploader'

  const LazyPopupSuccess = defineAsyncComponent(() => import('@/components/popup/PopupSuccess.vue'))

  const route = useRoute()
  const router = useRouter()
  const contractorId = computed(() => route.params.id as string | undefined)
  const {
    errorMessage,
    fieldsGroup,
    formErrors,
    getContractor,
    initialValues,
    isEditMode,
    isFormDisabled,
    isLoading,
    isSuccess,
    isViewMode,
    onSubmit,
    validationSchema,
  } = useContractorForm(contractorId.value)
  const { t } = useI18n()

  const contractorStore = useContractorStore()
  const { contractor } = storeToRefs(contractorStore)

  const {
    fetchDocuments,
    documentStatusOptions,
    validationError,
    onDocumentUpdate,
    initializeDocuments,
  } = useDocumentsUploader(contractor)

  const prevButtonOptions = computed(() => ({
    label: t('button.discard'),
    class: 'flex h-13.5 w-1/2 items-center justify-center gap-2 rounded-2xl bg-black/15'
      + 'px-4 py-4.25 font-medium text-content-secondary transition-colors hover:bg-black/20 sm:w-52.5',
  }))

  const nextButtonOptions = computed(() => ({
    asSubmit: true,
    class: 'w-1/2 py-3.75 btn-primary h-13.5 sm:w-75 bg-primary hover:bg-primary/90',
    label: isEditMode.value
      ? t('button.saveChanges')
      : t('button.createContractor'),
  }))

  function onSuccess() {
    isSuccess.value = false
    router.push({ name: 'ContractorsList' })
  }

  onMounted(async () => {
    await fetchDocuments(7)

    if (isEditMode.value || isViewMode.value) {
      await getContractor()
    }

    initializeDocuments()
  })

  onUnmounted(() => {
    contractor.value = null
  })
</script>

<style scoped>

</style>
