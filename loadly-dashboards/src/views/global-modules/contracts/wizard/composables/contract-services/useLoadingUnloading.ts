import { object } from 'yup'
import type { Schema } from '@/types/form'
import type { ExtraFeeTypeOption } from '@/types/admin-dashboard/response/contract/extra-fee-type'
import { useVeeValidate } from '@/composables/useVeeValidate'
import { useContractWizard } from '../useContractWizard'
import { useLookups } from '@/composables/useLookups'

export function useLoadingUnloading(formValues?: ComputedRef<Record<string, unknown> | undefined>) {
  const { t } = useI18n()
  const { isFormDisabled } = useContractWizard()
  useVeeValidate()
  const { extraFeeTypes: extraFeeTypesList } = useLookups()

  const loadingUnloadingFeeType = computed(
    () => extraFeeTypesList.value.find((i) => i.typeId === 5)
  )

  const loadingUnloadingCalculationOptions = computed<ExtraFeeTypeOption[]>(
    () => loadingUnloadingFeeType.value?.options ?? []
  )

  const getFieldName = (enumValue: string, suffix: string): string => {
    return `${enumValue}${suffix}`
  }

  const initialValues = computed(() => {
    const values: Record<string, unknown> = {}

    loadingUnloadingCalculationOptions.value.forEach((option) => {
      values[option.optionEnum.toString()] = false
      if (option.requiresValue) {
        values[getFieldName(option.optionEnum.toString(), 'Amount')] = undefined as unknown
      }
    })

    return values
  })

  const validationSchema = computed(() => {
    const requiredMessage = t('validation.required', { field: t('form.loadingUnloading') })
    const options = loadingUnloadingCalculationOptions.value
    const lastOption = options.length > 0 ? options[options.length - 1] : undefined

    return object({}).test({
      name: 'loadingUnloadingValidation',
      message: requiredMessage,
      test(value) {
        const data = value as Record<string, unknown> | undefined
        if (!data) return true

        const selectedOption = options.find((opt) => data[opt.optionEnum.toString()] === true)
        if (!selectedOption) return true

        if (selectedOption.requiresValue) {
          const amountField = getFieldName(selectedOption.optionEnum.toString(), 'Amount')
          const num = toNumberOrNaN(data[amountField])
          if (Number.isNaN(num) || num < 0) {
            return this.createError({ path: amountField, message: requiredMessage })
          }
        }

        const isLastOptionSelected = lastOption && selectedOption.optionEnum === lastOption.optionEnum
        if (loadingUnloadingFeeType.value?.requiresThreshold && isLastOptionSelected) {
          const thresholdNum = toNumberOrNaN(data.loadingUnloadingThreshold)
          if (Number.isNaN(thresholdNum) || thresholdNum < 0) {
            return this.createError({
              path: 'loadingUnloadingThreshold',
              message: requiredMessage,
            })
          }
        }

        return true
      },
    })
  })

  function toNumberOrNaN(v: unknown): number {
    if (v == null || v === '') return NaN

    const n = Number(v)

    return Number.isNaN(n) ? NaN : n
  }

  const fieldsSchema = computed<Schema[]>(() => {
    const fields: Schema[] = []

    loadingUnloadingCalculationOptions.value.forEach((option, index) => {
      const isSelected = computed(() => formValues?.value?.[option.optionEnum.toString()] === true)
      const amountField = getFieldName(option.optionEnum.toString(), 'Amount')

      fields.push(
        {
          name: option.optionEnum.toString(),
          as: 'BaseRadio',
          containerClass: `w-full ${option.requiresValue ? 'md:w-[calc(50%-0.75rem)]' : ''} mb-5`,
          label: option.label,
          description: option.description ?? undefined,
          labelClass: 'hidden',
          fieldSlot: true,
          disabled: isFormDisabled.value,
        },
        ...(option.requiresValue
          ? [{
              name: amountField,
              as: 'BaseInput',
              type: 'number',
              min: 0,
              placeholder: t('form.ex_number', { number: 30 }),
              containerClass: 'w-full md:w-[calc(50%-0.75rem)] mb-5',
              disabled: !isSelected.value || isFormDisabled.value,
              suffix: option.valueType ?? undefined,
            }]
          : []),
        ...(index === loadingUnloadingCalculationOptions.value.length - 1
          && loadingUnloadingFeeType.value?.requiresThreshold
          ? [{
              name: 'loadingUnloadingThreshold',
              as: 'BaseInput',
              type: 'number',
              min: 0,
              containerClass: 'flex justify-between flex-wrap w-full mb-6',
              wrapperClass: 'w-full md:w-[calc(50%-0.75rem)]',
              labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
              label: loadingUnloadingFeeType.value.thresholdLabel ?? t('form.description'),
              suffix: loadingUnloadingFeeType.value.thresholdValuePlaceHolder ?? undefined,
              disabled: !isSelected.value || isFormDisabled.value,
            }]
          : []),
      )
    })

    return fields
  })

  return {
    initialValues,
    validationSchema,
    fieldsSchema,
    loadingUnloadingCalculationOptions,
    getFieldName,
  }
}
