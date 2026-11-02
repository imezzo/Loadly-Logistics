import type { ComputedRef } from 'vue'
import type { TripBulkCreateItem } from '@/types/admin-dashboard/request/trip/bulk-import'
import {
  BULK_TRIPS_ERROR_MESSAGES,
  normalizeSheetHeaderKey,
  useBulkTrips,
} from './useBulkTrips'

export type BulkImportTripRow = Record<string, string | number>

export function normalizeSheetRowKeys(row: Record<string, unknown>): BulkImportTripRow {
  return Object.fromEntries(
    Object.entries(row).map(([key, value]) => [normalizeSheetHeaderKey(key), value]),
  ) as BulkImportTripRow
}

export function mapRowsToTrips(row: BulkImportTripRow): TripBulkCreateItem {
  return {
    startDate: String(row['start-date'] ?? ''),
    endDate: String(row['end-date'] ?? ''),
    from: String(row['from'] ?? ''),
    to: String(row['to'] ?? ''),
    distanceKms: Number(row['distance-kms']),
    contactName: String(row['contact-name'] ?? ''),
    contactMobile: String(row['contact-mobile'] ?? ''),
    helpersCount: Number(row['helpers-count']),
  }
}

export function useImportTrips(
  requiredHeaders: ComputedRef<readonly string[] | string[]>,
) {
  const { validateRows: validateBulkRows, errorMessage } = useBulkTrips()

  function validateRows(rows: Record<string, unknown>[]) {
    if (!rows.length) {
      errorMessage.value = BULK_TRIPS_ERROR_MESSAGES.noRows

      return
    }

    validateBulkRows(rows, requiredHeaders.value as string[])
  }

  return {
    errorMessage,
    validateRows,
  }
}
