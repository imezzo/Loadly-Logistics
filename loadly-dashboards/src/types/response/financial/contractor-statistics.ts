import type { ComparisonDirection, ComparisonMetric, ApiComparisonMetric } from '@/types/company-dashboard/response/dashboard-home'

export interface PeriodMetric {
  allTime: number
  currentMonth: number
  vsLastMonth: ComparisonMetric
}

export interface ContractorStatistics {
  contractorId: number | null
  contractorFullName: string | null
  trips: PeriodMetric
  loadlyProfitMargin: PeriodMetric
}

export interface ApiPeriodMetric {
  allTime: number
  currentMonth: number
  vsLastMonth: ApiComparisonMetric
}

export interface ApiContractorStatisticsResponse {
  contractorId: number | null
  contractorFullName: string | null
  trips: ApiPeriodMetric
  loadlyProfileMargin: ApiPeriodMetric
}

function comparisonMetricAdapter(metric: ApiComparisonMetric): ComparisonMetric {
  return {
    current: metric.current,
    previous: metric.previous,
    delta: metric.delta,
    percentChange: metric.percentChange,
    direction: metric.direction as ComparisonDirection,
  }
}

function periodMetricAdapter(metric: ApiPeriodMetric): PeriodMetric {
  return {
    allTime: metric.allTime,
    currentMonth: metric.currentMonth,
    vsLastMonth: comparisonMetricAdapter(metric.vsLastMonth),
  }
}

export function contractorStatisticsResponseAdapter(
  data: ApiContractorStatisticsResponse,
): ContractorStatistics {
  return {
    contractorId: data.contractorId,
    contractorFullName: data.contractorFullName,
    trips: periodMetricAdapter(data.trips),
    loadlyProfitMargin: periodMetricAdapter(data.loadlyProfileMargin),
  }
}
