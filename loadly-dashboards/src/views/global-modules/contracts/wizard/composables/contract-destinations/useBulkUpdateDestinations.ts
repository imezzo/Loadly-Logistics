import type { ContractWizardDestination } from '@/types/request/contract/contract-destinations.types'
import { useBulkDestinations } from './useBulkDestinations'

export function mergeSheetRowIntoDestination(
  existing: ContractWizardDestination,
  row: Record<string, string | boolean>,
): ContractWizardDestination {
  const numberOfDays = row['number-of-days']
  const kms = row['kms']
  const returnExpectedKms = row['return-expected-kms']
  const fixedTripPrice = row['fixed-destination-price']
  const pricePerTon = row['price-per-ton']
  const contractorFixedTripPrice = row['contractor-fixed-destination-price']
  const contractorPricePerTon = row['contractor-price-per-ton']

  return {
    ...existing,
    nameEn: String(row['name-en'] ?? existing.nameEn),
    nameAr: String(row['name-ar'] ?? existing.nameAr),
    numberOfDays: numberOfDays != null && numberOfDays !== ''
      ? Number(numberOfDays)
      : existing.numberOfDays,
    expectedKms: kms != null && kms !== ''
      ? Number(kms)
      : existing.expectedKms,
    returnExpectedKms: Number(returnExpectedKms),
    ...(fixedTripPrice != null && fixedTripPrice !== ''
      ? { fixedTripPrice: Number(fixedTripPrice) }
      : {}),
    ...(pricePerTon != null && pricePerTon !== ''
      ? { pricePerTon: Number(pricePerTon) }
      : {}),
    ...(contractorFixedTripPrice != null && contractorFixedTripPrice !== ''
      ? { contractorFixedTripPrice: Number(contractorFixedTripPrice) }
      : {}),
    ...(contractorPricePerTon != null && contractorPricePerTon !== ''
      ? { contractorPricePerTon: Number(contractorPricePerTon) }
      : {}),
    start: {
      ...existing.start,
      addressName: String(row['address-from'] ?? existing.start?.addressName ?? ''),
      googlePlaceId: existing.start?.googlePlaceId ?? '',
      latitude: existing.start?.latitude ?? '',
      longitude: existing.start?.longitude ?? '',
      ...(existing.start?.id != null ? { id: existing.start.id } : {}),
    },
    end: {
      ...existing.end,
      addressName: String(row['address-to'] ?? existing.end?.addressName ?? ''),
      googlePlaceId: existing.end?.googlePlaceId ?? '',
      latitude: existing.end?.latitude ?? '',
      longitude: existing.end?.longitude ?? '',
      ...(existing.end?.id != null ? { id: existing.end.id } : {}),
    },
  }
}

export function useBulkUpdateDestinations(
  requiredHeaders: ComputedRef<readonly string[] | string[]>,
  existingDestinations: Ref<ContractWizardDestination[] | undefined>
) {
  const { t } = useI18n()
  const { validateHeaders, errorMessage, validateRows } = useBulkDestinations()

  function validateIds(rows: Record<string, string>[]) {
    const destinationsWithId = existingDestinations.value?.filter(destination => destination.id != null) ?? []
    if (!destinationsWithId.length) {
      errorMessage.value = t('title.bulkUpdateDestinationsNoSavedIds')

      return
    }

    const seenIds = new Map<number, number>()

    for (const [i, row] of rows.entries()) {
      const rawId = row.id
      const id = Number(rawId)

      if (!rawId || rawId === '' || !Number.isFinite(id)) {
        errorMessage.value
          += `${t('title.bulkUpdateDestinationsMissingId', { row: i + 1 })}<br>`

        continue
      }

      const firstRow = seenIds.get(id)
      if (firstRow !== undefined) {
        errorMessage.value
          += `${t('title.bulkUpdateDestinationsDuplicateId', { row: i + 1, first: firstRow + 1, id })}<br>`
      } else {
        seenIds.set(id, i)
      }

      if (!destinationsWithId.some(d => d.id === id)) {
        errorMessage.value
          += `${t('title.bulkUpdateDestinationsUnknownId', { row: i + 1, id })}<br>`
      }
    }
  }

  function validateBulkUpdateSheet(rows: Record<string, string>[], headers: string[]) {
    errorMessage.value = ''
    validateHeaders(headers)
    if (errorMessage.value) return false
    validateIds(rows)
    if (errorMessage.value) return false
    validateRows(rows, requiredHeaders.value as string[])

    return true
  }

  return {
    validateBulkUpdateSheet,
    errorMessage,
  }
}
