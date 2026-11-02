<template>
  <PopupError
    v-if="reviewError"
    :model-value="reviewError !== null"
    :message="reviewError"
    @update:model-value="reviewError = null"
  />

  <PopupError
    v-if="deleteError"
    :model-value="deleteError !== null"
    :message="deleteError"
    @update:model-value="deleteError = null"
  />

  <DeleteDraftTripPopup
    v-model="isDeleteDraftPopupOpen"
    :is-loading="isDeleteDraftLoading"
    :trip-id-label="String(trip.referenceNumber || '—')"
    @confirm="onDeleteDraftConfirm"
  />

  <div
    class="
      mb-4 flex flex-col gap-4
      lg:flex-row lg:flex-wrap lg:items-center lg:justify-between
    "
  >
    <h1 class="shrink-0 text-xl font-bold text-neutral-800">
      {{ $t('content.tripId') }} {{ trip.referenceNumber || '—' }}
    </h1>

    <div
      v-if="trip.overNightCostBearer"
      class="
        inline-flex items-center gap-2 rounded-full border border-success-500 bg-success-50 px-3
        py-1.5 text-sm font-medium text-success-500 transition-colors
      "
    >
      <IconOvernight class="size-4 shrink-0" />
      {{ $t('form.overNight') }} / {{ trip.overNightCostBearerName }}
    </div>

    <div
      v-if="canDeleteDraftTrip || showEditTrip"
      class="me-auto flex flex-wrap items-center gap-3"
    >
      <button
        v-if="canDeleteDraftTrip"
        type="button"
        class="btn-error"
        :aria-label="$t('a11y.delete')"
        :disabled="isDeleteDraftLoading"
        @click="isDeleteDraftPopupOpen = true"
      >
        {{ $t('button.delete') }}
        <IconTrash class="size-4" />
      </button>

      <RouterLink
        v-if="showEditTrip"
        :to="{ name: 'EditTrip', params: { id: trip.id } }"
        class="btn-link flex items-center gap-2"
        :aria-label="$t('a11y.edit')"
      >
        {{ $t('button.edit') }}
        <IconEdit class="size-4" />
      </RouterLink>
    </div>

    <div
      class="
        flex w-auto flex-wrap items-center gap-2
        sm:gap-3
      "
    >
      <span class="text-sm text-content-secondary">
        {{ $t('content.tripEndedOn') }}
      </span>

      <span class="text-sm font-medium text-neutral-10">
        {{ endedAtFormatted }}
      </span>
    </div>
  </div>
</template>

<script setup lang="ts">
  import DeleteDraftTripPopup from './DeleteDraftTripPopup.vue'
  import { TripStatus } from '@/types/admin-dashboard/response/trip/trip'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { useLoading } from '@/composables/useLoading'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { Permission, Trip as TripPermission } from '@/types/response/permission-group'
  import { useApp } from '@/composables/useApp'

  const { t } = useI18n()
  const router = useRouter()
  const { setLoading } = useLoading()
  const { isAdminDashboard, isCompanyDashboard } = useApp()
  const { hasPermission: hasEditPermission } = usePermission(Permission.Trip, TripPermission.Edit)
  const { hasPermission: hasEditCompletedPermission } = usePermission(Permission.Trip, TripPermission.EditCompleted)
  const { hasPermission: hasEditInProgressPermission } = usePermission(Permission.Trip, TripPermission.EditInProgress)

  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)

  const reviewError = ref<string | null>(null)
  const deleteError = ref<string | null>(null)
  const isDeleteDraftPopupOpen = ref(false)
  const isDeleteDraftLoading = ref(false)

  const isDraftStatus = computed(
    () => Number(trip.value.status) === TripStatus.DRAFT,
  )

  const canDeleteDraftTrip = computed(
    () =>
      isCompanyDashboard
      && isDraftStatus.value
      && trip.value.id != null
      && String(trip.value.id).length > 0
  )

  const canEditTrip = computed(() => {
    if (!hasEditPermission.value && !hasEditCompletedPermission.value && !hasEditInProgressPermission.value) return false

    if (hasEditCompletedPermission.value && Number(trip.value.status) === TripStatus.COMPLETED) return true
    if (hasEditInProgressPermission.value && Number(trip.value.status) === TripStatus.IN_PROGRESS) return true

    return hasEditPermission.value
      && [
        TripStatus.REQUESTED,
        TripStatus.PENDING,
        TripStatus.SCHEDULED
      ].includes(Number(trip.value.status) as TripStatus)
  })

  const showEditTrip = computed(() =>
    canEditTrip.value
    && isAdminDashboard
    && !!trip.value.isEditable
    && !trip.value.isBackfill
  )

  const endedAtFormatted = computed(() => {
    if (!trip.value.endDate) return t('content.notAvailable')
    const tripDate = new Date(trip.value.endDate)
    if (Number.isNaN(tripDate.getTime())) return t('content.notAvailable')

    const date = tripDate.toLocaleDateString('en-GB', {
      day: '2-digit',
      month: 'long',
      year: '2-digit',
    })
    const time = tripDate.toLocaleTimeString('en-GB', {
      hour: '2-digit',
      minute: '2-digit',
      hour12: true,
    })

    return `${date} | ${time}`
  })

  async function onDeleteDraftConfirm(): Promise<void> {
    const id = trip.value.id
    if (id == null || id === '' || isDeleteDraftLoading.value) return

    const tripId = Number(id)
    if (!Number.isInteger(tripId) || tripId <= 0) return

    isDeleteDraftLoading.value = true
    deleteError.value = null
    setLoading(true)

    const { success, message } = await tripStore.deleteTrip(tripId)

    isDeleteDraftLoading.value = false
    setLoading(false)

    if (success) {
      isDeleteDraftPopupOpen.value = false
      await router.push({ name: 'AllTrips' })

      return
    }

    isDeleteDraftPopupOpen.value = false
    deleteError.value = message || t('error.somethingWentWrong')
  }
</script>

<style scoped>

</style>
