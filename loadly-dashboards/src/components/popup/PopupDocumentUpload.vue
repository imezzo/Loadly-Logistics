<template>
  <BasePopup
    v-model="model"
    :show-close-button="false"
    content-class="max-w-3/4! md:max-w-200!"
  >
    <div class="w-full px-6 py-4">
      <h2 class="mb-6 text-2xl font-bold text-neutral-800">
        {{ $t('content.documentUpload') }}
      </h2>

      <div class="space-y-6">
        <div>
          <label class="mb-1 block text-sm font-medium text-neutral-10">
            {{ $t('content.clickToUpload') }}
          </label>

          <Uploader
            class="w-full"
            :file="selectedFile"
            :has-error="false"
            @file-change="onFileChange"
            @file-remove="removeFile"
          />

          <BaseValidation :message="fileErrorMessage" />
        </div>

        <div>
          <label class="mb-1 block text-sm font-medium text-neutral-10">
            {{ $t('form.statusField') }}
          </label>
          <BaseSelect
            v-model="status"
            :options="documentStatusOptions"
            :placeholder="$t('form.selectStatus')"
            wrapper-class="[&>.select-button]:h-11 [&>.select-button]:rounded-lg"
          />

          <BaseValidation :message="statusErrorMessage" />
        </div>

        <div>
          <label class="mb-1 block text-sm font-medium text-neutral-10">
            {{ $t('form.expirationDate') }}
          </label>
          <BaseInput
            v-model="expirationDate"
            type="date"
            :min="getMinDate()"
            :placeholder="$t('form.datePlaceholder')"
          />

          <BaseValidation :message="expirationDateErrorMessage" />
        </div>
      </div>

      <BaseActions
        container-class="justify-center mt-6 gap-3"
        :prev-button-options="{ label: t('button.discard') }"
        :next-button-options="{ label: t('button.upload') }"
        @prev="discard"
        @next="onSubmit"
      />
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import type { SelectOption } from '@/types/ui'
  import { getMinDate } from '@/utils/date'

  const model = defineModel<boolean>('modelValue', { required: true })

  defineProps<{
    documentStatusOptions: SelectOption[]
  }>()

  const emit = defineEmits<{
    upload: [data: { file: File; status?: string; expirationDate?: string }]
  }>()

  const selectedFile = ref<File | undefined>()
  const status = ref<string>('')
  const expirationDate = ref<string>('')
  const fileErrorMessage = ref<string | null>(null)
  const statusErrorMessage = ref<string | null>(null)
  const expirationDateErrorMessage = ref<string | null>(null)

  function onFileChange(file: File): void {
    selectedFile.value = file
    fileErrorMessage.value = null
  }

  function removeFile(): void {
    selectedFile.value = undefined
    fileErrorMessage.value = null
  }

  const { t } = useI18n()

  function validateForm(): void {
    fileErrorMessage.value = !selectedFile.value
      ? t('validation.required', { field: t('form.file') })
      : null

    statusErrorMessage.value = !status.value
      ? t('validation.required', { field: t('form.statusField') })
      : null

    expirationDateErrorMessage.value = !expirationDate.value
      ? t('validation.required', { field: t('form.expirationDate') })
      : null
  }

  function onSubmit(): void {
    validateForm()

    if (fileErrorMessage.value
      || statusErrorMessage.value
      || expirationDateErrorMessage.value) {
      return
    }

    emit('upload', {
      file: selectedFile.value as File,
      status: status.value || undefined,
      expirationDate: expirationDate.value || undefined,
    })

    resetForm()
    model.value = false
  }

  function discard(): void {
    resetForm()
    model.value = false
  }

  function resetForm(): void {
    selectedFile.value = undefined
    status.value = ''
    expirationDate.value = ''
    fileErrorMessage.value = null
    statusErrorMessage.value = null
    expirationDateErrorMessage.value = null
  }

  watch(model, (newValue) => {
    if (!newValue) {
      resetForm()
    }
  })

  watch([status, expirationDate], () => {
    if (status.value) {
      statusErrorMessage.value = null
    }

    if (expirationDate.value) {
      expirationDateErrorMessage.value = null
    }
  })
</script>

<style scoped>

</style>

