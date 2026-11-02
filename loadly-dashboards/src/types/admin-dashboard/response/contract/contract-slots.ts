export interface ContractVehicleSlot {
  archivedAt?: string
  assignedAt?: string
  consumptionRate?: number
  dailyValue?: number
  id?: number
  maxKms?: number
  maxPayloadTons?: number
  minBillableWeightTons?: number
  monthlyRental?: number
  overConsumption?: number
  overWeightPricePerTon?: number
  contractorOverWeightPricePerTon?: number
  plateLetters?: string
  plateNo?: string
  replacedSlotId?: number
  status?: number
  tripsPerDay?: number
  tripsPerMonth?: number
  vehicleId?: number
  vehicleTypeId: number
}

export interface ApiContractSlot {
  archivedAt?: string | null
  assignedAt?: string | null
  consumptionRate?: number | null
  dailyValue?: number | null
  id: number
  maxKms?: number | null
  maxPayloadTons?: number | null
  minBillableWeightTons?: number | null
  monthlyRental?: number | null
  overConsumption?: number | null
  overWeightPricePerTon?: number | null
  contractorOverWeightPricePerTon?: number | null
  replacedSlotId?: number | null
  vehicle?: {
    plateLetters?: string | null
    plateNo?: string | null
  }
  status?: number | null
  tripsPerDay?: number | null
  tripsPerMonth?: number | null
  vehicleId?: number | null
  vehicleTypeId: number
}

export function contractSlotAdapter(slot: ApiContractSlot): ContractVehicleSlot {
  return {
    archivedAt: slot.archivedAt ?? undefined,
    assignedAt: slot.assignedAt ?? undefined,
    consumptionRate: slot.consumptionRate ?? undefined,
    dailyValue: slot.dailyValue ?? undefined,
    id: slot.id,
    maxKms: slot.maxKms ?? undefined,
    maxPayloadTons: slot.maxPayloadTons ?? undefined,
    minBillableWeightTons: slot.minBillableWeightTons ?? undefined,
    monthlyRental: slot.monthlyRental ?? undefined,
    overConsumption: slot.overConsumption ?? undefined,
    overWeightPricePerTon: slot.overWeightPricePerTon ?? undefined,
    contractorOverWeightPricePerTon: slot.contractorOverWeightPricePerTon ?? undefined,
    plateLetters: slot.vehicle?.plateLetters ?? undefined,
    plateNo: slot.vehicle?.plateNo ?? undefined,
    replacedSlotId: slot.replacedSlotId ?? undefined,
    status: slot.status ?? undefined,
    tripsPerDay: slot.tripsPerDay ?? undefined,
    tripsPerMonth: slot.tripsPerMonth ?? undefined,
    vehicleId: slot.vehicleId ?? undefined,
    vehicleTypeId: slot.vehicleTypeId,
  }
}
