import type { ApiPagination, Pagination } from '@/types/response/pagination'
import { apiPaginationAdapter } from '@/types/response/pagination'
import type { MfaChannel } from '@/types/request/mfa'
import { getLocalizedValue } from '@/utils/i18n'
import type { DocumentUploaderItem } from '@/types/documents.types'
import type { ApiDocumentUploaderItem } from '@/types/documents.types'

export interface Contractor {
  commissionType: string
  commissionValue: number
  contractorStatus: string
  documents: DocumentUploaderItem[]
  email: string
  fullName: string
  id: string
  mfaChannel: MfaChannel
  nationalId: string
  phone: string
  secondPhoneNumber?: string
  status: number
  totalCompletedPaidTrips: number
  totalCompletedReadyToPayTrips: number
  totalCompletedTrips: number
  totalCompletedUnderReviewTrips: number
  totalDrivers: number
  totalTrips: number
  totalVehicles: number
  userName: string
}

export interface ContractorsListResponse {
  contractors: Contractor[]
  pagination: Pagination
}


export interface ApiContractor {
  commissionType: string
  commissionValue: number
  contractorStatus: string
  createdAt: string
  documents: ApiDocumentUploaderItem[]
  email: string
  emailVerified: boolean
  fullName: string
  id: number
  mfaChannel: MfaChannel
  nationalId: string
  phone: string
  phoneVerified: boolean
  secondPhoneNumber?: string
  status: number
  totalCompletedPaidTrips: number
  totalCompletedReadyToPayTrips: number
  totalCompletedTrips: number
  totalCompletedUnderReviewTrips: number
  totalDrivers: number
  totalTrips: number
  totalVehicles: number
  updatedAt: string
  userName: string
}

export interface ApiContractorsListResponse {
  data: ApiContractor[]
  pagination: ApiPagination
}

export function contractorDocumentAdapter(document: ApiDocumentUploaderItem): DocumentUploaderItem {
  return {
    documentId: document.documentId,
    expirationDate: document.expiryDate ?? '',
    file: undefined,
    fileUrl: document.fileUrl,
    id: document.id,
    label: getLocalizedValue(document.document.nameEn, document.document.nameAr),
    required: document.document.required,
    status: document.status.toString(),
    uploadProgress: 0,
    uploadState: 'idle',
  }
}

export function contractorAdapter(contractor: ApiContractor): Contractor {
  return {
    commissionType: contractor.commissionType,
    commissionValue: contractor.commissionValue,
    contractorStatus: contractor.contractorStatus,
    documents: contractor.documents.map(contractorDocumentAdapter),
    email: contractor.email,
    fullName: contractor.fullName,
    id: contractor.id.toString(),
    mfaChannel: contractor.mfaChannel,
    nationalId: contractor.nationalId,
    phone: contractor.phone,
    ...(contractor.secondPhoneNumber ? { secondPhoneNumber: contractor.secondPhoneNumber } : {}),
    status: contractor.status,
    totalCompletedPaidTrips: contractor.totalCompletedPaidTrips,
    totalCompletedReadyToPayTrips: contractor.totalCompletedReadyToPayTrips,
    totalCompletedTrips: contractor.totalCompletedTrips,
    totalCompletedUnderReviewTrips: contractor.totalCompletedUnderReviewTrips,
    totalDrivers: contractor.totalDrivers,
    totalTrips: contractor.totalTrips,
    totalVehicles: contractor.totalVehicles,
    userName: contractor.userName,
  }
}

export function contractorsListResponseAdapter(response: ApiContractorsListResponse): ContractorsListResponse {
  return {
    contractors: response.data.map(contractorAdapter),
    pagination: apiPaginationAdapter(response.pagination),
  }
}
