<template>
  <BasePopup
    v-model="model"
    content-class="max-w-175!"
  >
    <div
      v-if="trip"
      class="w-full ps-6 pe-12"
    >
      <PopupHeader :trip="trip" />
      <PopupTripInfo :trip="trip" />
      <PopupWaypoints :trip="trip" />

      <p
        v-if="trip.destinationPrice != null"
        class="mb-4 text-sm text-neutral-800"
      >
        <span class="font-bold">{{ $t('form.fixedTripPrice') }}:</span>
        {{ trip.destinationPrice }}
      </p>

      <BaseForm
        ref="form"
        :validation-schema="validationSchema"
        :initial-values="initialValues"
        :fields-schema="fieldsSchema"
        :errors="formErrors"
        @submit="onSubmit"
      >
        <template #submit>
          <button
            type="submit"
            class="
              mt-2 flex w-full items-center justify-center gap-2 rounded-xl bg-neutral-800 px-4 py-3
              text-sm font-semibold text-white transition-colors
              hover:bg-neutral-700
              disabled:cursor-not-allowed disabled:opacity-60
            "
            :disabled="isSubmitting"
          >
            <IconBuildings class="size-4 shrink-0 stroke-white!" />
            {{ $t('button.pushToContractors') }}
          </button>
        </template>
      </BaseForm>
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import { number, object, string } from 'yup'
  import type { Schema } from '@/types/form'
  import type { Trip } from '@/types/admin-dashboard/response/trip/trip'
  import { ConsumptionType } from '@/types/request/contract/consumption-types.enums'
  import { useLookups } from '@/composables/useLookups'
  import { useLoading } from '@/composables/useLoading'
  import { useVeeValidate } from '@/composables/useVeeValidate'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import PopupHeader from './components/PopupHeader.vue'
  import PopupTripInfo from './components/PopupTripInfo.vue'
  import PopupWaypoints from './components/PopupWaypoints.vue'

  const props = defineProps<{
    trip: Trip | null
  }>()

  const emit = defineEmits<{
    error: [message: string]
    success: []
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })

  useVeeValidate()

  const { t } = useI18n()
  const tripStore = useTripStore()
  const { setLoading } = useLoading()
  const {
    contractVehicleTypes,
    getContractVehicleTypes,
    hasMoreContractVehicleTypes,
    loadMoreContractVehicleTypes,
  } = useLookups()

  const form = useTemplateRef('form')
  const isSubmitting = ref(false)
  const formErrors = ref<Record<string, string[]> | undefined>()

  const contractId = computed(() => props.trip?.contractId ?? '')

  const consumptionType = computed(() => Number(props.trip?.contract?.consumptionType))

  const isSpotPricing = computed(() => consumptionType.value === ConsumptionType.SpotPricing)

  const isCargoWeight = computed(() => consumptionType.value === ConsumptionType.CargoWeight)

  const vehicleTypeOptions = computed(() => contractVehicleTypes.value?.items ?? [])

  const tripContractorPrice = computed(() => {
    const price = props.trip?.contractorPrice ?? props.trip?.destinationPrice

    return price == null ? '' : String(price)
  })

  const tripSpotPrice = computed(() => {
    const price = props.trip?.spotPrice

    return price == null ? '' : String(price)
  })

  const initialValues = computed(() => ({
    contractorPrice: isCargoWeight.value ? '' : tripContractorPrice.value,
    spotPrice: tripSpotPrice.value,
    vehicleTypeId: '',
  }))

  const validationSchema = computed(() => object({
    contractorPrice: isSpotPricing.value
      ? number()
        .typeError(t('validation.required'))
        .required()
        .moreThan(0)
      : number().nullable().optional().typeError(t('validation.required')),
    ...(isSpotPricing.value ? {
      spotPrice: number().nullable().optional(),
    } : {}),
    vehicleTypeId: string().required(),
  }))

  async function fetchCargoWeightContractorPrice(vehicleTypeId: string | number): Promise<void> {
    if (!isCargoWeight.value || !vehicleTypeId) return

    const trip = props.trip
    const actualCargoWeightTons = trip?.actualCargoWeightTons
    const destinationId = trip?.destinationId
    const tripContractId = trip?.contractId

    if (
      actualCargoWeightTons == null
      || !destinationId
      || !tripContractId
    ) {
      return
    }

    const { success, data } = await tripStore.getContractorPriceCalculation({
      actualCargoWeightTons,
      contractId: tripContractId,
      destinationId,
      vehicleTypeId,
    })

    if (!success || !data) return

    form.value?.form?.setFieldValue('contractorPrice', data.expectedContractorPrice, false)
  }

  const fieldsSchema = computed<Schema[]>(() => [
    {
      name: 'vehicleTypeId',
      as: 'BaseSelect',
      options: vehicleTypeOptions.value,
      searchable: true,
      onScrollToEnd: hasMoreContractVehicleTypes.value
        ? loadMoreContractVehicleTypes
        : undefined,
      onSearch: (search: string) => getContractVehicleTypes(contractId.value, 1, search),
      onChange: (value: string | number) => {
        if (isCargoWeight.value) {
          fetchCargoWeightContractorPrice(value)
        }
      },
      label: t('form.vehicleType'),
      labelRequired: true,
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
      containerClass: 'mb-3 w-full',
    },
    {
      name: 'contractorPrice',
      as: 'BaseInput',
      type: 'number',
      label: t('form.contractorPrice'),
      labelRequired: isSpotPricing.value,
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
      containerClass: 'w-full',
      disabled: !isSpotPricing.value,
    },
    ...(isSpotPricing.value ? [{
      name: 'spotPrice',
      as: 'BaseInput',
      type: 'number',
      label: t('form.spotPrice'),
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
      containerClass: 'mt-3 w-full',
    }] : []),
  ])

  async function onSubmit(values: Record<string, unknown>): Promise<void> {
    if (!props.trip?.id || isSubmitting.value) return

    formErrors.value = undefined
    isSubmitting.value = true
    setLoading(true)

    const { success, message, errors } = await tripStore.pushToContractors(props.trip.id, {
      contractorPrice: Number(values.contractorPrice),
      sendToAllContractors: true,
      vehicleTypeId: Number(values.vehicleTypeId),
      ...(isSpotPricing.value
        ? { spotPrice: Number(values.spotPrice) }
        : {}),
    })

    isSubmitting.value = false
    setLoading(false)

    if (success) {
      form.value?.form?.resetForm({ values: initialValues.value })
      model.value = false
      emit('success')

      return
    }

    formErrors.value = errors || {}
    emit('error', message || t('error.somethingWentWrong'))
  }

  onMounted(() => {
    form.value?.form?.resetForm({ values: initialValues.value })
    formErrors.value = undefined

    if (!contractId.value) return

    getContractVehicleTypes(contractId.value, 1)
  })
</script>

<style scoped>

</style>
