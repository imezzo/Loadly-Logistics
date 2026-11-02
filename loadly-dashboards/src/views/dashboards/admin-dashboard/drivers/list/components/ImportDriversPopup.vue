<template>
  <BasePopup
    v-model="model"
    :show-close-button="true"
  >
    <div class="w-full px-6 py-4">
      <h2 class="mb-6 text-2xl font-bold text-neutral-800">
        {{ $t('title.importDrivers') }}
      </h2>

      <input
        ref="fileInputRef"
        type="file"
        accept=".xlsx,.xls"
        class="hidden"
        @change="onFileChange"
      >

      <button
        type="button"
        class="
          mb-6 flex w-full flex-col items-center justify-center gap-2 rounded-xl border-2
          border-dashed border-neutral-300 bg-neutral-100/50 py-10 transition-colors
          hover:border-primary hover:bg-primary/5
        "
        @click="fileInputRef?.click()"
      >
        <span class="text-sm font-medium text-neutral-700">
          {{ selectedFile ? selectedFile.name : $t('content.clickToUpload') }}
        </span>
        <span class="text-xs text-neutral-10">
          {{ $t('content.excelFilesOnly') }}
        </span>
      </button>

      <div class="flex justify-end gap-3">
        <button
          type="button"
          class="btn-secondary h-10 px-4"
          @click="onCancel"
        >
          {{ $t('button.cancel') }}
        </button>
        <button
          type="button"
          class="btn-primary h-10 px-4"
          :disabled="!selectedFile || isLoading"
          @click="onSubmit"
        >
          {{ isLoading ? $t('content.uploading') : $t('button.import') }}
        </button>
      </div>
    </div>
  </BasePopup>

  <PopupSuccess
    v-model="isSuccessPopupOpen"
    :message="successMessage"
  >
    <template #actions>
      <button
        type="button"
        class="btn-primary mt-4 px-6"
        @click="isSuccessPopupOpen = false"
      >
        {{ $t('button.close') }}
      </button>
    </template>
  </PopupSuccess>

  <PopupError
    v-model="isErrorPopupOpen"
    :message="errorMessage"
  />
</template>

<script setup lang="ts">
  import PopupSuccess from '@/components/popup/PopupSuccess.vue'
  import PopupError from '@/components/popup/PopupError.vue'
  import { useDriverStore } from '@/stores/admin-dashboard/driver'
  import { useLoading } from '@/composables/useLoading'

  const { t } = useI18n()
  const model = defineModel<boolean>('modelValue', { required: true })
  const emit = defineEmits<{ refresh: [] }>()

  const driverStore = useDriverStore()
  const { isLoading, setLoading } = useLoading()
  const fileInputRef = useTemplateRef<HTMLInputElement>('fileInputRef')

  const selectedFile = ref<File | null>(null)
  const successMessage = ref('')
  const errorMessage = ref<string | null>(null)
  const isSuccessPopupOpen = ref(false)
  const isErrorPopupOpen = ref(false)

  function onFileChange(event: Event): void {
    const input = event.target as HTMLInputElement
    const file = input.files?.[0]
    selectedFile.value = file ?? null
  }

  async function onSubmit(): Promise<void> {
    if (!selectedFile.value) return

    setLoading(true)

    const { success, message } = await driverStore.importDrivers(selectedFile.value)
    setLoading(false)

    if (success) {
      model.value = false
      selectedFile.value = null
      if (fileInputRef.value) fileInputRef.value.value = ''
      successMessage.value = t('success.importDriversSuccess')
      isSuccessPopupOpen.value = true
      emit('refresh')

      return
    }

    errorMessage.value = message || t('error.networkError')
    isErrorPopupOpen.value = true
  }

  function onCancel(): void {
    model.value = false
    selectedFile.value = null
    if (fileInputRef.value) fileInputRef.value.value = ''
  }

  watch(model, (isOpen) => {
    if (!isOpen) {
      selectedFile.value = null
    }
  })
</script>

<style scoped>

</style>
