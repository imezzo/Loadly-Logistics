<template>
  <BasePopup
    v-model="model"
    show-close-button
  >
    <div class="w-full px-6 py-4">
      <h2 class="mb-6 text-2xl font-bold text-neutral-800">
        {{ $t('title.importVehicles') }}
      </h2>

      <ExcelFileUpload
        ref="excelFileUploadRef"
        v-model="selectedFile"
      />

      <BaseActions
        container-class="justify-end gap-3 mt-0"
        :prev-button-options="prevButtonOptions"
        :next-button-options="nextButtonOptions"
        @prev="onCancel"
        @next="onSubmit"
      />
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
  import ExcelFileUpload from './ExcelFileUpload.vue'
  import { useVehicleStore } from '@/stores/admin-dashboard/vehicle'
  import { useLoading } from '@/composables/useLoading'

  const emit = defineEmits<{ refresh: [] }>()

  const model = defineModel<boolean>('modelValue', { required: true })
  const excelFileUploadRef = useTemplateRef<InstanceType<typeof ExcelFileUpload>>('excelFileUploadRef')

  const { t } = useI18n()
  const { isLoading, setLoading } = useLoading()

  const vehicleStore = useVehicleStore()

  const selectedFile = ref<File | null>(null)
  const successMessage = ref('')
  const errorMessage = ref<string | null>(null)
  const isSuccessPopupOpen = ref(false)
  const isErrorPopupOpen = ref(false)

  const prevButtonOptions = computed(() => ({
    label: t('button.cancel'),
    class: 'btn-secondary h-10 px-4',
  }))

  const nextButtonOptions = computed(() => ({
    label: isLoading.value ? t('content.uploading') : t('button.import'),
    class: 'btn-primary h-10 px-4',
    disabled: !selectedFile.value || isLoading.value,
  }))

  async function onSubmit(): Promise<void> {
    if (!selectedFile.value) return

    setLoading(true)

    const formData = new FormData()
    formData.append('ExcelFile', selectedFile.value)

    const { success, message } = await vehicleStore.importVehicles(formData)
    setLoading(false)

    if (success) {
      onCancel()
      successMessage.value = t('success.importVehiclesSuccess')
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
    excelFileUploadRef.value?.reset()
  }

  watch(model, (isOpen) => {
    if (!isOpen) {
      selectedFile.value = null
    }
  })
</script>

<style scoped>

</style>
