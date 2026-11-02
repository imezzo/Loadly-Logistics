<template>
  <BasePopup
    v-model="model"
    :show-close-button="false"
    content-class="max-w-3/4! md:max-w-200!"
  >
    <div class="w-full px-6 py-4">
      <h2 class="mb-6 text-2xl font-bold text-neutral-800">
        {{ $t('content.multipleLocations') }}
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
        container-class="flex flex-wrap justify-between"
        @submit="onSubmit"
      >
        <template #submit>
          <BaseActions
            container-class="mt-6 gap-3"
            :prev-button-options="{ label: t('button.cancel') }"
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
  import { useVeeValidate } from '@/composables/useVeeValidate'
  import type { Schema } from '@/types/form'
  import { useLookups } from '@/composables/useLookups'

  useVeeValidate()

  const { t } = useI18n()

  const model = defineModel<boolean>('modelValue', { required: true })

  const form = useTemplateRef('form')

  const errorMessage = ref<string | null>(null)

  const nextButtonOptions = computed(() => ({
    label: t('button.addAddress'),
    class: 'flex h-13.5 items-center justify-center rounded-2xl bg-primary px-6 py-3 text-sm font-medium text-white transition-colors hover:bg-primary/90 md:w-52.5',
    asSubmit: true,
  }))

  const {
    cities,
    governments,
    hasMoreCities,
    hasMoreGovernments,
    loadMoreCities,
    loadMoreGovernments,
    getCities,
  } = useLookups()

  const formValues = computed(() => form.value?.form?.values)

  watch(() => formValues.value?.governorate as string | undefined, async (newValue, oldValue) => {
    if (newValue && newValue !== oldValue) {
      const id = parseInt(newValue, 10)
      await getCities(id)
      if (form.value?.form) {
        form.value.form.setFieldValue('city', '')
      }
    }
  })

  const initialValues = {
    street: '',
    district: '',
    city: '',
    governorate: '',
    postalCode: '',
    gpsCoordinates: '',
  }

  const validationSchema = computed(() => object({
    street: string().required(),
    district: string().required(),
    city: string().required(),
    governorate: string().required(),
    postalCode: string().required(),
    gpsCoordinates: string().required(),
  }))

  const fieldsSchema = computed<Schema[]>(() => [
    {
      name: 'governorate',
      as: 'BaseSelect',
      containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
      label: t('form.governorate'),
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
      labelRequired: true,
      options: governments.value,
      onScrollToEnd: () => {
        if (hasMoreGovernments.value) {
          loadMoreGovernments()
        }
      },
    },
    {
      name: 'city',
      as: 'BaseSelect',
      containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
      label: t('form.city'),
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
      labelRequired: true,
      options: cities.value,
      onScrollToEnd: () => {
        if (hasMoreCities.value) {
          loadMoreCities()
        }
      },
    },
    {
      name: 'district',
      as: 'BaseInput',
      type: 'text',
      containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
      label: t('form.district'),
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
      labelRequired: true,
    },
    {
      name: 'street',
      as: 'BaseInput',
      type: 'text',
      label: t('form.street'),
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
      labelRequired: true,
      containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
    },
    {
      name: 'postalCode',
      as: 'BaseInput',
      type: 'text',
      label: t('form.postalCode'),
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
      labelRequired: true,
      containerClass: 'w-full',
    },
    {
      name: 'gpsCoordinates',
      as: 'BaseInput',
      type: 'text',
      label: t('form.gpsCoordinates'),
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
      labelRequired: true,
      containerClass: 'w-full',
    },
  ])

  const emit = defineEmits<{
    add: [location: {
      city: string
      governorate: string
      district: string
      street?: string
      postalCode?: string
      gpsCoordinates?: string
    }]
  }>()

  function onSubmit(values: Record<string, unknown>): void {
    errorMessage.value = null

    emit('add', {
      city: values.city as string,
      governorate: values.governorate as string,
      district: values.district as string,
      street: values.street as string,
      postalCode: values.postalCode as string,
      gpsCoordinates: values.gpsCoordinates as string,
    })

    form.value?.form?.resetForm()
    model.value = false
  }

  function discard(): void {
    form.value?.form?.resetForm()
    errorMessage.value = null
    model.value = false
  }

  watch(model, (newValue) => {
    if (!newValue && form.value) {
      form.value.form?.resetForm()
      errorMessage.value = null
    }
  })
</script>

<style scoped>

</style>

