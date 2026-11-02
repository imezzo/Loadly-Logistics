export interface CalculateSlotChargesRequest {
  periodStart: string
  periodEnd: string
}

export function calculateSlotChargesRequestAdapter(
  data: CalculateSlotChargesRequest,
): { periodStart: string, periodEnd: string } {
  return {
    periodStart: data.periodStart,
    periodEnd: data.periodEnd,
  }
}
