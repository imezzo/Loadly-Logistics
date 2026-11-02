import type { CompanyAddressFormData } from '@/types/admin-dashboard/response/company-detail'
import type { SecondaryContact } from '@/types/secondary-contact.types'
import {
  appendOptionalField,
  appendSecondaryContactsToFormData,
  appendDocumentsToFormData,
} from './company'
import type { DocumentInfo } from '../response/document'

export interface CompanyUpdateAddress {
  id?: number
  streetName?: string
  buildingNumber?: string | null
  district?: string
  city?: string
  governorate?: string
  postalCode?: string
}

export interface CompanyUpdateRequest {
  city?: string
  userName: string
  companySize?: string
  contactPersonName: string
  district?: string
  email: string
  governorate?: string
  industryType?: string
  nameAr?: string
  nameEn?: string
  operatingHours?: string
  phone: string
  postalCode?: string
  preferredCommunicationMethods?: string
  primaryAddressId?: number
  registrationNumber?: string
  taxId?: string
  serviceRequirements?: string
  street?: string
  buildingNumber?: string | null
  websiteUrl?: string
  verificationStatus?: string | number
  secondaryContacts?: SecondaryContact[]
  addresses?: CompanyUpdateAddress[]
  _documents?: {
    id: number
    documentId?: number
    expirationDate?: string
    status?: string
  }[]
  [key: string]: unknown
}

function appendPrimaryAddressToFormData(data: CompanyUpdateRequest, formData: FormData): void {
  appendOptionalField(formData, 'Addresses[0].Id', data.primaryAddressId?.toString())
  appendOptionalField(formData, 'Addresses[0].StreetName', data.street)
  appendOptionalField(formData, 'Addresses[0].BuildingNumber', data.buildingNumber || undefined)
  appendOptionalField(formData, 'Addresses[0].DistrictId', data.district)
  appendOptionalField(formData, 'Addresses[0].CityId', data.city)
  appendOptionalField(formData, 'Addresses[0].GovernerateId', data.governorate)
  appendOptionalField(formData, 'Addresses[0].PostalCode', data.postalCode)
}

function appendAdditionalAddressesToFormData(
  addresses: CompanyUpdateAddress[] | undefined,
  formData: FormData
): void {
  if (!addresses || addresses.length === 0) {
    return
  }

  addresses.forEach((address, index) => {
    const addressIndex = index + 1
    appendOptionalField(formData, `Addresses[${addressIndex}].Id`, address.id?.toString())
    appendOptionalField(formData, `Addresses[${addressIndex}].StreetName`, address.streetName)
    appendOptionalField(formData, `Addresses[${addressIndex}].BuildingNumber`, address.buildingNumber || undefined)
    appendOptionalField(formData, `Addresses[${addressIndex}].DistrictId`, address.district)
    appendOptionalField(formData, `Addresses[${addressIndex}].CityId`, address.city)
    appendOptionalField(formData, `Addresses[${addressIndex}].GovernerateId`, address.governorate)
    appendOptionalField(formData, `Addresses[${addressIndex}].PostalCode`, address.postalCode)
  })
}

function appendBasicCompanyFields(data: CompanyUpdateRequest, formData: FormData): void {
  formData.append('FullName', data.contactPersonName)
  formData.append('UserName', data.userName)
  formData.append('Email', data.email)
  formData.append('Phone', data.phone)

  appendOptionalField(formData, 'NameEn', data.nameEn)
  appendOptionalField(formData, 'NameAr', data.nameAr)
  appendOptionalField(formData, 'RegistrationNumber', data.registrationNumber)
  appendOptionalField(formData, 'TaxId', data.taxId)
  appendOptionalField(formData, 'WebsiteUrl', data.websiteUrl)
  appendOptionalField(formData, 'IndustryTypeId', data.industryType)
  appendOptionalField(formData, 'CompanySize', data.companySize)
  appendOptionalField(formData, 'ServiceRequirementsIds', data.serviceRequirements)
  appendOptionalField(formData, 'BusinessHourId', data.operatingHours)
  appendOptionalField(formData, 'CommunicationsMethodId', data.preferredCommunicationMethods)

  formData.append('VerificationStatus', String(data.verificationStatus))
}

function appendPrimaryContactToFormData(data: CompanyUpdateRequest, formData: FormData): void {
  formData.append('Contacts[0].Primary', 'true')
  formData.append('Contacts[0].Name', data.contactPersonName)
  formData.append('Contacts[0].Email', data.email)
}

function transformAddresses(
  addresses: CompanyAddressFormData[]
): CompanyUpdateAddress[] {
  return addresses.slice(1).map((addr) => ({
    id: addr.id,
    streetName: addr.streetName,
    buildingNumber: addr.buildingNumber,
    district: addr.district,
    city: addr.city,
    governorate: addr.governorate,
    postalCode: addr.postalCode,
  }))
}

function hasDocumentFile(doc: { id: number; documentId?: number }, documents: DocumentInfo[], formData: Record<string, unknown>): boolean {
  if (doc.documentId) {
    return true
  }

  const docIndex = documents.findIndex((d: DocumentInfo) => d.id === doc.id)

  return docIndex >= 0 && !!formData[`document_${docIndex}`]
}

function transformDocuments(
  documents: DocumentInfo[],
  formData: Record<string, unknown>
): {
  id: number
  documentId?: number
  expirationDate?: string
  status?: string
}[] {
  return documents.map((doc) => ({
    id: doc.id || 0,
    documentId: doc.documentId,
    expirationDate: doc.expirationDate,
    status: doc.status,
  })).filter((doc) => hasDocumentFile(doc, documents, formData))
}

function addDocumentFiles(
  documents: DocumentInfo[],
  formData: Record<string, unknown>,
  result: CompanyUpdateRequest & Record<string, File | unknown>
): void {
  documents.forEach((doc, index) => {
    const file = formData[`document_${index}`] as File | undefined

    if (file) {
      result[`document_${index}`] = file
    }
  })
}

function buildBasicFields(formData: Record<string, unknown>): Partial<CompanyUpdateRequest> {
  return {
    userName: String(formData.userName || ''),
    contactPersonName: String(formData.contactPersonName || ''),
    email: String(formData.email || ''),
    phone: String(formData.phone || ''),
    nameEn: formData.nameEn ? String(formData.nameEn) : undefined,
    nameAr: formData.nameAr ? String(formData.nameAr) : undefined,
    registrationNumber: formData.registrationNumber ? String(formData.registrationNumber) : undefined,
    taxId: formData.taxId ? String(formData.taxId) : undefined,
    websiteUrl: formData.websiteUrl ? String(formData.websiteUrl) : undefined,
    industryType: formData.industryType ? String(formData.industryType) : undefined,
    companySize: formData.companySize ? String(formData.companySize) : undefined,
    serviceRequirements: formData.serviceRequirements ? String(formData.serviceRequirements) : undefined,
    operatingHours: formData.operatingHours ? String(formData.operatingHours) : undefined,
    preferredCommunicationMethods: formData.preferredCommunicationMethods ? String(formData.preferredCommunicationMethods) : undefined,
    verificationStatus: String(formData.verificationStatus),
  }
}

function transformMultipleLocations(
  locations: {
    id?: number
    city?: string
    governorate?: string
    district?: string
    street?: string
    postalCode?: string
  }[]
): CompanyUpdateAddress[] {
  return locations.map((location) => ({
    id: location.id,
    streetName: location.street,
    buildingNumber: undefined,
    district: location.district,
    city: location.city,
    governorate: location.governorate,
    postalCode: location.postalCode,
  }))
}

function buildAddressFields(formData: Record<string, unknown>): Partial<CompanyUpdateRequest> {
  let addresses: CompanyUpdateAddress[] | undefined

  if (Array.isArray(formData.multipleLocations) && formData.multipleLocations.length > 0) {
    addresses = transformMultipleLocations(formData.multipleLocations as {
      id?: number
      city?: string
      governorate?: string
      district?: string
      street?: string
      postalCode?: string
    }[])
  }

  else if (Array.isArray(formData.addresses) && (formData.addresses as CompanyAddressFormData[]).length > 1) {
    addresses = transformAddresses(formData.addresses as CompanyAddressFormData[])
  }

  const addressesList = formData.addresses as CompanyAddressFormData[] | undefined
  const primaryAddressId = addressesList?.[0]?.id

  return {
    street: formData.street ? String(formData.street) : undefined,
    buildingNumber: formData.buildingNumber ? String(formData.buildingNumber) : undefined,
    district: formData.district ? String(formData.district) : undefined,
    city: formData.city ? String(formData.city) : undefined,
    governorate: formData.governorate ? String(formData.governorate) : undefined,
    postalCode: formData.postalCode ? String(formData.postalCode) : undefined,
    primaryAddressId,
    addresses,
  }
}

function buildDocumentFields(
  formData: Record<string, unknown>
): Partial<CompanyUpdateRequest> {
  const documentsArray = Array.isArray(formData.documents) ? formData.documents as DocumentInfo[] : null

  return {
    _documents: documentsArray ? transformDocuments(documentsArray, formData) : undefined,
  }
}

export function formDataToCompanyUpdateRequestAdapter(
  formData: Record<string, unknown>
): CompanyUpdateRequest & Record<string, File | unknown> {
  const result: CompanyUpdateRequest & Record<string, File | unknown> = {
    ...buildBasicFields(formData),
    ...buildAddressFields(formData),
    secondaryContacts: Array.isArray(formData.secondaryContacts) ? formData.secondaryContacts as CompanyUpdateRequest['secondaryContacts'] : undefined,
    ...buildDocumentFields(formData),
  } as CompanyUpdateRequest & Record<string, File | unknown>

  if (Array.isArray(formData.documents)) {
    addDocumentFiles(formData.documents as DocumentInfo[], formData, result)
  }

  return result
}

export function companyUpdateRequestAdapter(data: CompanyUpdateRequest): FormData {
  const formData = new FormData()

  appendBasicCompanyFields(data, formData)
  appendPrimaryAddressToFormData(data, formData)
  appendAdditionalAddressesToFormData(data.addresses, formData)

  appendPrimaryContactToFormData(data, formData)
  appendSecondaryContactsToFormData(data.secondaryContacts, formData, 1)
  appendDocumentsToFormData(data._documents, data, formData)

  return formData
}
