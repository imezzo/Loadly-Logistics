<template>
  <div
    v-if="sortedWaypoints.length"
    class="mt-6"
  >
    <CardPreview :title="$t('content.pickupPointAndDropoffPoint')">
      <div
        class="
          space-y-4 rounded-[0.625rem] border border-gray-50 bg-neutral-30 p-3
          sm:p-4
        "
      >
        <article
          v-for="(waypoint, index) in sortedWaypoints"
          :key="waypointKey(waypoint, index)"
          class="
            flex gap-3 rounded-lg border border-gray-50 bg-white p-4
            sm:gap-4 sm:p-5
          "
        >
          <div
            class="
              mt-1 size-3 shrink-0 rounded-full border-2 border-primary
              sm:size-3.5
            "
            aria-hidden="true"
          />

          <div class="min-w-0 flex-1">
            <p class="text-sm font-bold text-neutral-800">
              {{ stopTypeLabel(waypoint.stopType) }}
            </p>

            <div
              class="
                mt-4 flex flex-col gap-4
                sm:flex-row sm:items-start sm:gap-8
              "
            >
              <div
                class="
                  flex min-w-0 gap-2
                  sm:max-w-[55%]
                "
              >
                <IconLocation
                  class="mt-0.5 size-5 shrink-0 text-neutral-800"
                  :filled="false"
                />

                <div class="min-w-0">
                  <p class="text-sm font-bold text-neutral-800">
                    {{ waypoint.addressName?.trim() || '-' }}
                  </p>
                </div>
              </div>

              <div
                class="
                  flex min-w-0 gap-2
                  sm:max-w-[45%]
                "
              >
                <IconEditDocument class="mt-0.5 size-5 shrink-0 text-neutral-800" />

                <div class="min-w-0">
                  <p class="text-sm font-bold text-neutral-800">
                    {{ $t('content.noteLabel') }}
                  </p>

                  <p class="mt-1 text-sm text-content-secondary">
                    {{ noteText(waypoint.taskDetails) }}
                  </p>
                </div>
              </div>

              <BaseChip
                v-if="trip.destinationStatus"
                class="ms-auto shrink-0 self-start"
                :label="trip.destinationStatusName"
                :variant="contractDestinationStatusVariants[trip.destinationStatus]"
              />
            </div>
          </div>
        </article>
      </div>
    </CardPreview>
  </div>
</template>

<script setup lang="ts">
  import CardPreview from '@/components/card/CardPreview.vue'
  import IconEditDocument from '@/components/icon/IconEditDocument.vue'
  import IconLocation from '@/components/icon/IconLocation.vue'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import type { TripWizardWaypoint } from '@/types/admin-dashboard/request/trip/trip'
  import { waypointType } from '@/types/admin-dashboard/response/trip/trip'
  import { contractDestinationStatusVariants } from '@/utils/statusChipVariants'

  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)

  const { t } = useI18n()

  const sortedWaypoints = computed(() => {
    const list = trip.value.waypoints ?? []

    return [...list].sort((a, b) => (a.sequence ?? 0) - (b.sequence ?? 0))
  })

  function waypointKey(waypoint: TripWizardWaypoint, index: number): string {
    return `${waypoint.sequence ?? index}-${waypoint.stopType}-${index}`
  }

  function stopTypeLabel(stopType: string | undefined): string {
    if (stopType === waypointType.pickup) return t('content.pickupPoint')
    if (stopType === waypointType.dropoff) return t('content.dropoffPoint')

    return t('content.notAvailable')
  }

  function noteText(taskDetails: string | undefined): string {
    const trimmed = taskDetails?.trim()
    if (!trimmed) return t('form.noNote')

    return trimmed
  }
</script>

<style scoped>

</style>
