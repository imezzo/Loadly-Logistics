import { isValidPhoneNumber, validatePhoneNumberLength } from 'libphonenumber-js'

export const EXPECTED_TRIP_SHEET_HEADERS = [
  'start-date',
  'end-date',
  'from',
  'to',
  'distance-kms',
  'contact-name',
  'contact-mobile',
  'helpers-count',
] as const

export const BULK_TRIPS_ERROR_MESSAGES = {
  noHeaders: `No headers, or missing headers in the sheet. Expected headers: (${EXPECTED_TRIP_SHEET_HEADERS.join(', ')}).`,
  invalidHeader: 'Invalid header: {header}. Expected headers: ({expected}). regardless of case and whitespace.',
  invalidRowCellValue: 'Invalid row cell value. for row index: (#{index}) for the column: ({header}).<br>',
  noRows: 'The uploaded sheet has no trip rows.',
  invalidStartDate: 'Invalid start date. The start date must be a date value.',
  invalidEndDate: 'Invalid end date. The end date must be a date value.',
  invalidFromDestination: 'Invalid from destination. The from destination must be a valid Google Maps URL.',
  invalidToDestination: 'Invalid to destination. The to destination must be a valid Google Maps URL.',
  invalidDistanceKms: 'Invalid distance kms. The distance kms must be a number.',
  invalidContactMobile: 'Invalid contact mobile. The contact mobile must be a valid phone number.',
}

export function normalizeSheetHeaderKey(header: string) {
  return header.trim().toLowerCase().replace(/ /g, '-').replace(/_/g, '-')
}

const errorMessage = ref('')

export function useBulkTrips() {
  const expected = EXPECTED_TRIP_SHEET_HEADERS

  function validateHeaders(headers: string[]) {
    if (headers.length < expected.length) {
      errorMessage.value = BULK_TRIPS_ERROR_MESSAGES.noHeaders

      return
    }

    for (const header of headers) {
      const normalizedHeader = normalizeSheetHeaderKey(header)
      if (!(expected as readonly string[]).includes(normalizedHeader)) {
        errorMessage.value
          = BULK_TRIPS_ERROR_MESSAGES.invalidHeader
            .replace('{header}', normalizedHeader)
            .replace('{expected}', expected.join(', '))

        return
      }
    }

    errorMessage.value = ''
  }

  function validateDates(row: Record<string, unknown>) {
    if (row['start-date'] && !(row['start-date'] instanceof Date)) {
      errorMessage.value = BULK_TRIPS_ERROR_MESSAGES.invalidStartDate
    }

    if (row['end-date'] && !(row['end-date'] instanceof Date)) {
      errorMessage.value = BULK_TRIPS_ERROR_MESSAGES.invalidEndDate
    }
  }

  function hasValidDestinationLink(destination: string) {
    return [
      'https://www.google.com/maps',
      'https://maps.google.com',
      'https://goo.gl/maps',
      'https://maps.app.goo.gl',
    ].some(prefix => destination.startsWith(prefix))
  }

  function validateDestinations(row: Record<string, unknown>) {
    if (row['from'] && !hasValidDestinationLink(row['from'] as string)) {
      errorMessage.value = BULK_TRIPS_ERROR_MESSAGES.invalidFromDestination
    }

    if (row['to'] && !hasValidDestinationLink(row['to'] as string)) {
      errorMessage.value = BULK_TRIPS_ERROR_MESSAGES.invalidToDestination
    }
  }

  function validateDistance(row: Record<string, unknown>) {
    if (row['distance-kms'] && typeof row['distance-kms'] !== 'number') {
      errorMessage.value = BULK_TRIPS_ERROR_MESSAGES.invalidDistanceKms
    }
  }

  function validateContactInfo(row: Record<string, unknown>) {
    //? The mobile number is prefixed with +2 as a business requirements to have Egyptian numbers only
    const contactMobile = '+2' + String(row['contact-mobile'] ?? '')

    if (
      contactMobile
      && !isValidPhoneNumber(contactMobile)
      && validatePhoneNumberLength(contactMobile)
    ) {
      errorMessage.value = BULK_TRIPS_ERROR_MESSAGES.invalidContactMobile
    }
  }

  function validateRows(rows: Record<string, unknown>[], requiredHeaders: string[]) {
    for (const [index, row] of rows.entries()) {
      for (const header of requiredHeaders) {
        const key = normalizeSheetHeaderKey(header)
        if (row[key] === null || row[key] === undefined || String(row[key]).trim() === '') {
          errorMessage.value
            += BULK_TRIPS_ERROR_MESSAGES.invalidRowCellValue
              .replace('{index}', index.toString())
              .replace('{header}', header)
        }
      }

      validateDates(row)
      validateDestinations(row)
      validateDistance(row)
      validateContactInfo(row)
    }
  }

  return {
    errorMessage,
    validateHeaders,
    validateRows,
  }
}
