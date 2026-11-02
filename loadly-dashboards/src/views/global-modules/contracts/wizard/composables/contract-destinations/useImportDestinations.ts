import type { ComputedRef, Ref } from 'vue'
import type { ContractWizardDestination } from '@/types/request/contract/contract-destinations.types'
import { normalizeSheetHeaderKey, useBulkDestinations } from './useBulkDestinations'

export function normalizeSheetRowKeys(row: Record<string, string>): Record<string, string> {
  return Object.fromEntries(
    Object.entries(row).map(([key, value]) => [normalizeSheetHeaderKey(key), value]),
  ) as Record<string, string>
}

function reportDuplicateFromTo(
  name: string,
  rowIndex: number,
  firstByKey: Map<string, number>,
  messageKey: 'title.importDestinationsDuplicateEnFromTo' | 'title.importDestinationsDuplicateArFromTo',
  errorMessage: Ref<string>,
  t: (key: string, values?: Record<string, string | number>) => string,
) {
  const first = firstByKey.get(name)
  if (first !== undefined) {
    errorMessage.value
      += `${t(messageKey, { row: rowIndex + 1, first: first + 1, from: name, to: name })}<br>`
  } else {
    firstByKey.set(name, rowIndex)
  }
}

export function useImportDestinations(
  requiredHeaders: ComputedRef<readonly string[] | string[]>,
  existingDestinations: Ref<ContractWizardDestination[] | undefined>,
) {
  const { t } = useI18n()
  const { validateRows: validateBulkRows, errorMessage } = useBulkDestinations()

  function validateRows(rows: Record<string, unknown>[]) {
    validateBulkRows(rows, requiredHeaders.value as string[])
  }

  function validateUniqueFromToPairs(rows: Record<string, string>[]) {
    const enFirst = new Map<string, number>()
    const arFirst = new Map<string, number>()

    for (const [i, row] of rows.entries()) {
      const nameEn = row['name-en']
      const nameAr = row['name-ar']

      reportDuplicateFromTo(
        String(nameEn),
        i,
        enFirst,
        'title.importDestinationsDuplicateEnFromTo',
        errorMessage,
        t,
      )
      reportDuplicateFromTo(
        String(nameAr),
        i,
        arFirst,
        'title.importDestinationsDuplicateArFromTo',
        errorMessage,
        t,
      )
    }
  }

  function validateImportRowsAgainstExisting(rows: Record<string, string>[]) {
    if (!existingDestinations.value?.length) return

    for (const [i, row] of rows.entries()) {
      const nameEn = row['name-en']
      const nameAr = row['name-ar']

      if (existingDestinations.value.some(d => d.nameEn === nameEn)) {
        errorMessage.value
          += `${t('title.importDestinationsConflictsWithExisting', { row: i + 1, from: nameEn, to: nameEn })}<br>`
      }
      if (existingDestinations.value.some(d => d.nameAr === nameAr)) {
        errorMessage.value
          += `${t('title.importDestinationsConflictsWithExisting', { row: i + 1, from: nameAr, to: nameAr })}<br>`
      }
    }
  }

  return {
    errorMessage,
    validateImportRowsAgainstExisting,
    validateRows,
    validateUniqueFromToPairs,
  }
}
