import { object } from 'yup'
import type { Schema } from '@/types/form'
import type { ExtraFeeTypeOption } from '@/types/admin-dashboard/response/contract/extra-fee-type'
import { useVeeValidate } from '@/composables/useVeeValidate'
import { useContractWizard } from '../useContractWizard'
import { useLookups } from '@/composables/useLookups'

export function useIncompleteReturn(formValues?: ComputedRef<Record<string, unknown> | undefined>) {
  const { t } = useI18n()
  const { isFormDisabled } = useContractWizard()
  useVeeValidate()
  const { extraFeeTypes: extraFeeTypesList } = useLookups()

  const incompleteReturnFeeType = computed(
    () => extraFeeTypesList.value.find((i) => i.typeId === 7)
  )

  const incompleteReturnCalculationOptions = computed<ExtraFeeTypeOption[]>(
    () => incompleteReturnFeeType.value?.options ?? []
  )

  const getFieldName = (enumValue: string, suffix: string): string => {
    return `${enumValue}${suffix}`
  }

  const initialValues = computed(() => {
    const values: Record<string, unknown> = {}

    incompleteReturnCalculationOptions.value.forEach((option) => {
      values[option.optionEnum.toString()] = false
      if (option.requiresValue) {
        values[getFieldName(option.optionEnum.toString(), 'Amount')] = ''
      }
    })

    return values
  })

  const validationSchema = computed(() => {
    const optionKeys = incompleteReturnCalculationOptions.value.map((o) => o.optionEnum.toString())
    const selectMessage = t('validation.required', { field: t('form.incompleted') })

    return object({}).test({
      name: 'incompleteReturnValidation',
      message: selectMessage,
      test(value) {
        const values = value as Record<string, unknown> | undefined
        if (!values) return this.createError({ path: optionKeys[0], message: selectMessage })

        const selectedKey = optionKeys.find((key) => values[key] === true)

        const selectedOption = incompleteReturnCalculationOptions.value.find(
          (o) => o.optionEnum.toString() === selectedKey
        )
        if (selectedOption?.requiresValue) {
          const amountField = getFieldName(selectedKey ?? '', 'Amount')
          const amount = values[amountField]
          if (amount == null || String(amount).trim() === '') {
            return this.createError({
              path: amountField,
              message: t('validation.required', { field: selectedOption.label ?? 'Value' }),
            })
          }
        }

        return true
      },
    })
  })

  const fieldsSchema = computed<Schema[]>(() => {
    const fields: Schema[] = []

    incompleteReturnCalculationOptions.value.forEach((option) => {
      const isSelected = computed(
        () => formValues?.value?.[option.optionEnum.toString()] === true
      )
      const amountField = getFieldName(option.optionEnum.toString(), 'Amount')

      fields.push(
        {
          name: option.optionEnum.toString(),
          as: 'BaseRadio',
          containerClass: `w-full ${option.requiresValue ? 'md:w-[calc(50%-0.75rem)]' : 'mb-5'}`,
          label: option.label,
          description: option.description ?? undefined,
          labelClass: 'hidden',
          fieldSlot: true,
          disabled: isFormDisabled.value,
        },
        ...(option.requiresValue
          ? [
              {
                name: amountField,
                as: 'BaseInput',
                type: 'text',
                placeholder: t('form.ex_number', { number: 30 }),
                containerClass: 'w-full md:w-[calc(50%-0.75rem)] mb-5',
                disabled: !isSelected.value || isFormDisabled.value,
                suffix: option.valueType ?? undefined,
              },
            ]
          : []),
      )
    })

    return fields
  })

  return {
    initialValues,
    validationSchema,
    fieldsSchema,
    incompleteReturnCalculationOptions,
    getFieldName,
  }
}
