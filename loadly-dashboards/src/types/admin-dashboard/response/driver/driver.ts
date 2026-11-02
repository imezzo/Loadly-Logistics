import type { ExportKey } from '@/types/export-keys.types'
import type { Pagination } from '@/types/response/pagination'
import { getLocalizedValue } from '@/utils/i18n'

interface Contractor {
  email: string
  fullName: string
  id: string
  phone: string
}

export interface Driver {
  completedTripsCount?: number | null
  contractor: Contractor | null
  contractorId: string | null
  createdAt: string
  email: string
  fullName: string
  id: string
  licenseCategory?: string
  phone: string
  rate?: number
  status: number
  updatedAt: string
  userName: string
  workingStatus: number
}

export interface DriversListResponse {
  additional?: {
    exportKeys: ExportKey[]
  } | null
  drivers: Driver[]
  pagination: Pagination
}

export interface ApiDriver {
  address: unknown | null
  completedTripsCount?: number | null
  contractor: Contractor | null
  contractorId: number | null
  createdAt: string
  documents: ApiDriverDocument[]
  driverDetail: ApiDriverDetail | null
  email: string
  emailVerified: boolean
  fullNameAr: string
  fullNameEn: string
  id: number
  mfaChannel: number
  nationalId: string
  phone: string
  phoneVerified: boolean
  status: number
  updatedAt: string
  userName: string
  workingStatus: number
}

export interface ApiDriverDetail {
  licenseCategory?: string
  rate?: number
}

export interface ApiDriverDocument {
  addedBy: number
  createdAt: string
  document: {
    addedBy: number | null
    createdAt: string
    entityId: number
    id: number
    nameAr: string
    nameEn: string
    required: boolean
    status: number
    updatedAt: string
  }
  documentId: number
  driverId: number
  expiryDate: string | null
  fileUrl: string | null
  id: number
  path: string
  status: number
  updatedAt: string
}

export interface ApiDriversListResponse {
  additional?: {
    exportKeys: ExportKey[]
  } | null
  data: ApiDriver[]
  pagination: Pagination
}

export function driverAdapter(driver: ApiDriver): Driver {
  return {
    completedTripsCount: driver.completedTripsCount ?? null,
    contractor: driver.contractor
      ? {
          email: driver.contractor.email,
          fullName: driver.contractor.fullName,
          id: driver.contractor.id.toString(),
          phone: driver.contractor.phone,
        }
      : null,
    contractorId: driver.contractorId?.toString() ?? null,
    createdAt: driver.createdAt,
    email: driver.email,
    fullName: getLocalizedValue(driver.fullNameEn, driver.fullNameAr),
    id: driver.id.toString(),
    licenseCategory: driver.driverDetail?.licenseCategory,
    phone: driver.phone,
    rate: driver.driverDetail?.rate,
    status: driver.status,
    updatedAt: driver.updatedAt,
    userName: driver.userName,
    workingStatus: driver.workingStatus,
  }
}

export function driversListResponseAdapter(response: ApiDriversListResponse): DriversListResponse {
  return {
    additional: response.additional,
    drivers: response.data.map(driver => driverAdapter(driver)),
    pagination: response.pagination,
  }
}
