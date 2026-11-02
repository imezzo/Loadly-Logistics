import type {
  TripWizardDocument,
  TripWizardRecording,
  TripWizardTripCharge,
  TripCustomsStatus,
  TripContract,
} from "@/types/admin-dashboard/response/trip/trip"
import { DispatchingMode, TripType } from "@/types/admin-dashboard/response/trip/trip"
import type { TripBookingRequest } from "@/types/admin-dashboard/response/trip/trip-booking"
import { apiPaginationAdapter } from "@/types/request/pagination"
import type { ApiPagination, Pagination } from "@/types/request/pagination"
import { formatDateToISO } from "@/utils/date"
import { getValidProperty } from "@/utils/transformers"
import type { Sector } from "../../response/sector/sector"

export interface TripFilter {
  companyId?: string
  contractId?: string
  contractorIds?: string[]
  currentDriverStatus?: string
  destinationIds?: string[]
  dispatchingMode?: DispatchingMode | ''
  driverId?: string
  financialDateFrom?: string
  financialDateTo?: string
  leadIds?: string[]
  lifecycleActor?: string
  overNightCostBearer?: string
  search?: string
  sectorIds?: string[]
  status?: string
  tripDocumentStatuses?: string[]
  vehicleTypeId?: string
}

export interface TripWizardWaypoint {
  id?: number
  stopType: string
  googleMapLink?: string
  latitude?: string
  longitude?: string
  addressName?: string
  taskDetails?: string
  sequence?: number
  status?: number
  googlePlaceId?: string | null
}

export interface TripWizard {
  actualCargoWeightTons?: string | null
  actualDistanceKm?: number | null
  auditorName?: string | null
  cancellationReason?: string | null
  cargoPrice: string
  companyAddress?: string
  companyEmail?: string
  companyId?: string
  companyName?: string
  companyPhone?: string
  companyPostalCode?: string
  companyRegistrationNumber?: string
  contactPersonMobile: string
  contactPersonName: string
  contract?: TripContract
  contractConditions?: {
    documentId: number
    fileUrl: string
    name: string
  } | null
  contractDistanceKm: string
  contractId: string
  contractorId?: string
  contractorName?: string
  contractorPhone?: string
  contractorPrice: string
  createdAt?: string
  bookingRequests?: TripBookingRequest[]
  currentDriverStatus?: string
  currentLocation?: {
    latitude: number
    longitude: number
    recordedAt: string
  }
  customsStatus: TripCustomsStatus
  dailyValueCost?: number | null
  destinationId?: string
  destinationStatus?: number
  destinationStatusName?: string
  dispatcherNotes?: string | null
  documents?: TripWizardDocument[]
  driverId?: string
  driverName?: string
  driverPhone?: string
  dispatchingMode?: DispatchingMode
  endDate: string
  financialDate?: string
  hasCustoms?: boolean
  id?: string
  isEditable?: boolean
  isExternalTrip?: boolean
  isBackfill?: boolean
  isHistorical?: boolean
  isInvoiced?: boolean
  leadId?: string | null
  numberOfDays?: number
  overNightCostBearer?: number | null
  overNightCostBearerName?: string
  proofOfDelivery?: null | number
  recordings?: TripWizardRecording[]
  referenceNumber?: string
  returnDestinationId?: string
  sector?: Sector
  spotPrice?: string | null
  startDate: string
  status?: string
  submitRequest?: boolean
  type?: string
  tripCharges?: TripWizardTripCharge[]
  tripDocumentStatus?: number
  tripDocumentStatusName?: string
  vehicle?: {
    driverName?: string
    plateLetters: string
    plateNo: string
    tailVehicles?: {
      plateLetters: string
      plateNo: string
    }[]
  }
  vehicleId?: number | null
  vehicleType?: string
  vehicleTypeId?: string
  tailVehicleId?: number | null
  waypoints: TripWizardWaypoint[]
}

export interface TripListRequest extends Pagination, TripFilter {
  columns?: string[]
  contractTypeId?: string
  dispatchingMode?: DispatchingMode | ''
  hasContractorInvoice?: boolean
  isCreatedByCompany?: boolean
  isExternalTrip?: boolean
  isPushedToContractors?: boolean
}

export interface ApiTripListRequest extends ApiPagination, TripFilter {
  columns?: string[]
  contractTypeId?: string
  dispatchingMode?: DispatchingMode
  hasContractorInvoice?: boolean
  isCreatedByCompany?: boolean
  isExternalTrip?: boolean
  isPushedToContractors?: boolean
}

export interface ApiTripChargeUpsertPayload {
  amount: number
  chargeType: number
  companyShare?: number | null
  extraFeeTypeId: number | null
  id?: number
  notes?: string | null
}

export interface ApiTripWizardWaypointPayload {
  id: number | null
  sequence: number
  stopType: number | null
  latitude?: number
  longitude?: number
  addressName: string
  googlePlaceId: string | null
  taskDetails: string | null
}

export interface ApiTripWizardPayload {
  actualCargoWeightTons?: number | null
  cargoPrice?: number | null
  contractorId?: number
  contractorPrice?: number | null
  contactPersonMobile?: string
  contactPersonName?: string
  contractDistanceKm?: number
  contractId?: number
  destinationId?: number
  dispatcherNotes?: string | null
  dispatchingMode?: DispatchingMode
  driverId?: number
  endDate?: string
  financialDate?: string
  hasCustoms?: boolean
  isExternalTrip?: boolean
  leadId?: number | null
  PushToAllContractors?: boolean
  returnDestinationId?: number | null
  reviewCompleted?: boolean
  spotPrice?: number | null
  startDate?: string
  tripCharges?: ApiTripChargeUpsertPayload[]
  tripDocumentStatus?: number
  type?: number
  vehicleId?: number
  vehicleTypeId?: number
  tailVehicleId?: number
  waypoints: ApiTripWizardWaypointPayload[]
}

function tripWaypointToApiPayload(
  w: TripWizardWaypoint,
  index: number,
): ApiTripWizardWaypointPayload {
  const latitude = w.latitude != null && w.latitude !== '' ? Number.parseFloat(w.latitude) : Number.NaN
  const longitude = w.longitude != null && w.longitude !== '' ? Number.parseFloat(w.longitude) : Number.NaN

  const task = w.taskDetails?.trim() ?? ''

  const stopTypeNum = Number(w.stopType)

  return {
    id: w.id ?? 0,
    sequence: index,
    stopType: Number.isFinite(stopTypeNum) ? stopTypeNum : null,
    ...(latitude ? { latitude } : {}),
    ...(longitude ? { longitude } : {}),
    addressName: w.addressName ?? '',
    googlePlaceId: w.googlePlaceId ?? null,
    taskDetails: task.length ? task : null,
  }
}

export function tripChargesToApiPayload(
  charges: TripWizardTripCharge[] | undefined,
): ApiTripChargeUpsertPayload[] {
  if (!charges?.length) return []

  return charges.map((line) => {
    const payload: ApiTripChargeUpsertPayload = {
      amount: Number(line.amount),
      chargeType: line.chargeType,
      companyShare: Number(line.companyShare),
      extraFeeTypeId: Number(line.extraFeeTypeId) > 0 ? line.extraFeeTypeId : null,
      notes: line.notes,
    }
    if (line.id > 0) payload.id = line.id

    return payload
  })
}

function resolveWizardVehicleId(
  trip: TripWizard,
  vehicleId?: number | null,
): number | undefined {
  if (vehicleId != null && vehicleId > 0) return vehicleId
  if (trip.vehicleId != null && trip.vehicleId > 0) return trip.vehicleId

  return undefined
}

function parseOptionalPositiveInt(value: string | undefined): number | undefined {
  if (value == null || value === '') return undefined
  const n = Number(value)
  if (!Number.isFinite(n) || n <= 0) return undefined

  return n
}

function tripWizardVehicle(trip: TripWizard, vehicleId?: number | null) {
  const resolvedVehicleId = resolveWizardVehicleId(trip, vehicleId)
  const vehicleTypeId = parseOptionalPositiveInt(trip.vehicleTypeId)
  const driverId = parseOptionalPositiveInt(trip.driverId)

  return {
    ...(resolvedVehicleId != null ? { vehicleId: resolvedVehicleId } : {}),
    ...(trip.tailVehicleId ? { tailVehicleId: trip.tailVehicleId } : {}),
    ...(vehicleTypeId != null && !trip.isHistorical ? { vehicleTypeId } : {}),
    ...(driverId != null ? { driverId } : {}),
  }
}

export function tripWizardRequestAdapter(
  trip: TripWizard,
  vehicleId?: number | null,
  reviewCompleted?: boolean
): ApiTripWizardPayload {
  const contractKm = Number.parseFloat(trip.contractDistanceKm)
  const contractId = Number.parseInt(trip.contractId, 10)
  const destinationId = parseOptionalPositiveInt(trip.destinationId)
  const returnDestinationId = destinationId != null && Number(trip.type) === TripType.OpenJaw
    ? parseOptionalPositiveInt(trip.returnDestinationId)
    : undefined
  const pushToAllContractors = trip.dispatchingMode === DispatchingMode.Contractors
    && !trip.contractorId

  return {
    contactPersonMobile: trip.contactPersonMobile,
    contactPersonName: trip.contactPersonName,
    ...(Number.isFinite(contractKm) ? { contractDistanceKm: contractKm } : {}),
    ...(Number.isFinite(contractId) ? { contractId: contractId } : {}),
    ...(trip.endDate ? { endDate: formatDateToISO(trip.endDate, true) } : {}),
    ...getValidProperty('financialDate', trip.financialDate),
    hasCustoms: Boolean(trip.hasCustoms),
    startDate: formatDateToISO(trip.startDate, true),
    waypoints: trip.waypoints.map(tripWaypointToApiPayload),
    isExternalTrip: trip.isExternalTrip,
    ...(destinationId != null ? { destinationId } : {}),
    ...getValidProperty('returnDestinationId', returnDestinationId),
    ...(trip.contractorId ? { contractorId: Number(trip.contractorId) } : {}),
    ...(trip.contractorPrice
      ? { contractorPrice: Number(trip.contractorPrice) }
      : {}),
    ...(trip.dispatchingMode != null ? { dispatchingMode: trip.dispatchingMode } : {}),
    ...(pushToAllContractors ? { PushToAllContractors: true } : {}),
    ...(reviewCompleted ? { reviewCompleted: true } : {}),
    ...(trip?.submitRequest !== undefined ? { submitRequest: trip.submitRequest } : {}),
    ...getValidProperty('actualCargoWeightTons', trip.actualCargoWeightTons),
    ...getValidProperty('cargoPrice', trip.cargoPrice),
    ...tripWizardVehicle(trip, vehicleId),
    ...getValidProperty('spotPrice', trip.spotPrice),
    ...getValidProperty('type', trip.type),
    ...getValidProperty('leadId', trip.leadId),
    ...(trip.dispatcherNotes != null
      ? { dispatcherNotes: trip.dispatcherNotes.trim() || null }
      : {}),
  }
}

export function tripListRequestAdapter(data: TripListRequest): ApiTripListRequest {
  const contractorIds: Record<string, string> = {}
  data.contractorIds?.forEach((id, index) => {
    contractorIds[`contractorIds[${index}]`] = id
  })
  const destinationIds: Record<string, string> = {}
  data.destinationIds?.forEach((id, index) => {
    destinationIds[`destinationIds[${index}]`] = id
  })
  const sectorIds: Record<string, string> = {}
  data.sectorIds?.forEach((id, index) => {
    sectorIds[`sectorIds[${index}]`] = id
  })
  const leadIds: Record<string, string> = {}
  data.leadIds?.forEach((id, index) => {
    leadIds[`leadIds[${index}]`] = id
  })
  const tripDocumentStatuses: Record<string, string> = {}
  data.tripDocumentStatuses?.forEach((status, index) => {
    tripDocumentStatuses[`tripDocumentStatuses[${index}]`] = status
  })

  const columns: Record<string, string> = {}
  data.columns?.forEach((column, index) => {
    columns[`columns[${index}]`] = column
  })

  return {
    ...apiPaginationAdapter({ page: data.page, size: data.size }),
    ...getValidProperty('companyId', data.companyId),
    ...getValidProperty('contractId', data.contractId),
    ...(data.contractorIds ? contractorIds : {}),
    ...getValidProperty('currentDriverStatus', data.currentDriverStatus),
    ...(data.destinationIds?.length ? destinationIds : {}),
    ...getValidProperty('dispatchingMode', data.dispatchingMode),
    ...getValidProperty('driverId', data.driverId),
    ...getValidProperty('isCreatedByCompany', data.isCreatedByCompany),
    ...getValidProperty('contractTypeId', data.contractTypeId),
    isExternalTrip: data.isExternalTrip,
    ...(data.isPushedToContractors != null
      ? { isPushedToContractors: data.isPushedToContractors }
      : {}),
    ...(data.leadIds?.length ? leadIds : {}),
    ...getValidProperty('lifecycleActor', data.lifecycleActor),
    ...(data.tripDocumentStatuses?.length ? tripDocumentStatuses : {}),
    ...getValidProperty('search', data.search),
    ...(data.sectorIds?.length ? sectorIds : {}),
    ...getValidProperty('overNightCostBearer', data.overNightCostBearer),
    ...(data.financialDateFrom ? { financialDateFrom: data.financialDateFrom } : {}),
    ...(data.financialDateTo ? { financialDateTo: data.financialDateTo } : {}),
    ...getValidProperty('status', data.status),
    ...(data.columns?.length ? columns : {}),
    ...getValidProperty('vehicleTypeId', data.vehicleTypeId),
    ...(data.hasContractorInvoice !== undefined
      ? { hasContractorInvoice: data.hasContractorInvoice }
      : {}),
    ...getValidProperty('dispatchingMode', data.dispatchingMode),
  }
}

export interface TripsListRequest extends Pagination {
  companyId: string
  carType: string // TODO: change to vehicle type after backend implementation
  status: string
}

export interface ApiTripsListRequest extends Pagination {
  companyId: string
  status: string
}

export function tripsListRequestAdapter(data: TripsListRequest): ApiTripsListRequest {
  return {
    ...apiPaginationAdapter({
      page: data.page,
      size: data.size,
    }),
    companyId: data.companyId,
    status: data.status,
  }
}
