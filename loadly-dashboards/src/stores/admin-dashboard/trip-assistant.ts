import { api } from '@/utils/api'

export const useTripAssistantStore = defineStore('tripAssistant', () => {
  async function completeTripManagement(tripId: number, data?: FormData) {
    return api({
      method: 'POST',
      url: `/trips-management/${tripId}/complete`,
      data: data ?? new FormData(),
    })
  }

  async function startTripManagement(tripId: number) {
    return api({
      method: 'POST',
      url: `/trips-management/${tripId}/start`,
      data: {
        note: null // ?Required by backend
      },
    })
  }

  async function arriveTripWaypoint(tripId: number, waypointId: number) {
    return api({
      method: 'POST',
      url: `/trips-management/${tripId}/waypoints/${waypointId}/arrive`,
    })
  }

  async function arriveAndStartWaypointLoading(tripId: number, waypointId: number) {
    return api({
      method: 'POST',
      url: `/trips-management/${tripId}/waypoints/${waypointId}/loading/arrive-and-start`,
    })
  }

  async function completeAndDepartWaypointLoading(tripId: number, waypointId: number) {
    return api({
      method: 'POST',
      url: `/trips-management/${tripId}/waypoints/${waypointId}/loading/complete-and-depart`,
      data: new FormData(),
    })
  }

  async function startWaypointLoading(tripId: number, waypointId: number) {
    return api({
      method: 'POST',
      url: `/trips-management/${tripId}/waypoints/${waypointId}/loading/start`,
    })
  }

  async function startWaypointUnloading(tripId: number, waypointId: number) {
    return api({
      method: 'POST',
      url: `/trips-management/${tripId}/waypoints/${waypointId}/unloading/start`,
    })
  }

  async function arriveAndStartWaypointUnloading(tripId: number, waypointId: number) {
    return api({
      method: 'POST',
      url: `/trips-management/${tripId}/waypoints/${waypointId}/unloading/arrive-and-start`,
      data: new FormData(),
    })
  }

  async function completeAndDepartWaypointUnloading(tripId: number, waypointId: number) {
    return api({
      method: 'POST',
      url: `/trips-management/${tripId}/waypoints/${waypointId}/unloading/complete-and-depart`,
      data: new FormData(),
    })
  }

  async function completeWaypointLoading(tripId: number, waypointId: number, data: FormData) {
    return api({
      method: 'POST',
      url: `/trips-management/${tripId}/waypoints/${waypointId}/loading/complete`,
      data,
    })
  }

  async function completeWaypointUnloading(tripId: number, waypointId: number, data: FormData) {
    return api({
      method: 'POST',
      url: `/trips-management/${tripId}/waypoints/${waypointId}/unloading/complete`,
      data,
    })
  }

  async function departTripWaypoint(tripId: number, waypointId: number, data: FormData) {
    return api({
      method: 'POST',
      url: `/trips-management/${tripId}/waypoints/${waypointId}/depart`,
      data,
    })
  }

  async function enterTripCustoms(tripId: number, data: FormData) {
    return api({
      method: 'POST',
      url: `/trips-management/${tripId}/customs/enter`,
      data,
    })
  }

  async function exitTripCustoms(tripId: number, data: FormData) {
    return api({
      method: 'POST',
      url: `/trips-management/${tripId}/customs/exit`,
      data,
    })
  }

  return {
    arriveAndStartWaypointLoading,
    arriveAndStartWaypointUnloading,
    arriveTripWaypoint,
    completeAndDepartWaypointLoading,
    completeAndDepartWaypointUnloading,
    completeTripManagement,
    completeWaypointLoading,
    completeWaypointUnloading,
    departTripWaypoint,
    enterTripCustoms,
    exitTripCustoms,
    startTripManagement,
    startWaypointLoading,
    startWaypointUnloading,
  }
})
