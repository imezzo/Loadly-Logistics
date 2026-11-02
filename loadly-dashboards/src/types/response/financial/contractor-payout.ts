import type { ApiPagination } from '@/types/response/pagination'
import type { Pagination } from '@/types/response/pagination'
import { apiPaginationAdapter } from '@/types/response/pagination'
import type { ContractorPayoutStatus } from './contractor-payout.enums'

export interface ApiContractorPayoutTrip {
  tripId: number
  referenceNumber: string
  contractorPrice: number
}

export interface ApiContractorPayoutContractor {
  id: number
  fullName: string
  userName: string
  email: string
  phone: string
  address: string | null
}

export interface ApiContractorPayout {
  addedBy: number
  cancelledAt: string | null
  cancelledBy: number | null
  cancellationReason: string | null
  contractor: ApiContractorPayoutContractor
  contractorId: number
  createdAt: string
  id: number
  payoutNumber: string
  sequence: number
  status: ContractorPayoutStatus
  statusName: string
  loadlyCommissionAmount: number
  totalAmount: number
  totalContractorPriceAmount: number
  totalTripCostAmount: number
  tripCount: number
  trips: ApiContractorPayoutTrip[]
}

export interface ContractorPayoutTrip {
  tripId: number
  referenceNumber: string
  contractorPrice: number
}

export interface ContractorPayoutContractor {
  id: number
  fullName: string
  email: string
  phone: string
  address: string | null
}

export interface ContractorPayout {
  id: number
  payoutNumber: string
  status: ContractorPayoutStatus
  statusName: string
  totalContractorPriceAmount: number
  totalTripCostAmount: number
  tripCount: number
  loadlyCommissionAmount: number
}

export interface ContractorPayoutDetail {
  id: number
  payoutNumber: string
  status: ContractorPayoutStatus
  statusName: string
  totalAmount: number
  contractor: ContractorPayoutContractor
  trips: ContractorPayoutTrip[]
}

export interface ApiContractorPayoutsListResponse {
  data: ApiContractorPayout[]
  pagination: ApiPagination
}

export interface ContractorPayoutsListResponse {
  data: ContractorPayout[]
  pagination: Pagination
}

export function contractorPayoutAdapter(data: ApiContractorPayout): ContractorPayout {
  return {
    id: data.id,
    payoutNumber: data.payoutNumber,
    status: data.status,
    statusName: data.statusName,
    loadlyCommissionAmount: data.loadlyCommissionAmount,
    totalContractorPriceAmount: data.totalContractorPriceAmount,
    totalTripCostAmount: data.totalTripCostAmount,
    tripCount: data.tripCount,
  }
}

export function contractorPayoutDetailAdapter(data: ApiContractorPayout): ContractorPayoutDetail {
  return {
    id: data.id,
    payoutNumber: data.payoutNumber,
    status: data.status,
    statusName: data.statusName,
    totalAmount: data.totalAmount,
    contractor: {
      id: data.contractor.id,
      fullName: data.contractor.fullName,
      email: data.contractor.email,
      phone: data.contractor.phone,
      address: data.contractor.address,
    },
    trips: (data.trips ?? []).map(trip => ({
      tripId: trip.tripId,
      referenceNumber: trip.referenceNumber,
      contractorPrice: trip.contractorPrice,
    })),
  }
}

export function contractorPayoutsListResponseAdapter(
  data: ApiContractorPayoutsListResponse,
): ContractorPayoutsListResponse {
  return {
    data: (data.data ?? []).map(contractorPayoutAdapter),
    pagination: apiPaginationAdapter(data.pagination),
  }
}
