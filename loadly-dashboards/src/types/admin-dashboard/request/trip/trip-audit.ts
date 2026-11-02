import { tripChargesToApiPayload } from '@/types/admin-dashboard/request/trip/trip'
import type { ApiTripChargeUpsertPayload } from '@/types/admin-dashboard/request/trip/trip'
import type { TripWizardTripCharge } from '@/types/admin-dashboard/response/trip/trip'

export interface TripAuditRequest {
  confirmFinanceApproval: boolean
  financialDate: string
  proofOfDelivery: string
  tripCharges: TripWizardTripCharge[]
}

export interface ApiTripAuditRequest {
  confirmFinanceApproval: boolean
  financialDate: string
  proofOfDelivery: string
  tripCharges: ApiTripChargeUpsertPayload[]
}

export function tripAuditRequestAdapter(data: TripAuditRequest): ApiTripAuditRequest {
  return {
    confirmFinanceApproval: data.confirmFinanceApproval,
    financialDate: data.financialDate,
    proofOfDelivery: data.proofOfDelivery,
    tripCharges: tripChargesToApiPayload(data.tripCharges),
  }
}
