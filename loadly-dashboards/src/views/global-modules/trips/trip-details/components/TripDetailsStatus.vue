<template>
  <section
    class="
      mt-6 rounded-xl border border-neutral-100 bg-white p-4
      sm:p-6
    "
  >
    <p
      v-if="cancellationReasonDisplay"
      class="mb-3 rounded-lg bg-error-100 p-2.5 text-xs font-bold text-error-600"
    >
      {{ $t('content.cancellationReason') }}: <span class="font-normal">{{ cancellationReasonDisplay }}</span>
    </p>

    <div
      class="
        flex flex-wrap items-center gap-x-4 gap-y-2 border-b border-gray-50 pb-4 text-sm
        text-neutral-700
      "
    >
      <BaseChip
        :label="statusLabel"
        :variant="tripStatusVariants[trip.status as unknown as TripStatus]
          ?? 'neutral'"
        class="rounded-full!"
      >
        <IconTruck class="me-1 size-3.5 shrink-0" />
        {{ statusLabel }}
      </BaseChip>

      <BaseChip
        v-if="Number(trip.status) === TripStatus.IN_PROGRESS"
        :label="$t('content.driverStatus') + ': ' + getDisplayName('OperationalDriverStatus', Number(trip.currentDriverStatus))"
        variant="info"
        class="rounded-full!"
      />

      <BaseChip
        v-if="isAdminDashboard"
        :label="t('content.tripDocumentStatus') + ': ' + trip.tripDocumentStatusName"
        :variant="tripDocumentStatusVariants[Number(trip.tripDocumentStatus)] ?? 'neutral'"
        class="rounded-full!"
      />

      <BaseChip
        :label="trip.isExternalTrip ? t('content.externalTrip') : t('content.contractTrip')"
        :variant="trip.isExternalTrip ? 'success' : 'info'"
        class="rounded-full!"
      />

      <span>{{ numberOfDaysSummary }}</span>

      <template v-if="isAdminDashboard">
        <span
          class="
            hidden h-5 w-px shrink-0 bg-neutral-200
            sm:block
          "
          aria-hidden="true"
        />

        <span>
          {{ $t('content.sector') }}: {{ trip.sector?.name || emDash }}
        </span>
      </template>

      <div
        v-if="trip.isBackfill"
        class="relative flex items-center gap-0.75 text-sm font-medium text-warning-500"
      >
        <IconClock class="size-4" />
        {{ $t('form.historical') }}

        <span
          v-if="trip.createdAt"
          class="font-normal"
        >
          ({{ formatTripDate(trip.createdAt) }})
        </span>
      </div>
    </div>

    <div
      class="
        mt-6 grid gap-8
        sm:grid-cols-2
        lg:grid-cols-4
      "
    >
      <div>
        <p class="text-sm font-bold text-neutral-800">
          {{ $t('content.deliveryFrom') }}
        </p>

        <a
          v-if="deliveryFrom && !trip.destinationId"
          :href="getGoogleMapsLink(deliveryFrom?.latitude ?? '', deliveryFrom?.longitude ?? '')"
          target="_blank"
          class="mt-1 text-sm text-primary underline"
        >
          {{ deliveryFrom?.addressName || emDash }}
        </a>

        <p v-else-if="deliveryFrom">
          {{ deliveryFrom?.addressName || emDash }}
        </p>
      </div>

      <div>
        <p class="text-sm font-bold text-neutral-800">
          {{ $t('content.deliveryTo') }}
        </p>

        <a
          v-if="deliveryTo && !trip.destinationId"
          :href="getGoogleMapsLink(deliveryTo?.latitude ?? '', deliveryTo?.longitude ?? '')"
          target="_blank"
          class="mt-1 text-sm text-primary underline"
        >
          {{ deliveryTo?.addressName || emDash }}
        </a>

        <p v-else-if="deliveryTo">
          {{ deliveryTo?.addressName || emDash }}
        </p>
      </div>

      <div>
        <p class="text-sm font-bold text-neutral-800">
          {{ $t('form.startDate') }}
        </p>

        <p class="mt-1 text-sm text-neutral-600">
          {{ startDateFormatted }}
        </p>
      </div>

      <div>
        <p class="text-sm font-bold text-neutral-800">
          {{ $t('content.contractDistance') }}
        </p>

        <p class="mt-1 text-sm text-neutral-600">
          {{ contractDistanceDisplay }}
        </p>
      </div>

      <div>
        <p class="text-sm font-bold text-neutral-800">
          {{ $t('content.shipmentType') }}
        </p>

        <p class="mt-1 text-sm text-neutral-600">
          {{ shipmentTypeDisplay }}
        </p>
      </div>

      <div>
        <p class="text-sm font-bold text-neutral-800">
          {{ $t('content.estimatedTime') }}
        </p>

        <p class="mt-1 text-sm text-neutral-600">
          {{ estimatedDurationText }}
        </p>
      </div>

      <div>
        <p class="text-sm font-bold text-neutral-800">
          {{ $t('form.endDate') }}
        </p>

        <p class="mt-1 text-sm text-neutral-600">
          {{ endDateFormatted }}
        </p>
      </div>

      <div v-if="isAdminDashboard">
        <p class="text-sm font-bold text-neutral-800">
          {{ $t('content.actualDistance') }}
        </p>

        <p class="mt-1 text-sm text-neutral-600">
          {{ actualDistanceDisplay }}
        </p>
      </div>

      <div v-if="isAdminDashboard">
        <p class="text-sm font-bold text-neutral-800">
          {{ $t('form.actualCargoWeightTons') }}
        </p>

        <p class="mt-1 text-sm text-neutral-600">
          {{ trip.actualCargoWeightTons || '-' }}
        </p>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
  import { TripStatus } from '@/types/admin-dashboard/response/trip/trip'
  import { useEnum } from '@/composables/useEnum'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { tripStatusVariants, tripDocumentStatusVariants } from '@/utils/statusChipVariants'
  import { useApp } from '@/composables/useApp'

  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)

  const { t } = useI18n()
  const { isAdminDashboard } = useApp()
  const { getDisplayName } = useEnum()

  const emDash = '—'

  const deliveryFrom = computed(() =>
    trip.value.waypoints?.find(waypoint => waypoint.stopType === '0'),
  )

  const deliveryTo = computed(() =>
    trip.value.waypoints?.find(waypoint => waypoint.stopType === '1'),
  )

  const shipmentTypeDisplay = computed(() => trip.value.vehicleType?.trim() || emDash)

  const statusNumeric = computed(() => Number(trip.value.status))

  const statusLabel = computed(() => {
    if (!Number.isFinite(statusNumeric.value)) return t('content.notAvailable')

    return t('content.tripStatus') + ': ' + getDisplayName('TripStatus', statusNumeric.value)
  })

  const cancellationReasonDisplay = computed(() => {
    if (statusNumeric.value !== TripStatus.CANCELLED) return null

    const reason = trip.value.cancellationReason?.trim()

    return reason || null
  })

  const numberOfDaysSummary = computed(() => {
    const days = trip.value.numberOfDays
    if (typeof days !== 'number' || !Number.isFinite(days)) return t('content.notAvailable')

    return t('content.numberOfDaysSummary', { count: days })
  })

  function getGoogleMapsLink(lat: number | string, lng: number | string): string {
    return `https://www.google.com/maps?q=${lat},${lng}`;
  }

  function formatTripDate(value: string): string {
    const date = new Date(value)
    if (Number.isNaN(date.getTime())) return t('content.notAvailable')

    return date.toLocaleDateString('en-GB', {
      day: 'numeric',
      month: 'long',
      year: 'numeric',
    })
  }

  const startDateFormatted = computed(() => formatTripDate(trip.value.startDate))
  const endDateFormatted = computed(() => formatTripDate(trip.value.endDate))

  function formatKm(km: number | null | undefined): string {
    if (km == null || Number.isNaN(Number(km))) return emDash

    return `${Math.round(Number(km)).toLocaleString('en-GB')} KM`
  }

  const contractDistanceDisplay = computed(() => {
    const raw = trip.value.contractDistanceKm
    if (raw == null || raw === '') return emDash
    const n = Number.parseFloat(String(raw))

    return Number.isFinite(n) ? `${Math.round(n).toLocaleString('en-GB')} KM` : emDash
  })

  const actualDistanceDisplay = computed(() => formatKm(trip.value.actualDistanceKm ?? null))

  const estimatedDurationText = computed(() => {
    const start = new Date(trip.value.startDate)
    const end = new Date(trip.value.endDate)
    if (Number.isNaN(start.getTime()) || Number.isNaN(end.getTime())) {
      return t('content.notAvailable')
    }

    const diffInHours = Math.max(0, Math.round((end.getTime() - start.getTime()) / 3_600_000))
    if (diffInHours <= 0) return t('content.notAvailable')
    if (diffInHours === 1) return t('content.oneHour')

    return t('content.almostHours', { hours: diffInHours })
  })
</script>

<style scoped>

</style>
