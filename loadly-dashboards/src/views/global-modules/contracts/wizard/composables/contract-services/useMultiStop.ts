import { object } from 'yup'
import type { Schema } from '@/types/form'
import { useVeeValidate } from '@/composables/useVeeValidate'
import { useContractWizard } from '../useContractWizard'

export function useMultiStop() {
  const { t } = useI18n()
  const { isFormDisabled } = useContractWizard()
  useVeeValidate()

  const initialValues = ref({
    freeStops: '',
    perAdditionalStopFee: '',
  })

  const validationSchema = object({})

  const fieldsSchema = computed<Schema[]>(() => [
    {
      name: 'freeStops',
      as: 'BaseInput',
      type: 'text',
      containerClass: 'flex w-full',
      wrapperClass: 'w-full md:w-[calc(50%-0.75rem)]',
      labelClass: 'w-full flex-1 flex-shrink-0 md:w-[calc(50%-0.75rem)]',
      label: t('form.freeStops'),
      suffix: 'Stops',
      disabled: isFormDisabled.value,
    },
    {
      name: 'perAdditionalStopFee',
      as: 'BaseInput',
      type: 'text',
      containerClass: 'flex w-full',
      wrapperClass: 'w-full md:w-[calc(50%-0.75rem)]',
      labelClass: 'w-full flex-1 flex-shrink-0 md:w-[calc(50%-0.75rem)]',
      label: t('form.perAdditionalStopFee'),
      suffix: 'EGP/stop',
      disabled: isFormDisabled.value,
    },
  ])

  return {
    initialValues,
    validationSchema,
    fieldsSchema,
  }
}

