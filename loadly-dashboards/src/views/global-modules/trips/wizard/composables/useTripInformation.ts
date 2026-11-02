import { boolean, date, object, string } from 'yup'
import { useApp } from '@/composables/useApp'
import { useLookups } from '@/composables/useLookups'
import { createPhoneSchema } from '@/utils/validation/phone'
import { isBeforeToday } from '@/utils/date'
import { useTripWizard } from './useTripWizard'
import type { FieldsGroup } from '@/types/form'
import type { ContractDestinationDetail } from '@/types/admin-dashboard/response/contract/contract-destination-detail'
import { useTripStore } from '@/stores/admin-dashboard/trip'
import { useContractStore } from '@/stores/admin-dashboard/contract'
import { ConsumptionType } from '@/types/request/contract/consumption-types.enums'
import { TripStatus } from '@/types/admin-dashboard/response/trip/trip'

export function useTripInformation() {
  const { t } = useI18n()
  const { isAdminDashboard } = useApp()
  const { hasDisabledFields, historicalGroup, isFormDisabled, tripDateInputBounds } = useTripWizard()
  const {
    companies,
    contractReferenceNumbers,
    currentCompanyId,
    getCompanies,
    getContractDestinations,
    getContractReferenceNumbers,
    getContractVehicleTypes,
    hasMoreCompanies,
    hasMoreContractReferenceNumbers,
    loadMoreCompanies,
    loadMoreContractReferenceNumbers,
  } = useLookups()

  const contractStore = useContractStore()
  const { contractConsumptionType } = storeToRefs(contractStore)

  const tripStore = useTripStore()
  const { trip, selectedDriverVehicle } = storeToRefs(tripStore)

  const isInProgress = computed(() => Number(trip.value.status) === TripStatus.IN_PROGRESS)
  const isActualCargoWeightDisabled = computed(() =>
    isFormDisabled.value && Number(trip.value.status) !== TripStatus.COMPLETED,
  )

  const showFinancialDate = computed(() => {
    const consumptionType = Number(
      contractConsumptionType.value ?? trip.value.contract?.consumptionType ?? 0
    )

    return consumptionType === ConsumptionType.Trips
      || consumptionType === ConsumptionType.Kilometers
  })

  const pendingDestination = ref<ContractDestinationDetail | null>(null)
  const setEndDateFromStart = shallowRef<((endDate: string) => void) | null>(null)
  const initialValues = ref({
    actualCargoWeightTons: '',
    cargoPrice: '',
    companyId: '',
    contactPersonMobile: '',
    contactPersonName: '',
    contractId: '',
    destinationId: '',
    endDate: '',
    financialDate: '',
    hasCustoms: false,
    isHistorical: false,
    spotPrice: '',
    startDate: '',
    vehicleTypeId: '',
  })

  const validationSchema = computed(() => object({
    companyId: string()
      .required(t('validation.required', { field: t('form.chooseCompany') })),
    contractId: string()
      .required(t('validation.required', { field: t('form.contractRefNumber') })),
    isHistorical: boolean(),
    startDate: string()
      .required(t('validation.required', { field: t('form.startDate') }))
      .when('isHistorical', {
        is: true,
        then: (schema) => schema.test(
          'past-only',
          () => t('validation.maxDate', { max: t('content.today') }),
          (value) => !value || isBeforeToday(value),
        ),
      }),
    endDate: date()
      .required(t('validation.required', { field: t('form.endDate') }))
      .typeError(() => t('validation.invalidDate'))
      .when('isHistorical', {
        is: true,
        then: (schema) => schema.test(
          'past-only',
          () => t('validation.maxDate', { max: t('content.today') }),
          (value) => !value || isBeforeToday(value),
        ),
      })
      .when('startDate', (startDate, schema) => {
        if (startDate && startDate.toString()) {
          return schema.min(
            new Date(startDate.toString()),
            () => t('validation.minDate', { min: t('form.startDate') })
          )
        }

        return schema
      }),
    contactPersonName: string()
      .required(t('validation.required', { field: t('form.contactName') })),
    contactPersonMobile: createPhoneSchema(t, true),
    ...(isAdminDashboard ? {
      dispatcherNotes: string()
        .optional()
        .nullable()
        .max(255, t('validation.max', { max: 255 })),
    } : {}),
    ...(showFinancialDate.value ? {
      financialDate: date()
        .required(t('validation.required', { field: t('form.financialDate') }))
        .typeError(() => t('validation.invalidDate')),
    } : {}),
  }))

  const groups = computed<Record<string, FieldsGroup>>(() => ({
    ...(historicalGroup.value ? { historical: historicalGroup.value } : {}),
    generalTripDetails: {
      class: 'mb-6',
      wrapperClass: 'mt-4 flex flex-col flex-wrap gap-4',
      fields: [
        ...(isAdminDashboard ? [{
          name: 'companyId',
          as: 'BaseSelect',
          label: t('form.chooseCompany'),
          labelClass: 'mb-1 inline-block',
          labelRequired: true,
          options: companies.value?.items ?? [],
          wrapperClass: 'w-full mb-4',
          searchable: true,
          disabled: isFormDisabled.value || isInProgress.value,
          onScrollToEnd: () => {
            if (hasMoreCompanies.value) {
              loadMoreCompanies()
            }
          },
          onChange: (value: string | number) => {
            if (!value) return
            trip.value.companyId = String(value)

            getContractReferenceNumbers(String(value))
          },
          onSearch: (searchKeyword: string) => {
            getCompanies(1, searchKeyword)
          },
        }] : []),
        {
          name: 'contractId',
          as: 'BaseSelect',
          label: t('form.contractRefNumber'),
          labelClass: 'mb-1 inline-block',
          labelRequired: true,
          options: contractReferenceNumbers.value?.items ?? [],
          wrapperClass: 'w-full mb-4',
          searchable: true,
          disabled: isFormDisabled.value || isInProgress.value,
          onScrollToEnd: () => {
            if (hasMoreContractReferenceNumbers.value) {
              loadMoreContractReferenceNumbers()
            }
          },
          onChange: (value: string | number) => {
            if (trip.value.contractId) {
              trip.value.contractId = String(value)
            }
            trip.value.spotPrice = ''
            trip.value.vehicleId = null
            trip.value.actualCargoWeightTons = ''
            trip.value.vehicleTypeId = ''
            trip.value.driverId = ''
            trip.value.destinationId = ''
            trip.value.financialDate = ''
            trip.value.contractorPrice = ''
            if (trip.value.waypoints.length) {
              trip.value.waypoints = []
            }
            selectedDriverVehicle.value = null
            getContractVehicleTypes(value)
            getContractDestinations(value)
            contractStore.getContractConsumptionType(value)
          },
          onSearch: (searchKeyword: string) => {
            getContractReferenceNumbers(currentCompanyId.value ?? '', 1, searchKeyword)
          },
        },
        ...(contractConsumptionType.value === ConsumptionType.CargoWeight || trip.value.actualCargoWeightTons ? [{
          name: 'actualCargoWeightTons',
          as: 'BaseInput',
          type: 'number',
          min: 0,
          label: t('form.actualCargoWeightTons'),
          labelClass: 'mb-1 inline-block',
          labelRequired: false,
          disabled: isActualCargoWeightDisabled.value,
        }] : []),
        ...(contractConsumptionType.value === ConsumptionType.CargoPriceConsumption || trip.value.cargoPrice ? [{
          name: 'cargoPrice',
          as: 'BaseInput',
          type: 'number',
          min: 0,
          label: t('form.cargoPrice'),
          labelClass: 'mb-1 inline-block',
          labelRequired: false,
          disabled: isFormDisabled.value,
        }] : []),
        ...(isAdminDashboard
          && (
            contractConsumptionType.value === ConsumptionType.SpotPricing
            || trip.value.spotPrice
          ) ? [{
          name: 'spotPrice',
          as: 'BaseInput',
          type: 'number',
          min: 0,
          label: t('form.spotPrice'),
          labelClass: 'mb-1 inline-block',
          labelRequired: false,
          disabled: isFormDisabled.value,
        }] : []),
      ],
    },
    tripDate: {
      title: t('form.tripDate'),
      titleClass: 'font-bold',
      class: 'mb-6',
      wrapperClass: 'mt-4 flex flex-wrap gap-4',
      fields: [
        {
          name: 'startDate',
          as: 'BaseInput',
          type: 'datetime-local',
          label: t('form.startDate'),
          labelClass: 'mb-1 inline-block',
          labelRequired: true,
          containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
          min: tripDateInputBounds.value.min,
          max: tripDateInputBounds.value.max,
          disabled: isFormDisabled.value || hasDisabledFields.value,
        },
        {
          name: 'endDate',
          as: 'BaseInput',
          type: 'datetime-local',
          label: t('form.endDate'),
          labelClass: 'mb-1 inline-block',
          labelRequired: true,
          containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
          min: tripDateInputBounds.value.min,
          max: tripDateInputBounds.value.max,
          disabled: isFormDisabled.value || (hasDisabledFields.value && !isInProgress.value),
        },
      ],
    },
    pickupAndDelivery: {
      title: t('form.pickupAndDeliveryInformation'),
      titleClass: 'font-bold',
      class: 'mb-6 rounded-lg border border-neutral-200 p-4',
      wrapperClass: 'mt-4 flex flex-wrap gap-4',
      fields: [
        {
          name: 'contactPersonName',
          as: 'BaseInput',
          type: 'text',
          label: t('form.contactName'),
          labelClass: 'mb-1 inline-block',
          labelRequired: true,
          containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
          disabled: isFormDisabled.value,
        },
        {
          name: 'contactPersonMobile',
          as: 'BaseInputPhone',
          label: t('form.contactPhoneNumber'),
          labelClass: 'mb-1 inline-block',
          labelRequired: true,
          containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
          disabled: isFormDisabled.value,
        },
      ],
    },
    distance: {
      class: 'mb-6',
      wrapperClass: 'mt-4 flex flex-col flex-wrap gap-4',
      fields: [
        {
          name: 'hasCustoms',
          as: 'BaseCheckbox',
          label: t('form.allowCustomsDuties'),
          labelClass: 'font-semibold mb-2 inline-block',
          containerClass: 'w-full',
          disabled: isFormDisabled.value,
        },
        ...(isAdminDashboard ? [{
          name: 'dispatcherNotes',
          as: 'BaseTextarea',
          label: t('form.note'),
          labelClass: 'mb-1 inline-block',
          wrapperClass: 'w-full',
          maxlength: 255,
          showCounter: true,
          disabled: isFormDisabled.value,
        }] : []),
      ],
    },
    ...(showFinancialDate.value
      ? {
        financialDate: {
          class: 'mb-6 rounded-lg border border-neutral-200 bg-neutral-100 p-4 [&_div]:flex [&_div]:flex-wrap [&_div]:items-baseline [&_div]:gap-x-2',
          title: t('form.financialDate'),
          titleClass: '!mb-0 font-bold !text-neutral-10',
          description: t('form.financialDateAccountingDay'),
          wrapperClass: 'mt-4 flex flex-wrap gap-4',
          fields: [
            {
              name: 'financialDate',
              as: 'BaseInput',
              type: 'date',
              label: t('form.financialDate'),
              labelClass: 'mb-1 inline-block',
              labelRequired: true,
              containerClass: 'w-full',
              disabled: isFormDisabled.value && Number(trip.value.status) !== TripStatus.COMPLETED,
            },
          ],
        },
      }
      : {}),
  }))

  watch(showFinancialDate, (isVisible) => {
    if (!isVisible) {
      trip.value.financialDate = ''
    }
  })

  function bindSetEndDate(setter: (endDate: string) => void): void {
    setEndDateFromStart.value = setter
  }

  return {
    bindSetEndDate,
    groups,
    initialValues,
    contractConsumptionType,
    pendingDestination,
    validationSchema,
  }
}
