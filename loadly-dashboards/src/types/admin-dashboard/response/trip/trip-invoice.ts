import { getLocalizedValue } from "@/utils/i18n"

export interface InvoiceItem {
  chargeType: number
  chargeTypeName: string
  lineCount: number
  totalAmount: number
  totalBillable: number
  totalCompanyShare: number,
}

export interface Invoice {
  items: InvoiceItem[],
  lineCount: number,
  totalAmount: number,
  totalBillable: number,
  totalCompanyShare: number,
}

export interface TripInvoiceSummary {
  allChargesInvoiceTypeSet: boolean,
  // chargesCount: number,
  // chargesWithInvoiceTypeSet: number,
  claimInvoice: Invoice,
  claimInvoiceIds: number[],
  grandTotalBillable: number,
  isRoutingConsistent: boolean,
  // routingConflicts: number[],
  tripInvoice: Invoice,
  tripInvoiceIds: number[]
}

export interface ApiInvoiceItem {
  chargeType: number
  chargeTypeName: string
  chargeTypeNameAr: string
  chargeTypeNameEn: string
  lineCount: number
  totalAmount: number
  totalBillable: number
  totalCompanyShare: number
}

export interface ApiInvoice {
  items: ApiInvoiceItem[],
  lineCount: number,
  totalAmount: number,
  totalBillable: number,
  totalCompanyShare: number,
}

export interface ApiTripInvoiceSummary {
  allChargesInvoiceTypeSet: boolean,
  chargesCount: number,
  chargesWithInvoiceTypeSet: number,
  claimInvoice: ApiInvoice,
  claimInvoiceIds: number[],
  grandTotalBillable: number,
  isRoutingConsistent: boolean,
  routingConflicts: number[],
  tripInvoice: ApiInvoice,
  tripInvoiceIds: number[]
}

export function invoiceItemResponseAdapter(response: ApiInvoiceItem): InvoiceItem {
  return {
    chargeType: response.chargeType,
    chargeTypeName: response.chargeTypeName
      ?? getLocalizedValue(response.chargeTypeNameEn, response.chargeTypeNameAr),
    lineCount: response.lineCount,
    totalAmount: response.totalAmount,
    totalBillable: response.totalBillable,
    totalCompanyShare: response.totalCompanyShare,
  }
}

export function tripInvoiceSummaryResponseAdapter(response: ApiTripInvoiceSummary): TripInvoiceSummary {
  return {
    allChargesInvoiceTypeSet: response.allChargesInvoiceTypeSet,
    claimInvoice: {
      items: response.claimInvoice.items.map(invoiceItemResponseAdapter),
      lineCount: response.claimInvoice.lineCount,
      totalAmount: response.claimInvoice.totalAmount,
      totalBillable: response.claimInvoice.totalBillable,
      totalCompanyShare: response.claimInvoice.totalCompanyShare,
    },
    claimInvoiceIds: response.claimInvoiceIds,
    grandTotalBillable: response.grandTotalBillable,
    isRoutingConsistent: response.isRoutingConsistent,
    tripInvoice: {
      items: response.tripInvoice.items.map(invoiceItemResponseAdapter),
      lineCount: response.tripInvoice.lineCount,
      totalAmount: response.tripInvoice.totalAmount,
      totalBillable: response.tripInvoice.totalBillable,
      totalCompanyShare: response.tripInvoice.totalCompanyShare,
    },
    tripInvoiceIds: response.tripInvoiceIds,
  }
}
