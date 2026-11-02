import { appendToFormData } from '@/utils/formData'
import type { VehicleType } from './contract'
import { hasValue } from '@/utils/transformers'
import type { ContractWizardDestination } from './contract-destinations.types'
import { appendContractDestinations } from './contract-destinations.mappers'
import { mapVehicleTypeToRequest } from './contract-vehicle-types.mappers'
import { appendContractSlots } from './contract-slots.mappers'
import { ContractType } from '@/types/admin-dashboard/response/contract/contract-details'
import type { ContractVehicleSlot } from '@/types/admin-dashboard/response/contract/contract-slots'
import { formatDateToISO } from '@/utils/date'

export interface UpdateContractRequest {
  autoAssignMode: boolean
  nameAr: string
  nameEn: string
  companyId: number
  cargoCommission?: number
  contractType: number
  price: number
  paymentMethod: number
  paymentDueDays: number
  billingCycle: number
  startDate: string
  endDate: string
  consumptionType: number
  tripsPerMonth?: number
  monthlyWorkingDays?: number
  maxKms?: number | null
  submitForApproval?: boolean
  termsAndConditions?: string
  salesRepId?: number
  vehicleSlots?: ContractVehicleSlot[]
  vehicleTypes: VehicleType[]
  vehiclesCount?: number
  kmOverageMode?: number
  documents?: {
    documentId: number
    file?: File
    expiryDate?: string
    status?: string
  }[]
  extraFees?: {
    extraFeeTypeId: number
    calculationOption?: number | null
    value: number
    threshold?: number
    allowOverride?: boolean
  }[]
  destinations?: ContractWizardDestination[]
}

function mapExtraFee(
  feeData: Record<string, unknown> | undefined,
  extraFeeTypeId: number
): {
  extraFeeTypeId: number
  calculationOption?: number | null
  value: number
  threshold?: number
  allowOverride?: boolean
} | null {
  if (!feeData || typeof feeData !== 'object') return null
  if (!feeData.value && !feeData.calculationOption) return null

  return {
    extraFeeTypeId,
    calculationOption: feeData.calculationOption ? Number(feeData.calculationOption) : undefined,
    value: Number(feeData.value || 0),
    threshold: feeData.threshold ? Number(feeData.threshold) : undefined,
    allowOverride: feeData.allowOverride !== undefined ? Boolean(feeData.allowOverride) : undefined,
  }
}

type ExtraFeeEntry = NonNullable<UpdateContractRequest['extraFees']>[number]

function getHelperFeesEntry(data: Record<string, unknown>): ExtraFeeEntry | null {
  const entry = mapExtraFee(data.helperFees as Record<string, unknown> | undefined, 3)
  if (!entry) return null
  entry.threshold = data.includedHelpers ? Number(data.includedHelpers) : entry.threshold

  return entry
}

function getWaitingTimeEntry(data: Record<string, unknown>): ExtraFeeEntry | null {
  if (data.waitingTimeHourlyRate === undefined) return null

  return {
    extraFeeTypeId: 4,
    calculationOption: data.billingMethod ? Number(data.billingMethod) : undefined,
    value: Number(data.waitingTimeHourlyRate || 0),
    threshold: data.freeWaitingPeriod ? Number(data.freeWaitingPeriod) : undefined,
  }
}

function getLoadingUnloadingEntry(data: Record<string, unknown>): ExtraFeeEntry | null {
  const loadingUnloadingData = data.loadingUnloading as Record<string, unknown> | undefined
  const entry = mapExtraFee(loadingUnloadingData, 5)
  if (entry) return entry
  if (loadingUnloadingData?.threshold == null) return null

  return {
    extraFeeTypeId: 5,
    value: 0,
    threshold: Number(loadingUnloadingData.threshold),
  }
}

function getPerAdditionalStopEntry(data: Record<string, unknown>): ExtraFeeEntry | null {
  if (!data.perAdditionalStopFee && !data.freeStops) return null

  return {
    extraFeeTypeId: 6,
    value: Number(data.perAdditionalStopFee || 0),
    threshold: data.freeStops ? Number(data.freeStops) : undefined,
  }
}

function buildExtraFees(data: Record<string, unknown>): UpdateContractRequest['extraFees'] {
  const extraFees: ExtraFeeEntry[] = [
    getHelperFeesEntry(data),
    mapExtraFee(data.tollGates as Record<string, unknown> | undefined, 1),
    mapExtraFee(data.weightFees as Record<string, unknown> | undefined, 2),
    getWaitingTimeEntry(data),
    getLoadingUnloadingEntry(data),
    getPerAdditionalStopEntry(data),
    mapExtraFee(data.incompleteReturn as Record<string, unknown> | undefined, 7),
    mapExtraFee(data.overNightFees as Record<string, unknown> | undefined, 10),
  ].filter((entry): entry is ExtraFeeEntry => entry != null)

  return extraFees
}

function extractDocuments(data: Record<string, unknown>): NonNullable<UpdateContractRequest['documents']> {
  const documents: NonNullable<UpdateContractRequest['documents']> = []
  if (!data._documents || !Array.isArray(data._documents)) return documents

  data._documents.forEach((doc, index) => {
    const documentInfo = doc as { documentId?: number; id?: number; expirationDate?: string; status?: string }
    const file = data[`document_${index}`] as File | undefined

    if (documentInfo.expirationDate) {
      documents.push({
        documentId: documentInfo.documentId || documentInfo.id || 0,
        file,
        expiryDate: documentInfo.expirationDate,
        status: documentInfo.status,
      })
    }
  })

  return documents
}

function extractDestinations(data: Record<string, unknown>): ContractWizardDestination[] | undefined {
  const raw = data.destinations
  if (!Array.isArray(raw) || raw.length === 0) return undefined

  return raw as ContractWizardDestination[]
}

export function transformFormDataToUpdateRequest(data: Record<string, unknown>): UpdateContractRequest {
  const extraFees = buildExtraFees(data)
  const documents = extractDocuments(data)
  const destinations = extractDestinations(data)

  return {
    autoAssignMode: Boolean(data.autoAssignMode),
    nameAr: data.nameAr as string,
    nameEn: data.nameEn as string,
    companyId: Number(data.companyId),
    contractType: Number(data.contractType),
    price: Number(data.price),
    paymentMethod: Number(data.paymentMethod),
    paymentDueDays: Number(data.paymentDueDays),
    billingCycle: Number(data.billingCycle),
    startDate: data.startDate as string,
    endDate: data.endDate as string,
    consumptionType: Number(data.consumptionType),
    ...(hasValue(data.cargoCommission as number)
      ? { cargoCommission: Number(data.cargoCommission) }
      : {}),
    tripsPerMonth: data.tripsPerMonth ? Number(data.tripsPerMonth) : undefined,
    monthlyWorkingDays: data.monthlyWorkingDays ? Number(data.monthlyWorkingDays) : undefined,
    maxKms: data.maxKms !== undefined && data.maxKms !== null ? Number(data.maxKms) : undefined,
    submitForApproval: Boolean(data.submitForApproval),
    termsAndConditions: data.termsAndConditions as string | undefined,
    salesRepId: data.salesRepId ? Number(data.salesRepId) : undefined,
    vehicleSlots: (data.vehicleSlots as ContractVehicleSlot[]) || [],
    ...(hasValue(data.vehiclesCount as number)
      ? { vehiclesCount: Number(data.vehiclesCount) }
      : {}),
    ...(hasValue(data.kmOverageMode as number)
      ? { kmOverageMode: Number(data.kmOverageMode) }
      : {}),
    vehicleTypes: (data.vehicleTypes as UpdateContractRequest['vehicleTypes']) || [],
    documents: documents && documents.length > 0 ? documents : undefined,
    extraFees: extraFees && extraFees.length > 0 ? extraFees : undefined,
    destinations,
  }
}

function appendExtraFeesToUpdateFormData(formData: FormData, data: UpdateContractRequest): void {
  if (!data.extraFees || !Array.isArray(data.extraFees)) return

  data.extraFees.forEach((extraFee, index: number) => {
    if (!extraFee ||
      !hasValue(extraFee.value)
    ) return

    formData.append(`extraFees[${index}].extraFeeTypeId`, extraFee.extraFeeTypeId.toString())
    appendToFormData(formData, `extraFees[${index}].calculationOption`, extraFee.calculationOption?.toString())
    appendToFormData(formData, `extraFees[${index}].value`, extraFee.value.toString())
    appendToFormData(formData, `extraFees[${index}].threshold`, extraFee.threshold?.toString())
    appendToFormData(formData, `extraFees[${index}].allowOverride`, extraFee.allowOverride?.toString())
  })
}

function appendDocumentsToUpdateFormData(formData: FormData, data: UpdateContractRequest): void {
  if (!data.documents || !Array.isArray(data.documents)) return

  let counter = 0

  data.documents.forEach((document) => {
    formData.append(`documents[${counter}].documentId`, document.documentId.toString())

    if (document.file) {
      formData.append(`documents[${counter}].file`, document.file)
    }

    if (document.expiryDate) {
      formData.append(`documents[${counter}].expiryDate`, formatDateToISO(document.expiryDate))
    }

    if (document.status) {
      const statusValue = typeof document.status === 'string' ? Number(document.status) : document.status
      formData.append(`documents[${counter}].status`, statusValue.toString())
    }
    counter++
  })
}

function convertDateToISOTimestamp(dateString: string, isEndDate = false): string {
  const date = new Date(dateString)
  if (isNaN(date.getTime())) {
    return ''
  }

  if (isEndDate) {
    date.setHours(23, 59, 59, 999)
  } else {
    date.setHours(0, 0, 0, 0)
  }

  return date.toISOString()
}

export function updateContractRequestAdapter(
  data: UpdateContractRequest,
  options?: {
    originalVehicleSlots?: ContractVehicleSlot[]
    originalVehicleTypes?: VehicleType[]
  },
): FormData {
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
  formData.append('startDate', convertDateToISOTimestamp(data.startDate))
  formData.append('endDate', convertDateToISOTimestamp(data.endDate, true))
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

  appendToFormData(formData, 'submitForApproval', Boolean(data.submitForApproval).toString())

  if (data.tripsPerMonth !== undefined) {
    formData.append('tripsPerMonth', data.tripsPerMonth.toString())
  }

  appendToFormData(formData, 'monthlyWorkingDays', data.monthlyWorkingDays?.toString())

  appendToFormData(formData, 'maxKms', data.maxKms?.toString())

  if (data.termsAndConditions) {
    formData.append('termsAndConditions', data.termsAndConditions)
  }

  if (data.salesRepId) {
    formData.append('salesRepId', data.salesRepId.toString())
  }

  appendToFormData(
    formData,
    'vehiclesCount',
    (hasValue(data.vehiclesCount) ? String(data.vehiclesCount) : undefined)
  )

  appendToFormData(formData, 'kmOverageMode', data.kmOverageMode?.toString())

  formData.append('autoAssignMode', String(data.autoAssignMode || false))

  data.vehicleTypes.forEach((vehicleType: VehicleType, index: number) => {
    mapVehicleTypeToRequest(formData, {
      index,
      data,
      vehicleType,
    })
  })

  if (!data.autoAssignMode) {
    appendContractSlots(formData, {
      contractType: data.contractType,
      includeId: true,
      monthlyWorkingDays: data.monthlyWorkingDays,
      originalSlots: options?.originalVehicleSlots,
      originalVehicleTypes: options?.originalVehicleTypes,
      slots: data.vehicleSlots,
      vehicleTypes: data.vehicleTypes,
    })
  }

  appendDocumentsToUpdateFormData(formData, data)
  appendExtraFeesToUpdateFormData(formData, data)
  appendContractDestinations(formData, data.destinations)

  return formData
}
