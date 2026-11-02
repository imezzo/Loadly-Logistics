import { object, string, number } from 'yup'
import type { Schema } from '@/types/form'
import { useVeeValidate } from '@/composables/useVeeValidate'
import { useEnum } from '@/composables/useEnum'
import { useContractWizard } from './useContractWizard'
import { ContractType } from '@/types/admin-dashboard/response/contract/contract-details'
import type { VehicleType } from '@/types/request/contract/contract'

export function useContractValue() {
  const { t } = useI18n()
  const { enums } = useEnum()
  const { formData, isFormDisabled, isPricingFormEnabled, isCreateMode } = useContractWizard()

  useVeeValidate()

  const currencyOptions = computed(() => [
    { value: 'EGP', label: 'EGP' },
    { value: 'USD', label: 'USD' },
    { value: 'EUR', label: 'EUR' },
  ])

  const paymentDueDaysOptions = computed(() => [
    { value: '15', label: t('form.N_days', { days: 15 }) },
    { value: '30', label: t('form.N_days', { days: 30 }) },
    { value: '45', label: t('form.N_days', { days: 45 }) },
    { value: '60', label: t('form.N_days', { days: 60 }) },
  ])

  const contractType = computed(() => Number(formData.value?.contractType) as ContractType)

  const tripsPerMonthFieldLabel = computed(() => {
    if (contractType.value === ContractType.Excluded) return t('form.tripsPerMonth')

    if (contractType.value === ContractType.Seasonal) return t('form.totalTripsPerContractPeriod')

    return t('form.totalKilometersPerMonth')
  })

  const tripsPerMonth  = computed(() => {
    return (formData.value?.vehicleTypes as VehicleType[])?.reduce(
      (acc: number, vehicle: VehicleType) => acc + ((vehicle.tripsPerMonth ?? 0) * vehicle.carCount), 0
    )
  })

  const totalKilometersPerMonth  = computed(() => {
    return (formData.value?.vehicleTypes as VehicleType[])?.reduce(
      (acc: number, vehicle: VehicleType) => acc + ((vehicle.maxKms ?? 0) * vehicle.carCount), 0
    )
  })

  const hasTripsPerMonth = computed(() => {
    return contractType.value === ContractType.Excluded
    || contractType.value === ContractType.Seasonal
  })

  const isInputDisabled = computed(() => {
    return (isFormDisabled.value && !isPricingFormEnabled.value) && !isCreateMode.value
  })

  const initialValues = ref({
    currency: 'EGP',
    price: '',
    valueCalculation: '',
    tripsPerMonth: tripsPerMonth.value,
    maxKms: totalKilometersPerMonth.value,
    billingCycle: '',
    paymentMethod: '',
    paymentDueDays: '60',
  })

  const validationSchema = object({
    price: number().min(0).required(),
    valueCalculation: string().optional(),
    ...(hasTripsPerMonth.value
      ? {
        tripsPerMonth: number(),
      } : {}),
    billingCycle: string().required(t('validation.required', { field: t('form.paymentSchedule') })),
    paymentMethod: string().required(t('validation.required', { field: t('form.paymentMethod') })),
    paymentDueDays: string().required(t('validation.required', { field: t('form.paymentDueDays') })),
  })

  const fieldsSchema = computed<Schema[]>(() => [
    {
      name: 'currency',
      as: 'BaseRadioGroup',
      options: currencyOptions.value,
      label: t('form.currency'),
      labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
      labelRequired: true,
      containerClass: 'w-full',
      disabled: true,
    },
    {
      name: 'price',
      as: 'BaseInput',
      type: 'text',
      placeholder: t('form.ex_number', { number: 10000000 }),
      containerClass: 'w-full relative',
      disabled: true,
      fieldSlot: true,
    },
    ...(hasTripsPerMonth.value
      ? [{
      name: 'tripsPerMonth',
      as: 'BaseInput',
      type: 'text',
      placeholder: '(0-100)',
      containerClass: 'w-full',
      label: tripsPerMonthFieldLabel.value,
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
      labelRequired: true,
      disabled: true,
    }] : []),
    ...(contractType.value === ContractType.Included ? [{
      name: 'maxKms',
      as: 'BaseInput',
      type: 'text',
      placeholder: '(0-100)',
      containerClass: 'w-full',
      label: t('form.maxKms'),
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
      disabled: true,
    }] : []),
    {
      name: 'billingCycle',
      as: 'BaseSelect',
      options: enums.value?.ContractBillingCycles ?? [],
      label: t('form.paymentSchedule'),
      labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
      labelRequired: true,
      labelInfo: true,
      containerClass: 'w-full',
      disabled: isInputDisabled.value,
    },
    {
      name: 'paymentMethod',
      as: 'BaseSelect',
      options: enums.value?.PaymentMethod ?? [],
      label: t('form.paymentMethod'),
      labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
      labelRequired: true,
      labelInfo: true,
      containerClass: 'w-full',
      disabled: isInputDisabled.value,
    },
    {
      name: 'paymentDueDays',
      as: 'BaseSelect',
      options: paymentDueDaysOptions.value,
      label: t('form.paymentDueDays'),
      labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
      labelRequired: true,
      labelInfo: true,
      containerClass: 'w-full',
      disabled: isInputDisabled.value,
    },
  ])

  return {
    currencyOptions,
    fieldsSchema,
    hasTripsPerMonth,
    initialValues,
    paymentDueDaysOptions,
    totalKilometersPerMonth,
    tripsPerMonth,
    tripsPerMonthFieldLabel,
    validationSchema
  }
}
