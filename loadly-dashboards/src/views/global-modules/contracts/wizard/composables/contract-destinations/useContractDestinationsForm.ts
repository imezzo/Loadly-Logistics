import type { FieldsGroup } from '@/types/form'
import { object, string, number } from 'yup'
import { useVeeValidate } from '@/composables/useVeeValidate'
import { useContractWizard } from '../useContractWizard'
import type { ContractWizardDestination } from '@/types/request/contract/contract-destinations.types'
import { ContractType } from '@/types/admin-dashboard/response/contract/contract-details'
import { ConsumptionType } from '@/types/request/contract/consumption-types.enums'
import { hasValue } from '@/utils/transformers'

export function emptyContractDestinationDraft(): Record<string, unknown> {
  return {
    nameEn: '',
    nameAr: '',
    startGoogleMapLink: '',
    startLatitude: '',
    startLongitude: '',
    startAddressName: '',
    startGooglePlaceId: '',
    endGoogleMapLink: '',
    endLatitude: '',
    endLongitude: '',
    endAddressName: '',
    endGooglePlaceId: '',
    expectedKms: '',
    returnExpectedKms: '',
    numberOfDays: '',
    pricePerTon: '',
    fixedTripPrice: '',
    contractorPricePerTon: '',
    contractorFixedTripPrice: '',
  }
}

export function wizardRowToDraftValues(row: ContractWizardDestination): Record<string, unknown> {
  return {
    nameEn: row.nameEn,
    nameAr: row.nameAr,
    startGoogleMapLink: '',
    startLatitude: row.start?.latitude,
    startLongitude: row.start?.longitude,
    startAddressName: row.start?.addressName,
    startGooglePlaceId: row.start?.googlePlaceId,
    endGoogleMapLink: '',
    endLatitude: row.end?.latitude,
    endLongitude: row.end?.longitude,
    endAddressName: row.end?.addressName,
    endGooglePlaceId: row.end?.googlePlaceId,
    expectedKms: row.expectedKms,
    returnExpectedKms: String(row.returnExpectedKms),
    numberOfDays: row.numberOfDays != null ? String(row.numberOfDays) : '',
    pricePerTon: row.pricePerTon != null ? String(row.pricePerTon) : '',
    fixedTripPrice: row.fixedTripPrice != null ? String(row.fixedTripPrice) : '',
    contractorPricePerTon: row.contractorPricePerTon != null ? String(row.contractorPricePerTon) : '',
    contractorFixedTripPrice: row.contractorFixedTripPrice != null ? String(row.contractorFixedTripPrice) : '',
    startId: row.start?.id,
    endId: row.end?.id,
  }
}

function optionalNumberId(v: unknown): number | undefined {
  if (v == null || v === '') return undefined
  const n = Number(v)

  return Number.isFinite(n) ? n : undefined
}

// TODO: Refactor this function to use the apiDestinationPointToState function or remove it
export function draftValuesToWizardRow(
  values: Record<string, unknown>,
  clientId: string,
): ContractWizardDestination {
  const pricePerTon = optionalNumberId(values.pricePerTon)
  const fixedTripPrice = optionalNumberId(values.fixedTripPrice)
  const contractorPricePerTon = optionalNumberId(values.contractorPricePerTon)
  const contractorFixedTripPrice = optionalNumberId(values.contractorFixedTripPrice)
  const returnExpectedKms = optionalNumberId(values.returnExpectedKms)

  return {
    clientId,
    start: {
      id: optionalNumberId(values.startId),
      addressName: String(values.startAddressName ?? ''),
      googlePlaceId: String(values.startGooglePlaceId ?? ''),
      latitude: String(values.startLatitude ?? ''),
      longitude: String(values.startLongitude ?? ''),
    },
    end: {
      id: optionalNumberId(values.endId),
      addressName: String(values.endAddressName ?? ''),
      googlePlaceId: String(values.endGooglePlaceId ?? ''),
      latitude: String(values.endLatitude ?? ''),
      longitude: String(values.endLongitude ?? ''),
    },
    nameAr: String(values.nameAr ?? ''),
    nameEn: String(values.nameEn ?? ''),
    expectedKms: values.expectedKms ? Number(values.expectedKms) : undefined,
    ...(returnExpectedKms !== undefined ? { returnExpectedKms } : {}),
    numberOfDays: values.numberOfDays ? Number(values.numberOfDays) : undefined,
    ...(pricePerTon !== undefined ? { pricePerTon } : {}),
    ...(fixedTripPrice !== undefined ? { fixedTripPrice } : {}),
    ...(contractorPricePerTon !== undefined ? { contractorPricePerTon } : {}),
    ...(contractorFixedTripPrice !== undefined ? { contractorFixedTripPrice } : {}),
  }
}

export function useContractDestinationsForm(
  onStartMapLinkInput?: (url: string) => void,
  onEndMapLinkInput?: (url: string) => void,
) {
  const { t } = useI18n()
  const { isFormDisabled, formData, contract } = useContractWizard()

  useVeeValidate()

  const isOnDemandContract = computed(() => {
    const typeId = Number(formData.value?.contractType ?? contract.value?.contractType)

    return typeId === ContractType.OnDemand
  })

  const consumptionType = computed(() => {
    const raw = formData.value?.consumptionType ?? contract.value?.consumptionType

    return hasValue(raw as string) ? Number(raw) : NaN
  })

  const showPricePerTonField = computed(() => {
    return isOnDemandContract.value && consumptionType.value === ConsumptionType.CargoWeight
  })

  const showFixedTripPriceField = computed(() => {
    return isOnDemandContract.value && consumptionType.value === ConsumptionType.DestinationFixedPrice
  })

  const requireNumberOfDaysAndKms = computed(() => {
    return consumptionType.value === ConsumptionType.Trips || [
      ContractType.Excluded,
      ContractType.Seasonal,
    ].includes(Number(contract.value?.contractType))
  })

  const validationSchema = computed(() => {
    const kmsSchema = number()
      .nullable()
      .transform((value) => isNaN(value) ? undefined : value)
      .min(1)
    const daysSchema = number()
      .nullable()
      .transform((value) => isNaN(value) ? undefined : value)
      .min(1)
      .max(365)

    return object({
      nameEn: string().required(),
      nameAr: string().required(),
      expectedKms: requireNumberOfDaysAndKms.value ? kmsSchema.required() : kmsSchema.optional(),
      returnExpectedKms: number()
        .transform((value) => isNaN(value) ? undefined : value)
        .min(0)
        .required(),
      numberOfDays: requireNumberOfDaysAndKms.value ? daysSchema.required() : daysSchema.optional(),
    })
  })

  const labelClass = 'mb-1 inline-block mt-4'
  const wrapperClass = 'my-7 grid grid-cols-1 gap-4 md:grid-cols-2'
  const groups = computed<Record<string, FieldsGroup>>(() => ({
    names: {
      wrapperClass,
      fields: [
        {
          name: 'nameEn',
          as: 'BaseInput',
          type: 'text',
          label: t('form.destinationNameEn'),
          labelClass,
          labelRequired: true,
          disabled: isFormDisabled.value,
        },
        {
          name: 'nameAr',
          as: 'BaseInput',
          type: 'text',
          label: t('form.destinationNameAr'),
          labelClass,
          labelRequired: true,
          disabled: isFormDisabled.value,
        },
      ],
    },
    startLocation: {
      title: t('form.startLocation'),
      titleClass: 'text-lg font-bold text-neutral-10',
      wrapperClass,
      fields: [
        {
          name: 'startGoogleMapLink',
          as: 'BaseInput',
          type: 'text',
          label: t('form.googleMapLink'),
          labelClass,
          containerClass: 'col-span-2',
          placeholder: t('form.address'),
          disabled: isFormDisabled.value,
          onInput: (e: InputEvent) => {
            const url = (e.target as HTMLInputElement)?.value?.trim?.()
            if (url) onStartMapLinkInput?.(url)
          },
        },
        {
          name: 'startLatitude',
          as: 'BaseInput',
          type: 'text',
          label: t('form.lat'),
          labelClass,
          disabled: isFormDisabled.value,
        },
        {
          name: 'startLongitude',
          as: 'BaseInput',
          type: 'text',
          label: t('form.long'),
          labelClass,
          disabled: isFormDisabled.value,
        },
        {
          name: 'startAddressName',
          as: 'BaseTextarea',
          label: t('form.address'),
          labelClass,
          containerClass: 'col-span-2',
          disabled: isFormDisabled.value,
        },
      ],
    },
    endLocation: {
      title: t('form.endLocation'),
      titleClass: 'text-lg font-bold text-neutral-10',
      wrapperClass,
      fields: [
        {
          name: 'endGoogleMapLink',
          as: 'BaseInput',
          type: 'text',
          label: t('form.googleMapLink'),
          labelClass,
          containerClass: 'col-span-2',
          placeholder: t('form.address'),
          disabled: isFormDisabled.value,
          onInput: (e: InputEvent) => {
            const url = (e.target as HTMLInputElement)?.value?.trim?.()
            if (url) onEndMapLinkInput?.(url)
          },
        },
        {
          name: 'endLatitude',
          as: 'BaseInput',
          type: 'text',
          label: t('form.lat'),
          labelClass,
          disabled: isFormDisabled.value,
        },
        {
          name: 'endLongitude',
          as: 'BaseInput',
          type: 'text',
          label: t('form.long'),
          labelClass,
          disabled: isFormDisabled.value,
        },
        {
          name: 'endAddressName',
          as: 'BaseTextarea',
          label: t('form.address'),
          labelClass,
          containerClass: 'col-span-2',
          disabled: isFormDisabled.value,
        },
      ],
    },
    distance: {
      fields: [
        {
          name: 'expectedKms',
          as: 'BaseInput',
          type: 'text',
          label: t('form.kilometer'),
          labelClass,
          labelRequired: requireNumberOfDaysAndKms.value,
          disabled: isFormDisabled.value,
        },
        {
          name: 'returnExpectedKms',
          as: 'BaseInput',
          type: 'number',
          label: t('form.returnExpectedKms'),
          labelClass,
          labelRequired: true,
          disabled: isFormDisabled.value,
        },
        {
          name: 'numberOfDays',
          as: 'BaseInput',
          type: 'number',
          label: t('form.numberOfDays'),
          labelClass,
          labelRequired: requireNumberOfDaysAndKms.value,
          disabled: isFormDisabled.value,
        },
        ...(showPricePerTonField.value
          ? [
              {
                name: 'pricePerTon',
                as: 'BaseInput',
                type: 'number',
                label: t('form.pricePerTon'),
                labelClass,
                disabled: isFormDisabled.value,
              },
              {
                name: 'contractorPricePerTon',
                as: 'BaseInput',
                type: 'number',
                label: t('form.contractorPricePerTon'),
                labelClass,
                disabled: isFormDisabled.value,
              },
            ]
          : []),
        ...(showFixedTripPriceField.value
          ? [
              {
                name: 'fixedTripPrice',
                as: 'BaseInput',
                type: 'number',
                label: t('form.fixedTripPrice'),
                labelClass,
                disabled: isFormDisabled.value,
              },
              {
                name: 'contractorFixedTripPrice',
                as: 'BaseInput',
                type: 'number',
                label: t('form.contractorFixedTripPrice'),
                labelClass,
                disabled: isFormDisabled.value,
              },
            ]
          : []),
      ],
    },
  }))

  return {
    groups,
    validationSchema,
    showPricePerTonField,
    showFixedTripPriceField,
    requireNumberOfDaysAndKms,
  }
}
