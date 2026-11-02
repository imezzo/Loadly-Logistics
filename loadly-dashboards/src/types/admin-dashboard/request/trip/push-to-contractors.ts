export interface PushToContractorsRequest {
  contractorPrice: number
  sendToAllContractors: boolean
  spotPrice?: number | null
  vehicleTypeId: number
}

export interface ApiPushToContractorsRequest {
  contractorPrice: number
  sendToAllContractors: boolean
  spotPrice?: number | null
  vehicleTypeId: number
}

export function pushToContractorsRequestAdapter(
  data: PushToContractorsRequest,
): ApiPushToContractorsRequest {
  return {
    contractorPrice: data.contractorPrice,
    sendToAllContractors: data.sendToAllContractors,
    vehicleTypeId: data.vehicleTypeId,
    ...(data.spotPrice != null ? { spotPrice: data.spotPrice } : {}),
  }
}
