import type { ApiDriverDocument } from './driver'
import { extractISODate } from '@/utils/date'

export interface DriverDetail {
  age?: number
  buildingNumber?: string
  buildingType?: string
  cityId?: string
  contractorId?: string
  createdAt: string
  dateOfBirth?: string
  districtId?: string
  documents: DriverDocument[]
  email: string
  emailVerified: boolean
  emergencyContactName?: string
  emergencyContactPhone?: string
  fullNameAr: string
  fullNameEn: string
  governerateId?: string
  id: number
  spokenLanguages?: string
  licenseCategory?: string
  licenseExpirationDate?: string
  licenseIssueDate?: string
  licenseNo?: string
  licenseTypeId?: string
  mfaChannel: number
  nationalId: string
  phone: string
  phoneVerified: boolean
  postalCode?: string
  status: number
  streetName?: string
  updatedAt: string
  userName: string
  vehicleTypeIds?: number[]
}

export interface DriverDocument {
  documentId: number
  expiryDate?: string
  file?: File
  fileUrl?: string | null
  id?: number
  status?: number
}

export interface ApiDriverDetailResponse {
  addresses: ApiDriverAddress[] | null
  contractor?: { id: number } | null
  contractorId?: number | null
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
}

export interface ApiDriverDetail {
  age?: number
  dateOfBirth?: string
  emergencyContactName?: string
  emergencyContactPhone?: string
  languagesSpoken?: string
  licenseCategory?: string
  licenseExpirationDate?: string
  licenseIssueDate?: string
  licenseNo?: string
  licenseTypeId?: number
  vehicleTypeIds?: number[]
}

export interface ApiDriverAddress {
  buildingNumber?: string
  buildingType?: string
  cityId?: number
  districtId?: number
  governerateId?: number
  postalCode?: string
  streetName?: string
}

export function driverDocumentAdapter(doc: ApiDriverDocument): DriverDocument {
  return {
    id: doc.documentId,
    documentId: doc.documentId,
    expiryDate: doc.expiryDate || undefined,
    status: doc.status,
    fileUrl: doc.fileUrl,
  }
}

export function driverDetailAdapter(response: ApiDriverDetailResponse): DriverDetail {
  return {
    age: response.driverDetail?.age,
    buildingNumber: response.addresses?.[0]?.buildingNumber,
    buildingType: response.addresses?.[0]?.buildingType,
    cityId: response.addresses?.[0]?.cityId?.toString(),
    contractorId: response.contractorId?.toString()
      ?? response.contractor?.id?.toString(),
    createdAt: response.createdAt,
    dateOfBirth: extractISODate(response.driverDetail?.dateOfBirth),
    districtId: response.addresses?.[0]?.districtId?.toString(),
    documents: response.documents.map(driverDocumentAdapter),
    email: response.email,
    emailVerified: response.emailVerified,
    emergencyContactName: response.driverDetail?.emergencyContactName,
    emergencyContactPhone: response.driverDetail?.emergencyContactPhone,
    fullNameAr: response.fullNameAr,
    fullNameEn: response.fullNameEn,
    governerateId: response.addresses?.[0]?.governerateId?.toString(),
    id: response.id,
    spokenLanguages: response.driverDetail?.languagesSpoken,
    licenseCategory: response.driverDetail?.licenseCategory ?? '',
    licenseExpirationDate: extractISODate(response.driverDetail?.licenseExpirationDate),
    licenseIssueDate: extractISODate(response.driverDetail?.licenseIssueDate),
    licenseNo: response.driverDetail?.licenseNo,
    licenseTypeId: response.driverDetail?.licenseTypeId?.toString(),
    mfaChannel: response.mfaChannel,
    nationalId: response.nationalId,
    phone: response.phone,
    phoneVerified: response.phoneVerified,
    postalCode: response.addresses?.[0]?.postalCode,
    status: response.status,
    streetName: response.addresses?.[0]?.streetName,
    updatedAt: response.updatedAt,
    userName: response.userName,
    vehicleTypeIds: response.driverDetail?.vehicleTypeIds,
  }
}
