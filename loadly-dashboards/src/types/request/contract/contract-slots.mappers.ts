import { ContractType } from '@/types/admin-dashboard/response/contract/contract-details'
import { ContractVehicleSlotStatus } from '@/types/admin-dashboard/response/contract/contract-slots.enums'
import type { ContractVehicleSlot } from '@/types/admin-dashboard/response/contract/contract-slots'
import type { VehicleType } from './contract'
import { appendToFormData } from '@/utils/formData'
import { hasTruthyValue } from '@/utils/transformers'

export interface AppendContractSlotsOptions {
  contractType?: number | null
  includeId?: boolean
  monthlyWorkingDays?: number | null
  originalSlots?: ContractVehicleSlot[]
  originalVehicleTypes?: VehicleType[]
  slots?: ContractVehicleSlot[]
  vehicleTypes: VehicleType[]
}

export interface ComputeSlotDailyValueOptions {
  contractType?: number | null
  monthlyRental?: number
  monthlyWorkingDays?: number | null
  tripsPerMonth?: number | null
}

const VEHICLE_TYPE_COMPARISON_KEYS = [
  'ExtraWorkingDayFees',
  'OverWeightPricePerTon',
  'carCount',
  'consumptionRate',
  'contractorOverWeightPricePerTon',
  'extraTrip',
  'maxKms',
  'maxPayloadTons',
  'minBillableWeightTons',
  'monthlyRental',
  'tripsPerMonth',
] as const satisfies readonly (keyof VehicleType)[]

function divide(numerator?: number | null, denominator?: number | null): number | undefined {
  if (numerator == null || !denominator) return undefined

  return Number((numerator / denominator).toFixed(2))
}

export function computeSlotDailyValue(options: ComputeSlotDailyValueOptions): number | undefined {
  const { monthlyRental, monthlyWorkingDays } = options

  return divide(monthlyRental, monthlyWorkingDays)
}

export function computeSlotTripsPerDay(
  tripsPerMonth?: number | null,
  monthlyWorkingDays?: number | null,
): number | undefined {
  return divide(tripsPerMonth, monthlyWorkingDays)
}

function normalizeMetric(value: unknown): number | null {
  if (value == null || value === '') return null

  const numericValue = Number(value)

  return Number.isFinite(numericValue) ? numericValue : null
}

function isVehicleTypeDataChanged(
  currentType: VehicleType,
  originalType: VehicleType,
): boolean {
  return VEHICLE_TYPE_COMPARISON_KEYS.some((key) =>
    currentType[key] != null
    && originalType[key] != null
    && normalizeMetric(currentType[key]) !== normalizeMetric(originalType[key])
  )
}

function getChangedVehicleTypeIds(
  vehicleTypes: VehicleType[] = [],
  originalVehicleTypes: VehicleType[] = [],
): Set<number> {
  const originalById = new Map(
    originalVehicleTypes
      .filter((type): type is VehicleType & { vehicleTypeId: number } => type.vehicleTypeId != null)
      .map((type) => [type.vehicleTypeId, type]),
  )
  const changedIds = new Set<number>()

  vehicleTypes.forEach((currentType) => {
    if (currentType.vehicleTypeId == null) return

    const originalType = originalById.get(currentType.vehicleTypeId)
    if (!originalType) return

    if (isVehicleTypeDataChanged(currentType, originalType)) {
      changedIds.add(currentType.vehicleTypeId)
    }
  })

  return changedIds
}

function getVehicleTypeMetrics(vehicleType?: VehicleType): Partial<ContractVehicleSlot> {
  if (!vehicleType) return {}

  return {
    consumptionRate: vehicleType.consumptionRate,
    dailyValue: undefined,
    maxKms: vehicleType.maxKms,
    maxPayloadTons: vehicleType.maxPayloadTons,
    minBillableWeightTons: vehicleType.minBillableWeightTons,
    monthlyRental: vehicleType.monthlyRental,
    overConsumption: vehicleType.extraTrip,
    overWeightPricePerTon: vehicleType.OverWeightPricePerTon,
    contractorOverWeightPricePerTon: vehicleType.contractorOverWeightPricePerTon,
    tripsPerMonth: vehicleType.tripsPerMonth,
  }
}

function expandReplacedSlots(
  slots: ContractVehicleSlot[],
  options: AppendContractSlotsOptions,
): ContractVehicleSlot[] {
  const { originalSlots = [], originalVehicleTypes = [], vehicleTypes } = options
  const originalById = new Map(
    originalSlots
      .filter((slot): slot is ContractVehicleSlot & { id: number } => slot.id != null)
      .map((slot) => [slot.id, slot]),
  )
  const originalTypeById = new Map(
    originalVehicleTypes
      .filter((type): type is VehicleType & { vehicleTypeId: number } => type.vehicleTypeId != null)
      .map((type) => [type.vehicleTypeId, type]),
  )
  const changedVehicleTypeIds = getChangedVehicleTypeIds(vehicleTypes, originalVehicleTypes)
  const nowUtc = new Date(new Date().setSeconds(0, 0)).toISOString()
  const expandedSlots: ContractVehicleSlot[] = []

  slots.forEach((slot) => {
    const originalSlot = slot.id != null ? originalById.get(slot.id) : undefined
    const isVehicleReplaced = Boolean(
      originalSlot?.vehicleId
      && slot.vehicleId
      && originalSlot.vehicleId !== slot.vehicleId,
    )
    const isVehicleTypeDataChanged = Boolean(
      slot.vehicleTypeId
      && changedVehicleTypeIds.has(slot.vehicleTypeId),
    )

    const computedDailyValue = computeSlotDailyValue({
      contractType: Number(options.contractType),
      monthlyRental: slot.monthlyRental,
      monthlyWorkingDays: options.monthlyWorkingDays,
      tripsPerMonth: slot.tripsPerMonth,
    })

    const isDailyValueChanged = computedDailyValue !== Number((originalSlot?.dailyValue ?? 0).toFixed(2))
    if (
      ((!isVehicleReplaced && !isVehicleTypeDataChanged && !isDailyValueChanged)
      || !originalSlot)
    ) {
      expandedSlots.push(slot)

      return
    }

    const originalVehicleType = originalSlot.vehicleTypeId != null
      ? originalTypeById.get(originalSlot.vehicleTypeId)
      : undefined

    expandedSlots.push({
      ...originalSlot,
      ...getVehicleTypeMetrics(originalVehicleType),
      archivedAt: nowUtc,
      status: ContractVehicleSlotStatus.Archived,
    })
    expandedSlots.push({
      ...slot,
      archivedAt: undefined,
      assignedAt: nowUtc,
      id: undefined,
      replacedSlotId: originalSlot.id,
      status: ContractVehicleSlotStatus.Active,
    })
  })

  return expandedSlots
}

function resolveSlotMetric(
  slotValue: number | undefined,
  vehicleTypeValue: number | undefined,
): number | undefined {
  return vehicleTypeValue ?? slotValue
}

function appendSlotFields(
  formData: FormData,
  slot: ContractVehicleSlot,
  index: number,
  options: AppendContractSlotsOptions,
): void {
  const { contractType, includeId, monthlyWorkingDays, originalVehicleTypes = [], vehicleTypes } = options
  const typeSources = slot.status === ContractVehicleSlotStatus.Archived && slot.archivedAt
    ? originalVehicleTypes
    : vehicleTypes
  const vehicleType = typeSources.find((item) => item.vehicleTypeId === slot.vehicleTypeId)
  const prefix = `slots[${index}]`

  if (includeId) {
    appendToFormData(formData, `${prefix}.id`, slot.id?.toString())
  }

  appendToFormData(formData, `${prefix}.vehicleTypeId`, slot.vehicleTypeId?.toString())
  appendToFormData(formData, `${prefix}.vehicleId`, slot.vehicleId?.toString())
  appendToFormData(formData, `${prefix}.assignedAt`, slot.assignedAt)
  appendToFormData(formData, `${prefix}.archivedAt`, slot.archivedAt)
  appendToFormData(formData, `${prefix}.replacedSlotId`, slot.replacedSlotId?.toString())
  appendToFormData(formData, `${prefix}.status`, (slot.status ?? ContractVehicleSlotStatus.Active).toString())

  const consumptionRate = resolveSlotMetric(slot.consumptionRate, vehicleType?.consumptionRate)
  const tripsPerMonth = resolveSlotMetric(slot.tripsPerMonth, vehicleType?.tripsPerMonth)
  const maxKms = resolveSlotMetric(slot.maxKms, vehicleType?.maxKms)
  const monthlyRental = resolveSlotMetric(slot.monthlyRental, vehicleType?.monthlyRental)
  const overConsumption = resolveSlotMetric(slot.overConsumption, vehicleType?.extraTrip)
  const minBillableWeightTons = resolveSlotMetric(slot.minBillableWeightTons, vehicleType?.minBillableWeightTons)
  const maxPayloadTons = resolveSlotMetric(slot.maxPayloadTons, vehicleType?.maxPayloadTons)
  const overWeightPricePerTon = resolveSlotMetric(
    slot.overWeightPricePerTon,
    vehicleType?.OverWeightPricePerTon,
  )
  const contractorOverWeightPricePerTon = resolveSlotMetric(
    slot.contractorOverWeightPricePerTon,
    vehicleType?.contractorOverWeightPricePerTon,
  )

  appendToFormData(formData, `${prefix}.consumptionRate`, hasTruthyValue(consumptionRate)?.toString())
  appendToFormData(formData, `${prefix}.tripsPerMonth`, hasTruthyValue(tripsPerMonth)?.toString())

  const tripsPerDay = resolveSlotMetric(
    slot.tripsPerDay,
    computeSlotTripsPerDay(tripsPerMonth, monthlyWorkingDays),
  )
  appendToFormData(
    formData,
    `${prefix}.tripsPerDay`,
    hasTruthyValue(tripsPerDay)?.toString()
  )

  appendToFormData(formData, `${prefix}.maxKms`, hasTruthyValue(maxKms)?.toString())
  appendToFormData(formData, `${prefix}.monthlyRental`, hasTruthyValue(monthlyRental)?.toString())

  const dailyValue = resolveSlotMetric(
    slot.dailyValue,
    computeSlotDailyValue({
      contractType,
      monthlyRental,
      monthlyWorkingDays,
      tripsPerMonth,
    }),
  )
  appendToFormData(
    formData,
    `${prefix}.dailyValue`,
    hasTruthyValue(dailyValue)?.toString()
  )

  appendToFormData(formData, `${prefix}.overConsumption`, hasTruthyValue(overConsumption)?.toString())
  appendToFormData(formData, `${prefix}.minBillableWeightTons`, hasTruthyValue(minBillableWeightTons)?.toString())
  appendToFormData(formData, `${prefix}.maxPayloadTons`, hasTruthyValue(maxPayloadTons)?.toString())
  appendToFormData(
    formData,
    `${prefix}.overWeightPricePerTon`,
    hasTruthyValue(overWeightPricePerTon)?.toString(),
  )
  appendToFormData(
    formData,
    `${prefix}.contractorOverWeightPricePerTon`,
    hasTruthyValue(contractorOverWeightPricePerTon)?.toString(),
  )

  if (Number(contractType) === ContractType.Included
  || Number(contractType) === ContractType.Excluded) {
    appendToFormData(
      formData,
      `${prefix}.ExtraWorkingDayFees`,
      hasTruthyValue(vehicleType?.ExtraWorkingDayFees)?.toString(),
    )
  }
}

export function appendContractSlots(formData: FormData, options: AppendContractSlotsOptions): void {
  const { slots } = options
  if (!slots?.length) return

  const readySlots = slots.filter((slot) => slot.vehicleTypeId && slot.vehicleId)
  if (!readySlots.length) return

  const payloadSlots = expandReplacedSlots(readySlots, options)

  payloadSlots.forEach((slot, index) => {
    appendSlotFields(formData, slot, index, options)
  })
}
