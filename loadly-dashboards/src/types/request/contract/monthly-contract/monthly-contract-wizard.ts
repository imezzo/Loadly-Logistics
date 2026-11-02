export interface MonthlyContractWizardRequest {
  contractSlotId: number | null
  name: string
  periodEnd: string
  periodStart: string
}

export function monthlyContractWizardRequestAdapter(
  data: MonthlyContractWizardRequest,
): MonthlyContractWizardRequest {
  return {
    contractSlotId: data.contractSlotId,
    name: data.name,
    periodEnd: data.periodEnd,
    periodStart: data.periodStart,
  }
}
