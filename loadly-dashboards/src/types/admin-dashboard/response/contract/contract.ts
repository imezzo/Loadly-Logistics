
import type { LookupResponse } from "@/types/response/lookup"
import type { Pagination } from "@/types/response/pagination"
import { formatDate } from "@/utils/date"
import { getLocalizedValue } from "@/utils/i18n"

export interface Contract {
  companyName: string
  contractType: string
  date: string
  endDate: string
  id: string
  name: string
  referenceNumber: string
  status: string
  totalCompletedTrips: number,
  totalCompletedTripsCost: number,
  totalTrips: number
  totalTripsCost: number
}

export interface ContractsListResponse {
  contracts: Contract[]
  pagination: Pagination
}

export interface Company {
  nameAr: string
  nameEn: string
}

export interface ApiContract {
  addedBy: number
  billingCycle: number
  cargoCommission?: number
  company: Company
  companyId: number
  consumptionType: number
  contractType: number
  createdAt: string
  endDate: string
  id: number
  kmOverageMode?: number
  maxKms: number | null
  monthlyWorkingDays: number
  nameAr: string
  nameEn: string
  price: number
  referenceNumber: string
  salesRepId: number
  startDate: string
  status: number
  termsAndConditions: string
  totalCompletedTrips: number
  totalCompletedTripsCost: number
  totalTrips: number
  totalTripsCost: number
  tripsPerMonth: number
  updatedAt: string
}

export interface ApiContractsListResponse {
  data: ApiContract[]
  pagination: Pagination
}

export function contractAdapter(data: ApiContract): Contract {
  return {
    companyName: getLocalizedValue(data.company.nameEn, data.company.nameAr),
    contractType: data.contractType.toString(),
    date: formatDate(data.startDate) + ' - ' + formatDate(data.endDate),
    endDate: formatDate(data.endDate),
    id: data.id.toString(),
    name: getLocalizedValue(data.nameEn, data.nameAr),
    referenceNumber: data.referenceNumber,
    status: data.status.toString(),
    totalCompletedTrips: data.totalCompletedTrips,
    totalCompletedTripsCost: data.totalCompletedTripsCost,
    totalTrips: data.totalTrips,
    totalTripsCost: data.totalTripsCost,
  }
}

export function contractsListResponseAdapter(data: ApiContractsListResponse): ContractsListResponse {
  return {
    contracts: data.data.map(contractAdapter),
    pagination: data.pagination,
  }
}

export function contractListAsLookupResponseAdapter(data: ApiContractsListResponse): LookupResponse {
  return {
    items: data.data.map(item => ({
      label: getLocalizedValue(item.nameEn, item.nameAr),
      value: item.id.toString(),
    })),
    pagination: data.pagination,
  }
}
