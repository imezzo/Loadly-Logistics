import type { CreateContractRequest, VehicleType } from "./contract"
import type { UpdateContractRequest } from "./contract-update"

export interface MapVehicleTypeToRequestOptions {
  index: number
  vehicleType: VehicleType
  data: CreateContractRequest | UpdateContractRequest // TODO: Remove this when we have a unified type for the data
}
