import { object, string } from 'yup'
import type { Schema } from '@/types/form'
import { useVeeValidate } from '@/composables/useVeeValidate'
import { useContractWizard } from '../useContractWizard'
import { useLookups } from '@/composables/useLookups'

export function useWeightFees() {
  const { isFormDisabled } = useContractWizard()

  useVeeValidate()
  const { extraFeeTypes: extraFeeTypesList } = useLookups()

  const initialValues = ref({
    weightFeeCalculation: '',
  })

  const validationSchema = object({
    weightFeeCalculation: string().required(),
  })

  const fieldsSchema = computed<Schema[]>(() => [
    {
      name: 'weightFeeCalculation',
      as: 'BaseRadioGroup',
      options: extraFeeTypesList.value.find(i => i.typeId === 2)?.options.map(option => ({
        value: option.optionEnum.toString(),
        label: option.label,
        description: option.description ?? undefined,
      })) ?? [],
      containerClass: 'w-full',
      wrapperClass: 'flex-col gap-4',
      disabled: isFormDisabled.value,
    },
  ])

  return {
    initialValues,
    validationSchema,
    fieldsSchema,
  }
}

