import { ContractType } from "@/types/admin-dashboard/response/contract/contract-details";
import type { MapVehicleTypeToRequestOptions } from "./contract-vehicle-types.types";
import { appendToFormData } from "@/utils/formData";
import { ConsumptionType } from "./consumption-types.enums";

function appendToFormDataWithCondition(formData: FormData, key: string, value?: string | number): void {
  if (value) {
    appendToFormData(formData, key, value.toString())
  }
}

function createOnDemandVehicleType(formData: FormData, options: MapVehicleTypeToRequestOptions): void {
  const { index, data, vehicleType } = options

  if (!data.autoAssignMode) {
    appendToFormDataWithCondition(formData, `vehicleTypes[${index}].quantity`, vehicleType.carCount)
  }

  switch (Number(data.consumptionType)) {
    case ConsumptionType.Kilometers:
      appendToFormDataWithCondition(formData, `vehicleTypes[${index}].overConsumption`, vehicleType.extraTrip)
      appendToFormDataWithCondition(formData, `vehicleTypes[${index}].consumptionRate`, vehicleType.consumptionRate)
      appendToFormDataWithCondition(formData, `vehicleTypes[${index}].monthlyRental`, vehicleType.monthlyRental)
      break
    case ConsumptionType.CargoWeight:
      appendToFormDataWithCondition(formData, `vehicleTypes[${index}].OverWeightPricePerTon`, vehicleType.OverWeightPricePerTon)
      appendToFormDataWithCondition(formData, `vehicleTypes[${index}].contractorOverWeightPricePerTon`, vehicleType.contractorOverWeightPricePerTon)
      appendToFormDataWithCondition(formData, `vehicleTypes[${index}].minBillableWeightTons`, vehicleType.minBillableWeightTons)
      appendToFormDataWithCondition(formData, `vehicleTypes[${index}].maxPayloadTons`, vehicleType.maxPayloadTons)
    // case ConsumptionType.CargoPriceConsumption:
      break
    // case ConsumptionType.DestinationFixedPrice:
    //   break
  }
}

function createMainVehicleType(formData: FormData, options: MapVehicleTypeToRequestOptions): void {
  const { index, vehicleType } = options

  appendToFormDataWithCondition(formData, `vehicleTypes[${index}].quantity`, vehicleType.carCount)
  appendToFormDataWithCondition(formData, `vehicleTypes[${index}].overConsumption`, vehicleType.extraTrip)
  appendToFormDataWithCondition(formData, `vehicleTypes[${index}].monthlyRental`, vehicleType.monthlyRental)
}

export function mapVehicleTypeToRequest(formData: FormData, options: MapVehicleTypeToRequestOptions): void {
  const { index, data, vehicleType } = options

  appendToFormData(formData, `vehicleTypes[${index}].vehicleTypeId`, vehicleType.vehicleTypeId.toString())


  switch (Number(data.contractType)) {
    case ContractType.OnDemand:
      createOnDemandVehicleType(formData, options)
      break
    case ContractType.Included:
       createMainVehicleType(formData, options)
       appendToFormDataWithCondition(formData, `vehicleTypes[${index}].maxKms`, vehicleType.maxKms)
       break
    case ContractType.Excluded:
    case ContractType.Seasonal:
      appendToFormDataWithCondition(formData, `vehicleTypes[${index}].tripsPerMonth`, vehicleType.tripsPerMonth)
      appendToFormDataWithCondition(formData, `vehicleTypes[${index}].consumptionRate`, vehicleType.consumptionRate)
      createMainVehicleType(formData, options)
      break
  }

  if (Number(data.contractType) === ContractType.Included
    || Number(data.contractType) === ContractType.Excluded) {
    appendToFormDataWithCondition(
      formData,
      `vehicleTypes[${index}].ExtraWorkingDayFees`,
      vehicleType.ExtraWorkingDayFees,
    )
  }
}
