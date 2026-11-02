export const EXPECTED_DESTINATION_SHEET_HEADERS = [
  'name-en',
  'name-ar',
  'address-from',
  'address-to',
  'number-of-days',
  'kms',
  'return-expected-kms',
  'fixed-destination-price',
  'contractor-fixed-destination-price',
  'price-per-ton',
  'contractor-price-per-ton',
  'id',
] as const

export const BULK_DESTINATIONS_ERROR_MESSAGES = {
  noHeaders: `No headers, or missing headers in the sheet. Expected headers: (${EXPECTED_DESTINATION_SHEET_HEADERS.join(', ')}).`,
  invalidHeader: 'Invalid header: {header}. Expected headers: ({expected}). regardless of case and whitespace.',
  invalidRowCellValue: 'Invalid row cell value. for row index: (#{index}) for the column: ({header}).<br>',
  duplicateFromTo: 'Duplicate from/to: row {row} matches row {first} ({from} → {to}).<br>',
  conflictsWithExisting: 'Row {row} matches a destination that is already in this list ({from} → {to}).<br>',
}

export function normalizeSheetHeaderKey(header: string) {
  return header.trim().toLowerCase().replace(/ /g, '-').replace(/_/g, '-')
}

export function useBulkDestinations() {
  const expected = EXPECTED_DESTINATION_SHEET_HEADERS

  const errorMessage = ref('')

  function validateHeaders(headers: string[]) {
    if (headers.length < expected.length) {
      errorMessage.value = BULK_DESTINATIONS_ERROR_MESSAGES.noHeaders

      return
    }

    for (const header of headers) {
      const normalizedHeader = normalizeSheetHeaderKey(header)
      if (!(expected as readonly string[]).includes(normalizedHeader)) {
        errorMessage.value
          = BULK_DESTINATIONS_ERROR_MESSAGES.invalidHeader.replace('{header}', normalizedHeader).replace('{expected}', expected.join(', '))

        return
      }
    }

    errorMessage.value = ''
  }

  const POSITIVE_REQUIRED_HEADERS = new Set(['number-of-days', 'kms'])

  function isInvalidRequiredValue(header: string, value: unknown): boolean {
    if (value === null || value === undefined || value === '') return true
    if (!POSITIVE_REQUIRED_HEADERS.has(header)) return false

    const numeric = Number(value)

    return !Number.isFinite(numeric) || numeric <= 0
  }

  function validateRows(rows: Record<string, unknown>[], requiredHeaders: string[]) {
    for (const [index, row] of rows.entries()) {
      for (const header of requiredHeaders) {
        const key = normalizeSheetHeaderKey(header)
        if (isInvalidRequiredValue(key, row[key])) {
          errorMessage.value
            += BULK_DESTINATIONS_ERROR_MESSAGES.invalidRowCellValue.replace('{index}', index.toString()).replace('{header}', header)
        }
      }
    }
  }

  return {
    errorMessage,
    validateHeaders,
    validateRows,
  }
}
