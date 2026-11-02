<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <TripDetailsHeader />
    <TripDetailsLocation
      v-if="trip.currentLocation"
      :location="trip.currentLocation"
    />

    <TripDetailsTabs
      v-model="activeTripTab"
      :is-trip-completed
    />

    <template v-if="activeTripTab === 'information'">
      <div
        v-if="trip.financialDate"
        class="
          flex items-center gap-2 rounded-xl border border-success-500 bg-success-50 px-4 py-3
          text-sm font-medium text-success-700
        "
      >
        <IconCalendar class="size-5 shrink-0" />

        <span>{{ $t('form.financialDate') }} : {{ trip.financialDate }}</span>
      </div>

      <div class="mt-4 flex items-stretch gap-4">
        <TripDetailsDriver
          class="min-w-0 flex-1"
          :driver-name="trip.driverName"
          :driver-phone="trip.driverPhone"
        />
        <TripNotes v-if="isAdminDashboard && (trip.isEditable || trip.dispatcherNotes)" />
      </div>
      <TripDetailsStatus />
      <TripDetailsContractor v-if="trip.contractorName" />
      <TripDetailsContactPerson />
      <TripDetailsWaypoints />
    </template>

    <TripDetailsVehicleTypes v-if="activeTripTab === 'vehicleType'" />
    <TripCharges
      v-if="activeTripTab === 'charge' && isTripCompleted && hasEditChargesPermission"
      v-model:charges="charges"
      :disabled="isChargesDisabled"
    />
    <TripDetailsDocuments v-if="activeTripTab === 'documents'" />
    <TripDetailsActivity v-if="activeTripTab === 'activity'" />
  </div>
</template>

<script setup lang="ts">
  import TripDetailsActivity from './components/trip-activity/TripActivity.vue'
  import { TripStatus } from '@/types/admin-dashboard/response/trip/trip.ts'
  import type { TripWizardTripCharge } from '@/types/admin-dashboard/response/trip/trip.ts'
  import TripDetailsContactPerson from './components/TripDetailsContactPerson.vue'
  import TripDetailsDocuments from './components/TripDetailsDocuments.vue'
  import TripDetailsDriver from './components/TripDetailsDriver.vue'
  import TripNotes from './components/trip-notes/TripNotes.vue'
  import TripDetailsHeader from './components/TripDetailsHeader.vue'
  import TripDetailsLocation from './components/TripDetailsLocation.vue'
  import TripDetailsStatus from './components/TripDetailsStatus.vue'
  import TripDetailsTabs, { type TripDetailsTabId } from './components/TripDetailsTabs.vue'
  import TripDetailsVehicleTypes from './components/TripDetailsVehicleTypes.vue'
  import TripDetailsWaypoints from './components/TripDetailsWaypoints.vue'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { useLoading } from '@/composables/useLoading.ts'
  import { useApp } from '@/composables/useApp.ts'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { Permission, Trip as TripPermission } from '@/types/response/permission-group'
  import { TripDocumentStatus } from '@/types/admin-dashboard/response/trip/trip-document-status.enums.ts'

  const TripDetailsContractor = defineAsyncComponent(() => import('./components/TripDetailsContractor.vue'))

  const { t } = useI18n()
  const { setLoading } = useLoading()
  const { isAdminDashboard } = useApp()
  const { hasPermission: hasEditChargesPermission } = usePermission(
    Permission.Trip,
    TripPermission.EditCharges,
  )

  const route = useRoute()
  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)

  const breadcrumbItems = [
    { label: t('sidebar.tripManagement'), to: '/trips' },
    { label: 'Trip Details' },
  ]

  const activeTripTab = ref<TripDetailsTabId>(route.query.tab as TripDetailsTabId ?? 'information')

  const isTripCompleted = computed(() => Number(trip.value.status) === TripStatus.COMPLETED)

  const isChargesDisabled = computed(() =>
    !isTripCompleted.value
    || !!trip.value.isInvoiced
    || trip.value.tripDocumentStatus === TripDocumentStatus.Approved,
  )

  const charges = ref<TripWizardTripCharge[]>([])

  onMounted(async () => {
    const tripId = Number(route.params.id)
    if (Number.isNaN(tripId)) return

    setLoading(true)
    await tripStore.fetchTrip(tripId)
    charges.value = trip.value.tripCharges ?? []
    setLoading(false)
  })

  onUnmounted(tripStore.resetTrip)

  watch(() => route.params.id, async (id) => {
    await tripStore.fetchTrip(Number(id))
  })
</script>

<style scoped>

</style>
