<template>
  <TripsFilters @apply-filters="applyFilters" />

  <BaseTable
    :columns="tableColumns"
    :data="trips"
  >
    <template #cell-dispatchingMode="{ row }">
      <template v-if="row.dispatchingMode === DispatchingMode.Contractors">
        <span class="me-1 font-medium">{{ $t('content.contr') }}:</span>
        {{ row.contractorName || '-' }}
      </template>

      <template v-else>
        {{ $t('form.loadly') }}
      </template>
    </template>

    <template #cell-sector="{ row }">
      {{ row.sector?.name ?? '-' }}
    </template>

    <template #cell-lifecycleActor="{ row }">
      <TripListLifecycleActor :trip="row" />
    </template>

    <template #cell-plateNumber="{ row }">
      <TripPlateNumber
        head-only
        :plate-letters="row.plateLetters"
        :plate-no="row.plateNumber"
        :vehicle-snapshot="row.vehicleSnapshot"
      />
    </template>

    <template #cell-status="{ row }">
      <TripListStatus
        :trip-id="row.id"
        :status="row.status"
        :label="getDisplayName('TripStatus', row.status)"
        @updated="fetchTrips"
      />

      <div
        v-if="row.overNightCostBearerName"
        class="relative flex items-center gap-1 ps-2 text-xs font-medium text-warning-500"
      >
        <IconMoon class="size-4" />
        {{ $t('form.overNight') }}: {{ row.overNightCostBearerName }}
      </div>

      <div
        v-if="row.currentDriverStatus && row.status === TripStatus.IN_PROGRESS"
        class="relative flex items-center gap-1 ps-2 text-xs font-medium text-primary"
      >
        <span
          class="
            relative size-2 rounded-full bg-primary
            after:absolute after:start-0.5 after:top-1/2 after:size-0.5 after:translate-x-1/2
            after:-translate-y-1/2 after:rounded-full after:bg-white after:content-['']
          "
        />
        {{ $t('table.status') }}: {{ getDisplayName('OperationalDriverStatus', row.currentDriverStatus) }}
      </div>
    </template>

    <template #cell-financialDate="{ row }">
      {{ row.financialDate || '-' }}

      <div
        v-if="row.isBackfill"
        class="relative flex items-center gap-1 text-xs font-medium text-warning-500"
      >
        <IconClock class="size-3" />
        {{ $t('form.historical') }}
      </div>
    </template>

    <template #cell-destinationName="{ row }">
      <bdi class="block truncate text-xs text-neutral-600">
        {{ getTripPickupAddress(row) }}
      </bdi>

      <bdi class="block truncate text-xs text-neutral-600">
        {{ getTripDropOffAddress(row) }}
      </bdi>
    </template>

    <template #cell-action="{ row }">
      <div class="flex items-center gap-2">
        <RouterLink
          v-if="hasViewPermission || isCompanyDashboard"
          :to="{ name: 'ViewTrip', params: { id: row.id } }"
          class="btn-info-outline"
          :aria-label="$t('a11y.view')"
        >
          <IconEye
            visible
            class="size-4"
          />
        </RouterLink>

        <template v-if="row.isEditable">
          <RouterLink
            v-if="canEditTrip(row.status) && !row.isBackfill"
            :to="{ name: 'EditTrip', params: { id: row.id } }"
            class="btn-action-success"
            :aria-label="$t('a11y.edit')"
          >
            <IconEdit class="size-4" />
          </RouterLink>

          <button
            v-if="isAdminDashboard && hasDeletePermission"
            type="button"
            class="btn-action-error"
            :aria-label="$t('a11y.delete')"
            @click="openDeletePopup(row)"
          >
            <IconTrash class="size-4" />
          </button>
        </template>
      </div>
    </template>
  </BaseTable>

  <BasePagination
    :pagination="pagination"
    @set-page="setPage"
    @next-page="nextPage"
    @prev-page="prevPage"
    @update-items-per-page="updateItemsPerPage"
  />

  <DeleteTripPopup
    v-if="isDeletePopupOpen"
    v-model="isDeletePopupOpen"
    :is-loading="isDeleting"
    :message="deleteErrorMessage"
    @confirm="confirmDeleteTrip"
    @cancel="selectedTrip = null"
  />
</template>

<script setup lang="ts">
  import type { TableColumn } from '@/types/ui'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { Permission, Trip as TripPermission } from '@/types/response/permission-group'
  import { usePagination } from '@/composables/usePagination'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { useEnum } from '@/composables/useEnum'
  import TripsFilters from '../components/TripsFilters.vue'
  import TripListLifecycleActor from './components/TripListLifecycleActor.vue'
  import TripPlateNumber from './components/TripPlateNumber.vue'
  import { useTripListLifecycleActor } from './composables/useTripListLifecycleActor'
  import { DispatchingMode, TripStatus } from '@/types/admin-dashboard/response/trip/trip'
  import type { Trip } from '@/types/admin-dashboard/response/trip/trip'
  import { useApp } from '@/composables/useApp'
  import { getTripDropOffAddress, getTripPickupAddress } from '@/utils/trip'

  const TripListStatus = defineAsyncComponent(() => import('./components/TripListStatus.vue'))
  const DeleteTripPopup = defineAsyncComponent(() => import('./components/DeleteTripPopup.vue'))

  const { t } = useI18n()
  const route = useRoute()
  const { getDisplayName } = useEnum()
  const { isAdminDashboard, isCompanyDashboard } = useApp()
  const { hasPermission: hasViewPermission } = usePermission(Permission.Trip, TripPermission.View)
  const { hasPermission: hasEditPermission } = usePermission(Permission.Trip, TripPermission.Edit)
  const { hasPermission: hasEditCompletedPermission } = usePermission(Permission.Trip, TripPermission.EditCompleted)
  const { hasPermission: hasEditInProgressPermission } = usePermission(Permission.Trip, TripPermission.EditInProgress)
  const { hasPermission: hasDeletePermission } = usePermission(Permission.Trip, TripPermission.Delete)

  const tripStore = useTripStore()
  const { tripsList, filters } = storeToRefs(tripStore)
  const { fetchAssistedAdmins } = useTripListLifecycleActor()

  const {
    currentPage,
    itemsPerPage,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  } = usePagination(computed(() => tripsList.value?.pagination ?? null))

  const trips = computed(() => tripsList.value?.data ?? [])
  const isExternalTrip = computed(() => route.path.includes('external'))

  const tableColumns = computed<TableColumn[]>(() => [
    { key: 'referenceNumber', label: t('table.tripId') },
    ...(isAdminDashboard
      ? [{ key: 'dispatchingMode', label: t('form.carrier') }]
      : []),
    ...(isAdminDashboard && !isExternalTrip.value
      ? [{ key: 'companyName', label: t('table.companyName'), cellClass: 'font-medium text-neutral-20 max-w-32' }]
      : []),
    ...(isAdminDashboard
      ? [{ key: 'sector', label: t('title.sectors') }]
      : []),
    ...(isAdminDashboard && isExternalTrip.value
      ? [{ key: 'contactPersonName', label: t('table.leadName') }]
      : []),
    ...(isAdminDashboard && isExternalTrip.value
      ? [{ key: 'contactPersonMobile', label: t('table.leadMobile') }]
      : []),
    ...(isAdminDashboard
      ? [{ key: 'lifecycleActor', label: t('table.doneBy'), cellClass: 'max-w-32' }]
      : []),
    { key: 'financialDate', label: t('form.financialDate'), cellClass: 'flex flex-col items-start gap-1' },
    { key: 'driverName', label: t('table.driverName'), cellClass: 'max-w-32' },
    { key: 'plateNumber', label: t('table.plateNumber') },
    { key: 'destinationName', label: t('table.destination'), cellClass: 'max-w-32 break-all' },
    { key: 'status', label: t('table.status'), cellClass: 'flex flex-col items-start gap-1' },
    { key: 'action', label: t('table.action') },
  ])

  const isDeletePopupOpen = ref(false)
  const selectedTrip = ref<Trip | null>(null)
  const deleteErrorMessage = ref('')
  const isDeleting = ref(false)

  function applyFilters() {
    if (currentPage.value === 1) fetchTrips()
    currentPage.value = 1
  }

  async function fetchTrips() {
    await tripStore.fetchTrips({
      page: currentPage.value,
      size: itemsPerPage.value,
      ...filters.value,
      isExternalTrip: isAdminDashboard && isExternalTrip.value,
    })

    if (isAdminDashboard) await fetchAssistedAdmins()
  }

  function canEditTrip(status: TripStatus): boolean {
    if (
      !hasEditPermission.value
      && !hasEditCompletedPermission.value
      && !hasEditInProgressPermission.value
    ) return false

    if (isCompanyDashboard) return status === TripStatus.DRAFT

    if (hasEditCompletedPermission.value && status === TripStatus.COMPLETED) return true
    if (hasEditInProgressPermission.value && status === TripStatus.IN_PROGRESS) return true

    return hasEditPermission.value
      && [
        TripStatus.REQUESTED,
        TripStatus.PENDING,
        TripStatus.SCHEDULED
      ].includes(status)
  }

  function openDeletePopup(trip: Trip): void {
    selectedTrip.value = trip
    isDeletePopupOpen.value = true
  }

  async function confirmDeleteTrip(): Promise<void> {
    if (!selectedTrip.value) return

    const tripId = Number(selectedTrip.value.id)
    if (!Number.isInteger(tripId) || tripId <= 0) return

    isDeleting.value = true
    const { success, message } = await tripStore.deleteTrip(tripId)
    isDeleting.value = false

    if (success) {
      isDeletePopupOpen.value = false
      selectedTrip.value = null
      fetchTrips()

      return
    }

    deleteErrorMessage.value = message
  }

  watch([currentPage, itemsPerPage], fetchTrips)
  watch(() => route.path, () => {
    fetchTrips()
    currentPage.value = 1
  })

  onMounted(fetchTrips)

  onUnmounted(() => {
    if (!route.path.includes('/trip')) {
      tripStore.resetFilters()
    }
  })
</script>

<style scoped>

</style>
