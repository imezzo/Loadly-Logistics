export interface UpdateCommercialTermsRequest {
  effectiveEnd: string
  effectiveStart: string
  extraWorkingDayFees?: number
  monthlyRental?: number
  overConsumption?: number | null
  reason: string
  seatRootSlotIds: number[]
}

export function updateCommercialTermsRequestAdapter(
  data: UpdateCommercialTermsRequest,
): UpdateCommercialTermsRequest {
  return {
    effectiveEnd: data.effectiveEnd,
    effectiveStart: data.effectiveStart,
    ...(data.extraWorkingDayFees ? { extraWorkingDayFees: data.extraWorkingDayFees } : {}),
    ...(data.monthlyRental ? { monthlyRental: data.monthlyRental } : {}),
    ...(data.overConsumption ? { overConsumption: data.overConsumption } : {}),
    reason: data.reason,
    seatRootSlotIds: data.seatRootSlotIds,
  }
}
