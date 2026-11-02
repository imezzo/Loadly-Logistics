import { formatDateToISO } from "@/utils/date"
import { appendToFormData } from "@/utils/formData"
import { toNumber } from "@/utils/transformers"

export interface DriverWizardRequest {
  cityId?: string
  contractorId?: number | null
  dateOfBirth?: string
  districtId?: string
  documents?: DriverDocumentRequest[]
  email: string
  emergencyContactName?: string
  emergencyContactPhone?: string
  fullNameAr: string
  fullNameEn: string
  governerateId?: string
  id?: string
  spokenLanguages?: string
  licenseCategory?: string
  licenseExpirationDate?: string
  licenseIssueDate?: string
  licenseNo?: string
  nationalId: string
  phone: string
  postalCode?: string
  status: number
  streetName?: string
  userName: string
}

export interface DriverDocumentRequest {
  documentId: number
  expiryDate?: string
  file?: File
  status?: number
}

export function driverWizardInformationAdapter(values: Record<string, unknown>) {
  return {
    cityId: values.cityId?.toString(),
    contractorId: values.hasContractor
      ? toNumber(values.contractorId) ?? null
      : null,
    dateOfBirth: values.dateOfBirth?.toString(),
    districtId: values.districtId?.toString(),
    email: values.email?.toString() || '',
    emergencyContactName: values.emergencyContactName?.toString(),
    emergencyContactPhone: values.emergencyContactPhone?.toString(),
    fullNameAr: values.fullNameAr?.toString() || '',
    fullNameEn: values.fullNameEn?.toString() || '',
    governerateId: values.governerateId?.toString(),
    spokenLanguages: Array.isArray(values.spokenLanguages)
      ? values.spokenLanguages.join(',')
      : values.spokenLanguages?.toString(),
    licenseCategory: values.licenseCategory?.toString(),
    licenseExpirationDate: values.licenseExpirationDate?.toString(),
    licenseIssueDate: values.licenseIssueDate?.toString(),
    licenseNo: values.licenseNo?.toString(),
    nationalId: values.nationalId?.toString() || '',
    phone: values.phone?.toString() || '',
    postalCode: values.postalCode?.toString(),
    status: 1,
    streetName: values.streetName?.toString(),
    userName: values.userName?.toString() || '',
  }
}

export function driverDocumentRequestAdapter(
  formData: FormData,
  doc: DriverDocumentRequest,
  index: number
): void {
  if (!doc.file) return

  appendToFormData(
    formData,
    `Documents[${index}].ExpiryDate`,
    doc.expiryDate ? formatDateToISO(doc.expiryDate) : undefined,
  )
  formData.append(`Documents[${index}].DocumentId`, String(doc.documentId))
  formData.append(`Documents[${index}].File`, doc.file)
  appendToFormData(formData, `Documents[${index}].Status`, doc.status?.toString())
}

export function driverWizardRequestAdapter(data: DriverWizardRequest): FormData {
  const formData = new FormData()

  formData.append('Email', data.email)
  formData.append('FullNameAr', data.fullNameAr)
  formData.append('FullNameEn', data.fullNameEn)
  formData.append('NationalId', data.nationalId)
  formData.append('Phone', data.phone)
  formData.append('Status', String(data.status))
  formData.append('UserName', data.userName)
  appendToFormData(formData, 'CityId', data.cityId?.toString())
  appendToFormData(formData, 'ContractorId', data.contractorId?.toString())
  appendToFormData(formData, 'DateOfBirth', formatDateToISO(data.dateOfBirth ?? ''))
  appendToFormData(formData, 'DistrictId', data.districtId?.toString())
  appendToFormData(formData, 'EmergencyContactName', data.emergencyContactName)
  appendToFormData(formData, 'EmergencyContactPhone', data.emergencyContactPhone)
  appendToFormData(formData, 'GovernerateId', data.governerateId?.toString())
  appendToFormData(formData, 'SpokenLanguages', data.spokenLanguages)
  appendToFormData(formData, 'LicenseCategory', data.licenseCategory)
  appendToFormData(formData, 'LicenseExpirationDate', formatDateToISO(data.licenseExpirationDate ?? ''))
  appendToFormData(formData, 'LicenseIssueDate', formatDateToISO(data.licenseIssueDate ?? ''))
  appendToFormData(formData, 'LicenseNo', data.licenseNo)
  appendToFormData(formData, 'PostalCode', data.postalCode)
  appendToFormData(formData, 'StreetName', data.streetName)

  data.documents?.filter(doc => doc.file).forEach((doc, index) => {
    driverDocumentRequestAdapter(formData, doc, index)
  })

  return formData
}
