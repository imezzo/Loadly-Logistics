import { apiPaginationAdapter } from '@/types/request/pagination'
import type { ApiPagination, Pagination } from '@/types/request/pagination'
import type { DocumentInfo } from '../response/document'
import type { SecondaryContact } from '@/types/secondary-contact.types'
import { formatDateToISO } from '@/utils/date'
import { appendToFormData } from '@/utils/formData'

export interface CompaniesListRequest {
  pagination: Pagination
}

export interface CompanyCreateRequest {
  city: string
  userName: string
  companySize: string
  contactPersonName: string
  district: string
  email: string
  governorate: string
  industryType: string
  nameAr: string
  nameEn: string
  operatingHours: string
  phone: string
  postalCode: string
  preferredCommunicationMethods: string
  registrationNumber: string
  taxId: string
  serviceRequirements: string
  street: string
  websiteUrl: string
  secondaryContacts?: SecondaryContact[]
  _documents?: DocumentInfo[]
  [key: string]: unknown
}


export type ApiCompaniesListRequest = ApiPagination

export function companiesListRequestAdapter(data: CompaniesListRequest): ApiCompaniesListRequest {
  return {
    ...apiPaginationAdapter(data.pagination),
  }
}

export function appendDocumentsToFormData(
  documents: DocumentInfo[] | undefined,
  data: CompanyCreateRequest | Record<string, unknown>,
  formData: FormData
): void {
  if (!documents || !Array.isArray(documents)) {
    return
  }

  let counter = 0

  documents.forEach((doc, index) => {
    const file = data[`document_${index}`] as File | undefined
    const documentId = (doc as { documentId?: number }).documentId || doc.id

    if (documentId && file) {
      formData.append(`companyDocuments[${counter}].DocumentId`, String(documentId))
      formData.append(`companyDocuments[${counter}].File`, file)
      appendToFormData(
        formData,
        `companyDocuments[${counter}].ExpiryDate`,
        doc.expirationDate ? formatDateToISO(doc.expirationDate) : undefined
      )
      appendToFormData(formData, `companyDocuments[${counter}].Status`, doc.status)

      counter++
    }
  })
}

export function appendSecondaryContactsToFormData(
  contacts: SecondaryContact[] | undefined,
  formData: FormData,
  startIndex = 0
): void {
  if (!contacts || !Array.isArray(contacts)) {
    return
  }

  contacts.forEach((contact, index) => {
    const contactIndex = startIndex + index
    appendOptionalField(formData, `Contacts[${contactIndex}].Id`, contact.id?.toString())
    formData.append(`Contacts[${contactIndex}].Primary`, 'false')
    formData.append(`Contacts[${contactIndex}].Name`, contact.name)
    formData.append(`Contacts[${contactIndex}].Email`, contact.email)
    formData.append(`Contacts[${contactIndex}].Phone`, contact.phone)
    formData.append(`Contacts[${contactIndex}].Position`, contact.position)
    formData.append(`Contacts[${contactIndex}].CommunicationMethodId`, String(contact.communicationMethod))
  })
}

export function companyCreateRequestAdapter(data: CompanyCreateRequest): FormData {
  const formData = new FormData()

  formData.append('FullName', data.contactPersonName)
  formData.append('UserName', data.userName)
  formData.append('Email', data.email)
  formData.append('Phone', data.phone)
  formData.append('NameEn', data.nameEn)
  formData.append('NameAr', data.nameAr)
  formData.append('RegistrationNumber', data.registrationNumber)
  formData.append('TaxId', data.taxId)
  formData.append('WebsiteUrl', data.websiteUrl)
  formData.append('IndustryTypeId', data.industryType)
  formData.append('CompanySize', data.companySize)
  formData.append('ServiceRequirementsIds', data.serviceRequirements)
  formData.append('BusinessHourId', data.operatingHours)
  formData.append('CommunicationsMethodId', data.preferredCommunicationMethods)
  formData.append('VerificationStatus', '0')
  formData.append('Addresses[0].StreetName', data.street)
  formData.append('Addresses[0].DistrictId', data.district)
  formData.append('Addresses[0].CityId', data.city)
  formData.append('Addresses[0].GovernerateId', data.governorate)
  formData.append('Addresses[0].PostalCode', data.postalCode)

  appendDocumentsToFormData(data._documents, data, formData)
  appendSecondaryContactsToFormData(data.secondaryContacts, formData)

  return formData
}

export function appendOptionalField(formData: FormData, key: string, value: string | undefined): void {
  if (value) {
    formData.append(key, value)
  }
}

export function appendAddressesToFormData(
  addresses: {
    streetName?: string
    buildingNumber?: string | null
    district?: string
    city?: string
    governorate?: string
    postalCode?: string
  }[] | undefined,
  formData: FormData
): void {
  if (!addresses || !Array.isArray(addresses)) {
    return
  }

  addresses.forEach((address, index) => {
    if (address.streetName) {
      formData.append(`Addresses[${index}].StreetName`, address.streetName)
    }
    if (address.buildingNumber) {
      formData.append(`Addresses[${index}].BuildingNumber`, address.buildingNumber)
    }
    if (address.district) {
      formData.append(`Addresses[${index}].DistrictId`, address.district)
    }
    if (address.city) {
      formData.append(`Addresses[${index}].CityId`, address.city)
    }
    if (address.governorate) {
      formData.append(`Addresses[${index}].GovernerateId`, address.governorate)
    }
    if (address.postalCode) {
      formData.append(`Addresses[${index}].PostalCode`, address.postalCode)
    }
  })
}
