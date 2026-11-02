import { useApp } from "@/composables/useApp"
import { useTripStore } from "@/stores/admin-dashboard/trip"
import { TripStatus } from "@/types/admin-dashboard/response/trip/trip"
import { usePermission } from "@/composables/admin-dashboard/usePermission"
import { Permission, Trip } from "@/types/response/permission-group"
import type { FieldsGroup } from "@/types/form"
import { getMinDate, getYesterdayDate } from "@/utils/date"

export function useTripWizard() {
  const { t } = useI18n()
  const route = useRoute()
  const { isAdminDashboard } = useApp()
  const { hasPermission: hasEditCompletedPermission } = usePermission(Permission.Trip, Trip.EditCompleted)
  const { hasPermission: hasCreateHistoricalTripPermission } = usePermission(
    Permission.Trip,
    Trip.CreateHistoricalTrip,
  )

  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)

  const isEditMode = computed(() => route.name === 'EditTrip')
  const isCreateMode = computed(() => route.name === 'CreateTrip' || route.name === 'CreateExternalTrip')
  const isExternalTripMode = computed(() => route.name === 'CreateExternalTrip' || trip.value.isExternalTrip)
  const isHistorical = computed(() => Boolean(trip.value.isHistorical))

  const tripDateInputBounds = computed<{
    min?: string;
    max?: string;
    minDate?: string;
    maxDate?: string
  }>(() => {
    if (isCreateMode.value && isHistorical.value) {
      return { max: `${getYesterdayDate()}T23:59` }
    }

    return { min: getMinDate() }
  })

  const historicalGroup = computed<FieldsGroup | null>(() => {
    if (!isCreateMode.value || !hasCreateHistoricalTripPermission.value) return null

    return {
      class: 'mb-6',
      wrapperClass: 'mt-2',
      fields: [{
        name: 'isHistorical',
        as: 'BaseCheckbox',
        label: t('form.historicalTripHint'),
        labelClass: 'hidden',
        onCheckedChange: (value: boolean) => {
          trip.value.isHistorical = value
          trip.value.vehicleTypeId = ''
        },
      }],
    }
  })

  const pageTitle = computed(() => {
    if (isEditMode.value) return t('title.editTrip')
    if (isExternalTripMode.value) return t('title.createExternalTrip')

    return t('title.createTrip')
  })

  const isFormDisabled = computed(() => {
    if (isCreateMode.value) return false
    const notAllowedStatuses = [TripStatus.CANCELLED, TripStatus.REVIEWED , TripStatus.COMPLETED]

    return notAllowedStatuses.includes(Number(trip.value?.status) as TripStatus)
  })

  const hasDisabledFields = computed(() => {
    if (isCreateMode.value) return false
    const notAllowedStatuses = [
      TripStatus.DRIVER_ACCEPTED,
      TripStatus.FIRST_PICKUP_LOADING,
      TripStatus.IN_PROGRESS,
      TripStatus.CANCELLED,
      TripStatus.REVIEWED,
      TripStatus.COMPLETED
    ]

    return notAllowedStatuses.includes(Number(trip.value?.status) as TripStatus)
  })

  const isFormDisabledWithCompletedStatus = computed(() => (
    isFormDisabled.value
    && !(isAdminDashboard && Number(trip.value.status) === TripStatus.COMPLETED && hasEditCompletedPermission.value)
  ))

  return {
    isEditMode,
    isCreateMode,
    isExternalTripMode,
    isFormDisabled,
    isFormDisabledWithCompletedStatus,
    hasDisabledFields,
    historicalGroup,
    pageTitle,
    tripDateInputBounds,
  }
}
