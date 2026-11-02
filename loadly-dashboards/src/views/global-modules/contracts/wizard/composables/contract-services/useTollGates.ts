import { object } from 'yup'
import type { Schema } from '@/types/form'
import { useVeeValidate } from '@/composables/useVeeValidate'
import type { EnumItem } from '@/types/response/enum'
import { useContractWizard } from '../useContractWizard'
import { useLookups } from '@/composables/useLookups'

export function useTollGates(formValues?: ComputedRef<Record<string, unknown> | undefined>) {
  const { t } = useI18n()
  const { isFormDisabled } = useContractWizard()
  useVeeValidate()
  const { extraFeeTypes: extraFeeTypesList } = useLookups()

  const tollGateCalculationOptions = computed<EnumItem[]>(() => extraFeeTypesList.value.find(i => i.typeId === 1)?.options.map(option => ({
    value: option.optionEnum.toString(),
    label: option.label,
  })) ?? [])

  const getFieldName = (enumValue: string, suffix: string): string => {
    return `${enumValue}${suffix}`
  }

  const initialValues = computed(() => {
    const values: Record<string, unknown> = {}

    tollGateCalculationOptions.value.forEach((option) => {
      values[option.value] = false
      values[getFieldName(option.value, 'Amount')] = undefined as unknown
      values[getFieldName(option.value, 'AllowOverride')] = false
    })

    return values
  })

  const validationSchema = computed(() => {
    const type1Options = extraFeeTypesList.value.find(i => i.typeId === 1)?.options ?? []
    const optionKeys = tollGateCalculationOptions.value.map((o) => o.value)
    const message = t('validation.required', { field: t('form.tollGates') })
    const amountMessage = t('validation.required', { field: t('form.tollGates') })

    return object({}).test({
      name: 'tollGatesValidation',
      message,
      test(value) {
        const data = value as Record<string, unknown> | undefined
        if (!data || !optionKeys.some((key) => data[key] === true)) {
          return this.createError({ path: optionKeys[0] ?? 'tollGates', message })
        }

        const selectedOption = type1Options.find((opt) => data[opt.optionEnum] === true)
        if (selectedOption?.requiresValue) {
          const amountField = getFieldName(selectedOption.optionEnum.toString(), 'Amount')
          const amount = data[amountField]
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

    extraFeeTypesList.value.find(i => i.typeId === 1)?.options.forEach((option) => {
      const isSelected = computed(() => formValues?.value?.[option.optionEnum] === true)
      const amountField = getFieldName(option.optionEnum.toString(), 'Amount')
      const allowOverrideField = getFieldName(option.optionEnum.toString(), 'AllowOverride')

      fields.push(
        {
          name: option.optionEnum.toString(),
          as: 'BaseRadio',
          containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
          label: option.label,
          description: option.description ?? undefined,
          labelClass: 'hidden',
          fieldSlot: true,
          disabled: isFormDisabled.value,
        },
        ...(option.requiresValue ? [{
          name: amountField,
          as: 'BaseInput',
          type: 'number',
          min: 0,
          placeholder: t('form.ex_number', { number: 30 }),
          containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
          disabled: !isSelected.value || isFormDisabled.value,
          suffix: option.valueType ?? undefined,
        }] : []),
        {
          name: allowOverrideField,
          as: 'BaseCheckbox',
          label: t('form.allowDispatcherOverrideTollAmount'),
          labelClass: 'hidden',
          containerClass: 'w-full mt-4 mb-6',
          disabled: !isSelected.value || isFormDisabled.value,
        },
      )
    })

    return fields
  })

  return {
    initialValues,
    validationSchema,
    fieldsSchema,
    tollGateCalculationOptions,
    getFieldName,
  }
}

