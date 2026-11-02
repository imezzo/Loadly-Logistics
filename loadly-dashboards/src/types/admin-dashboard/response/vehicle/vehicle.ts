import type { ExportKey } from '@/types/export-keys.types'
import type { Pagination } from '@/types/response/pagination'
import { getLocalizedValue } from '@/utils/i18n'
import { vehicleFacilityAdapter, vehicleFacilityVisitAdapter } from '../vehicle-facility'
import type {
  ApiVehicleFacility,
  ApiVehicleFacilityVisit,
  VehicleFacility,
  VehicleFacilityVisit,
} from '../vehicle-facility'
import { VehicleStatus } from './vehicle.enums'
import type { ApiPlateChangeHistory } from './vehicle-detail'
import { sectorAdapter, type ApiSector, type Sector } from '../sector/sector'

export interface Contract {
  name: string
  referenceNumber: string
}

export interface Vehicle {
  assignedParkingFacility: VehicleFacility | null
  brandModelId: number
  brandModelName: string
  capacityUnitId: number | null
  companyName: string
  completedTripsCount: number
  contract: Contract | null
  contractorId: number | null
  currentServiceFacility: VehicleFacility | null
  displayText: string
  driverId: number | null
  driverName: string | null
  driverPhone: string | null
  headDisplayText?: string | null
  headVehicleId: number | null
  headVehicleTypeId?: number
  id: number
  initialOdometerReading: number
  makeYear: number
  plateChangeHistory?: ApiPlateChangeHistory[]
  plateLetters: string
  plateNo: string
  sector: Sector | null
  status: VehicleStatus
  tail: {
    id?: number
    plateLetters?: string
    plateNo?: string
    vehicleType?: string | null
    vehicleTypeId?: number
  }
  tailDisplayText?: string | null
  tailVehicleId: number | null
  tailVehicleTypeId?: number
  vehicleFacilityVisits: VehicleFacilityVisit[]
  vehicleTypeName: string
  vehicleUnitType: string
  vinNo: string | null
  waitingForLoading: boolean
}

export interface VehiclesListResponse {
  additional?: {
    exportKeys: ExportKey[]
  } | null
  vehicles: Vehicle[]
  pagination: Pagination
}

export interface ApiVehicleType {
  id: number
  name: string
  nameAr: string
  nameEn: string
  category: string
  vehicleUnitType: string
  status: string
}

export interface ApiVehicleBrandModel {
  id: number
  name: string
  nameAr: string
  nameEn: string
  status: number
  vehicleBrand: unknown
}

export interface ApiVehicleDriver {
  id: number
  fullNameEn: string
  fullNameAr: string
  nationalId: string
  userName: string
  email: string
  phone: string
  status: number
}

export interface ApiContract {
  company: {
    email: string
    fullName: string
    id: number
    nameAr: string
    nameEn: string
    phone: string
    registrationNumber: string
  }
  consumptionType: number
  id: number
  nameAr: string
  nameEn: string
  referenceNumber: string
}

export interface ApiContractor {
  email: string
  fullName: string
  id: number
  name: string
  phone: string
  userName: string
}

export interface ApiVehicle {
  addedBy: number | null
  assignedParkingFacility: ApiVehicleFacility | null
  brandModelID: number
  capacity: number
  capacityUnitId: number | null
  colorId: number
  completedTripsCount: number
  consumption?: number | null
  contract: ApiContract | null
  contractor: ApiContractor | null
  createdAt: string
  currentServiceFacility: ApiVehicleFacility | null
  driver?: ApiVehicleDriver | null
  driverId: number | null
  headVehicle?: { id?: number; plateLetters?: string; plateNo?: string; vehicleTypeId?: number } | null
  headVehicleId: number | null
  id: number
  initialOdometerReading: number
  insuranceId: number | null
  makeYear: number
  plateChangeHistory?: ApiPlateChangeHistory[]
  plateLetters: string
  plateNo: string
  purchaseDate: string | null
  status: VehicleStatus
  tailVehicle?: { id?: number; plateLetters?: string; plateNo?: string } | null
  tailVehicleId: number | null
  tailVehicles?: {
    id?: number
    plateLetters?: string
    plateNo?: string
    vehicleTypeId?: number
    vehicleTypeName?: string
    vehicleType?: {
      name?: string
      nameAr?: string
      nameEn?: string
    } | null
  }[]
  tankCapacity: number | null
  totalTrips?: number | null
  updatedAt: string
  vehicleBrandModel?: ApiVehicleBrandModel | null
  vehicleFacilityVisits?: ApiVehicleFacilityVisit[]
  vehicleType?: ApiVehicleType | null
  vehicleTypeId: number
  vehicleTypeSettings: string | null
  vinNo: string | null
  waitingForLoading: boolean
  sector: ApiSector | null
}

export interface ApiVehiclesListResponse {
  additional?: {
    exportKeys: ExportKey[]
  } | null
  data: ApiVehicle[]
  pagination: Pagination
}

function formatVehicleDisplayText(vehicle: ApiVehicle): string {
  const plateInfo = `${vehicle.plateNo} ${vehicle.plateLetters}`.trim()
  const maintenanceDate = vehicle.purchaseDate
    ? new Date(vehicle.purchaseDate).toLocaleDateString('en-GB', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
    })
    : null
  const odometer = vehicle.initialOdometerReading
    ? vehicle.initialOdometerReading.toLocaleString('en-US')
    : null
  const statusText = vehicle.status === VehicleStatus.Available ? 'Available' : 'Unavailable'


  const parts = [
    `Car ID - ${plateInfo}`,
    maintenanceDate ? `maintenance ${maintenanceDate}` : null,
    `${odometer} KM`,
    statusText,
  ].filter(Boolean)

  return parts.join(' - ')
}

function formatLinkedPlate(vehicle: { plateNo?: string; plateLetters?: string }): string {
  return [vehicle.plateNo, vehicle.plateLetters].filter(Boolean).join(' - ').trim()
}

function contractAdapter(data: ApiContract): Contract {
  return {
    name: getLocalizedValue(data.nameEn, data.nameAr),
    referenceNumber: data.referenceNumber,
  }
}

export function vehicleAdapter(data: ApiVehicle): Vehicle {
  const tail = data.tailVehicles?.[0]

  return {
    assignedParkingFacility: data.assignedParkingFacility
      ? vehicleFacilityAdapter(data.assignedParkingFacility)
      : null,
    brandModelId: data.brandModelID,
    brandModelName: data.vehicleBrandModel?.name ?? '',
    capacityUnitId: data.capacityUnitId,
    companyName: getLocalizedValue(data.contract?.company.nameEn ?? '', data.contract?.company.nameAr ?? ''),
    completedTripsCount: data.completedTripsCount,
    contract: data.contract ? contractAdapter(data.contract) : null,
    contractorId: data.contractor?.id ?? null,
    currentServiceFacility: data.currentServiceFacility
      ? vehicleFacilityAdapter(data.currentServiceFacility)
      : null,
    displayText: formatVehicleDisplayText(data),
    driverId: data.driverId,
    driverName: getLocalizedValue(data.driver?.fullNameEn ?? '', data.driver?.fullNameAr ?? ''),
    driverPhone: data.driver?.phone ?? null,
    headDisplayText: formatLinkedPlate(data.headVehicle ?? {}),
    headVehicleId: data.headVehicleId,
    headVehicleTypeId: data.vehicleTypeId,
    id: data.id,
    initialOdometerReading: data.initialOdometerReading,
    makeYear: data.makeYear,
    plateChangeHistory: data.plateChangeHistory ?? [],
    plateLetters: data.plateLetters,
    plateNo: data.plateNo,
    sector: data.sector ? sectorAdapter(data.sector) : null,
    status: data.status,
    tail: {
      id: tail?.id,
      plateLetters: tail?.plateLetters,
      plateNo: tail?.plateNo,
      vehicleTypeId: tail?.vehicleTypeId,
      vehicleType: tail?.vehicleType?.name,
    },
    tailDisplayText: formatLinkedPlate(data.tailVehicles?.[0] ?? {}),
    tailVehicleId: data.tailVehicleId,
    tailVehicleTypeId: data.tailVehicles?.[0]?.vehicleTypeId,
    vehicleFacilityVisits: (data.vehicleFacilityVisits ?? []).map(vehicleFacilityVisitAdapter),
    vehicleTypeName: getLocalizedValue(data.vehicleType?.nameEn ?? '', data.vehicleType?.nameAr ?? ''),
    vehicleUnitType: data.vehicleType?.vehicleUnitType ?? '',
    vinNo: data.vinNo,
    waitingForLoading: data.waitingForLoading,
  }
}

export function vehiclesListResponseAdapter(
  response: ApiVehiclesListResponse
): VehiclesListResponse {
  return {
    additional: response.additional,
    vehicles: response.data.map(vehicleAdapter),
    pagination: response.pagination,
  }
}
