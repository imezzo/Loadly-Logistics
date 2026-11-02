import { apiPaginationAdapter } from '@/types/request/pagination'
import type { ApiPagination, Pagination } from '@/types/request/pagination'
import type { SecondaryContact } from '@/types/secondary-contact.types'

export interface VehicleFacilitiesListRequest {
  pagination: Pagination
  search?: string
}

export interface VehicleFacilityFormValues {
  address: string
  communicationMethod: string
  companyId: string
  contactPersonName: string
  email: string
  facilityType: string
  googleMapsUrl: string
  lat: string
  lng: string
  name: string
  phone: string
  position: string
  secondaryContacts?: SecondaryContact[]
  status: string
}

export interface ApiVehicleFacilitiesListRequest extends ApiPagination {
  search?: string
}

export interface ApiVehicleFacilityContactRequest {
  communicationMethodId: number | null
  email: string
  name: string
  phone: string
  position: string | null
  primary: boolean
}

export interface ApiVehicleFacilityCreateRequest {
  address: string
  companyId?: number
  contacts: ApiVehicleFacilityContactRequest[]
  facilityType: number
  googleMapsUrl: string
  lat?: number
  lng?: number
  name: string
  status: number
}

export function vehicleFacilitiesListRequestAdapter(
  data: VehicleFacilitiesListRequest
): ApiVehicleFacilitiesListRequest {
  return {
    ...apiPaginationAdapter(data.pagination),
    ...(data.search ? { search: data.search } : {}),
  }
}

function secondaryContactsAdapter(
  contacts: SecondaryContact[] | undefined
): ApiVehicleFacilityContactRequest[] {
  if (!contacts?.length) return []

  return contacts.map((contact) => ({
    communicationMethodId: contact.communicationMethod
      ? Number(contact.communicationMethod)
      : null,
    email: contact.email,
    name: contact.name,
    phone: contact.phone,
    position: contact.position || null,
    primary: false,
  }))
}

export function vehicleFacilityCreateRequestAdapter(
  data: VehicleFacilityFormValues
): ApiVehicleFacilityCreateRequest {
  return {
    address: data.address,
    ...(data.companyId ? { companyId: Number(data.companyId) } : {}),
    contacts: [
      {
        communicationMethodId: data.communicationMethod
          ? Number(data.communicationMethod)
          : null,
        email: data.email,
        name: data.contactPersonName,
        phone: data.phone,
        position: data.position,
        primary: true,
      },
      ...secondaryContactsAdapter(data.secondaryContacts),
    ],
    facilityType: Number(data.facilityType),
    googleMapsUrl: data.googleMapsUrl,
    ...(data.lat ? { lat: Number(data.lat) } : {}),
    ...(data.lng ? { lng: Number(data.lng) } : {}),
    name: data.name,
    status: Number(data.status),
  }
}
