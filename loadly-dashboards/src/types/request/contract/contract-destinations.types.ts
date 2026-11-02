export interface ContractDestinationPointState {
  addressName: string
  googlePlaceId: string
  id?: number
  latitude: string
  longitude: string
}

export interface ContractWizardDestination {
  clientId: string
  contractorFixedTripPrice?: number
  contractorPricePerTon?: number
  createdAt?: string
  description?: string | null
  end?: ContractDestinationPointState
  expectedKms?: number
  fixedTripPrice?: number
  id?: number
  nameAr: string
  nameEn: string
  numberOfDays?: number
  pricePerTon?: number
  returnExpectedKms?: number
  start?: ContractDestinationPointState
  updatedAt?: string
}
