export interface ContractorPriceCalculation {
  expectedContractorPrice: string
}

interface ApiContractorPriceCalculation {
  expectedContractorPrice?: number | null
}

export function contractorPriceCalculationResponseAdapter(
  data: ApiContractorPriceCalculation,
): ContractorPriceCalculation {
  return {
    expectedContractorPrice: String(data?.expectedContractorPrice ?? ''),
  }
}
