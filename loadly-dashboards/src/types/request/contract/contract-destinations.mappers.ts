import { hasValue } from "@/utils/transformers"
import type { ContractDestinationPointState, ContractWizardDestination } from "./contract-destinations.types"
import { appendToFormData } from "@/utils/formData"

function appendDestinationPoint(
  formData: FormData,
  path: string,
  point: ContractDestinationPointState,
): void {
  if (hasValue(point.id)) {
    formData.append(`${path}.Id`, String(point.id))
  }

  formData.append(`${path}.AddressName`, point.addressName ?? '')
  formData.append(`${path}.GooglePlaceId`, point.googlePlaceId ?? '')
  formData.append(`${path}.Latitude`, point.latitude ?? '')
  formData.append(`${path}.Longitude`, point.longitude ?? '')
}

export function appendContractDestinations(
  formData: FormData,
  destinations?: ContractWizardDestination[] | null,
): void {
  if (!destinations?.length) return

  destinations.forEach((dest, i) => {
    const path = `Destinations[${i}]`

    if (dest.start) {
      appendDestinationPoint(formData, `${path}.Start`, dest.start)
    }
    if (dest.end) {
      appendDestinationPoint(formData, `${path}.End`, dest.end)
    }

    formData.append(`${path}.NameAr`, dest.nameAr ?? '')
    formData.append(`${path}.NameEn`, dest.nameEn ?? '')
    if (dest.expectedKms) {
      formData.append(`${path}.ExpectedKms`, String(dest.expectedKms))
    }
    appendToFormData(formData, `${path}.ReturnExpectedKms`, dest.returnExpectedKms?.toString())
    if (dest.numberOfDays) {
      formData.append(`${path}.NumberOfDays`, String(dest.numberOfDays))
    }
    formData.append(`${path}.PricePerTon`, dest.pricePerTon != null ? String(dest.pricePerTon) : '')
    formData.append(`${path}.FixedTripPrice`, dest.fixedTripPrice != null ? String(dest.fixedTripPrice) : '')
    appendToFormData(formData, `${path}.ContractorPricePerTon`, dest.contractorPricePerTon?.toString())
    appendToFormData(formData, `${path}.ContractorFixedTripPrice`, dest.contractorFixedTripPrice?.toString())
  })
}
