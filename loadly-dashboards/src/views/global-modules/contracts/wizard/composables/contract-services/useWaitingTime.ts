import { number, object, string } from 'yup'
import type { Schema } from '@/types/form'
import { useVeeValidate } from '@/composables/useVeeValidate'
import { useContractWizard } from '../useContractWizard'
import { useLookups } from '@/composables/useLookups'

export function useWaitingTime() {
  const { t } = useI18n()
  const { isFormDisabled } = useContractWizard()
  const { extraFeeTypes: extraFeeTypesList } = useLookups()
  useVeeValidate()

  const waitingTimeFeeType = computed(
    () => extraFeeTypesList.value.find((i) => i.typeId === 4)
  )

  const waitingTimeFeeCalculationOptions = computed(() => waitingTimeFeeType.value?.options.map(option => ({
    value: option.optionEnum.toString(),
    label: option.label,
    description: option.description ?? undefined,
  })) ?? [])

  const initialValues = computed(() => {
    const values: Record<string, string> = {
      waitingTimeHourlyRate: '',
      billingMethod: '',
    }

    if (waitingTimeFeeType.value?.requiresThreshold) {
      values.freeWaitingPeriod = ''
    }

    return values
  })

  const validationSchema = computed(() =>
    object({
      waitingTimeHourlyRate: number().optional().nullable().transform((v, orig) => (orig === '' || orig == null ? undefined : v)),
      billingMethod: number().optional().nullable().transform((v, orig) => (orig === '' || orig == null ? undefined : v)),
      ...(waitingTimeFeeType.value?.requiresThreshold
        ? { freeWaitingPeriod: string() }
        : {}),
    })
  )

  const fieldsSchema = computed<Schema[]>(() => {
    const fields: Schema[] = []

    if (waitingTimeFeeType.value?.requiresThreshold) {
      fields.push({
        name: 'freeWaitingPeriod',
        as: 'BaseInput',
        type: 'text',
        containerClass: 'flex w-full flex-wrap',
        wrapperClass: 'w-full md:w-[calc(50%-0.75rem)]',
        labelClass: 'w-full flex-1 flex-shrink-0 md:w-[calc(50%-0.75rem)]',
        label: waitingTimeFeeType.value.thresholdLabel ?? t('form.freeWaitingPeriod'),
        disabled: isFormDisabled.value,
        suffix: waitingTimeFeeType.value.thresholdValuePlaceHolder ?? undefined,
      })
    }

    fields.push(
      {
        name: 'waitingTimeHourlyRate',
        as: 'BaseInput',
        type: 'text',
        containerClass: 'flex w-full flex-wrap',
        wrapperClass: 'w-full md:w-[calc(50%-0.75rem)]',
        labelClass: 'w-full flex-1 flex-shrink-0 md:w-[calc(50%-0.75rem)]',
        label: t('form.extraFeesForWaiting'),
        suffix: 'EGP/hour',
        disabled: isFormDisabled.value,
      },
      {
        name: 'billingMethod',
        as: 'BaseRadioGroup',
        options: waitingTimeFeeCalculationOptions.value,
        containerClass: 'w-full',
        wrapperClass: 'flex-col gap-4',
        label: t('form.billingMethodSelector'),
        labelClass: 'mb-4 flex items-center font-bold text-neutral-10',
        disabled: isFormDisabled.value,
      },
    )

    return fields
  })

  return {
    initialValues,
    validationSchema,
    fieldsSchema,
  }
}
