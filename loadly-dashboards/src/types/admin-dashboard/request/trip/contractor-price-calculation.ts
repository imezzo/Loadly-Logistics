export interface ContractorPriceCalculationRequest {
  actualCargoWeightTons: number | string
  contractId: number | string
  destinationId: number | string
  vehicleTypeId: number | string
}

export interface ApiContractorPriceCalculationRequest {
  actualCargoWeightTons: number
  contractId: number
  destinationId: number
  vehicleTypeId: number
}

export function contractorPriceCalculationRequestAdapter(
  data: ContractorPriceCalculationRequest,
): ApiContractorPriceCalculationRequest {
  return {
    actualCargoWeightTons: Number(data.actualCargoWeightTons),
    contractId: Number(data.contractId),
    destinationId: Number(data.destinationId),
    vehicleTypeId: Number(data.vehicleTypeId),
  }
}
