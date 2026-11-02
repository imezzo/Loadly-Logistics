<template>
  <button
    type="button"
    class="btn-primary-outline"
    @click="exportDestinations"
  >
    {{ $t('button.export') }}
  </button>
</template>

<script setup lang="ts">
  import type { ContractWizardDestination } from '@/types/request/contract/contract-destinations.types'
  import { utils, writeFile } from 'xlsx'

  const props = withDefaults(
    defineProps<{
      existingDestinations?: ContractWizardDestination[]
    }>(),
    { existingDestinations: () => [] },
  )

  const formattedData = computed(() => {
    return props.existingDestinations?.map(item => ({
      'id': item.id,
      'name-en': item.nameEn,
      'name-ar': item.nameAr,
      'address-from': item.start?.addressName,
      'address-to': item.end?.addressName,
      'number-of-days': item.numberOfDays,
      'kms': item.expectedKms,
      'return-expected-kms': item.returnExpectedKms,
      'fixed-destination-price': item.fixedTripPrice,
      'contractor-fixed-destination-price': item.contractorFixedTripPrice,
      'price-per-ton': item.pricePerTon,
      'contractor-price-per-ton': item.contractorPricePerTon,
    }))
  })

  function exportDestinations() {
    const worksheet = utils.json_to_sheet(formattedData.value)

    const workbook = utils.book_new()

    utils.book_append_sheet(
      workbook,
      worksheet,
      'Destinations'
    )

    const fileName = `destinations-export-${new Date().toISOString()}.xlsx`
    writeFile(workbook, fileName)
  }

</script>

<style scoped>

</style>
