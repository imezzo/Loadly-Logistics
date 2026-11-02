import type { DocumentInfo } from "./document"
import type { SecondaryContact } from "@/types/secondary-contact.types"
import { formatDateLocal } from "@/utils/date"

export interface ApiCompanyDetail {
  id: number
  companyId: number
  nameEn: string
  nameAr: string
  registrationNumber: string
  taxId: string
  websiteUrl: string
  industryTypeId: number | null
  companySize: string | null
  serviceRequirementsIds: number[] | null
  businessHourId: number | null
  communicationsMethodId: number | null
  verificationStatus: number
  qrCode: string | null
  addedBy: number | null
  createdAt: string
  updatedAt: string
}

export interface ApiCompanyAddress {
  id: number
  modelType: string
  modelId: number
  streetName: string
  buildingNumber: string | null
  buildingType: string | null
  governerateId: number
  cityId: number
  districtId: number
  postalCode: string
  status: number
  addedBy: number | null
  createdAt: string
  updatedAt: string
}

export interface ApiCompanyContact {
  id: number
  modelType: string
  modelId: number
  primary: boolean
  name: string
  email: string | null
  phone: string | null
  position: string | null
  communicationMethodId: number | null
  addedBy?: number | null
  createdAt?: string
  updatedAt?: string
}

export interface ApiCompanyDocument {
  id: number
  documentId: number
  companyId: number
  path: string | null
  fileUrl: string | null
  expiryDate: string | null
  status: number
  addedBy: number | null
  createdAt: string
  updatedAt: string
  document: unknown | null
  company: unknown | null
}

export interface ApiCompany {
  id: number
  fullName: string
  userName: string
  email: string
  phone: string
  mfaChannel: number
  emailVerified: boolean
  phoneVerified: boolean
  status: number
  type: number
  companyId: number | null
  createdAt: string
  updatedAt: string
  companyDetail: ApiCompanyDetail
  contacts?: ApiCompanyContact[]
}

export type ApiCompanyWithDetails = ApiCompany & {
  addresses?: ApiCompanyAddress[]
  contacts?: ApiCompanyContact[]
  companyDocuments?: ApiCompanyDocument[]
}

// ============

export interface CompanyAddressFormData {
  id?: number
  streetName: string
  buildingNumber?: string | null
  buildingType?: string | null
  district: string
  city: string
  governorate: string
  postalCode: string
}

export interface CompanyUpdateFormData {
  nameEn: string
  nameAr: string
  registrationNumber: string
  taxId: string
  websiteUrl: string
  contactPersonName: string
  userName: string
  email: string
  phone: string
  street?: string
  buildingNumber?: string | null
  buildingType?: string | null
  district?: string
  city?: string
  governorate?: string
  postalCode?: string
  industryType?: string
  companySize?: string
  serviceRequirements?: string
  operatingHours?: string
  preferredCommunicationMethods?: string
  secondaryContacts?: SecondaryContact[]
  documents?: DocumentInfo[]
  addresses?: CompanyAddressFormData[]
  [key: string]: unknown
}

// ============

export function apiContactToSecondaryContactAdapter(contact: ApiCompanyContact): SecondaryContact {
  return {
    id: contact.id,
    name: contact.name,
    phone: contact.phone || '',
    email: contact.email || '',
    position: contact.position || '',
    communicationMethod: contact.communicationMethodId?.toString() || '',
  }
}

export function apiDocumentToDocumentFormDataAdapter(doc: ApiCompanyDocument): DocumentInfo {
  return {
    id: doc.id,
    documentId: doc.documentId,
    expirationDate: doc.expiryDate ? formatDateLocal(doc.expiryDate) : undefined,
    status: doc.status?.toString(),
    fileUrl: doc.fileUrl,
    path: doc.path,
  }
}

export function apiAddressToAddressFormDataAdapter(addr: ApiCompanyAddress): CompanyAddressFormData {
  return {
    id: addr.id,
    streetName: addr.streetName,
    buildingNumber: addr.buildingNumber,
    buildingType: addr.buildingType,
    district: addr.districtId?.toString(),
    city: addr.cityId?.toString(),
    governorate: addr.governerateId?.toString(),
    postalCode: addr.postalCode,
  }
}

export function companyToFormDataAdapter(company: ApiCompanyWithDetails): CompanyUpdateFormData {
  const { companyDetail, addresses, contacts, companyDocuments } = company

  const primaryAddress = addresses && addresses.length > 0
    ? addresses[0]
    : null

  const secondaryContacts = contacts
    ?.filter((contact: ApiCompanyContact) => !contact.primary)
    .map(apiContactToSecondaryContactAdapter) || []

  return {
    nameEn: companyDetail?.nameEn || '',
    nameAr: companyDetail?.nameAr || '',
    registrationNumber: companyDetail?.registrationNumber || '',
    taxId: companyDetail?.taxId || '',
    websiteUrl: companyDetail?.websiteUrl || '',
    contactPersonName: company.fullName || '',
    userName: company.userName || '',
    email: company.email || '',
    phone: company.phone || '',
    street: primaryAddress?.streetName || '',
    buildingNumber: primaryAddress?.buildingNumber || null,
    buildingType: primaryAddress?.buildingType || null,
    district: primaryAddress?.districtId?.toString() || '',
    city: primaryAddress?.cityId?.toString() || '',
    governorate: primaryAddress?.governerateId?.toString() || '',
    postalCode: primaryAddress?.postalCode || '',
    industryType: companyDetail?.industryTypeId?.toString() || '',
    companySize: companyDetail?.companySize || '',
    serviceRequirements: companyDetail?.serviceRequirementsIds?.toString() || '',
    operatingHours: companyDetail?.businessHourId?.toString() || '',
    preferredCommunicationMethods: companyDetail?.communicationsMethodId?.toString() || '',
    secondaryContacts,
    documents: companyDocuments?.map(apiDocumentToDocumentFormDataAdapter) || [],
    addresses: addresses?.map(apiAddressToAddressFormDataAdapter) || [],
  }
}
