import { boolean, number, object, string } from 'yup'
import type { FieldsGroup, Schema } from '@/types/form'
import { useEnum } from '@/composables/useEnum'
import { useVeeValidate } from '@/composables/useVeeValidate'
import { useVehicleStore } from '@/stores/admin-dashboard/vehicle'
import { useVehicleWizard } from './useVehicleWizard'
import { useLookups } from '@/composables/useLookups'
import { useLookupStore } from '@/stores/lookup'
import { ContractorStatus } from '@/types/admin-dashboard/response/contractor/contractor.enums'

export function useVehicleRegistration() {
  const { t } = useI18n()
  const { isFormDisabled: disabled, isUpdateMode } = useVehicleWizard()
  const { enums } = useEnum()
  useVeeValidate()
  const {
    contractors,
    fuelTypes,
    getContractors,
    getFuelTypes,
    getSectors,
    getVehicleBrandModels,
    getVehicleBrands,
    getVehicleInsurances,
    getVehicleTypes,
    hasMoreContractors,
    hasMoreFuelTypes,
    hasMoreSectors,
    hasMoreVehicleBrandModels,
    hasMoreVehicleBrands,
    hasMoreVehicleInsurances,
    hasMoreVehicleTypes,
    loadMoreContractors,
    loadMoreFuelTypes,
    loadMoreSectors,
    loadMoreVehicleBrandModels,
    loadMoreVehicleBrands,
    loadMoreVehicleInsurances,
    loadMoreVehicleTypes,
    vehicleBrands,
    vehicleCapacityUnits,
    vehicleInsurances,
    vehicleTypes,
    sectors,
  } = useLookups()

  const vehicleStore = useVehicleStore()
  const lookupStore = useLookupStore()
  const { vehicleBrandModels } = storeToRefs(lookupStore)

  const selectedBrandId = ref<number | string | undefined>()
  const hasContractor = ref(false)
  const showNewPlate = ref(false)

  const unitTypeAndCategoryDisabled = computed(
    () => disabled.value || isUpdateMode.value
  )

  const vehicleUnitTypeOptions = computed(() => enums.value?.VehicleUnitType ?? [])
  const vehicleTypeCategoryOptions = computed(() => enums.value?.VehicleTypeCategory ?? [])
  const loadingMechanismTypeOptions = computed(() => enums.value?.LoadingMechanismType ?? [])

  const fridgeCategoryValue = computed(
    () =>
      vehicleTypeCategoryOptions.value.find((o: { value?: string }) =>
        o.value?.toLowerCase() === '0'
      )?.value ?? null
  )

  const isFridgeCategory = computed(
    () => vehicleStore.registrationVehicleTypeCategory === fridgeCategoryValue.value
  )

  const openedCategoryValue = computed(
    () =>
      vehicleTypeCategoryOptions.value.find((o: { value?: string }) =>
        o.value?.toLowerCase() === '1'
      )?.value ?? null
  )

  const isOpenedCategory = computed(
    () => vehicleStore.registrationVehicleTypeCategory === openedCategoryValue.value
  )

  const boxCategoryValue = computed(
    () =>
      vehicleTypeCategoryOptions.value.find((o: { value?: string }) =>
        o.value?.toLowerCase() === '2'
      )?.value ?? null
  )

  const isBoxCategory = computed(
    () => vehicleStore.registrationVehicleTypeCategory === boxCategoryValue.value
  )

  const vehicleColorOptions = computed(() =>
    vehicleStore.vehicleColorsList?.vehicleColors?.map((vehicle) => ({
      value: String(vehicle.id),
      label: vehicle.name,
      content: () => `<div class="flex items-center gap-2 w-full">
        <span class="block w-4 h-4 rounded-sm" style="background-color: ${vehicle.colorHash}"></span>
        <span class="text-sm">${vehicle.name}</span>
      </div>`,
    })) ?? []
  )

  const initialValues = ref({
    hasContractor: false,
    contractorId: '',
    sectorId: '',
    vehicleUnitType: '',
    vehicleTypeCategory: '',
    vehicleTypeId: null as number | null,
    fuelTypeId: '',
    plateLetters: '',
    plateNo: '',
    oldPlateLetters: '',
    oldPlateNo: '',
    brandID: '',
    brandModelID: '',
    makeYear: '',
    capacityUnitId: '',
    capacity: '',
    colorId: '',
    vinNo: '',
    initialOdometerReading: '',
    insuranceId: '',
    insuranceDetails: '',
    loadingMechanismType: '',
    maximumLoadHeight: '',
    internalHeight: '',
    internalWidth: '',
    internalLength: '',
    temperatureRangeSettings: '',
    coolingUnitSpecification: '',
    temperatureMonotoringCapability: '',
    purchaseDate: null as Date | null,
  })

  const validationSchema = computed(() => {
    return object({
      hasContractor: boolean(),
      ...(hasContractor.value ? { contractorId: string().required() } : {}),
      sectorId: string().required(),
      vehicleUnitType: string().required(),
      vehicleTypeCategory: string().required(),
      vehicleTypeId: number().required(),
      fuelTypeId: string().required(),
      plateLetters: string()
        .required()
        .max(3, () => t('form.plateLettersMax'))
        .matches(/^[A-Za-z\u0600-\u06FF]+$/, () => t('form.plateLettersLettersOnly')),
      plateNo: string()
        .required()
        .max(4, () => t('form.plateNumbersMax'))
        .matches(/^\d+$/, () => t('form.plateNumbersNumbersOnly')),
      brandID: string().required(),
      brandModelID: string().required(),
      makeYear: number().required().min(1900).max(new Date().getFullYear()),
      capacityUnitId: string().required(),
      capacity: number().required().positive(),
      colorId: string().required(),
      initialOdometerReading: number()
        .optional()
        .nullable()
        .transform((v, orig) => (orig === '' || orig == null ? undefined : v))
        .min(0),
    })
  })

  function plateFields(): Schema[] {
    const plateDisabled = disabled.value || (isUpdateMode.value && !showNewPlate.value)

    return [
      {
        name: 'plateLetters',
        as: 'BaseInputOtp',
        type: 'text',
        length: 3,
        label: t('form.plateLetters'),
        labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
        labelRequired: true,
        labelInfo: true,
        class: '[&>input]:size-11 [&>input]:rounded-lg',
        containerClass: 'w-full md:w-[calc(40%-1rem)]',
        disabled: plateDisabled,
        autoFocus: false,
      },
      {
        name: 'plateNo',
        as: 'BaseInput',
        type: 'text',
        placeholder: t('form.plateNumbers'),
        label: t('form.plateNumbers'),
        labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
        labelRequired: true,
        labelInfo: true,
        containerClass: 'w-full md:w-[calc(40%-1rem)]',
        disabled: plateDisabled,
      },
    ]
  }

  const fieldsGroup = computed<Record<string, FieldsGroup>>(() => ({
    contractorRelated: {
      class: 'w-full rounded-lg border border-neutral-200 p-4',
      wrapperClass: 'flex flex-col gap-4',
      fields: [
        {
          name: 'hasContractor',
          as: 'BaseCheckbox',
          label: t('form.vehicleRelatedToContractors'),
          labelClass: 'hidden',
          containerClass: 'w-full',
          disabled: disabled.value,
          onCheckedChange: (value: boolean) => {
            hasContractor.value = value
          },
        },
        ...(hasContractor.value
          ? [{
              name: 'contractorId',
              as: 'BaseSelect' as const,
              options: contractors.value?.items ?? [],
              placeholder: t('form.select'),
              searchable: true,
              label: t('form.chooseContractorName'),
              labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
              labelRequired: true,
              containerClass: 'w-full',
              disabled: disabled.value,
              onSearch: (searchKeyword: string) => {
                getContractors(1, ContractorStatus.ACTIVE, searchKeyword)
              },
              onScrollToEnd: () => {
                if (hasMoreContractors.value) {
                  loadMoreContractors()
                }
              },
            }]
          : []),
      ],
    },
    vehicleRegistration: {
      class: 'w-full',
      wrapperClass: 'flex flex-wrap gap-6',
      fields: [
        {
          name: 'vehicleUnitType',
          as: 'BaseSelect',
          options: vehicleUnitTypeOptions.value,
          placeholder: t('form.chooseTheUnit'),
          searchable: true,
          label: t('form.vehicleUnitTypes'),
          labelClass: 'mb-3 flex items-center gap-1 font-medium text-neutral-10',
          labelRequired: true,
          containerClass: 'w-full',
          disabled: unitTypeAndCategoryDisabled.value,
        },
        {
          name: 'sectorId',
          as: 'BaseSelect' as const,
          options: sectors.value,
          placeholder: t('form.select'),
          searchable: true,
          label: t('form.chooseSector'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
          labelRequired: true,
          containerClass: 'w-full',
          disabled: disabled.value,
          onSearch: (searchKeyword: string) => {
            getSectors(1, searchKeyword)
          },
          onScrollToEnd: () => {
            if (hasMoreSectors.value) {
              loadMoreSectors()
            }
          },
        },
        {
          name: 'vehicleTypeCategory',
          as: 'BaseSelect',
          options: vehicleTypeCategoryOptions.value,
          placeholder: t('form.chooseVehicleCategory'),
          searchable: true,
          label: t('form.chooseVehicleCategory'),
          labelClass: 'mb-3 flex items-center gap-1 font-medium text-neutral-10',
          labelRequired: true,
          containerClass: 'w-full',
          disabled: unitTypeAndCategoryDisabled.value,
        },
        {
          name: 'vehicleTypeId',
          as: 'BaseSelect',
          options: vehicleTypes.value,
          placeholder: t('form.chooseTheType'),
          searchable: true,
          label: t('content.chooseVehicleType'),
          labelClass: 'mb-3 flex items-center gap-1 font-medium text-neutral-10',
          labelRequired: true,
          containerClass: 'w-full',
          disabled: disabled.value,
          onSearch: (searchKeyword: string) => {
            getVehicleTypes(1, searchKeyword)
          },
          onScrollToEnd: () => {
            if (hasMoreVehicleTypes.value) {
              loadMoreVehicleTypes()
            }
          },
        },
        {
          name: 'fuelTypeId',
          as: 'BaseSelect',
          options: fuelTypes.value,
          placeholder: t('form.chooseTheType'),
          searchable: true,
          label: t('form.fuelType'),
          labelClass: 'mb-3 flex items-center gap-1 font-medium text-neutral-10',
          labelRequired: true,
          containerClass: 'w-full',
          disabled: disabled.value,
          onSearch: (searchKeyword: string) => {
            getFuelTypes(1, searchKeyword)
          },
          onScrollToEnd: () => {
            if (hasMoreFuelTypes.value) {
              loadMoreFuelTypes()
            }
          },
        },
        ...plateFields(),
        {
          name: 'brandID',
          as: 'BaseSelect',
          options: vehicleBrands.value,
          searchable: true,
          label: t('form.vehicleBrand'),
          labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
          labelRequired: true,
          containerClass: 'w-full md:w-[calc(33%-1rem)]',
          disabled: disabled.value,
          onSearch: (searchKeyword: string) => {
            getVehicleBrands(1, searchKeyword)
          },
          onScrollToEnd: () => {
            if (hasMoreVehicleBrands.value) {
              loadMoreVehicleBrands()
            }
          },
          onChange: (value: string) => {
            vehicleBrandModels.value = null
            selectedBrandId.value = value
            getVehicleBrandModels(1, value)
          },
        },
        {
          name: 'brandModelID',
          as: 'BaseSelect',
          searchable: true,
          options: vehicleBrandModels.value?.items ?? [],
          label: t('form.vehicleModel'),
          labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
          labelRequired: true,
          labelInfo: true,
          containerClass: 'w-full md:w-[calc(33%-1rem)]',
          disabled: disabled.value || !vehicleBrandModels.value,
          onSearch: (searchKeyword: string) => {
            getVehicleBrandModels(1, selectedBrandId.value, searchKeyword)
          },
          onScrollToEnd: () => {
            if (hasMoreVehicleBrandModels.value) {
              loadMoreVehicleBrandModels()
            }
          },
        },
        {
          name: 'makeYear',
          as: 'BaseInput',
          type: 'number',
          placeholder: new Date().getFullYear().toString(),
          label: t('form.modelYear'),
          labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
          labelRequired: true,
          labelInfo: true,
          containerClass: 'w-full md:w-[calc(33%-1rem)]',
          disabled: disabled.value,
        },
        {
          name: 'capacityUnitId',
          as: 'BaseRadioGroup',
          options: vehicleCapacityUnits.value,
          label: t('content.capacityUnit'),
          labelClass: 'mt-6 mb-3 flex items-center gap-1 font-medium text-neutral-10',
          labelRequired: true,
          containerClass: 'w-full',
          disabled: disabled.value,
        },
        {
          name: 'capacity',
          as: 'BaseInput',
          type: 'text',
          label: t('content.theCapacity'),
          labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
          containerClass: 'w-full relative',
          disabled: disabled.value,
          fieldSlot: true,
        },
        {
          name: 'colorId',
          as: 'BaseSelect',
          options: vehicleColorOptions.value,
          searchable: true,
          label: t('form.vehicleColor'),
          labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
          labelRequired: true,
          labelInfo: true,
          containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
          disabled: disabled.value,
        },
        {
          name: 'vinNo',
          as: 'BaseInput',
          type: 'text',
          label: t('form.vinNumber'),
          labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
          labelInfo: true,
          containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
          disabled: disabled.value,
        },
        {
          name: 'initialOdometerReading',
          as: 'BaseInput',
          type: 'number',
          label: t('form.initialOdometerReading'),
          labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
          labelInfo: true,
          containerClass: 'w-full',
          disabled: disabled.value,
        },
        {
          name: 'insuranceId',
          as: 'BaseSelect',
          options: vehicleInsurances.value,
          searchable: true,
          label: t('form.insuranceDetails'),
          labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
          labelInfo: true,
          containerClass: 'w-full',
          placeholder: t('form.select'),
          disabled: disabled.value,
          onSearch: (searchKeyword: string) => {
            getVehicleInsurances(1, searchKeyword)
          },
          onScrollToEnd: () => {
            if (hasMoreVehicleInsurances.value) {
              loadMoreVehicleInsurances()
            }
          },
        },
        ...(function categoryDependentFields(): Schema[] {
          if (isOpenedCategory.value) {
            return [
              {
                name: 'loadingMechanismType',
                as: 'BaseRadioGroup' as const,
                options: loadingMechanismTypeOptions.value,
                label: t('form.loadingMechanismType'),
                labelClass: 'mt-6 mb-3 flex items-center gap-1 font-medium text-neutral-10',
                containerClass: 'w-full',
                disabled: disabled.value,
              },
              {
                name: 'maximumLoadHeight',
                as: 'BaseInput' as const,
                type: 'text' as const,
                label: t('form.maximumLoadHeight'),
                labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
                containerClass: 'w-full',
                disabled: disabled.value,
              },
            ]
          }

          if (isBoxCategory.value) {
            return [
              {
                name: 'internalHeight',
                as: 'BaseInput' as const,
                type: 'text' as const,
                label: t('form.height'),
                labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
                labelInfo: true,
                containerClass: 'w-full md:w-[calc(33%-1rem)]',
                disabled: disabled.value,
              },
              {
                name: 'internalWidth',
                as: 'BaseInput' as const,
                type: 'text' as const,
                label: t('form.width'),
                labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
                labelInfo: true,
                containerClass: 'w-full md:w-[calc(33%-1rem)]',
                disabled: disabled.value,
              },
              {
                name: 'internalLength',
                as: 'BaseInput' as const,
                type: 'text' as const,
                label: t('form.length'),
                labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
                labelInfo: true,
                containerClass: 'w-full md:w-[calc(33%-1rem)]',
                disabled: disabled.value,
              },
              {
                name: 'loadingMechanismType',
                as: 'BaseRadioGroup' as const,
                options: loadingMechanismTypeOptions.value,
                label: t('form.loadingMechanismType'),
                labelClass: 'mt-6 mb-3 flex items-center gap-1 font-medium text-neutral-10',
                containerClass: 'w-full',
                disabled: disabled.value,
              },
              {
                name: 'maximumLoadHeight',
                as: 'BaseInput' as const,
                type: 'text' as const,
                label: t('form.maximumLoadHeight'),
                labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
                containerClass: 'w-full',
                disabled: disabled.value,
              },
            ]
          }

          return []
        })(),
        ...(isFridgeCategory.value
          ? [
              {
                name: 'temperatureRangeSettings',
                as: 'BaseInput' as const,
                type: 'text' as const,
                label: t('form.temperatureRangeSettings'),
                labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
                containerClass: 'w-full',
                disabled: disabled.value,
              },
              {
                name: 'coolingUnitSpecification',
                as: 'BaseInput' as const,
                type: 'text' as const,
                label: t('form.coolingUnitSpecifications'),
                labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
                containerClass: 'w-full',
                disabled: disabled.value,
              },
              {
                name: 'temperatureMonotoringCapability',
                as: 'BaseInput' as const,
                type: 'text' as const,
                label: t('form.temperatureMonitoringCapability'),
                labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
                containerClass: 'w-full',
                disabled: disabled.value,
              },
            ]
          : []),
      ],
    },
  }))

  return {
    fieldsGroup,
    initialValues,
    validationSchema,
    isFridgeCategory,
    isOpenedCategory,
    isBoxCategory,
    hasContractor,
    showNewPlate,
  }
}
