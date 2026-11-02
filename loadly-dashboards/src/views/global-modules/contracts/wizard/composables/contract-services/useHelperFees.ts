import { number, object } from 'yup'
import type { Schema } from '@/types/form'
import type { ExtraFeeTypeOption } from '@/types/admin-dashboard/response/contract/extra-fee-type'
import { useVeeValidate } from '@/composables/useVeeValidate'
import { useContractWizard } from '../useContractWizard'
import { useLookups } from '@/composables/useLookups'

export function useHelperFees(formValues?: ComputedRef<Record<string, unknown> | undefined>) {
  const { t } = useI18n()
  const { isFormDisabled } = useContractWizard()
  useVeeValidate()
  const { extraFeeTypes: extraFeeTypesList } = useLookups()

  const helpersFeeType = computed(
    () => extraFeeTypesList.value.find((i) => i.typeId === 3)
  )

  const helpersFeeCalculationOptions = computed<ExtraFeeTypeOption[]>(
    () => helpersFeeType.value?.options ?? []
  )

  const getFieldName = (enumValue: string, suffix: string): string => {
    return `${enumValue}${suffix}`
  }

  const initialValues = computed(() => {
    const values: Record<string, unknown> = {}

    if (helpersFeeType.value?.requiresThreshold) {
      values.includedHelpers = ''
    }

    helpersFeeCalculationOptions.value.forEach((option) => {
      values[option.optionEnum.toString()] = false
      values[getFieldName(option.label, 'Amount')] = undefined as unknown
    })

    return values
  })

  const validationSchema = computed(() => {
    const schema: Record<string, ReturnType<typeof number>> = {}

    if (helpersFeeType.value?.requiresThreshold) {
      schema.includedHelpers = number()
        .transform((v, orig) => (orig === '' || orig == null ? undefined : v))
        .optional()
        .integer()
        .min(1)
        .max(500)
        .typeError(t('validation.integer')) as ReturnType<typeof number>
    }

    const message = t('validation.required', { field: t('form.helperFees') })

    const amountMessage = t('validation.required', { field: t('form.helperFees') })

    return object(schema).test({
      name: 'helperFeesValidation',
      message,
      test(value) {
        const data = value as Record<string, unknown> | undefined

        const selectedOption = helpersFeeCalculationOptions.value.find(
          (opt) => data?.[opt.optionEnum.toString()] === true
        )
        if (selectedOption) {
          const amountField = getFieldName(selectedOption.label, 'Amount')
          const amount = data?.[amountField]
          const num = amount != null && amount !== '' ? Number(amount) : NaN
          if (Number.isNaN(num) || num < 0) {
            return this.createError({
              path: amountField,
              message: amountMessage,
            })
          }
        }

        return true
      },
    })
  })

  const fieldsSchema = computed<Schema[]>(() => {
    const fields: Schema[] = []

    if (helpersFeeType.value?.requiresThreshold) {
      fields.push({
        name: 'includedHelpers',
        as: 'BaseInput',
        type: 'text',
        containerClass: 'w-full mb-8',
        label: helpersFeeType.value.thresholdLabel ?? t('form.includedHelpers'),
        suffix: helpersFeeType.value.thresholdValuePlaceHolder ?? undefined,
        labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
        fieldSlot: true,
        disabled: isFormDisabled.value,
      })
    }

    helpersFeeCalculationOptions.value.forEach((option) => {
      const isSelected = computed(() => formValues?.value?.[option.optionEnum] === true)
      const amountField = getFieldName(option.label, 'Amount')

      fields.push(
        {
          name: option.optionEnum.toString(),
          as: 'BaseRadio',
          containerClass: 'w-full md:w-[calc(50%-0.75rem)] mb-5',
          label: option.label,
          description: option.description ?? undefined,
          labelClass: 'hidden',
          fieldSlot: true,
          disabled: isFormDisabled.value,
        },
        {
          name: amountField,
          as: 'BaseInput',
          type: 'number',
          min: 0,
          placeholder: t('form.ex_number', { number: 30 }),
          containerClass: 'w-full md:w-[calc(50%-0.75rem)] mb-5',
          disabled: !isSelected.value || isFormDisabled.value,
          suffix: option.valueType ?? undefined,
        },
      )
    })

    return fields
  })

  return {
    initialValues,
    validationSchema,
    fieldsSchema,
    helpersFeeCalculationOptions,
    helpersFeeType,
    getFieldName,
  }
}
