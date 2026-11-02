<template>
  <BasePopup v-model="model">
    <div class="w-full p-4">
      <h2 class="mb-6 text-2xl font-bold text-neutral-800">
        {{ $t('title.editVehicleType') }}
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
        :validation-schema
        :initial-values
        :fields-schema
        :errors="formErrors"
        container-class="grid grid-cols-2 gap-4"
        @submit="submit"
      >
        <template #submit>
          <BaseActions
            container-class="mt-6 justify-center gap-3"
            :prev-button-options
            :next-button-options
            @prev="discard"
          />
        </template>
      </BaseForm>
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import { useVeeValidate } from '@/composables/useVeeValidate'
  import { useLoading } from '@/composables/useLoading'
  import { useVehicleTypeSettingsStore } from '@/stores/admin-dashboard/vehicle-type-settings'
  import type { VehicleType } from '@/types/admin-dashboard/response/vehicle/vehicle-type'
  import type { VehicleTypeWizardRequest } from '@/types/admin-dashboard/request/vehicle-type-settings'
  import { useList } from '../composables/useList'
  import type { ApiResponse } from '@/types/admin-dashboard/response/response'
  import { useVehicleTypesWizard } from '../composables/useVehicleTypesWizard'

  const props = defineProps<{
    vehicleType: VehicleType | null
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })
  const form = useTemplateRef('form')

  useVeeValidate()
  const { t } = useI18n()
  const { isLoading, setLoading } = useLoading()
  const { fetchVehicleTypes } = useList()
  const { validationSchema, fieldsSchema, initialValues } = useVehicleTypesWizard(computed(() => props.vehicleType))

  const vehicleTypeSettingsStore = useVehicleTypeSettingsStore()

  const errorMessage = ref<string | null>(null)
  const formErrors = ref<Record<string, string[]> | undefined>()

  const prevButtonOptions = computed(() => ({
    label: t('button.discard'),
    class: 'btn-secondary w-52.5 h-13.5',
  }))

  const nextButtonOptions = computed(() => ({
    label: t('button.saveChanges'),
    class: 'btn-primary w-52.5 h-13.5',
    disabled: isLoading.value,
    asSubmit: true,
  }))

  function buildVehicleTypeRequest(values: Record<string, unknown>): VehicleTypeWizardRequest {
    return {
      category: Number(values.category),
      nameAr: String(values.nameAr),
      nameEn: String(values.nameEn),
      status: Number(values.status),
      tagIds: Array.isArray(values.tagIds) ? values.tagIds.map(Number) : [],
      vehicleUnitType: Number(values.vehicleUnitType),
    }
  }

  async function submit(values: Record<string, unknown>): Promise<void> {
    const request = buildVehicleTypeRequest(values)

    setLoading(true)
    const response = props.vehicleType
      ? await vehicleTypeSettingsStore.update(props.vehicleType.id, request)
      : await vehicleTypeSettingsStore.create(request)
    setLoading(false)

    handleSubmitResponse(response)
  }

  function handleSubmitResponse(response: ApiResponse<VehicleType | null>): void {
    if (response.success) {
      discard()
      fetchVehicleTypes()

      return
    }

    errorMessage.value = response.message || t('error.networkError')
    formErrors.value = response.errors || {}
  }

  function discard(): void {
    form.value?.form?.resetForm()
    errorMessage.value = null
    model.value = false
  }
</script>

<style scoped>

</style>
