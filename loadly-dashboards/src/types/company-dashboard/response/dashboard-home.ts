import { getLocalizedValue } from '@/utils/i18n'

export type ComparisonDirection = 'up' | 'down' | 'flat'

export interface ApiComparisonMetric {
  current: number
  previous: number
  delta: number
  percentChange: number
  direction: ComparisonDirection
}

export interface ApiSpendingByVehicleType {
  vehicleTypeId: number
  vehicleTypeNameEn: string
  vehicleTypeNameAr: string
  amount: number
  sharePercent: number
  vsLastWeek: ApiComparisonMetric
}

export interface ApiDashboardHomeData {
  totalTrips: {
    total: number
    vsYesterday: ApiComparisonMetric
  }
  totalSpending: {
    amount: number
    vsYesterday: ApiComparisonMetric
  }
  totalKm: {
    amount: number
    vsYesterday: ApiComparisonMetric
  }
  onTimeDelivery: {
    rate: number
    vsYesterday: ApiComparisonMetric
  }
  spendingByVehicleType: ApiSpendingByVehicleType[]
  invoiceSummary: {
    pendingInvoices: number
    paidThisMonth: number
  }
}

export interface ComparisonMetric {
  current: number
  previous: number
  delta: number
  percentChange: number
  direction: ComparisonDirection
}

export interface SpendingByVehicleType {
  vehicleTypeId: number
  name: string
  amount: number
  sharePercent: number
  vsLastWeek: ComparisonMetric
}

export interface DashboardHome {
  totalTrips: {
    total: number
    vsYesterday: ComparisonMetric
  }
  totalSpending: {
    amount: number
    vsYesterday: ComparisonMetric
  }
  totalKm: {
    amount: number
    vsYesterday: ComparisonMetric
  }
  onTimeDelivery: {
    rate: number
    vsYesterday: ComparisonMetric
  }
  spendingByVehicleType: SpendingByVehicleType[]
  invoiceSummary: {
    pendingInvoices: number
    paidThisMonth: number
  }
}

function comparisonMetricAdapter(metric: ApiComparisonMetric): ComparisonMetric {
  return {
    current: metric.current,
    previous: metric.previous,
    delta: metric.delta,
    percentChange: metric.percentChange,
    direction: metric.direction,
  }
}

export function dashboardHomeResponseAdapter(data: ApiDashboardHomeData): DashboardHome {
  return {
    totalTrips: {
      total: data.totalTrips.total,
      vsYesterday: comparisonMetricAdapter(data.totalTrips.vsYesterday),
    },
    totalSpending: {
      amount: data.totalSpending.amount,
      vsYesterday: comparisonMetricAdapter(data.totalSpending.vsYesterday),
    },
    totalKm: {
      amount: data.totalKm.amount,
      vsYesterday: comparisonMetricAdapter(data.totalKm.vsYesterday),
    },
    onTimeDelivery: {
      rate: data.onTimeDelivery.rate,
      vsYesterday: comparisonMetricAdapter(data.onTimeDelivery.vsYesterday),
    },
    spendingByVehicleType: data.spendingByVehicleType.map((item) => ({
      vehicleTypeId: item.vehicleTypeId,
      name: getLocalizedValue(item.vehicleTypeNameEn, item.vehicleTypeNameAr),
      amount: item.amount,
      sharePercent: item.sharePercent,
      vsLastWeek: comparisonMetricAdapter(item.vsLastWeek),
    })),
    invoiceSummary: {
      pendingInvoices: data.invoiceSummary.pendingInvoices,
      paidThisMonth: data.invoiceSummary.paidThisMonth,
    },
  }
}
