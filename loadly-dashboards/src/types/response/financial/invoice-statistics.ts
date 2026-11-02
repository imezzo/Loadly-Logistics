import { formatValueWithCurrency } from "@/utils/currency"

export interface InvoicesStatisticsResponse {
  paidInvoicesCount: number
  unpaidInvoicesCount: number
  partiallyPaidInvoicesCount: number
  totalInvoicesAmount: string
  totalPaidAmount: string
  totalRemainingAmount: string
}

export interface ApiInvoicesStatisticsResponse {
  paidInvoicesCount: number
  unpaidInvoicesCount: number
  partiallyPaidInvoicesCount: number
  totalInvoicesAmount: number
  totalPaidAmount: number
  totalRemainingAmount: number
}

export function invoicesStatisticsResponseAdapter(
  data: ApiInvoicesStatisticsResponse,
): InvoicesStatisticsResponse {
  return {
    paidInvoicesCount: data.paidInvoicesCount,
    unpaidInvoicesCount: data.unpaidInvoicesCount,
    partiallyPaidInvoicesCount: data.partiallyPaidInvoicesCount,
    totalInvoicesAmount: formatValueWithCurrency(+data.totalInvoicesAmount),
    totalPaidAmount: formatValueWithCurrency(+data.totalPaidAmount),
    totalRemainingAmount: formatValueWithCurrency(+data.totalRemainingAmount),
  }
}
