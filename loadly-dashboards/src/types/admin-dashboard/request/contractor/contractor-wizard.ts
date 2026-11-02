import { appendToFormData } from "@/utils/formData"
import { formatDateToISO } from "@/utils/date"
import type { DocumentUploaderItem } from "@/types/documents.types"

export interface ContractorCreateRequest {
  contractorStatus: boolean
  documents?: DocumentUploaderItem[]
  email: string
  fullName: string
  nationalId: string
  phone: string
  secondPhoneNumber?: string
  status: string
}

export interface ContractorUpdateRequest extends ContractorCreateRequest {
  id: string
}

export interface ApiContractorCreateRequest {
  documents: DocumentUploaderItem[]
  email: string
  fullName: string
  isActive: boolean
  nationalId: string
  phone: string
  secondPhoneNumber?: string
  status: number
}

export function contractorCreateRequestAdapter(data: ContractorCreateRequest): FormData {
  const formData = new FormData()

  formData.append('FullName', data.fullName)
  formData.append('Phone', data.phone)
  appendToFormData(formData, 'SecondPhoneNumber', data.secondPhoneNumber ?? '')
  formData.append('Email', data.email)
  formData.append('Status', data.status)
  formData.append('IsActive', Boolean(data.contractorStatus).toString())
  formData.append('NationalId', data.nationalId)

  data.documents?.filter(document => document.file || document.id)
    .forEach((document: DocumentUploaderItem, index: number) => {
      formData.append(`Documents[${index}].DocumentId`, document.documentId?.toString() ?? '')
      appendToFormData(formData, `Documents[${index}].id`, document.id?.toString() ?? '')

      if (document.file) {
        formData.append(`Documents[${index}].File`, document.file)
      }

      formData.append(`Documents[${index}].ExpiryDate`, document.expirationDate
        ? formatDateToISO(document.expirationDate ?? '')
        : '')
      formData.append(`Documents[${index}].Status`, document.status?.toString() ?? '')
  })

  return formData
}
