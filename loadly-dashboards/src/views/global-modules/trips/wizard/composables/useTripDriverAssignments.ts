import { number, object, string } from 'yup'
import type { FieldsGroup } from '@/types/form'
import { useVeeValidate } from '@/composables/useVeeValidate'
import { useTripStore } from '@/stores/admin-dashboard/trip'
import { DispatchingMode } from '@/types/admin-dashboard/response/trip/trip'
import { useTripWizard } from './useTripWizard'
import { useLookups } from '@/composables/useLookups'
import { ConsumptionType } from '@/types/request/contract/consumption-types.enums'
import { useContractStore } from '@/stores/admin-dashboard/contract'

export function useTripDriverAssignments() {
  const { t } = useI18n()
  const { isFormDisabled, hasDisabledFields, isExternalTripMode } = useTripWizard()

  const tripStore = useTripStore()
  const { trip, selectedDriverVehicle } = storeToRefs(tripStore)

  const contractStore = useContractStore()
  const { contractConsumptionType } = storeToRefs(contractStore)

  useVeeValidate()

  const showSwitchToLoadlyPopup = ref(false)
  const {
    contractVehicleTypes,
    getContractVehicleTypes,
    hasMoreContractVehicleTypes,
    loadMoreContractVehicleTypes,
  } = useLookups()

  const disabled = computed(() => isFormDisabled.value || hasDisabledFields.value)
  const vehicleTypesOptions = computed(() => [
    { value: '', label: t('form.select') },
    ...contractVehicleTypes.value?.items ?? [],
  ])

  const isOnDemandContract = computed(() => {
    const consumptionType = Number(contractConsumptionType.value ?? trip.value.contract?.consumptionType ?? 0)

    return ![
      ConsumptionType.Trips,
      ConsumptionType.Kilometers
    ].includes(consumptionType)
  })

  const vehicleSourceOptions = computed(() => [
    { value: DispatchingMode.Loadly, label: t('form.loadlyDriversVehicles') },
    ...(isOnDemandContract.value
      ? [{ value: DispatchingMode.Contractors, label: t('form.contractorsDriversVehicles') }]
      : []),
  ])

  function resetDriverVehicle(): void {
    selectedDriverVehicle.value = null
    trip.value.driverId = ''
    trip.value.vehicleId = undefined
    trip.value.tailVehicleId = null
  }

  function clearContractor(): void {
    trip.value.contractorId = ''
    trip.value.contractorName = ''
    trip.value.contractorPhone = ''
  }

  function unassignContractor(): void {
    clearContractor()
    resetDriverVehicle()
  }

  function applyDispatchingMode(value: DispatchingMode): void {
    trip.value.dispatchingMode = value
    if (value !== DispatchingMode.Contractors) {
      clearContractor()
    }
    resetDriverVehicle()
  }

  function confirmSwitchToLoadly(): void {
    applyDispatchingMode(DispatchingMode.Loadly)
    showSwitchToLoadlyPopup.value = false
  }

  function cancelSwitchToLoadly(): void {
    showSwitchToLoadlyPopup.value = false
  }

  const isContractorsDispatchingMode = computed(() => {
    return trip.value.dispatchingMode === DispatchingMode.Contractors
  })

  const validationSchema = computed(() => object({
    vehicleSource: number().required(),
    ...(!isExternalTripMode.value && isContractorsDispatchingMode.value ? {
      vehicleTypeId: string()
        .required(t('validation.required', { field: t('form.contractVehicleType') })),
    } : {}),
    contractorId: string().when('vehicleSource', {
      is: true,
      then: (schema) => schema.required(),
      otherwise: (schema) => schema.optional(),
    }),
  }))

  const fieldsGroup = computed<Record<string, FieldsGroup>>(() => ({
    contractorRelated: {
      class: 'mb-6 w-full rounded-lg border border-neutral-200 p-4',
      wrapperClass: 'flex flex-col gap-4',
      fields: [
        {
          name: 'vehicleSource',
          as: 'BaseRadioGroup',
          options: vehicleSourceOptions.value,
          label: t('form.vehicleRelatedToContractors'),
          labelClass: 'hidden',
          containerClass: 'w-full',
          wrapperClass: 'flex flex-col gap-4',
          disabled: disabled.value,
          onChange: (value: DispatchingMode) => {
            if (
              value === DispatchingMode.Loadly
              && trip.value.contractorId
              && trip.value.dispatchingMode === DispatchingMode.Contractors
            ) {
              showSwitchToLoadlyPopup.value = true

              return
            }

            applyDispatchingMode(value)
          },
        },
        ...(!isExternalTripMode.value
          ? [{
              name: 'vehicleTypeId',
              as: 'BaseSelect' as const,
              label: t('form.contractVehicleType'),
              labelClass: 'mb-1 inline-block',
              labelRequired: isContractorsDispatchingMode.value,
              options: vehicleTypesOptions.value,
              wrapperClass: 'w-full',
              searchable: true,
              disabled: disabled.value,
              onScrollToEnd: () => {
                if (hasMoreContractVehicleTypes.value) {
                  loadMoreContractVehicleTypes()
                }
              },
              onSearch: (searchKeyword: string) => {
                getContractVehicleTypes(trip.value.contractId ?? '', 1, searchKeyword)
              },
              onChange: (value: string | number) => {
                trip.value.vehicleTypeId = value != null ? String(value) : ''
                resetDriverVehicle()
              },
            }]
          : []),
      ],
    },
  }))

  return {
    cancelSwitchToLoadly,
    clearContractor,
    confirmSwitchToLoadly,
    fieldsGroup,
    isOnDemandContract,
    showSwitchToLoadlyPopup,
    unassignContractor,
    validationSchema,
  }
}
