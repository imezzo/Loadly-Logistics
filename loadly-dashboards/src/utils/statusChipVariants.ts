import { ContractDestinationStatus } from "@/types/admin-dashboard/response/contract/contract-destination.enums";
import { TripStatus } from "@/types/admin-dashboard/response/trip/trip";
import { VehicleFacilityType } from "@/types/admin-dashboard/response/vehicle-facility.enums";
import { TripDocumentStatus } from "@/types/admin-dashboard/response/trip/trip-document-status.enums";
import type { ChipVariant } from "@/types/ui";
import { ApiUserStatus } from "@/types/user";
import { ContractorPayoutStatus } from "@/types/response/financial/contractor-payout.enums";
import { MonthlyContractStatus } from "@/types/admin-dashboard/response/contract/monthly-contract/monthly-contract.enums";

export const contractDestinationStatusVariants: Record<number, ChipVariant> = {
  [ContractDestinationStatus.Active]: 'success',
  [ContractDestinationStatus.Archived]: 'neutral',
}

export const contractStatusVariants: Record<number, ChipVariant> = {
  0: 'info',
  1: 'warning',
  2: 'success',
  3: 'disabled',
  4: 'disabled',
  5: 'active',
  6: 'error',
}

export const contractApprovalStatusVariants: Record<number, ChipVariant> = {
  1: 'success',
  2: 'error',
}

export const userStatusVariants: Record<number, ChipVariant> = {
  [ApiUserStatus.ACTIVE]: 'success',
  [ApiUserStatus.SUSPENDED]: 'error',
  [ApiUserStatus.DISABLED]: 'disabled',
}

export const driverStatusVariants: Record<number, ChipVariant> = {
  0: 'neutral',
  1: 'success',
  2: 'warning',
  3: 'error',
  4: 'info',
  5: 'disabled',
}

export const vehicleStatusVariants: Record<number, ChipVariant> = {
  0: 'neutral',
  1: 'success',
  2: 'warning',
  3: 'error',
  4: 'info',
  5: 'disabled',
}

export const invoiceStatusVariants: Record<number, ChipVariant> = {
  0: 'error',
  1: 'warning',
  2: 'success',
}

export const tripStatusVariants: Record<TripStatus, ChipVariant> = {
  [TripStatus.REQUESTED]: 'success',
  [TripStatus.PENDING]: 'warning',
  [TripStatus.SCHEDULED]: 'neutral',
  [TripStatus.DRIVER_ACCEPTED]: 'success',
  [TripStatus.FIRST_PICKUP_LOADING]: 'warning',
  [TripStatus.IN_PROGRESS]: 'info',
  [TripStatus.COMPLETED]: 'active',
  [TripStatus.CANCELLED]: 'error',
  [TripStatus.REVIEWED]: 'error',
  [TripStatus.DRAFT]: 'info',
}

export const tripDocumentStatusVariants: Record<number, ChipVariant> = {
  [TripDocumentStatus.Pending]: 'neutral',
  [TripDocumentStatus.PartiallyCollected]: 'warning',
  [TripDocumentStatus.WithFactoryCollector]: 'active',
  [TripDocumentStatus.PartiallyConfirmed]: 'info',
  [TripDocumentStatus.PendingFinanceApproval]: 'warning',
  [TripDocumentStatus.Approved]: 'success',
}

export const contractorStatusVariants: Record<number, ChipVariant> = {
  0: 'active',
  1: 'success',
}

export const broadcastNotificationStatusVariants: Record<string, ChipVariant> = {
  PENDING: 'warning',
  SENT: 'success',
  FAILED: 'error',
}

export const documentStatusVariants: Record<number, ChipVariant> = {
  1: 'success',
  2: 'disabled',
}

export const facilityTypeVariants: Record<VehicleFacilityType, ChipVariant> = {
  [VehicleFacilityType.Garage]: 'success',
  [VehicleFacilityType.Agency]: 'info',
  [VehicleFacilityType.Workshop]: 'warning',
  [VehicleFacilityType.Parking]: 'active',
}

export const discountStatusVariants: Record<number, ChipVariant> = {
  0: 'error',
  1: 'success',
}

export const taxStatusVariants: Record<number, ChipVariant> = {
  0: 'error',
  1: 'success',
}

export const tripInvoicingStatusVariants: Record<number, ChipVariant> = {
  0: 'error',
  1: 'warning',
  2: 'success',
}

export const contractorPayoutStatusVariants: Record<ContractorPayoutStatus, ChipVariant> = {
  [ContractorPayoutStatus.Pending]: 'warning',
  [ContractorPayoutStatus.Paid]: 'success',
  [ContractorPayoutStatus.Cancelled]: 'error',
}

export const monthlyContractStatusVariants: Record<MonthlyContractStatus, ChipVariant> = {
  [MonthlyContractStatus.Draft]: 'neutral',
  [MonthlyContractStatus.Finalized]: 'success',
  [MonthlyContractStatus.PartiallyInvoiced]: 'warning',
}
