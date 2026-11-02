import type { ExportKey } from "@/types/export-keys.types"
import type { Pagination } from "@/types/response/pagination"
import { formatDateLocal, toDateTimeLocal } from "@/utils/date"
import { getLocalizedValue } from "@/utils/i18n"
import { sectorAdapter, type ApiSector, type Sector } from "../sector/sector"
import type { TripDocumentStatus } from "./trip-document-status.enums"
import type { TripLifecycleActor } from "./trip-lifecycle-actor.enums"
import { tripBookingRequestAdapter } from "./trip-booking"
import type { ApiTripBookingRequest, TripBookingRequest } from "./trip-booking"
import { tripContractDestinationsAdapter } from "./trip-contract-destination"
import type { ApiTripContractDestination, TripContractDestination } from "./trip-contract-destination"

export interface CurrentLocation {
  latitude: number
  longitude: number
  recordedAt: string
}

export enum DispatchingMode {
  Contractors = 1,
  Loadly,
}

export enum TripStatus {
  REQUESTED,
  PENDING,
  SCHEDULED,
  DRIVER_ACCEPTED,
  FIRST_PICKUP_LOADING,
  IN_PROGRESS,
  COMPLETED,
  REVIEWED,
  CANCELLED,
  DRAFT = -1,
}

export enum OperationalDriverStatus {
  TRIP_STARTED = 4,
  TRIP_COMPLETED = 7,
  TRIP_CANCELLED = 10,
}

export enum TripCustomsStatus {
  NONE,
  PENDING_ENTER,
  UNDER_INSPECTION,
  CLEARED,
}

export enum fuelPricingCalculationOption {
  IncludedInContractPrice = 1,
  CalculatedPerTrip,
}

export enum RecordingModelType {
  Admin = 'admin',
  Driver = 'driver',
}

export enum TripType {
  OneWay = 1,
  RoundTrip,
  OpenJaw,
}

export const waypointType = {
  pickup: '0',
  dropoff: '1',
  depot: '2',
  complete: '-1',
  customs: '-2',
} as const

export const tripWaypointStatus = {
  Pending: 0,
  Arrived: 1,
  LoadingStarted: 2,
  LoadingCompleted: 3,
  Departed: 4,
  Completed: 5,
  Failed: 6,
  UnloadingStarted: 7,
  UnloadingCompleted: 8,
} as const

export interface TripContractConditions {
  documentId: number
  fileUrl: string
  name: string
}

export interface TripWizardWaypoint {
  id?: number
  addressName?: string
  googleMapLink?: string
  googlePlaceId?: string | null
  latitude?: string
  longitude?: string
  sequence?: number
  status?: number
  stopType: string
  taskDetails?: string
}

export interface TripContract {
  consumptionType: number
  contractTypeName: string
  id: number
  name: string
  referenceNumber: string
}

export interface TripWizard {
  actualCargoWeightTons?: number | null
  actualDistanceKm?: number | null
  auditorName?: string | null
  bookingRequests?: TripBookingRequest[]
  cancellationReason?: string | null
  cargoPrice?: number | null
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
  contractConditions?: TripContractConditions | null
  contractDistanceKm: string | null
  contractId: string
  contractorId?: string
  contractorName?: string
  contractorPhone?: string
  contractorPrice?: string
  createdAt?: string
  currentDriverStatus?: string
  currentLocation: CurrentLocation | null
  customsStatus: TripCustomsStatus
  dailyValueCost?: number | null
  destinationId?: string
  destinationStatus?: number
  destinationStatusName?: string
  dispatcherNotes?: string | null
  dispatchingMode?: DispatchingMode
  documents?: TripWizardDocument[]
  driverId: string
  driverName?: string
  driverPhone?: string
  endDate: string
  financialDate?: string
  hasCustoms?: boolean
  id?: string
  isBackfill?: boolean
  isEditable?: boolean
  isExternalTrip?: boolean
  isInvoiced?: boolean
  leadId?: number | null
  numberOfDays?: number
  overNightCostBearer?: number | null
  overNightCostBearerName?: string
  proofOfDelivery?: null | number
  recordings?: TripWizardRecording[]
  referenceNumber?: string
  returnDestinationId?: string
  sector?: Sector
  spotPrice?: number | null
  startDate: string
  status?: TripStatus
  tripCharges?: TripWizardTripCharge[]
  tripContractDestinations?: TripContractDestination[]
  tripDocumentStatus?: TripDocumentStatus
  tripDocumentStatusName?: string
  type?: string
  vehicle?: {
    driverName?: string
    plateLetters: string
    plateNo: string
    tailVehicles?: {
      plateLetters: string
      plateNo: string
    }[]
  }
  vehicleId?: number
  vehicleType?: string
  vehicleTypeId?: string
  waypoints: TripWizardWaypoint[]
}

export interface Trip {
  actualCargoWeightTons?: number | null
  actualDistanceKm: number | null
  auditedAt: string | null
  bookingRequests?: TripBookingRequest[]
  claimInvoiceNumber?: string
  claimInvoiceTypeName?: string
  companyId: string
  companyName: string
  consumptionRate?: number | null
  contactPersonMobile?: string
  contactPersonName?: string
  contract?: TripContract
  contractDistanceKm?: string | null
  contractId?: string
  contractRef: string
  contractorId?: number | null
  contractorName?: string | null
  contractorPrice: number | null
  currentDriverStatus: number
  destinationId?: string
  destinationName?: string | null
  destinationPrice?: number | null
  dispatchingMode?: DispatchingMode | null
  driverName?: string
  financialDate: string
  fuelPricingCalculationOption: fuelPricingCalculationOption | null
  id: string
  invoiceNumber?: string
  invoiceTypeName?: string
  invoicingStatus?: number | null
  invoicingStatusName?: string | null
  isBackfill: boolean
  isEditable: boolean
  isExternalTrip?: boolean
  lifecycleActor: TripLifecycleActor | null
  lifecycleActorName: string | null
  overNightCostBearer?: number | null
  overNightCostBearerName?: string
  plateLetters?: string
  plateNumber?: string
  tailPlateLetters?: string
  tailPlateNumber?: string
  proofOfDelivery?: null | number | string
  recordings?: TripWizardRecording[]
  referenceNumber?: string
  requiresClaimInvoice: boolean
  requiresMainInvoice: boolean
  sector?: Sector | null
  spotPrice?: number | null
  startDate: string
  status: TripStatus
  loadlyCommission?: number | null
  totalAddonsCost?: number | null
  totalBillableAmount: null | number
  totalBillableFuelAmount?: number | null
  totalBillableTollGatesAmount?: number | null
  totalFuelAmount?: number | null
  totalTripCost?: number | null
  tripCharges?: TripWizardTripCharge[]
  tripContractDestinations?: TripContractDestination[]
  tripCostWithoutAddon: number | null
  tripDocumentStatus?: TripDocumentStatus
  tripDocumentStatusName?: string
  vehicleSnapshot?: ApiVehicleSnapshot | null
  vehicleType?: string
  waypoints?: TripWizardWaypoint[]
}

export interface TripList {
  additional?: {
    exportKeys: ExportKey[]
  } | null
  data: Trip[]
  pagination: Pagination
}

export interface TripTrackItem {
  waypoints: TripWizardWaypoint[]
  companyName: string
  currentLocation: CurrentLocation
  driverMobile: string
  driverName: string
  id: number
  vehicleColor: string
  vehiclePlateNumber: string
  vehicleType: string
}

export interface TripTrackResponse {
  data: TripTrackItem[]
}

export interface ApiTripDriver {
  email: string
  fullName: string
  fullNameAr: string
  id: number
  phone: string
}

export interface ApiTripContract {
  consumptionType: number
  contractTypeName: string
  id: number
  nameAr: string
  nameEn: string
  referenceNumber: string
}

export interface ApiVehicleSnapshot {
  headPlateChangedAt: string | null
  headPlateLetters?: string | null
  headPlateNo?: string | null
  tailPlateChangedAt: string | null
  tailPlateLetters?: string | null
  tailPlateNo?: string | null
}

export interface ApiTripTailVehicle {
  brandName: string | null
  chassisNumber: string
  driverId: number | null
  id: number
  modelName: string | null
  plateLetters: string
  plateNo: string
  status: number
  vehicleType: {
    id: number
    nameAr: string
    nameEn: string
  } | null
  vehicleTypeId: number
  year: number
}

export interface ApiTripVehicle {
  brandName: null | string
  chassisNumber: string
  driver?: {
    fullName: string
    fullNameAr: string
    fullNameEn: string
  }
  id: number
  modelName: null | string
  plateLetters: string
  plateNo: string
  tailVehicles?: {
    plateLetters: string
    plateNo: string
  }[]
  sector: ApiSector | null
  vehicleType: {
    id: number
    nameAr: string
    nameEn: string
  }
  vehicleTypeId: number
  year: number
}

export interface ApiTripCompany {
  id: number
  nameAr: string
  nameEn: string
  address?: null | string
  email?: null | string
  phone?: null | string
  postalCode?: null | string
  registrationNumber?: null | string
}

export interface ApiTripWaypoint {
  addressName: string
  googlePlaceId: null | string
  id: number
  latitude: number
  longitude: number
  sequence: number
  status: TripStatus
  stopType: number
  taskDetails: string
}

export interface ApiTripRecordingLocation {
  Accuracy: null | number
  Heading: null | number
  Latitude: number
  Longitude: number
  RecordedAt: string
  Speed: null | number
}

export interface ApiTripRecordingWaypoint {
  AddressName: string
  Id: number
  Latitude: number
  Longitude: number
  Status: number
}

/** Raw `data` payload from trip recording API (PascalCase keys). */
export interface ApiTripRecordingData {
  DeliveryNoteImagePath?: null | string
  Location?: ApiTripRecordingLocation
  Note?: null | string
  Odometer?: null | number
  OdometerImagePath?: null | string
  ReceiptNoteImagePath?: null | string
  ShipmentImagePaths?: string[]
  TollgateImagePaths?: string[]
  TollgatesTotalAmount?: number
  UnloadingImagePaths?: string[]
  Waypoint?: ApiTripRecordingWaypoint
}

export interface ApiTripRecording {
  addedBy: number
  addedByModelType: RecordingModelType
  createdAt: string
  data: ApiTripRecordingData
  eventName: string
  eventType: number
  id: number
}

export interface TripWizardRecordingLocation {
  accuracy: null | number | undefined
  heading: null | number | undefined
  latitude: number
  longitude: number
  recordedAt: string
  speed: null | number | undefined
}

export interface TripWizardRecordingWaypoint {
  addressName: string
  id: number
  latitude: number
  longitude: number
  status: number
}

export interface TripWizardRecordingData {
  deliveryNoteImagePath?: null | string
  location?: TripWizardRecordingLocation
  note?: null | string
  odometer?: null | number
  odometerImagePath?: null | string
  receiptNoteImagePath?: null | string
  shipmentImagePaths?: string[]
  tollgateImagePaths?: string[]
  tollgatesTotalAmount?: number
  unloadingImagePaths?: string[]
  waypoint?: TripWizardRecordingWaypoint
}

export interface TripWizardRecording {
  addedBy: number
  addedByModelType: RecordingModelType
  createdAt: string
  data: TripWizardRecordingData
  eventName: string
  eventType: number
  id: number
}

export interface ApiTripDocument {
  createdAt: string
  paths: string[]
  fileUrls: string[]
  recordingId: number
  type: string
  typeName: string
  waypointId: number
}

export interface TripWizardDocument {
  createdAt: string
  files: {path: string, url: string}[]
  recordingId: number
  type: string
  typeName: string
  waypointId: number
}

export interface ApiTripContractConditions {
  documentId: number
  fileUrl: string
  nameAr: string
  nameEn: string
}

export interface ApiTripContractVehicleType {
  id: number
  vehicleTypeId: number
  consumptionRate?: number | null
  vehicleType: {
    id: number
    nameAr: string
    nameEn: string
  }
}

export interface ApiTripChargeExtraFeeType {
  id: number
  nameAr?: string
  nameEn: string
}

export interface ApiTripCharge {
  amount: number
  chargeType: number
  chargeTypeName: string
  companyShare: number | null
  extraFeeType: ApiTripChargeExtraFeeType
  extraFeeTypeId: number
  id: number
  notes: string | null
}

export interface TripWizardTripCharge {
  amount: number
  chargeType: number
  chargeTypeName: string
  companyShare: number | null
  extraFeeTypeId: number | null
  id: number
  notes: string | null
}

export interface ApiTripInvoice {
  id: number
  invoiceNumber: string
  invoiceType: number
  invoiceTypeName: string
  status: number
  totalAmount: number
}

export interface ApiTrip {
  actualCargoWeightTons: number | null
  actualDistanceKm: number
  auditedAt: string | null
  auditedBy?: {
    email: string
    fullName: string
    id: number
    userName: string
  } | null
  bookingRequests?: ApiTripBookingRequest[] | null
  cancellationReason?: string | null
  cargoPrice: number | null
  claimInvoice?: ApiTripInvoice
  company: ApiTripCompany
  contactPersonMobile: null | string
  contactPersonName: null | string
  contract: ApiTripContract
  contractConditions?: ApiTripContractConditions | null
  contractDistanceKm: null | number
  contractId: number
  contractVehicleType: ApiTripContractVehicleType | null
  contractor?: {
    email: string
    fullName: string
    id: number
    phone: string
  } | null
  contractorId?: number | null
  contractorPrice: number | null
  createdAt: string
  currentDriverStatus: number
  currentLocation: CurrentLocation | null
  customsStatus: TripCustomsStatus
  dailyValueCost: null | number
  dispatcherNotes?: string | null
  dispatchingMode?: DispatchingMode | null
  documents?: ApiTripDocument[]
  driver: ApiTripDriver
  driverId: number
  endDate: string
  financialDate: string | null
  extraKmCost: null | number
  extraTripCost: null | number
  fuelCost: null | number
  fuelPricingCalculationOption: fuelPricingCalculationOption | null
  hasCustoms?: boolean
  headPlateChangedAt: string | null
  id: number
  invoice?: ApiTripInvoice
  invoicingStatus?: number | null
  invoicingStatusName?: string | null
  isBackfill: boolean
  isDailyExtraTrip: boolean
  isEditable: boolean
  isExternalTrip: boolean
  isExtraTrip: null | boolean
  lead: {
    email: string | null
    fullName: string
    id: number
    phone: string
  }
  lifecycleActor: TripLifecycleActor | null
  lifecycleActorName: string | null
  numberOfDays: number
  overCapacityCost: null | number
  overNightCostBearer?: number | null
  overNightCostBearerName?: string
  proofOfDelivery?: null | number
  recordings?: ApiTripRecording[]
  referenceNumber: string
  requiresClaimInvoice: boolean
  requiresMainInvoice: boolean
  spotPrice: number | null
  startDate: string
  status: TripStatus
  loadlyCommission?: number | null
  tailPlateChangedAt: string | null
  tailVehicle: ApiTripTailVehicle | null
  totalAddonsCost: null | number
  totalBillableAmount: null | number
  totalBillableFuelAmount: null | number
  totalBillableTollGatesAmount: null | number
  totalFuelAmount: null | number
  totalTripCost: null | number
  tripCharges?: ApiTripCharge[]
  tripContractDestinations: ApiTripContractDestination[] | null
  tripCostWithoutAddon: null | number
  tripDocumentStatus?: TripDocumentStatus
  tripDocumentStatusName?: string
  type?: number | null
  updatedAt: string
  vehicle: ApiTripVehicle
  vehicleId: number
  vehicleSnapshot?: ApiVehicleSnapshot | null
  vehicleTypeId: number
  waypoints: ApiTripWaypoint[]
}

export interface ApiTripList {
  additional?: {
    exportKeys: ExportKey[]
  } | null
  data: ApiTrip[]
  pagination: Pagination
}

export function waypointAdapter(waypoint: ApiTripWaypoint): TripWizardWaypoint {
  return {
    id: waypoint.id,
    addressName: waypoint.addressName,
    googlePlaceId: waypoint.googlePlaceId,
    latitude: waypoint.latitude?.toString(),
    longitude: waypoint.longitude?.toString(),
    sequence: waypoint.sequence,
    status: waypoint.status,
    stopType: waypoint.stopType.toString(),
    taskDetails: waypoint.taskDetails,
  }
}
export function tripAdapter(trip: ApiTrip): Trip {
  const tripContractDestinations = tripContractDestinationsAdapter(trip.tripContractDestinations)

  return {
    actualCargoWeightTons: trip.actualCargoWeightTons,
    actualDistanceKm: trip.actualDistanceKm,
    auditedAt: trip.auditedAt,
    bookingRequests: (trip.bookingRequests ?? []).map(tripBookingRequestAdapter),
    claimInvoiceNumber: trip.claimInvoice?.invoiceNumber,
    claimInvoiceTypeName: trip.claimInvoice?.invoiceTypeName,
    companyId: trip.company?.id.toString(),
    companyName: trip.company ? getLocalizedValue(trip.company.nameEn, trip.company.nameAr) : '',
    consumptionRate: trip.contractVehicleType?.consumptionRate,
    contactPersonMobile: trip.contactPersonMobile ?? '',
    contactPersonName: trip.contactPersonName ?? '',
    contract: trip.contract
      ? {
          consumptionType: trip.contract.consumptionType,
          contractTypeName: trip.contract.contractTypeName,
          id: trip.contract.id,
          name: getLocalizedValue(trip.contract.nameEn, trip.contract.nameAr),
          referenceNumber: trip.contract.referenceNumber,
        }
      : undefined,
    contractDistanceKm: trip.contractDistanceKm?.toString() ?? '',
    contractId: trip.contract?.id?.toString() ?? trip.contractId?.toString() ?? '',
    contractRef: trip.contract?.referenceNumber,
    contractorId: trip.contractorId ?? trip.contractor?.id ?? null,
    contractorName: trip.contractor?.fullName ?? null,
    contractorPrice: trip.contractorPrice,
    currentDriverStatus: trip.currentDriverStatus,
    destinationId: tripContractDestinations[0]?.destinationId.toString(),
    destinationPrice: tripContractDestinations[0]?.destination?.contractorFixedTripPrice ?? null,
    dispatchingMode: trip.dispatchingMode ?? null,
    driverName: trip.driver
      ? getLocalizedValue(trip.driver.fullName, trip.driver.fullNameAr)
      : undefined,
    financialDate: trip.financialDate ?? '',
    fuelPricingCalculationOption: trip.fuelPricingCalculationOption,
    id: trip.id.toString(),
    invoiceNumber: trip.invoice?.invoiceNumber,
    invoiceTypeName: trip.invoice?.invoiceTypeName,
    invoicingStatus: trip.invoicingStatus ?? null,
    invoicingStatusName: trip.invoicingStatusName ?? null,
    isBackfill: trip.isBackfill,
    isEditable: trip.isEditable,
    isExternalTrip: trip.isExternalTrip,
    lifecycleActor: trip.lifecycleActor,
    lifecycleActorName: trip.lifecycleActorName,
    overNightCostBearer: trip.overNightCostBearer,
    overNightCostBearerName: trip.overNightCostBearerName,
    plateLetters: trip.vehicleSnapshot
      ? trip.vehicleSnapshot.headPlateLetters?.toString()
      : trip.vehicle?.plateLetters,
    plateNumber: trip.vehicleSnapshot
      ? trip.vehicleSnapshot.headPlateNo?.toString()
      : trip.vehicle?.plateNo,
    proofOfDelivery: trip.proofOfDelivery,
    recordings: tripRecordingsResponseAdapter(trip.recordings),
    referenceNumber: trip.referenceNumber,
    requiresClaimInvoice: trip.requiresClaimInvoice,
    requiresMainInvoice: trip.requiresMainInvoice,
    sector: trip.vehicle?.sector ? sectorAdapter(trip.vehicle.sector) : undefined,
    spotPrice: trip.spotPrice,
    startDate: formatDateLocal(trip.startDate) ?? '',
    status: trip.status,
    loadlyCommission: trip.loadlyCommission,
    tailPlateLetters: trip.vehicleSnapshot
      ? trip.vehicleSnapshot.tailPlateLetters?.toString()
      : trip.tailVehicle?.plateLetters,
    tailPlateNumber: trip.vehicleSnapshot
      ? trip.vehicleSnapshot.tailPlateNo?.toString()
      : trip.tailVehicle?.plateNo,
    totalAddonsCost: trip.totalAddonsCost,
    totalBillableAmount: trip.totalBillableAmount,
    totalBillableFuelAmount: trip.totalBillableFuelAmount,
    totalBillableTollGatesAmount: trip.totalBillableTollGatesAmount,
    totalFuelAmount: trip.totalFuelAmount,
    totalTripCost: trip.totalTripCost,
    tripCharges: tripChargesResponseAdapter(trip.tripCharges),
    tripContractDestinations,
    tripCostWithoutAddon: trip.tripCostWithoutAddon,
    tripDocumentStatus: trip.tripDocumentStatus,
    tripDocumentStatusName: trip.tripDocumentStatusName,
    vehicleSnapshot: {
      headPlateChangedAt: trip.headPlateChangedAt,
      headPlateLetters: trip.vehicle?.plateLetters,
      headPlateNo: trip.vehicle?.plateNo,
      tailPlateChangedAt: trip.tailPlateChangedAt,
      tailPlateLetters: trip.tailVehicle?.plateLetters,
      tailPlateNo: trip.tailVehicle?.plateNo,
    },
    vehicleType: trip.contractVehicleType
      ? getLocalizedValue(trip.contractVehicleType.vehicleType.nameEn, trip.contractVehicleType.vehicleType.nameAr)
      : undefined,
    waypoints: trip.waypoints.map(waypointAdapter),
  }
}

export function tripListResponseAdapter(response: ApiTripList): TripList {
  return {
    additional: response.additional,
    data: response.data.map(tripAdapter),
    pagination: response.pagination,
  }
}

export function tripChargesResponseAdapter(
  charges: ApiTripCharge[] | undefined,
): TripWizardTripCharge[] {
  return (charges ?? []).map(charge => ({
    amount: Number(charge.amount),
    chargeType: charge.chargeType,
    chargeTypeName: charge.chargeTypeName,
    companyShare: charge.companyShare || 0,
    extraFeeTypeId: charge.extraFeeTypeId,
    id: charge.id,
    notes: charge.notes,
  }))
}

export function tripDocumentsResponseAdapter(
  documents: ApiTripDocument[] | undefined,
): TripWizardDocument[] {
  return (documents ?? []).map(document => ({
    createdAt: document.createdAt,
    files: document.fileUrls.map((url, index) => ({
      path: document.paths[index] ?? '',
      url: url,
    })),
    recordingId: document.recordingId,
    type: document.type,
    typeName: document.typeName,
    waypointId: document.waypointId,
  }))
}

function normalizeTripRecordingData(data: ApiTripRecordingData): TripWizardRecordingData {
  const loc = data.Location

  return {
    deliveryNoteImagePath: data.DeliveryNoteImagePath ?? undefined,
    location: loc
      ? {
          accuracy: loc.Accuracy ?? undefined,
          heading: loc.Heading ?? undefined,
          latitude: loc.Latitude,
          longitude: loc.Longitude,
          recordedAt: loc.RecordedAt,
          speed: loc.Speed ?? undefined,
        }
      : undefined,
    note: data.Note ?? undefined,
    odometer: data.Odometer ?? undefined,
    odometerImagePath: data.OdometerImagePath ?? undefined,
    receiptNoteImagePath: data.ReceiptNoteImagePath ?? undefined,
    shipmentImagePaths: data.ShipmentImagePaths ? [...data.ShipmentImagePaths] : undefined,
    tollgateImagePaths: data.TollgateImagePaths ? [...data.TollgateImagePaths] : undefined,
    tollgatesTotalAmount: data.TollgatesTotalAmount,
    unloadingImagePaths: data.UnloadingImagePaths ? [...data.UnloadingImagePaths] : undefined,
    waypoint: data.Waypoint
      ? {
          addressName: data.Waypoint.AddressName,
          id: data.Waypoint.Id,
          latitude: data.Waypoint.Latitude,
          longitude: data.Waypoint.Longitude,
          status: data.Waypoint.Status,
        }
      : undefined,
  }
}

export function tripRecordingsResponseAdapter(recordings: ApiTripRecording[] | undefined): TripWizardRecording[] {
  return (recordings ?? []).map(recording => ({
    addedBy: recording.addedBy,
    addedByModelType: recording.addedByModelType,
    createdAt: recording.createdAt,
    data: normalizeTripRecordingData(recording.data),
    eventName: recording.eventName,
    eventType: recording.eventType,
    id: recording.id,
  }))
}

export function tripResponseAdapter(response: ApiTrip): TripWizard {
  const tripContractDestinations = tripContractDestinationsAdapter(response.tripContractDestinations)

  return {
    actualCargoWeightTons: response.actualCargoWeightTons,
    actualDistanceKm: response.actualDistanceKm,
    auditorName: response.auditedBy?.fullName ?? null,
    cancellationReason: response.cancellationReason ?? null,
    cargoPrice: response.cargoPrice,
    companyAddress: response.company?.address ?? '',
    companyEmail: response.company?.email ?? '',
    companyId: response.company?.id.toString(),
    companyName: response.company ? getLocalizedValue(response.company.nameEn, response.company.nameAr) : '',
    companyPhone: response.company?.phone ?? '',
    companyPostalCode: response.company?.postalCode ?? '',
    companyRegistrationNumber: response.company?.registrationNumber ?? '',
    contactPersonMobile: response.contactPersonMobile ?? '',
    contactPersonName: response.contactPersonName ?? '',
    contract: response.contract
      ? {
          consumptionType: response.contract.consumptionType,
          contractTypeName: response.contract.contractTypeName,
          id: response.contract.id,
          name: getLocalizedValue(response.contract.nameEn, response.contract.nameAr),
          referenceNumber: response.contract.referenceNumber,
        }
      : undefined,
    contractConditions: response.contractConditions
      ? {
          documentId: response.contractConditions.documentId,
          fileUrl: response.contractConditions.fileUrl,
          name: getLocalizedValue(
            response.contractConditions.nameEn,
            response.contractConditions.nameAr,
          ),
        }
      : null,
    contractDistanceKm: response.contractDistanceKm?.toString() ?? '',
    contractId: response.contract?.id.toString(),
    contractorId:
      response.contractorId?.toString() ??
      response.contractor?.id?.toString() ??
      '',
    contractorName: response.contractor?.fullName ?? '',
    contractorPhone: response.contractor?.phone ?? '',
    contractorPrice: response.contractorPrice != null ? String(response.contractorPrice) : '',
    createdAt: response.createdAt,
    bookingRequests: (response.bookingRequests ?? []).map(tripBookingRequestAdapter),
    currentDriverStatus: response.currentDriverStatus?.toString(),
    currentLocation: response.currentLocation,
    customsStatus: response.customsStatus,
    dailyValueCost: response.dailyValueCost,
    destinationId: tripContractDestinations[0]?.destinationId.toString(),
    destinationStatus: tripContractDestinations[0]?.destination?.status,
    destinationStatusName: tripContractDestinations[0]?.destination?.statusName,
    dispatcherNotes: response.dispatcherNotes ?? null,
    dispatchingMode: response.dispatchingMode
      ?? ((response.contractorId ?? response.contractor?.id)
        ? DispatchingMode.Contractors
        : DispatchingMode.Loadly),
    documents: tripDocumentsResponseAdapter(response.documents),
    driverId: response.driver?.id.toString(),
    driverName: response.driver
      ? getLocalizedValue(response.driver.fullName, response.driver.fullNameAr)
      : undefined,
    driverPhone: response.driver?.phone ?? '',
    endDate: toDateTimeLocal(response.endDate),
    financialDate: response.financialDate ?? '',
    hasCustoms: response.hasCustoms ?? false,
    id: response.id.toString(),
    isEditable: response.isEditable,
    isExternalTrip: response.isExternalTrip,
    isBackfill: response.isBackfill,
    isInvoiced: !!response.invoice || !!response.claimInvoice,
    leadId: response.lead?.id,
    numberOfDays: response.numberOfDays,
    overNightCostBearer: response.overNightCostBearer,
    overNightCostBearerName: response.overNightCostBearerName,
    proofOfDelivery: response.proofOfDelivery,
    recordings: tripRecordingsResponseAdapter(response.recordings),
    referenceNumber: response.referenceNumber,
    returnDestinationId: tripContractDestinations[0]?.returnDestinationId?.toString(),
    sector: response.vehicle?.sector ? sectorAdapter(response.vehicle.sector) : undefined,
    spotPrice: response.spotPrice,
    startDate: toDateTimeLocal(response.startDate),
    status: response.status,
    tripCharges: tripChargesResponseAdapter(response.tripCharges),
    tripContractDestinations,
    tripDocumentStatus: response.tripDocumentStatus,
    tripDocumentStatusName: response.tripDocumentStatusName,
    type: response.type != null ? String(response.type) : '',
    vehicle: {
      driverName: response.vehicle?.driver?.fullName,
      plateLetters: response.vehicleSnapshot
        ? response.vehicleSnapshot.headPlateLetters?.toString() ?? ''
        : response.vehicle?.plateLetters,
      plateNo: response.vehicleSnapshot
        ? response.vehicleSnapshot.headPlateNo?.toString() ?? ''
        : response.vehicle?.plateNo,
      tailVehicles: response.vehicle?.tailVehicles?.map(tail => {
        return {
          plateLetters: response.vehicleSnapshot
            ? response.vehicleSnapshot.tailPlateLetters?.toString() ?? ''
            : tail.plateLetters,
          plateNo: response.vehicleSnapshot
            ? response.vehicleSnapshot.tailPlateNo?.toString() ?? ''
            : tail.plateNo,
        }
      }),
    },
    vehicleId: response.vehicle?.id,
    vehicleType: getLocalizedValue(response.vehicle?.vehicleType.nameEn, response.vehicle?.vehicleType.nameAr),
    vehicleTypeId: response.contractVehicleType?.vehicleTypeId.toString(),
    waypoints: response.waypoints.map(waypointAdapter),
  }
}
