export interface ContractDestinationsCalculateRequest {
  contractType: number
  monthly_working_days: number
  destinations: {
    kms: number
    numberOfDays: number
    name_ar: string
    name_en: string
  }[]
  vehicle_types: {
    vehicleTypeId: number
    consumptionRate: number
    monthlyRental: number
    tripsPerMonth: number | null
  }[]
}

export interface ContractDestinationsCalculatedRow {
  kms: number
  numberOfDays: number
  name_ar: string
  name_en: string
  monthlyRental: number
  dailyValue: number
  fuelPrice: number
  totalTripCost: number
}

export interface ContractDestinationsCalculatedVehicleType {
  vehicleTypeId: number
  vehicleTypeNameAr: string
  vehicleTypeNameEn: string
  destinations: ContractDestinationsCalculatedRow[]
}
