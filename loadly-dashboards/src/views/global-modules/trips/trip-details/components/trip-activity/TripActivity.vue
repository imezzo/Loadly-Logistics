<template>
  <ol
    v-if="trip.recordings?.length"
    class="flex flex-col gap-4"
  >
    <TripActivityItem
      v-for="recording in trip.recordings"
      :key="recording.id"
      :recording
      :admin-name="resolveUserName(recording)"
    />
  </ol>

  <div
    v-else
    class="
      rounded-xl border border-gray-50 bg-white px-4 py-6 text-center text-sm text-content-secondary
    "
  >
    {{ $t('content.noData') }}
  </div>
</template>

<script setup lang="ts">
  import TripActivityItem from './TripActivityItem.vue'
  import { useLookups } from '@/composables/useLookups'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import type { TripWizardRecording } from '@/types/admin-dashboard/response/trip/trip'
  import { RecordingModelType } from '@/types/admin-dashboard/response/trip/trip'

  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)
  const { admins, getAdmins } = useLookups()

  const adminNameById = computed(() => {
    const map = new Map<number, string>()

    for (const item of admins.value?.items ?? []) {
      map.set(Number(item.value), item.label)
    }

    return map
  })

  function isModelType(recording: TripWizardRecording, type: RecordingModelType): boolean {
    return String(recording.addedByModelType).toLowerCase() === type
  }

  function resolveUserName(recording: TripWizardRecording): string | undefined {
    if (isModelType(recording, RecordingModelType.Driver)) {
      return trip.value.driverName
    }

    if (isModelType(recording, RecordingModelType.Admin)) {
      return adminNameById.value.get(recording.addedBy)
    }

    return undefined
  }

  onMounted(async () => {
    const adminIds = [ ...new Set(
      (trip.value.recordings ?? [])
        .filter(recording => isModelType(recording, RecordingModelType.Admin))
        .map(recording => recording.addedBy)
        .filter(Boolean),
    ) ]

    if (!adminIds.length) return

    await getAdmins(1, undefined, adminIds)
  })
</script>

<style scoped>

</style>
