import { date, number, object, string } from 'yup'
import type { Schema } from '@/types/form'
import type { LookupItem } from '@/types/response/lookup'
import { useVeeValidate } from '@/composables/useVeeValidate'
import { createArabicTextSchema } from '@/utils/validation/arabic'
import { useLookups } from '@/composables/useLookups'
import { useContractWizard } from '../useContractWizard'
import { CONTRACT_STATUS } from '@/types/admin-dashboard/contract'
import { useApp } from '@/composables/useApp'
import { useEnum } from '@/composables/useEnum'
import { ContractType, type VehicleType } from '@/types/admin-dashboard/response/contract/contract-details';
import { ConsumptionType } from '@/types/request/contract/consumption-types.enums'

function resetVehicleTypes(vehicleTypes: VehicleType[]) {
  return vehicleTypes.map((vehicle) => ({
    ...vehicle,
    consumptionType: null,
    cargoCommission: null,
    extraTrip: null,
    ExtraWorkingDayFees: null,
    tripsPerMonth: null,
    OverWeightPricePerTon: null,
    contractorOverWeightPricePerTon: null,
    dailyValue: null,
    maxKms: null,
    minBillableWeightTons: null,
    maxPayloadTons: null,
    monthlyWorkingDays: null,
  }))
}

export function useCompanyInformation() {
  const { t } = useI18n()
  const { isAdminDashboard } = useApp()
  const {
    isFormDisabled: disabled,
    isUpdateMode,
    isViewMode,
    contract,
    wizardFormData,
  } = useContractWizard()

  useVeeValidate()
  const {
    admins: adminsLookup,
    getAdmins,
    getCompanies,
    companies,
    loadMoreCompanies,
    hasMoreCompanies,
    hasMoreAdmins,
    loadMoreAdmins,
  } = useLookups()
  const { enums } = useEnum()

  const salesRepOptions = computed<LookupItem[]>(() => {
    const baseItems = adminsLookup.value?.items ?? []
    const rep = contract.value?.salesRep
    const includeRepFromContract = (isUpdateMode.value || isViewMode.value)
      && rep
      && (isViewMode.value || contract.value?.status !== CONTRACT_STATUS.DRAFT)

    if (includeRepFromContract) {
      const repItem: LookupItem = { value: String(rep.id), label: rep.fullName }
      const exists = baseItems.some((item) => item.value === repItem.value)

      return exists ? baseItems : [repItem, ...baseItems]
    }

    return baseItems
  })

  const isDisabled = computed(() => {
    return disabled.value
      || (isUpdateMode.value && contract.value?.status !== CONTRACT_STATUS.DRAFT)
  })

  const consumptionTypes = computed(() => {
    return enums.value?.ContractConsumptionTypes?.filter(item => Number(item.value) !== 1) ?? []
  })

  const isCargoPriceConsumption = computed(() => {
    return wizardFormData.value?.consumptionType === ConsumptionType.CargoPriceConsumption
      || consumptionType.value === ConsumptionType.CargoPriceConsumption
  })

  const isIncludedContract = computed(() => {
    return contract.value?.contractType === ContractType.Included
      || contractType.value === ContractType.Included
  })

  const isOnDemandContract = computed(() => {
    return Number(wizardFormData.value?.contractType) === ContractType.OnDemand
      || contractType.value === ContractType.OnDemand
  })

  const hasMonthlyWorkingDays = computed(() => {
    const type = Number(wizardFormData.value?.contractType) || contractType.value

    return type !== ContractType.OnDemand
  })

  const contractType = ref<number | null>(null)
  const consumptionType = ref<number | null>(null)

  const initialValues = ref({
    companyId: '',
    startDate: '',
    endDate: '',
    contractType: '',
    salesRepId: '',
    termsAndConditions: '',
    cargoCommission: '',
    vehiclesCount: '',
    kmOverageMode: '',
  })

  const validationSchema = object({
    companyId: string().required(t('validation.required', { field: t('form.companyName') })),
    startDate: string().required(),
    endDate: date()
      .typeError(() => t('validation.invalidDate'))
      .required(t('validation.required', { field: t('form.endDate') }))
      .when('startDate', (startDate, schema) => {
        if (startDate && startDate.toString()) {
          return schema.min(new Date(startDate.toString()), () => t('validation.minDate', { min: t('form.startDate') }))
        }

        return schema
      }),
    contractType: string().required(t('validation.required', { field: t('form.contractType') })),
    nameEn: string().required(),
    nameAr: createArabicTextSchema(t, 100),
    salesRepId: string().required(t('validation.required', { field: t('form.salesRepresentative') })),
    termsAndConditions: string().max(5000).optional(),
    vehiclesCount: number()
      .optional()
      .nullable()
      .typeError(() => t('validation.integer'))
      .transform((value, original) => (original === '' || original == null ? undefined : value)),
    kmOverageMode: string().when('contractType', {
      is: (value: string | number) => Number(value) === ContractType.Included,
      then: (schema) => schema.required(t('validation.required', { field: t('form.kmOverageMode') })),
      otherwise: (schema) => schema.optional().nullable(),
    }),
    monthlyWorkingDays: number().when('contractType', {
      is: (value: string | number) => {
        const type = Number(value)

        return type === ContractType.Included
          || type === ContractType.Excluded
          || type === ContractType.Seasonal
      },
      then: (schema) => schema
        .positive()
        .typeError(t('validation.required', { field: t('form.monthlyWorkingDays') }))
        .required(t('validation.required', { field: t('form.monthlyWorkingDays') }))
        .max(31)
        .min(1),
      otherwise: (schema) => schema.optional().nullable(),
    }),
  })

  const fieldsSchema = computed<Schema[]>(() => [
    ...(isAdminDashboard ? [{
      name: 'companyId',
      as: 'BaseSelect',
      options: companies.value?.items ?? [],
      label: t('form.selectCompany'),
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
      labelRequired: true,
      labelInfo: true,
      searchable: true,
      containerClass: 'w-full',
      disabled: isDisabled.value,
      onScrollToEnd: () => {
        if (hasMoreCompanies.value) {
          loadMoreCompanies()
        }
      },
      onSearch: (searchKeyword: string) => {
        getCompanies(1, searchKeyword)
      },
    }] : []),
    {
      name: 'nameEn',
      as: 'BaseInput',
      type: 'text',
      placeholder: t('form.nameEn'),
      label: t('form.nameEn'),
      labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
      labelRequired: true,
      labelInfo: true,
      containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
      disabled: isDisabled.value,
    },
    {
      name: 'nameAr',
      as: 'BaseInput',
      type: 'text',
      placeholder: t('form.nameAr'),
      label: t('form.nameAr'),
      labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
      labelRequired: true,
      labelInfo: true,
      containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
      disabled: isDisabled.value,
    },
    {
      name: 'startDate',
      as: 'BaseInput',
      type: 'date',
      placeholder: 'dd-mm-yyyy',
      label: t('form.startDate'),
      labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
      labelRequired: true,
      labelInfo: true,
      containerClass: 'w-full',
      disabled: isDisabled.value,
    },
    {
      name: 'endDate',
      as: 'BaseInput',
      type: 'date',
      placeholder: 'dd-mm-yyyy',
      label: t('form.endDate'),
      labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
      labelRequired: true,
      labelInfo: true,
      containerClass: 'w-full',
      fieldSlot: true,
      disabled: isDisabled.value,
    },
    {
      name: 'contractType',
      as: 'BaseSelect',
      options: enums.value?.ContractType ?? [],
      label: t('form.contractType'),
      labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
      labelRequired: true,
      labelInfo: true,
      containerClass: 'w-full',
      disabled: isDisabled.value,
      onChange: (value: string | number) => {
        if (wizardFormData.value && wizardFormData.value.contractType) {
          wizardFormData.value.contractType = Number(value)
          wizardFormData.value.vehicleTypes = resetVehicleTypes(wizardFormData.value.vehicleTypes as VehicleType[])
        }
        contractType.value = Number(value)
      },
    },
    ...(isOnDemandContract.value ? [
      {
        name: 'consumptionType',
        as: 'BaseSelect',
        options: consumptionTypes.value,
        label: t('form.consumptionType'),
        labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
        labelRequired: true,
        labelInfo: true,
        containerClass: 'w-full',
        disabled: isDisabled.value,
        onChange: (value: string | number) => {
          if (wizardFormData.value.consumptionType) {
            wizardFormData.value.consumptionType = Number(value)
            wizardFormData.value.vehicleTypes = resetVehicleTypes(wizardFormData.value.vehicleTypes as VehicleType[])
          }
          consumptionType.value = Number(value)
        }
      },
      ...(isCargoPriceConsumption.value ? [{
        name: 'cargoCommission',
        as: 'BaseInput',
        type: 'number',
        label: t('form.cargoCommission') + ' (%)',
        labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
        labelRequired: true,
        labelInfo: true,
        containerClass: 'w-full',
        disabled: isDisabled.value,
      }] : [])
    ] : []),
    {
      name: 'vehiclesCount',
      as: 'BaseInput',
      type: 'text',
      placeholder: t('form.unspecified'),
      label: t('form.vehiclesCount'),
      containerClass: 'w-full',
      disabled: disabled.value,
    },
    ...(isIncludedContract.value ? [{
      name: 'kmOverageMode',
      as: 'BaseSelect',
      options: enums.value?.ContractKmOverageMode ?? [],
      label: t('form.kmOverageMode'),
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
      labelRequired: true,
      containerClass: 'w-full',
      disabled: disabled.value,
    }] : []),
    ...(hasMonthlyWorkingDays.value
      ? [{
        name: 'monthlyWorkingDays',
        as: 'BaseInput',
        type: 'text',
        placeholder: '(1-31)',
        containerClass: 'w-full',
        label: t('form.monthlyWorkingDays'),
        labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
        labelRequired: true,
        disabled: disabled.value,
      }] : []),
    {
      name: 'salesRepId',
      as: 'BaseSelect',
      options: salesRepOptions.value,
      label: t('form.salesRepresentativeAssignment'),
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
      labelRequired: true,
      labelInfo: true,
      searchable: true,
      containerClass: 'w-full',
      disabled: isDisabled.value,
      onScrollToEnd: () => {
        if (hasMoreAdmins.value) {
          loadMoreAdmins()
        }
      },
      onSearch: (searchKeyword: string) => {
        getAdmins(1, searchKeyword)
      },
    },
    {
      name: 'termsAndConditions',
      as: 'BaseTextarea',
      label: t('form.contractDescription'),
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
      containerClass: 'w-full',
      rows: 8,
      maxlength: 5000,
      showCounter: true,
      disabled: disabled.value,
    },
  ])

  return {
    companies,
    fieldsSchema,
    getCompanies,
    initialValues,
    validationSchema,
    isDisabled,
  }
}

