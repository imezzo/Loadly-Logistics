import { useLookups } from '@/composables/useLookups'
import { useTripStore } from '@/stores/admin-dashboard/trip'
import type { Trip, TripWizardRecording } from '@/types/admin-dashboard/response/trip/trip'
import { RecordingModelType } from '@/types/admin-dashboard/response/trip/trip'
import { TripLifecycleActor } from '@/types/admin-dashboard/response/trip/trip-lifecycle-actor.enums'

export function useTripListLifecycleActor() {
  const tripStore = useTripStore()
  const { tripsList } = storeToRefs(tripStore)
  const { admins, getAdmins } = useLookups()

  const adminNameById = computed(() => {
    const map = new Map<number, string>()

    for (const item of admins.value?.items ?? []) {
      map.set(Number(item.value), item.label)
    }

    return map
  })

  function isAdminRecording(recording: TripWizardRecording): boolean {
    return String(recording.addedByModelType).toLowerCase() === RecordingModelType.Admin
  }

  function getAdminIdsFromRecordings(recordings: TripWizardRecording[] | undefined): number[] {
    return [ ...new Set(
      (recordings ?? [])
        .filter(isAdminRecording)
        .map(recording => recording.addedBy)
        .filter(Boolean),
    ) ]
  }

  function getAssistedAdminNames(trip: Trip): string[] {
    return getAdminIdsFromRecordings(trip.recordings)
      .map(id => adminNameById.value.get(id))
      .filter((name): name is string => Boolean(name))
  }

  async function fetchAssistedAdmins(): Promise<void> {
    const adminIds = [ ...new Set(
      (tripsList.value?.data ?? [])
        .filter(trip => trip.lifecycleActor !== TripLifecycleActor.Driver)
        .flatMap(trip => getAdminIdsFromRecordings(trip.recordings)),
    ) ]

    if (!adminIds.length) return

    await getAdmins(1, undefined, adminIds)
  }

  return {
    getAssistedAdminNames,
    fetchAssistedAdmins,
  }
}
