<template>
  <BasePopup v-model="model">
    <div class="w-full p-4">
      <h2 class="mb-6 text-2xl font-bold text-neutral-800">
        {{ popupTitle }}
      </h2>

      <BaseAlert
        v-if="errorMessage"
        :model-value="!!errorMessage"
        :message="errorMessage"
        variant="error"
        class="mb-4"
      />

      <BaseForm
        ref="form"
        :validation-schema="validationSchema"
        :initial-values="initialValues"
        :fields-schema="fieldsSchema"
        :errors="formErrors"
        container-class="grid grid-cols-2 gap-4"
        @submit="submit"
      >
        <template #submit>
          <BaseActions
            container-class="mt-6 justify-end gap-3"
            :prev-button-options="prevButtonOptions"
            :next-button-options="nextButtonOptions"
            @prev="discard"
          />
        </template>
      </BaseForm>
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import type { VehicleTag } from '@/types/admin-dashboard/response/vehicle/vehicle-tag'
  import type { VehicleTagRequest } from '@/types/admin-dashboard/request/vehicle-tag-settings'
  import type { ApiResponse } from '@/types/admin-dashboard/response/response'
  import { useVeeValidate } from '@/composables/useVeeValidate'
  import { useLoading } from '@/composables/useLoading'
  import { useVehicleTagSettingsStore } from '@/stores/admin-dashboard/vehicle-tag-settings'
  import { useList } from '../composables/useList'
  import { useVehicleTagWizard } from '../composables/useVehicleTagWizard'

  const props = defineProps<{
    vehicleTag: VehicleTag | null
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })
  const form = useTemplateRef('form')

  useVeeValidate()

  const { t } = useI18n()
  const { isLoading, setLoading } = useLoading()
  const { fetchVehicleTags } = useList()
  const vehicleTagSettingsStore = useVehicleTagSettingsStore()
  const { fieldsSchema, initialValues, validationSchema } = useVehicleTagWizard(
    computed(() => props.vehicleTag)
  )

  const photo = ref<File | undefined>(undefined)
  const errorMessage = ref<string | null>(null)
  const formErrors = ref<Record<string, string[]> | undefined>()

  const popupTitle = computed(() => props.vehicleTag
    ? t('title.editVehicleTags')
    : t('title.addVehicleTags'))

  const prevButtonOptions = computed(() => ({
    class: 'btn-secondary h-13.5 w-52.5',
    label: t('button.discard'),
  }))

  const nextButtonOptions = computed(() => ({
    asSubmit: true,
    class: 'btn-primary h-13.5 w-52.5',
    disabled: isLoading.value,
    label: t('button.save'),
  }))

  function buildRequest(values: Record<string, unknown>): VehicleTagRequest {
    return {
      isPricing: Boolean(values.isPricing),
      nameAr: String(values.nameAr ?? ''),
      nameEn: String(values.nameEn ?? ''),
    }
  }

  async function submit(values: Record<string, unknown>): Promise<void> {
    const request = buildRequest(values)

    setLoading(true)
    const response = props.vehicleTag
      ? await vehicleTagSettingsStore.update(props.vehicleTag.id, request)
      : await vehicleTagSettingsStore.create(request)
    setLoading(false)

    handleSubmitResponse(response)
  }

  function handleSubmitResponse(response: ApiResponse<unknown>): void {
    if (response.success) {
      discard()
      fetchVehicleTags()

      return
    }

    errorMessage.value = response.message || t('error.networkError')
    formErrors.value = response.errors || {}
  }

  function discard(): void {
    form.value?.form?.resetForm()
    photo.value = undefined
    errorMessage.value = null
    formErrors.value = undefined
    model.value = false
  }
</script>

<style scoped>

</style>
