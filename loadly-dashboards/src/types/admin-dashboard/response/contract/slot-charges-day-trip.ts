export interface SlotCalendarDayTrip {
  hasOvernight: boolean
  overnightAmount: number
  roleName: string
  tripId: string
}

export interface ApiSlotCalendarDayTrip {
  hasOvernight?: boolean
  overnightAmount?: number
  roleName?: string
  tripId: string
}

export function calendarDayTripsAdapter(trips: ApiSlotCalendarDayTrip[] | undefined): SlotCalendarDayTrip[] {
  return (trips ?? []).map(trip => ({
    hasOvernight: Boolean(trip.hasOvernight),
    overnightAmount: trip.overnightAmount ?? 0,
    roleName: trip.roleName ?? '',
    tripId: trip.tripId,
  }))
}
