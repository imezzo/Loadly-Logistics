<template>
  <PopupSuccess
    v-if="showSuccessPopup"
    :model-value="showSuccessPopup"
    :message="successMessage"
    @update:model-value="showSuccessPopup = false"
  >
    <template #actions>
      <RouterLink
        :to="{ name: 'AllTrips' }"
        class="btn-primary mt-6 w-full"
        @click="showSuccessPopup = false"
      >
        {{ $t('button.close') }}
      </RouterLink>
    </template>
  </PopupSuccess>

  <PopupError
    v-if="createError !== null"
    :model-value="createError !== null"
    :message="createError"
    :errors="createErrors"
    @update:model-value="clearCreateError"
  />

  <div class="mb-6 flex flex-wrap items-center gap-4">
    <h1 class="text-xl font-bold text-neutral-800">
      {{ pageTitle }}
    </h1>

    <template v-if="isEditMode && Number(trip.status) !== TripStatus.COMPLETED">
      <OvernightPopup v-if="!trip.overNightCostBearer" />
      <div
        v-else
        class="
          inline-flex items-center gap-2 rounded-full border border-success-500 bg-success-50 px-3
          py-1.5 text-sm font-medium text-success-500 transition-colors
        "
      >
        <IconOvernight class="size-4 shrink-0" />
        {{ $t('form.overNightStarted') }}
      </div>
    </template>
  </div>

  <div
    class="
      flex flex-col gap-6
      lg:flex-row lg:gap-12
    "
  >
    <NavigationMenu
      :steps="steps"
      :current-step="currentStep"
      :clickable="isEditMode"
      @select-step="selectStep"
    />

    <div
      class="
        flex-1 ps-6
        2md:w-1/2
      "
    >
      <template v-if="currentStep === TripWizardStep.TripInformation">
        <TripExternalInformation
          v-if="isExternalTripMode"
          :errors="createErrors"
          @next="currentStep = TripWizardStep.DriverAssignments"
        />

        <TripInformation
          v-else
          :errors="createErrors"
          @next="isAdminDashboard
            ? currentStep = TripWizardStep.DriverAssignments
            : currentStep = TripWizardStep.PickPointsAndDropPoints"
        />
      </template>

      <TripHistoricalAssignments
        v-else-if="currentStep === TripWizardStep.DriverAssignments && isAdminDashboard && trip.isHistorical"
        @prev="currentStep = TripWizardStep.TripInformation"
        @next="currentStep = TripWizardStep.PickPointsAndDropPoints"
      />

      <TripDriverAssignments
        v-else-if="currentStep === TripWizardStep.DriverAssignments && isAdminDashboard"
        @prev="currentStep = TripWizardStep.TripInformation"
        @next="currentStep = TripWizardStep.PickPointsAndDropPoints"
      />

      <TripPoints
        v-else-if="currentStep === TripWizardStep.PickPointsAndDropPoints"
        :errors="createErrors"
        @prev="isAdminDashboard
          ? currentStep = TripWizardStep.DriverAssignments
          : currentStep = TripWizardStep.TripInformation"
        @next="currentStep = TripWizardStep.ReviewAndSubmit"
      />

      <TripPreview
        v-else-if="currentStep === TripWizardStep.ReviewAndSubmit"
        @prev="currentStep = TripWizardStep.PickPointsAndDropPoints"
        @submit="onSubmitTrip"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
  import IconEditForm from '@/components/icon/IconEditForm.vue'
  import IconLocation from '@/components/icon/IconLocation.vue'
  import IconSearch from '@/components/icon/IconSearch.vue'
  import IconTruck from '@/components/icon/IconTruck.vue'
  import { useLoading } from '@/composables/useLoading'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { useTripWizard } from '../composables/useTripWizard'
  import TripDriverAssignments from './trip-assignments/TripDriverAssignments.vue'
  import TripPoints from './trip-waypoints/TripPoints.vue'
  import TripPreview from './trip-preview/TripPreview.vue'
  import { useApp } from '@/composables/useApp'
  import { TripStatus } from '@/types/admin-dashboard/response/trip/trip'

  const OvernightPopup = defineAsyncComponent(() => import('./OvernightPopup.vue'))
  const TripExternalInformation = defineAsyncComponent(() => import('./trip-external-information/TripExternalInformation.vue'))
  const TripInformation = defineAsyncComponent(() => import('./trip-information/TripInformation.vue'))
  const TripHistoricalAssignments = defineAsyncComponent(
    () => import('./trip-historical-assignments/TripHistoricalAssignments.vue'))

  enum TripWizardStep {
    TripInformation,
    DriverAssignments,
    PickPointsAndDropPoints,
    ReviewAndSubmit,
  }

  const TRIP_INFO_FIELDS = [
    'companyId',
    'contractId',
    'startDate',
    'endDate',
    'financialDate',
    'contactPersonName',
    'contactPersonMobile',
    'hasCustoms',
    'dispatcherNotes',
  ]
  const DRIVER_FIELDS = ['contractorId', 'driverId', 'vehicleId', 'tailVehicleId', 'vehicleTypeId']
  const EXTERNAL_TRIP_FIELDS = [
    'leadId',
    'contactPersonName',
    'contactPersonMobile',
    'startDate',
    'endDate',
    'spotPrice',
    'dispatcherNotes',
  ]
  const WAYPOINT_FIELDS = ['contractDistanceKm']
  const WAYPOINT_FIELDS_PREFIX = 'waypoints'

  const route = useRoute()
  const { t } = useI18n()
  const { isAdminDashboard } = useApp()
  const { isLoading, setLoading } = useLoading()
  const { isEditMode, isExternalTripMode, pageTitle } = useTripWizard()

  const tripStore = useTripStore()
  const { selectedDriverVehicle, trip } = storeToRefs(tripStore)

  const currentStep = ref(TripWizardStep.TripInformation)
  const showSuccessPopup = ref(false)
  const successMessage = ref('')
  const createError = ref<string | null>(null)
  const createErrors = ref<Record<string, string[]>>({})

  const steps = computed(() => [
    { title: t('content.tripInformation'), icon: IconEditForm, id: TripWizardStep.TripInformation },
    ...(isAdminDashboard ? [{ title: t('content.vehicleAssignments'), icon: IconTruck, id: TripWizardStep.DriverAssignments }] : []),
    { title: t('content.pickPointsAndDropPoints'), icon: IconLocation, id: TripWizardStep.PickPointsAndDropPoints },
    { title: t('content.reviewAndSubmit'), icon: IconSearch, id: TripWizardStep.ReviewAndSubmit },
  ])

  function stepForErrorKeys(errors: Record<string, string[]>): TripWizardStep {
    const keys = Object.keys(errors)

    for (const key of keys) {
      if (EXTERNAL_TRIP_FIELDS.includes(key) || TRIP_INFO_FIELDS.includes(key)) {
        return TripWizardStep.TripInformation
      }
      if (DRIVER_FIELDS.includes(key)) return TripWizardStep.DriverAssignments
      if (WAYPOINT_FIELDS.includes(key)
        || key.startsWith(WAYPOINT_FIELDS_PREFIX)
        || key === 'waypoints'
        || key === 'contractorPrice') {
        return TripWizardStep.PickPointsAndDropPoints
      }
    }

    return TripWizardStep.ReviewAndSubmit
  }

  function clearCreateError() {
    createError.value = null
    createErrors.value = {}
  }

  async function onSubmitTrip(submitRequest = true) {
    if (isLoading.value) return

    setLoading(true)
    clearCreateError()

    const vehicleId = selectedDriverVehicle.value?.id
    const tripId = isEditMode.value ? Number(route.params.id) : NaN
    const isUpdate = isEditMode.value && Number.isInteger(tripId) && tripId > 0
    trip.value.submitRequest = submitRequest

    let saveTrip
    if (isUpdate) {
      saveTrip = tripStore.updateTrip(tripId, vehicleId)
    } else if (trip.value.isHistorical) {
      saveTrip = tripStore.createHistoricalTrip(vehicleId)
    } else {
      saveTrip = tripStore.createTrip(vehicleId)
    }

    const { success, message, errors } = await saveTrip

    setLoading(false)

    if (success) {
      successMessage.value =
        message || (isUpdate ? t('success.tripUpdated') : t('success.tripCreated'))
      showSuccessPopup.value = true

      return
    }

    createError.value = message || t('error.somethingWentWrong')
    createErrors.value = (errors as Record<string, string[]>) ?? {}
    currentStep.value = stepForErrorKeys(createErrors.value)
  }

  function selectStep(step: TripWizardStep) {
    if (!isEditMode.value) return

    currentStep.value = step
  }
</script>

<style scoped>

</style>
