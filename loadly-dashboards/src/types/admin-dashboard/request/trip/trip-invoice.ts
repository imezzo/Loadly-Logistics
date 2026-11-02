export interface UpdateTripChargesInvoiceTypeRequest {
  tripIds: number[]
  charges: {
    chargeType: number
    invoiceType: number
  }[]
}

export interface ApiUpdateTripChargesInvoiceTypeRequest {
  tripIds: number[]
  charges: {
    chargeType: number
    invoiceType: number
  }[]
}

export function updateTripChargesInvoiceTypeRequestAdapter(
  data: UpdateTripChargesInvoiceTypeRequest,
): ApiUpdateTripChargesInvoiceTypeRequest {
  return {
    tripIds: data.tripIds,
    charges: data.charges.map(charge => ({
      chargeType: charge.chargeType,
      invoiceType: charge.invoiceType,
    })),
  }
}
