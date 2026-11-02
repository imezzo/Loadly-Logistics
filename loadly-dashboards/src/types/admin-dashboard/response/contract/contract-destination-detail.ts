export interface ContractDestinationDetailPoint {
  id: number
  googlePlaceId: string | null
  latitude: string | number | null
  longitude: string | number | null
  addressName: string | null
}

export interface ContractDestinationDetail {
  contractId: number
  contractorFixedTripPrice?: number | null
  contractorPricePerTon?: number | null
  id: number
  start: ContractDestinationDetailPoint
  end: ContractDestinationDetailPoint
  nameAr: string
  nameEn: string
  expectedKms: number
  returnExpectedKms: number | null
  numberOfDays: number
  description: string | null
  fixedTripPrice?: number | null
  pricePerTon?: number | null
}

export function contractDestinationDetailResponseAdapter(
  payload: unknown,
): ContractDestinationDetail | null {
  if (payload == null || typeof payload !== 'object') return null

  return payload as ContractDestinationDetail
}
