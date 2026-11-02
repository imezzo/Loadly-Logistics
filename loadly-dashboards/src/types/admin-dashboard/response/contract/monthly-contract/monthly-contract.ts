import type { SlotChargeLine, SlotKm } from '@/types/admin-dashboard/response/contract/included-slot-charges-calculate'
import { calendarDayTripsAdapter, type ApiSlotCalendarDayTrip, type SlotCalendarDayTrip } from '@/types/admin-dashboard/response/contract/slot-charges-day-trip'
import type { VehicleFacilityType } from '@/types/admin-dashboard/response/vehicle-facility.enums'
import type { Pagination } from '@/types/response/pagination'
import { getLocalizedValue } from '@/utils/i18n'
import { MonthlyContractStatus } from './monthly-contract.enums'
import type { ConsumptionType } from '@/types/request/contract/consumption-types.enums'

export interface MonthlyContractVehicleType {
  category: number
  id: number
  name: string
  vehicleUnitType: number
}

export interface MonthlyContractVehicle {
  brandName: string
  chassisNumber: string | null
  driver: unknown
  id: number
  modelName: string
  plateLetters: string
  plateNo: string
  sector: unknown
  sectorId: number | null
  tailVehicles: unknown[]
  vehicleType: MonthlyContractVehicleType
  vehicleTypeId: number
  year: number
}

export interface MonthlyContractCalendarDay {
  calendarDayId: number
  date: string
  disposition: number
  dispositionName: string
  dispositionSource: number
  facilityId: number | null
  facilityName: string | null
  facilityType: VehicleFacilityType | null
  isExtra: boolean
  overrideReason: string | null
  tripCount: number
  tripIds: string[]
  trips?: SlotCalendarDayTrip[]
  workingDayIndex: number | null
}

export interface MonthlyContractCalendar {
  billingSnapshotSeatRootSlotId: number
  days: MonthlyContractCalendarDay[]
  slotId: number
}

export interface MonthlyContractKmPool {
  overConsumptionRate: number
  overageKm: number
  suggestedOverageAmount: number
  totalConsumption: number
  totalMaxKms: number
}

export interface MonthlyContractCalculation {
  calendar: MonthlyContractCalendar[]
  commercialTermOverrides: unknown[]
  kmPool: MonthlyContractKmPool
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
  vehicles: Record<string, MonthlyContractVehicle>
}

export interface MonthlyContractInvoicing {
  claimsInvoiceId: number | null
  extraKmOverageInvoiceType: number | null
  extraKmOverageInvoiceTypeName: string | null
  requiresClaimsInvoice: boolean
  requiresTripsInvoice: boolean
  tripsInvoiceId: number | null
}

export interface MonthlyContract {
  calculation: MonthlyContractCalculation
  companyId: number
  companyName: string
  consumptionType: ConsumptionType
  contractId: number
  contractName: string
  engineVersion: number
  finalizedAt: string | null
  invoiceId: number | null
  invoicing: MonthlyContractInvoicing
  name: string
  periodEnd: string
  periodStart: string
  snapshotId: number
  status: MonthlyContractStatus
  statusName: string
}

export interface MonthlyContractsListResponse {
  data: MonthlyContract[]
  pagination: Pagination
}

interface ApiMonthlyContractVehicleType {
  category: number
  id: number
  name: string
  nameAr: string
  nameEn: string
  vehicleUnitType: number
}

interface ApiMonthlyContractVehicle {
  brandName: string
  chassisNumber: string | null
  driver: unknown
  id: number
  modelName: string
  plateLetters: string
  plateNo: string
  sector: unknown
  sectorId: number | null
  tailVehicles: unknown[]
  vehicleType: ApiMonthlyContractVehicleType
  vehicleTypeId: number
  year: number
}

interface ApiMonthlyContractCalendarDay {
  calendarDayId: number
  date: string
  disposition: number
  dispositionName: string
  dispositionSource: number
  facilityId: number | null
  facilityName: string | null
  facilityType: number | null
  isExtra: boolean
  overrideReason: string | null
  tripCount: number
  tripIds?: string[]
  trips?: ApiSlotCalendarDayTrip[]
  workingDayIndex: number | null
}

interface ApiMonthlyContractCalendar {
  billingSnapshotSeatRootSlotId?: number
  days: ApiMonthlyContractCalendarDay[]
  slotId: number
}

interface ApiMonthlyContractCalculation {
  calendar: ApiMonthlyContractCalendar[]
  commercialTermOverrides: unknown[]
  kmPool: MonthlyContractKmPool
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
  vehicles: Record<string, ApiMonthlyContractVehicle>
}

interface ApiMonthlyContract {
  calculation: ApiMonthlyContractCalculation
  companyId: number
  companyName: string
  consumptionType: ConsumptionType
  contractId: number
  contractName: string
  engineVersion: number
  finalizedAt: string | null
  invoiceId: number | null
  invoicing: MonthlyContractInvoicing
  name: string
  periodEnd: string
  periodStart: string
  snapshotId: number
  status: MonthlyContractStatus
  statusName: string
}

interface ApiMonthlyContractsListResponse {
  data: ApiMonthlyContract[]
  pagination: Pagination
}

function vehicleTypeAdapter(data: ApiMonthlyContractVehicleType): MonthlyContractVehicleType {
  return {
    category: data.category,
    id: data.id,
    name: getLocalizedValue(data.nameEn, data.nameAr),
    vehicleUnitType: data.vehicleUnitType,
  }
}

function vehicleAdapter(data: ApiMonthlyContractVehicle): MonthlyContractVehicle {
  return {
    brandName: data.brandName,
    chassisNumber: data.chassisNumber,
    driver: data.driver,
    id: data.id,
    modelName: data.modelName,
    plateLetters: data.plateLetters,
    plateNo: data.plateNo,
    sector: data.sector,
    sectorId: data.sectorId,
    tailVehicles: data.tailVehicles ?? [],
    vehicleType: vehicleTypeAdapter(data.vehicleType),
    vehicleTypeId: data.vehicleTypeId,
    year: data.year,
  }
}

function calendarDayAdapter(day: ApiMonthlyContractCalendarDay): MonthlyContractCalendarDay {
  const trips = calendarDayTripsAdapter(day.trips)

  return {
    calendarDayId: day.calendarDayId,
    date: day.date,
    disposition: day.disposition,
    dispositionName: day.dispositionName,
    dispositionSource: day.dispositionSource,
    facilityId: day.facilityId,
    facilityName: day.facilityName,
    facilityType: day.facilityType as VehicleFacilityType | null,
    isExtra: day.isExtra,
    overrideReason: day.overrideReason,
    tripCount: day.tripCount,
    tripIds: day.tripIds?.length ? day.tripIds : trips.map(trip => trip.tripId),
    trips,
    workingDayIndex: day.workingDayIndex,
  }
}

function calculationAdapter(data: ApiMonthlyContractCalculation): MonthlyContractCalculation {
  const vehicles = Object.fromEntries(
    Object.entries(data.vehicles ?? {}).map(([slotId, vehicle]) => [slotId, vehicleAdapter(vehicle)]),
  )

  return {
    calendar: data.calendar.map(slot => ({
      billingSnapshotSeatRootSlotId: slot.billingSnapshotSeatRootSlotId ?? slot.slotId,
      days: slot.days.map(calendarDayAdapter),
      slotId: slot.slotId,
    })),
    commercialTermOverrides: data.commercialTermOverrides,
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
    vehicles,
  }
}

function invoicingAdapter(data: MonthlyContractInvoicing): MonthlyContractInvoicing {
  return {
    claimsInvoiceId: data.claimsInvoiceId,
    extraKmOverageInvoiceType: data.extraKmOverageInvoiceType,
    extraKmOverageInvoiceTypeName: data.extraKmOverageInvoiceTypeName,
    requiresClaimsInvoice: data.requiresClaimsInvoice,
    requiresTripsInvoice: data.requiresTripsInvoice,
    tripsInvoiceId: data.tripsInvoiceId,
  }
}

export function monthlyContractAdapter(item: ApiMonthlyContract): MonthlyContract {
  return {
    calculation: calculationAdapter(item.calculation),
    companyId: item.companyId,
    companyName: item.companyName,
    consumptionType: item.consumptionType,
    contractId: item.contractId,
    contractName: item.contractName,
    engineVersion: item.engineVersion,
    finalizedAt: item.finalizedAt,
    invoiceId: item.invoiceId,
    invoicing: invoicingAdapter(item.invoicing),
    name: item.name,
    periodEnd: item.periodEnd,
    periodStart: item.periodStart,
    snapshotId: item.snapshotId,
    status: item.status,
    statusName: item.statusName,
  }
}

export function monthlyContractsListResponseAdapter(
  data: ApiMonthlyContractsListResponse,
): MonthlyContractsListResponse {
  return {
    data: (data.data ?? []).map(monthlyContractAdapter),
    pagination: data.pagination,
  }
}
