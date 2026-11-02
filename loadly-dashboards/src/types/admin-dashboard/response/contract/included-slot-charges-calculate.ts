import type { VehicleFacilityType } from '@/types/admin-dashboard/response/vehicle-facility.enums'
import type { MonthlyContractKmPool } from './monthly-contract/monthly-contract'
import { calendarDayTripsAdapter, type ApiSlotCalendarDayTrip, type SlotCalendarDayTrip } from './slot-charges-day-trip'

export interface SlotCalendarDay {
  amount?: number | null
  calendarDayId?: number
  date: string
  disposition: number
  dispositionName: string
  dispositionSource?: number
  facilityId: number | null
  facilityName: string | null
  facilityType: VehicleFacilityType | null
  isExtra: boolean
  overrideReason?: string | null
  tripCount: number
  tripIds?: string[]
  trips?: SlotCalendarDayTrip[]
  workingDayIndex: number | null
}

export interface SlotCalendar {
  days: SlotCalendarDay[]
  slotId: number
}

export interface SlotChargeLine {
  amount: number
  chargeKind: number
  chargeKindName: string
  contractSlotId: number
  date: string
}

export interface SlotKm {
  consumption: number
  overageKm: number
  overConsumptionRate: number
  slotId: number
  suggestedOverageAmount: number
}

export interface SlotChargeVehicle {
  plateLetters: string
  plateNo: string
}

export interface CalculateSlotChargesResponse {
  calendar: SlotCalendar[]
  kmPool?: MonthlyContractKmPool | null
  kms: SlotKm[]
  lines: SlotChargeLine[]
  totalAmount: number
  totalDaysWithTripsCount: number
  totalExtraKmsAmount: number
  totalExtraKmsCount: number
  totalExtraTripsAmount: number
  totalExtraTripsCount: number
  totalExtraWorkingDaysAmount: number
  totalExtraWorkingDaysCount: number
  totalFuelAmount: number
  totalFuelKms: number
  totalOvernightCustomerAmount: number
  totalOvernightLoadlyAmount: number
  totalOvernightTripsCount: number
  totalTripsCount: number
  totalWaitingForLoadingAmount: number
  totalWaitingForLoadingDaysCount: number
  totalWaitingForLoadingExtraDaysCount: number
  totalWaitingForLoadingWorkingDaysCount: number
  totalWorkingDaysAmount: number
  totalWorkingDaysCount: number
  vehicles: Record<string, SlotChargeVehicle>
}

interface ApiSlotCalendarDay {
  date: string
  disposition: number
  dispositionName: string
  facilityId: number | null
  facilityName: string | null
  facilityType: number | null
  isExtra: boolean
  tripCount: number
  tripIds?: string[]
  trips?: ApiSlotCalendarDayTrip[]
  workingDayIndex: number | null
}

interface ApiSlotCalendar {
  days: ApiSlotCalendarDay[]
  slotId: number
}

interface ApiSlotChargeVehicle {
  plateLetters?: string | null
  plateNo?: string | null
}

interface ApiCalculateSlotChargesResponse {
  calendar: ApiSlotCalendar[]
  kmPool?: MonthlyContractKmPool | null
  kms: SlotKm[]
  lines: SlotChargeLine[]
  totalAmount: number
  totalDaysWithTripsCount: number
  totalExtraKmsAmount: number
  totalExtraKmsCount: number
  totalExtraTripsAmount: number
  totalExtraTripsCount: number
  totalExtraWorkingDaysAmount: number
  totalExtraWorkingDaysCount: number
  totalFuelAmount: number
  totalFuelKms: number
  totalOvernightCustomerAmount: number
  totalOvernightLoadlyAmount: number
  totalOvernightTripsCount: number
  totalTripsCount: number
  totalWaitingForLoadingAmount: number
  totalWaitingForLoadingDaysCount: number
  totalWaitingForLoadingExtraDaysCount: number
  totalWaitingForLoadingWorkingDaysCount: number
  totalWorkingDaysAmount: number
  totalWorkingDaysCount: number
  vehicles?: Record<string, ApiSlotChargeVehicle> | null
}

function calendarDayAdapter(day: ApiSlotCalendarDay): SlotCalendarDay {
  const trips = calendarDayTripsAdapter(day.trips)

  return {
    date: day.date,
    disposition: day.disposition,
    dispositionName: day.dispositionName,
    facilityId: day.facilityId,
    facilityName: day.facilityName,
    facilityType: day.facilityType,
    isExtra: day.isExtra,
    tripCount: day.tripCount,
    tripIds: day.tripIds?.length ? day.tripIds : trips.map(trip => trip.tripId),
    trips,
    workingDayIndex: day.workingDayIndex,
  }
}

function vehiclesAdapter(
  vehicles: Record<string, ApiSlotChargeVehicle> | null | undefined,
): Record<string, SlotChargeVehicle> {
  if (!vehicles) return {}

  return Object.fromEntries(
    Object.entries(vehicles).map(([slotId, vehicle]) => [
      slotId,
      {
        plateLetters: vehicle.plateLetters ?? '',
        plateNo: vehicle.plateNo ?? '',
      },
    ]),
  )
}

export function calculateSlotChargesResponseAdapter(
  data: ApiCalculateSlotChargesResponse,
): CalculateSlotChargesResponse {
  return {
    calendar: data.calendar.map(slot => ({
      days: slot.days.map(calendarDayAdapter),
      slotId: slot.slotId,
    })),
    kmPool: data.kmPool,
    kms: data.kms,
    lines: data.lines,
    totalAmount: data.totalAmount,
    totalDaysWithTripsCount: data.totalDaysWithTripsCount,
    totalExtraKmsAmount: data.totalExtraKmsAmount,
    totalExtraKmsCount: data.totalExtraKmsCount,
    totalExtraTripsAmount: data.totalExtraTripsAmount,
    totalExtraTripsCount: data.totalExtraTripsCount,
    totalExtraWorkingDaysAmount: data.totalExtraWorkingDaysAmount,
    totalExtraWorkingDaysCount: data.totalExtraWorkingDaysCount,
    totalFuelAmount: data.totalFuelAmount,
    totalFuelKms: data.totalFuelKms,
    totalOvernightCustomerAmount: data.totalOvernightCustomerAmount,
    totalOvernightLoadlyAmount: data.totalOvernightLoadlyAmount,
    totalOvernightTripsCount: data.totalOvernightTripsCount,
    totalTripsCount: data.totalTripsCount,
    totalWaitingForLoadingAmount: data.totalWaitingForLoadingAmount,
    totalWaitingForLoadingDaysCount: data.totalWaitingForLoadingDaysCount,
    totalWaitingForLoadingExtraDaysCount: data.totalWaitingForLoadingExtraDaysCount,
    totalWaitingForLoadingWorkingDaysCount: data.totalWaitingForLoadingWorkingDaysCount,
    totalWorkingDaysAmount: data.totalWorkingDaysAmount,
    totalWorkingDaysCount: data.totalWorkingDaysCount,
    vehicles: vehiclesAdapter(data.vehicles),
  }
}
