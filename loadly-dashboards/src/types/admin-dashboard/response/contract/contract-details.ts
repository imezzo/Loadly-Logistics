import type { ApiContract } from './contract'
import { getLocalizedValue } from '@/utils/i18n'
import type { DocumentInfo } from '../document'
import type { ContractWizardDestination, ContractDestinationPointState } from '@/types/request/contract/contract-destinations.types'
import { formatDateLocal } from '@/utils/date'
import { contractSlotAdapter } from './contract-slots'
import type { ApiContractSlot, ContractVehicleSlot } from './contract-slots'

export interface ContractsFilter {
  company: string
  contractId: string
  contractType: string
  dateRange: string
  salesRepresentative: string
  search: string
  status: string
  valueRange: string
}

export interface VehicleType {
  ExtraWorkingDayFees?: number
  OverWeightPricePerTon: number
  carCount: number
  consumptionRate: number
  contractorOverWeightPricePerTon?: number
  extraTrip: number
  id?: number
  maxKms?: number
  maxPayloadTons?: number
  minBillableWeightTons?: number
  monthlyRental: number
  name: string
  tripsPerMonth?: number
  vehicleTypeId?: number
}

export interface ExtraFees {
  calculationOption?: string
  extraFeeTypeId?: number
  notes?: string
  threshold?: number
  value?: string
}

export enum ContractType {
  Excluded = 1,
  Included,
  OnDemand,
  Seasonal,
}

export interface SalesRep {
  email: string
  fullName: string
  id: number
  userName: string
}

export interface ContractCompany {
  email: string
  fullName: string
  nameAr: string
  nameEn: string
  phone: string
  registrationNumber: string
}

export interface ContractApprovalHistory {
  actionDate: string
  approverName: string
  comments: string | null
  id: number
  stageName: string
  status: number
  statusName: string
}

export interface ContractDetails {
  _documents: DocumentInfo[]
  approvalHistory: ContractApprovalHistory[]
  autoAssignMode: boolean
  billingCycle: number
  billingMethod?: number
  cargoCommission?: number
  company?: ContractCompany
  companyId: number
  consumptionType: number
  contractType: ContractType
  destinations?: ContractWizardDestination[]
  endDate?: string
  freeStops?: number
  freeWaitingPeriod?: number
  helperFees: ExtraFees
  includedHelpers: string
  incompleteReturn: ExtraFees
  kmOverageMode?: number
  loadingUnloading: ExtraFees
  maxKms: number | null
  monthlyWorkingDays: number
  nameAr: string
  nameEn: string
  overNightFees: ExtraFees
  paymentDueDays: number
  paymentMethod: number
  perAdditionalStopFee?: string
  perAdditionalStopFeeDescription?: string
  price: number
  referenceNumber: string
  salesRep?: SalesRep
  salesRepId: number
  startDate?: string
  status: number
  termsAndConditions: string
  tollGates: ExtraFees
  tripsPerMonth: number
  vehicleSlots?: ContractVehicleSlot[]
  vehicleTypes: VehicleType[]
  vehiclesCount?: number
  waitingTimeHourlyRate?: number
  weightFees: ExtraFees
}


export interface ApiVehicleType {
  addedBy: number
  consumptionRate: number
  contractId: number
  createdAt: string
  id: number
  maxKms: number | null
  maxPayloadTons?: number | null
  minBillableWeightTons?: number | null
  minCapacity: number | null
  monthlyRental: number
  monthlyWorkingDays: number | null
  extraWorkingDayFees?: number | null
  ExtraWorkingDayFees?: number | null
  overConsumption: number
  overWeightPricePerTon: number | null
  contractorOverWeightPricePerTon?: number | null
  quantity: number
  tripsPerMonth: number | null
  updatedAt: string
  vehicleType: {
    addedBy: number | null
    category: number
    createdAt: string
    id: number
    nameAr: string
    nameEn: string
    status: number
    updatedAt: string
  }
  vehicleTypeId: number
}

export interface ApiContractDocument {
  addedBy: number
  contractId: number
  createdAt: string
  document: {
    addedBy: number | null
    createdAt: string
    entityId: number
    id: number
    nameAr: string
    nameEn: string
    required: boolean
    status: number
    updatedAt: string
  }
  documentId: number
  expiryDate: string
  fileUrl: string | null
  id: number
  path: string | null
  status: number
  updatedAt: string
}

export interface ApiExtraFee {
  addedBy: number
  calculationOption: number | null
  contractId: number
  createdAt: string
  extraFeeType: {
    addedBy: number | null
    code: string
    createdAt: string
    id: number
    nameAr: string
    nameEn: string
    status: number
    updatedAt: string
  }
  extraFeeTypeId: number
  id: number
  notes: string | null
  threshold: number | null
  updatedAt: string
  value: number
}

export interface ApiSalesRep {
  email: string
  fullName: string
  id: number
  userName: string
}

export interface ApiContractCompany {
  email: string
  fullName: string
  nameAr: string
  nameEn: string
  phone: string
  registrationNumber: string
}

export interface ApiContractDestinationPoint {
  addressName: string | null
  googlePlaceId: string | null
  id?: number
  latitude: number | null
  longitude: number | null
}

export interface ApiContractDestination {
  contractorFixedTripPrice?: number | null
  contractorPricePerTon?: number | null
  createdAt: string
  description: string | null
  end: ApiContractDestinationPoint
  expectedKms: number
  fixedTripPrice?: number | null
  id: number
  nameAr: string
  nameEn: string
  numberOfDays: number
  pricePerTon?: number | null
  returnExpectedKms?: number | null
  start: ApiContractDestinationPoint
  updatedAt: string
}

export interface ApiContractApprovalHistory {
  actionDate: string
  approverId: number
  approverName: string
  approverRole: string
  comments: string | null
  contractId: number
  createdAt: string
  id: number
  stage: number
  stageName: string
  stageOrder: number
  status: number
  statusName: string
}

export interface ApiContractDetail extends Omit<ApiContract, 'company'> {
  approvals: ApiContractApprovalHistory[]
  autoAssignMode: boolean
  company?: ApiContractCompany
  destinations?: ApiContractDestination[]
  documents: ApiContractDocument[]
  extraFees: ApiExtraFee[]
  paymentDueDays: number
  paymentMethod: number
  referenceNumber: string
  salesRep?: ApiSalesRep
  slots?: ApiContractSlot[]
  vehicleTypes: ApiVehicleType[]
  vehiclesCount?: number
}


export function extraFeesToMapAdapter(extraFees: ApiExtraFee[]): Record<number, ExtraFees> {
  const extraFeesMap: Record<number, ExtraFees> = {}
  extraFees.forEach((fee) => {
    extraFeesMap[fee.extraFeeTypeId] = {
      extraFeeTypeId: fee.extraFeeTypeId,
      calculationOption: fee.calculationOption?.toString() || undefined,
      value: fee.value?.toString() || undefined,
      threshold: fee.threshold || undefined,
      notes: fee.notes || undefined,
    }
  })

  return extraFeesMap
}

export function vehicleTypeAdapter(vehicle: ApiVehicleType): VehicleType {
  return {
    vehicleTypeId: vehicle.vehicleTypeId,
    name: getLocalizedValue(vehicle.vehicleType.nameEn, vehicle.vehicleType.nameAr),
    consumptionRate: vehicle.consumptionRate,
    extraTrip: vehicle.overConsumption,
    ExtraWorkingDayFees: vehicle.extraWorkingDayFees ?? vehicle.ExtraWorkingDayFees ?? 0,
    carCount: vehicle.quantity,
    monthlyRental: vehicle.monthlyRental,
    OverWeightPricePerTon: vehicle.overWeightPricePerTon || 0,
    tripsPerMonth: vehicle.tripsPerMonth || 0,
    maxKms: vehicle.maxKms || 0,
    ...(vehicle.minBillableWeightTons != null
      ? { minBillableWeightTons: vehicle.minBillableWeightTons }
      : {}),
    ...(vehicle.maxPayloadTons != null ? { maxPayloadTons: vehicle.maxPayloadTons } : {}),
    ...(vehicle.contractorOverWeightPricePerTon != null
      ? { contractorOverWeightPricePerTon: vehicle.contractorOverWeightPricePerTon }
      : {}),
  }
}

export function documentAdapter(document: ApiContractDocument): DocumentInfo {
  return {
    id: document.id,
    documentId: document.documentId,
    expirationDate: document.expiryDate
      ? formatDateLocal(document.expiryDate)
      : undefined,
    status: document.status?.toString(),
    fileUrl: document.fileUrl,
    path: document.path,
  }
}

export function apiDestinationPointToState(point: ApiContractDestinationPoint): ContractDestinationPointState {
  return {
    id: point?.id,
    addressName: point?.addressName ?? '',
    googlePlaceId: point?.googlePlaceId ?? '',
    latitude: point?.latitude != null ? String(point?.latitude) : '',
    longitude: point?.longitude != null ? String(point?.longitude) : '',
  }
}

export function apiDestinationToWizardRow(destination: ApiContractDestination): ContractWizardDestination {
  return {
    clientId: destination.id.toString(),
    id: destination.id,
    description: destination.description,
    start: apiDestinationPointToState(destination.start),
    end: apiDestinationPointToState(destination.end),
    nameAr: destination.nameAr,
    nameEn: destination.nameEn,
    createdAt: destination.createdAt,
    updatedAt: destination.updatedAt,
    expectedKms: destination.expectedKms,
    ...(destination.returnExpectedKms != null ? { returnExpectedKms: destination.returnExpectedKms } : {}),
    numberOfDays: destination.numberOfDays,
    ...(destination.pricePerTon != null ? { pricePerTon: destination.pricePerTon } : {}),
    ...(destination.fixedTripPrice != null ? { fixedTripPrice: destination.fixedTripPrice } : {}),
    ...(destination.contractorPricePerTon != null ? { contractorPricePerTon: destination.contractorPricePerTon } : {}),
    ...(destination.contractorFixedTripPrice != null ? { contractorFixedTripPrice: destination.contractorFixedTripPrice } : {}),
  }
}

export function approvalHistoryAdapter(statusHistory: ApiContractApprovalHistory): ContractApprovalHistory {
  const { comments, stageName, actionDate, approverName, id, statusName, status } = statusHistory

  return {
    comments,
    stageName,
    actionDate: formatDateLocal(actionDate),
    approverName,
    id,
    status,
    statusName,
  }
}

export function contractDetailResponseAdapter(data: ApiContractDetail): ContractDetails {
  const extraFees = extraFeesToMapAdapter(data.extraFees)

  const waitingTimeFee = data.extraFees.find((fee) => fee.extraFeeTypeId === 4)
  const billingMethod = waitingTimeFee?.calculationOption ?? undefined
  const freeWaitingPeriod = waitingTimeFee?.threshold ?? undefined
  const multiStopFee = data.extraFees.find((fee) => fee.extraFeeTypeId === 6)
  const freeStops = multiStopFee?.threshold ?? undefined
  const perAdditionalStopFee = multiStopFee?.value.toString()
  const perAdditionalStopFeeDescription = multiStopFee?.notes ?? undefined
  const waitingTimeHourlyRate = waitingTimeFee?.value ?? undefined

  return {
    _documents: data.documents.map(documentAdapter),
    autoAssignMode: data.autoAssignMode,
    approvalHistory: data.approvals.map(approvalHistoryAdapter),
    billingCycle: data.billingCycle,
    billingMethod,
    company: data.company,
    companyId: data.companyId,
    consumptionType: data.consumptionType,
    contractType: data.contractType,
    ...(data.cargoCommission != null
      ? { cargoCommission: data.cargoCommission * 100 }
      : {}),
    destinations: data.destinations?.map(apiDestinationToWizardRow),
    endDate: formatDateLocal(data.endDate),
    freeStops,
    freeWaitingPeriod,
    helperFees: extraFees[3] || {},
    includedHelpers: extraFees[3]?.threshold?.toString() || '',
    incompleteReturn: extraFees[7] || {},
    loadingUnloading: extraFees[5] || {},
    kmOverageMode: data.kmOverageMode,
    maxKms: data.maxKms,
    monthlyWorkingDays: data.monthlyWorkingDays,
    nameAr: data.nameAr,
    nameEn: data.nameEn,
    overNightFees: extraFees[10] || {},
    paymentDueDays: data.paymentDueDays,
    paymentMethod: data.paymentMethod,
    perAdditionalStopFee,
    perAdditionalStopFeeDescription,
    price: data.price,
    referenceNumber: data.referenceNumber,
    salesRep: data.salesRep,
    salesRepId: data.salesRep?.id ?? data.salesRepId,
    startDate: formatDateLocal(data.startDate),
    status: data.status,
    termsAndConditions: data.termsAndConditions || '',
    tollGates: extraFees[1] || {},
    tripsPerMonth: data.tripsPerMonth,
    vehicleSlots: (data.slots ?? []).map(contractSlotAdapter),
    vehicleTypes: data.vehicleTypes.map((vehicleType) => vehicleTypeAdapter(vehicleType)),
    vehiclesCount: data.vehiclesCount,
    waitingTimeHourlyRate,
    weightFees: extraFees[2] || {},
  }
}
