import type { Pagination } from '@/types/response/pagination'
import type { SecondaryContact } from '@/types/secondary-contact.types'
import { getLocalizedValue } from '@/utils/i18n'
import { VehicleFacilityType, VehicleFacilityVisitEvent } from './vehicle-facility.enums'

export interface VehicleFacilityContact {
  email: string
  id: number
  name: string
  phone: string
  position: string | null
  primary: boolean
}

export interface VehicleFacilityVisitVehicle {
  id: number
  plateLetters: string
  plateNo: string
  vehicleTypeName: string
}

export interface VehicleFacilityVisitAddedBy {
  email: string
  fullName: string
  id: number
  userName: string
}

export interface VehicleFacilityVisit {
  addedBy: VehicleFacilityVisitAddedBy | null
  createdAt: string
  event: VehicleFacilityVisitEvent
  eventDate: string
  id: number
  reason: string | null
  vehicle: VehicleFacilityVisitVehicle | null
}

export interface VehicleFacility {
  address: string
  communicationMethod: string
  companyId: number | null
  contactPersonName: string
  email: string
  facilityType: VehicleFacilityType
  googleMapsUrl: string
  id: number
  lat: number
  lng: number
  name: string
  phone: string
  position: string
  secondaryContacts: SecondaryContact[]
  status: number
  vehicleFacilityVisits: VehicleFacilityVisit[]
}

export interface VehicleFacilitiesListResponse {
  facilities: VehicleFacility[]
  pagination: Pagination
}

export interface ApiVehicleFacilityContact {
  communicationMethod: string | null
  communicationMethodId: number | null
  email: string
  id: number
  modelId: number
  modelType: string
  name: string
  phone: string
  position: string | null
  primary: boolean
}

export interface ApiVehicleFacilityVisitVehicle {
  brandName: string | null
  chassisNumber: string | null
  driver: unknown | null
  id: number
  modelName: string | null
  plateLetters: string
  plateNo: string
  sector: unknown | null
  sectorId: number | null
  tailVehicles: unknown[]
  vehicleType: { id?: number; name?: string; nameAr?: string; nameEn?: string } | null
  vehicleTypeId: number
  year: number
}

export interface ApiVehicleFacilityVisitAddedBy {
  email: string
  fullName: string
  id: number
  userName: string
}

export interface ApiVehicleFacilityVisit {
  addedBy: ApiVehicleFacilityVisitAddedBy | null
  createdAt: string
  driver: unknown | null
  event: number
  eventDate: string
  id: number
  reason: string | null
  vehicle: ApiVehicleFacilityVisitVehicle | null
  vehicleFacility: unknown | null
}

export interface ApiVehicleFacility {
  addedBy: number
  address: string
  companyId: number | null
  contacts: ApiVehicleFacilityContact[]
  facilityType: number
  googleMapsUrl: string
  id: number
  lat: number
  lng: number
  name: string
  status: number
  vehicleFacilityVisits?: ApiVehicleFacilityVisit[]
}

export interface ApiVehicleFacilitiesListResponse {
  data: ApiVehicleFacility[]
  pagination: Pagination
}

export function vehicleFacilityContactAdapter(
  data: ApiVehicleFacilityContact
): SecondaryContact {
  return {
    email: data.email,
    id: data.id,
    name: data.name,
    phone: data.phone,
    position: data.position ?? '',
    communicationMethod: data.communicationMethodId?.toString() ?? '',
  }
}

export function vehicleFacilityVisitAdapter(data: ApiVehicleFacilityVisit): VehicleFacilityVisit {
  const vehicleType = data.vehicle?.vehicleType

  return {
    addedBy: data.addedBy
      ? {
          email: data.addedBy.email,
          fullName: data.addedBy.fullName.trim(),
          id: data.addedBy.id,
          userName: data.addedBy.userName,
        }
      : null,
    createdAt: data.createdAt,
    event: data.event,
    eventDate: data.eventDate,
    id: data.id,
    reason: data.reason,
    vehicle: data.vehicle
      ? {
          id: data.vehicle.id,
          plateLetters: data.vehicle.plateLetters,
          plateNo: data.vehicle.plateNo,
          vehicleTypeName: vehicleType
            ? (vehicleType.name
              ?? getLocalizedValue(vehicleType.nameEn ?? '', vehicleType.nameAr ?? ''))
            : '',
        }
      : null,
  }
}

export function vehicleFacilityAdapter(data: ApiVehicleFacility): VehicleFacility {
  const primaryContact = data.contacts?.find((contact) => contact.primary) ?? data.contacts?.[0]
  const secondaryContacts = data.contacts?.filter((contact) => !contact.primary && contact.id !== primaryContact?.id)
    .map((contact): SecondaryContact => ({
      communicationMethod: contact.communicationMethodId?.toString() ?? '',
      email: contact.email,
      id: contact.id,
      name: contact.name,
      phone: contact.phone,
      position: contact.position ?? '',
    })) ?? []

  return {
    address: data.address,
    communicationMethod: primaryContact?.communicationMethodId?.toString() ?? '',
    companyId: data.companyId ?? null,
    contactPersonName: primaryContact?.name ?? '',
    email: primaryContact?.email ?? '',
    facilityType: data.facilityType as VehicleFacilityType,
    googleMapsUrl: data.googleMapsUrl,
    id: data.id,
    lat: data.lat,
    lng: data.lng,
    name: data.name,
    phone: primaryContact?.phone ?? '',
    position: primaryContact?.position ?? '',
    secondaryContacts,
    status: data.status,
    vehicleFacilityVisits: (data.vehicleFacilityVisits ?? []).map(vehicleFacilityVisitAdapter),
  }
}

export function vehicleFacilitiesListResponseAdapter(
  response: ApiVehicleFacilitiesListResponse
): VehicleFacilitiesListResponse {
  return {
    facilities: response.data.map(vehicleFacilityAdapter),
    pagination: response.pagination,
  }
}
