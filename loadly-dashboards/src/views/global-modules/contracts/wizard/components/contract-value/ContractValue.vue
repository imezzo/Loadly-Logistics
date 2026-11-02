<template>
  <h1 class="mb-6 font-bold text-neutral-10">
    {{ $t('form.monthlyContractValue') }}
  </h1>

  <BaseForm
    ref="contractValueForm"
    :initial-values="initialValues"
    :validation-schema="validationSchema"
    :fields-schema="fieldsSchema"
    :errors="props.errors"
    container-class="flex flex-wrap justify-between gap-6"
    @submit="onSubmit"
  >
    <template #field-price>
      <p class="absolute end-4 bottom-1/2 translate-y-1/2 font-medium text-neutral-10">
        {{ currency }}
      </p>
    </template>

    <template
      v-if="totalPrice && !isNaN(totalPrice)"
      #field-after-price
    >
      <ValueCalculationFormula
        :price="totalPrice.toString()"
        :currency="currency"
        :contract-type-id="Number(formData.contractType)"
      />
    </template>

    <template #field-after-maxKms>
      <p class="mt-6 font-bold text-neutral-10">
        {{ $t('form.paymentTerms') }}
      </p>
    </template>

    <template #submit>
      <BaseActions
        :prev-button-options="{ hasScrollTop: true }"
        :next-button-options="{ label: saveButtonTitle, asSubmit: true, hasScrollTop: true }"
        @prev="$emit('prev')"
      />
    </template>
  </BaseForm>
</template>

<script setup lang="ts">
  import { useVeeValidate } from '@/composables/useVeeValidate'
  import { useContractValue } from '../../composables/useContractValue'
  import { useContractWizard } from '../../composables/useContractWizard'
  import ValueCalculationFormula from './ValueCalculationFormula.vue'
  import type { VehicleType } from '@/types/request/contract/contract'

  const props = defineProps<{
    errors?: Record<string, string[]>
  }>()

  const emit = defineEmits<{
    update: [data: Record<string, unknown>]
    prev: []
    next: []
  }>()

  useVeeValidate()
  const { formData, saveButtonTitle } = useContractWizard()
  const {
    initialValues: defaultInitialValues,
    validationSchema,
    fieldsSchema,
    tripsPerMonth,
    totalKilometersPerMonth,
  } = useContractValue()

  const contractValueForm = useTemplateRef<HTMLFormElement>('contractValueForm')

  const totalPrice = computed(() => {
    return (formData.value?.vehicleTypes as VehicleType[])?.reduce(
      (acc: number, vehicle: VehicleType) => acc + (vehicle.carCount * vehicle.monthlyRental), 0
    ) || 0
  })

  const initialValues = computed(() => {
    const formValues = formData.value
    const base = { ...defaultInitialValues.value }
    const calculatedPrice = totalPrice.value?.toString() ?? ''

    if (formValues && Object.keys(formValues).length > 0) {
      const merged = { ...base, ...formValues }
      merged.price = calculatedPrice || base.price

      merged.tripsPerMonth = tripsPerMonth.value || base.tripsPerMonth
      merged.maxKms = totalKilometersPerMonth.value || base.maxKms

      return merged
    }

    if (calculatedPrice) {
      base.price = calculatedPrice
    }

    return base
  })

  const currency = computed(() => {
    const savedCurrency = formData.value?.currency as string | undefined

    return savedCurrency || initialValues.value.currency as string || defaultInitialValues.value.currency
  })

  function onSubmit(values: Record<string, unknown>): void {
    emit('update', values)
    emit('next')
  }

  function submitStep(): void {
    (contractValueForm.value as { submitForm?: () => Promise<void> } | null)?.submitForm?.()
  }

  defineExpose({ submitStep })
</script>

<style scoped>

</style>
