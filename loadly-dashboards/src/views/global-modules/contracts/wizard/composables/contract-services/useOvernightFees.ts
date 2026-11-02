import { number, object } from 'yup'
import type { Schema } from '@/types/form'
import { useVeeValidate } from '@/composables/useVeeValidate'
import { useContractWizard } from '../useContractWizard'
import { useLookups } from '@/composables/useLookups'

export function useOvernightFees() {
  useVeeValidate()
  const { t } = useI18n()
  const { isFormDisabled } = useContractWizard()
  const { extraFeeTypes: extraFeeTypesList } = useLookups()

  const overnightFeesCalculationOptions = computed(() => extraFeeTypesList.value.find(item => item.typeId === 10))

  const initialValues = ref({
    overNightFeesAmount: '',
  })

  const validationSchema = object({
    overNightFeesAmount: number()
      .optional()
      .nullable()
      .transform(
        (value, original) => (
          original === '' || original == null
          ? undefined
          : value
        ))
      .min(0),
  })

  const fieldsSchema = computed<Schema[]>(() => [
    {
      name: 'overNightFeesAmount',
      as: 'BaseInput',
      type: 'number',
      min: 0,
      placeholder: t('form.ex_number', { number: 30 }),
      containerClass: 'flex w-full grid grid-cols-1 gap-4 md:grid-cols-2',
      label: t('form.amount'),
      hint: overnightFeesCalculationOptions.value?.thresholdDescription,
      suffix: overnightFeesCalculationOptions.value?.thresholdValuePlaceHolder,
      disabled: isFormDisabled.value,
    },
  ])

  return {
    fieldsSchema,
    initialValues,
    validationSchema,
  }
}
