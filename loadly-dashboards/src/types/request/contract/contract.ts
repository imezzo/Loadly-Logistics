import type { ApiPagination, Pagination } from "@/types/request/pagination"
import { apiPaginationAdapter } from "@/types/request/pagination"
import { formatDateToISO } from "@/utils/date"
import type { DocumentInfo } from "../../admin-dashboard/response/document"
import { appendToFormData } from "@/utils/formData"
import { hasValue } from "@/utils/transformers"
import type { ContractWizardDestination } from "./contract-destinations.types"
import { appendContractDestinations } from "./contract-destinations.mappers"
import { mapVehicleTypeToRequest } from "./contract-vehicle-types.mappers"
import { appendContractSlots } from "./contract-slots.mappers"
import { ContractType } from "@/types/admin-dashboard/response/contract/contract-details"
import type { ContractVehicleSlot } from "@/types/admin-dashboard/response/contract/contract-slots"
export interface ContractsListRequest extends Pagination {
  companyId?: string
  contractId?: string
  contractType?: string
  salesRepId?: string
  search?: string
  status?: string
}

export interface ExtraFees {
  allowOverride?: boolean
  calculationOption?: string | null
  extraFeeTypeId?: number
  threshold?: number
  value?: string
}

export interface VehicleType {
  ExtraWorkingDayFees?: number
  carCount: number
  consumptionRate: number
  extraTrip: number
  name: string
  tripsPerMonth?: number
  vehicleTypeId: number
  OverWeightPricePerTon: number
  contractorOverWeightPricePerTon?: number
  monthlyRental: number
  maxKms?: number
  minBillableWeightTons?: number
  maxPayloadTons?: number
}

export interface CreateContractRequest {
  RenewedFromContractId?: string
  _documents: DocumentInfo[]
  autoAssignMode: boolean
  billingCycle: number
  billingMethod: number
  cargoCommission?: number
  companyId: number
  consumptionType: number
  contractType: number
  destinations?: ContractWizardDestination[]
  endDate: string
  freeStops: number
  freeWaitingPeriod: number
  helperFees: ExtraFees
  includedHelpers: string
  incompleteReturn: ExtraFees
  loadingUnloading: ExtraFees
  maxKms: number | null
  monthlyWorkingDays: number
  nameAr: string
  nameEn: string
  overNightFees: ExtraFees
  paymentDueDays: number
  paymentForExtraTripFees: string
  paymentMethod: number
  perAdditionalStopFee: string
  perAdditionalStopFeeDescription: string
  price: number
  salesRepId: number
  startDate: string
  submitForApproval?: boolean
  termsAndConditions: string
  tollGates: ExtraFees
  tripsPerMonth: number
  vehicleSlots?: ContractVehicleSlot[]
  vehicleTypes: VehicleType[]
  vehiclesCount?: number
  kmOverageMode: number
  waitingTimeHourlyRate: number
  weightFees: ExtraFees
  [key: `document_${number}`]: File
}

export interface ApiContractsListRequest extends ApiPagination {
  companyId?: string
  contractId?: string
  contractType?: string
  salesRepId?: string
  search?: string
  status?: string
}

export function contractsListRequestAdapter(data: ContractsListRequest): ApiContractsListRequest {
  return {
    ...apiPaginationAdapter({ page: data.page, size: data.size }),
    ...(data.companyId ? { companyId: data.companyId } : {}),
    ...(data.contractId ? { contractId: data.contractId } : {}),
    ...(data.contractType ? { contractType: data.contractType } : {}),
    ...(data.salesRepId ? { salesRepId: data.salesRepId } : {}),
    ...(data.search ? { search: data.search } : {}),
    ...(data.status ? { status: data.status } : {}),
  }
}

function appendExtraFeesToFormData(formData: FormData, data: CreateContractRequest): void {
  const extraFees: ExtraFees[] = [
    data.tollGates,
    data.weightFees,
    {
      extraFeeTypeId: 10,
      ...data.overNightFees
    },
    {
      ...data.helperFees,
      threshold: +data.includedHelpers,
    },
    {
      value: data.waitingTimeHourlyRate?.toString(),
      threshold: data.freeWaitingPeriod ? Number(data.freeWaitingPeriod) : undefined,
      calculationOption: data.billingMethod?.toString(),
    },
    data.loadingUnloading,
    {
      threshold: data.freeStops,
      value: data.perAdditionalStopFee,
      calculationOption: null,
    },
    data.incompleteReturn,
  ]

  extraFees.forEach((extraFee: ExtraFees, index: number) => {
    if (!extraFee) return

    if (hasValue(extraFee.calculationOption) || hasValue(extraFee.value)) {
      formData.append(`extraFees[${index}].extraFeeTypeId`, (
        extraFee.extraFeeTypeId?.toString() ?? (index + 1).toString()
      ))
      appendToFormData(formData, `extraFees[${index}].value`, extraFee.value)
      appendToFormData(formData, `extraFees[${index}].calculationOption`, extraFee.calculationOption?.toString())
      appendToFormData(formData, `extraFees[${index}].threshold`, extraFee.threshold?.toString())
      appendToFormData(formData, `extraFees[${index}].allowOverride`, extraFee.allowOverride?.toString())
    }
  })
}

function appendDocumentsToFormData(formData: FormData, data: CreateContractRequest): void {
  if (!data._documents
    || !Array.isArray(data._documents)) return

  let counter = 0

  data._documents
    .map((document: DocumentInfo, index: number) => ({ document, index }))
    .forEach(({ document, index }) => {
      const file = data[`document_${index}`] as File
      if (!file) return
      appendToFormData(formData, `documents[${counter}].documentId`, document.documentId?.toString())
      formData.append(`documents[${counter}].file`, file)

      if (document.expirationDate) {
        formData.append(`documents[${counter}].expiryDate`, formatDateToISO(document.expirationDate))
      }

      if (document.status) {
        const statusValue = typeof document.status === 'string' ? Number(document.status) : document.status
        formData.append(`documents[${counter}].status`, statusValue.toString())
      }
      counter++
    })
}

export function createContractRequestAdapter(data: CreateContractRequest): FormData {
  const consumptionType = [
    ContractType.Excluded,
    ContractType.Seasonal,
  ].includes(Number(data.contractType)) ? 1 : 2

  const formData = new FormData()
  formData.append('nameAr', data.nameAr)
  formData.append('nameEn', data.nameEn)
  formData.append('companyId', data.companyId.toString())
  formData.append('contractType', data.contractType.toString())
  formData.append('price', data.price.toString())
  formData.append('paymentMethod', data.paymentMethod.toString())
  formData.append('paymentDueDays', data.paymentDueDays.toString())
  formData.append('billingCycle', data.billingCycle.toString())
  formData.append('startDate', formatDateToISO(data.startDate))
  formData.append('endDate', formatDateToISO(data.endDate))
  appendToFormData(formData, 'consumptionType',
    Number(data.contractType) === ContractType.OnDemand
    ? data.consumptionType?.toString()
    : consumptionType.toString())

  if (data.cargoCommission) {
    appendToFormData(
      formData,
      'cargoCommission',
      data.cargoCommission != null ? String(Number(data.cargoCommission) / 100) : undefined,
    )
  }

  appendToFormData(formData, 'tripsPerMonth', data.tripsPerMonth?.toString())
  appendToFormData(formData, 'monthlyWorkingDays', data.monthlyWorkingDays?.toString())
  appendToFormData(formData, 'maxKms', data.maxKms?.toString())
  formData.append('submitForApproval', Boolean(data.submitForApproval).toString())
  formData.append('termsAndConditions', data.termsAndConditions)
  formData.append('salesRepId', data.salesRepId.toString())

  appendToFormData(
    formData,
    'vehiclesCount',
    (hasValue(data.vehiclesCount) ? String(data.vehiclesCount) : undefined)
  )

  appendToFormData(formData, 'kmOverageMode', data.kmOverageMode?.toString())

  appendToFormData(formData, 'RenewedFromContractId', data.RenewedFromContractId)

  appendExtraFeesToFormData(formData, data)
  appendDocumentsToFormData(formData, data)

  formData.append('autoAssignMode', String(data.autoAssignMode || false))

  data.vehicleTypes.forEach((vehicleType: VehicleType, index: number) => {
    mapVehicleTypeToRequest(formData, {
      index,
      vehicleType,
      data,
    })

    if (data.autoAssignMode) return
  })

  if (!data.autoAssignMode) {
    appendContractSlots(formData, {
      contractType: data.contractType,
      monthlyWorkingDays: data.monthlyWorkingDays,
      slots: data.vehicleSlots,
      vehicleTypes: data.vehicleTypes,
    })
  }

  appendContractDestinations(formData, data.destinations)

  return formData
}
