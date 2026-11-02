<template>
  <BasePopup
    v-model="model"
    :show-close-button="false"
  >
    <div class="w-full px-6 py-4">
      <h2 class="mb-6 text-2xl font-bold text-neutral-800">
        {{ $t('title.editVehicleBrandModel') }}
      </h2>

      <BaseAlert
        v-if="errorMessage"
        :model-value="!!errorMessage"
        :message="errorMessage"
        variant="error"
        class="mb-4 w-full"
      />

      <BaseForm
        ref="form"
        :validation-schema="validationSchema"
        :initial-values="initialValues"
        :fields-schema="fieldsSchema"
        :errors="formErrors"
        @submit="onSubmit"
      >
        <template #submit>
          <BaseActions
            container-class="mt-6 justify-center gap-3"
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
  import { object, string } from 'yup'
  import type { VehicleBrandModel } from '@/types/admin-dashboard/response/vehicle/vehicle-brand-model'
  import type { VehicleBrand } from '@/types/admin-dashboard/response/vehicle/vehicle-brand'
  import { useVeeValidate } from '@/composables/useVeeValidate'
  import { useVehicleBrandModelSettingsStore } from '@/stores/admin-dashboard/vehicle-brand-model-settings'
  import { useLoading } from '@/composables/useLoading'

  useVeeValidate()
  const { t } = useI18n()

  const props = defineProps<{
    vehicleBrandModel: VehicleBrandModel | null
    vehicleBrands: VehicleBrand[]
  }>()

  const emit = defineEmits<{
    refresh: []
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })

  const form = useTemplateRef('form')
  const vehicleBrandModelSettingsStore = useVehicleBrandModelSettingsStore()
  const { isLoading, setLoading } = useLoading()

  const errorMessage = ref<string | null>(null)
  const formErrors = ref<Record<string, string[]> | undefined>()

  const prevButtonOptions = computed(() => ({
    label: t('button.discard'),
    class: 'flex h-13.5 w-52.5 items-center justify-center rounded-2xl bg-black/15 px-6 font-medium text-content-secondary transition-colors hover:bg-black/20',
  }))

  const nextButtonOptions = computed(() => ({
    label: t('button.saveChanges'),
    class: 'flex h-13.5 w-52.5 items-center justify-center rounded-2xl bg-primary px-6 font-medium text-white transition-colors hover:bg-primary/90',
    disabled: isLoading.value,
    asSubmit: true,
  }))

  const vehicleBrandOptions = computed(() =>
    (props.vehicleBrands ?? []).map((b) => ({
      value: String(b.id),
      label: b.name,
    }))
  )

  const initialValues = computed(() => ({
    nameEn: props.vehicleBrandModel?.nameEn || '',
    nameAr: props.vehicleBrandModel?.nameAr || '',
    vehicleBrandId: props.vehicleBrandModel ? String(props.vehicleBrandModel.vehicleBrandId) : '',
  }))

  const validationSchema = computed(() =>
    object({
      nameEn: string()
        .required()
        .max(255),
      nameAr: string()
        .required()
        .max(255),
      vehicleBrandId: string()
        .required(),
    })
  )

  const fieldsSchema = computed(() => [
    {
      name: 'nameEn',
      as: 'BaseInput',
      type: 'text',
      placeholder: t('form.nameEn'),
      label: t('form.nameEn'),
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
      labelRequired: true,
      containerClass: 'w-full',
    },
    {
      name: 'nameAr',
      as: 'BaseInput',
      type: 'text',
      placeholder: t('form.nameAr'),
      label: t('form.nameAr'),
      labelClass: 'mb-1 mt-3 flex items-center font-medium text-neutral-10',
      labelRequired: true,
      containerClass: 'w-full',
    },
    {
      name: 'vehicleBrandId',
      as: 'BaseSelect',
      options: vehicleBrandOptions.value,
      label: t('form.vehicleBrand'),
      labelClass: 'mb-1 mt-3 flex items-center font-medium text-neutral-10',
      labelRequired: true,
      containerClass: 'w-full',
    },
  ])

  async function onSubmit(values: Record<string, unknown>) {
    if (!props.vehicleBrandModel) return

    errorMessage.value = null
    setLoading(true)

    const payload = {
      nameAr: values.nameAr as string,
      nameEn: values.nameEn as string,
      vehicleBrandId: Number(values.vehicleBrandId),
    }

    const { success, message, errors } =
      await vehicleBrandModelSettingsStore.update(
        props.vehicleBrandModel.id,
        payload
      )
    setLoading(false)

    if (success) {
      form.value?.form?.resetForm()
      model.value = false
      emit('refresh')

      return
    }

    errorMessage.value = message || t('error.networkError')
    formErrors.value = errors || {}
  }

  function discard() {
    form.value?.form?.resetForm()
    errorMessage.value = null
    model.value = false
  }

  watch(
    () => model.value,
    () => {
      if (!model.value && form.value) {
        form.value.form?.resetForm()
        errorMessage.value = null
      }
    }
  )
</script>

<style scoped>

</style>
