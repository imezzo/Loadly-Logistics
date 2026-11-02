<template>
  <BasePopup
    v-model="open"
    content-class="max-h-[85vh] !max-w-3xl overflow-y-auto"
  >
    <div
      v-if="destination"
      class="w-full max-w-full px-2 text-start"
    >
      <CardPreview
        class="mt-6"
        :title="$t('content.destinationDetails')"
      >
        <template #content>
          <CardPreviewLabel
            :label="$t('table.destinationName')"
            :content="getLocalizedValue(destination.nameEn, destination.nameAr)"
          />

          <CardPreviewLabel
            :label="$t('form.description')"
            :content="destination.description?.trim() ? destination.description : $t('content.notAvailable')"
          />

          <CardPreviewLabel
            :label="$t('table.kilometer')"
            :content="String(destination.expectedKms)"
          />

          <CardPreviewLabel
            :label="$t('table.numberOfDays')"
            :content="String(destination.numberOfDays)"
          />

          <CardPreviewLabel
            :label="$t('form.pricePerTon')"
            :content="formatOptionalNumber(destination.pricePerTon)"
          />

          <CardPreviewLabel
            :label="$t('form.fixedTripPrice')"
            :content="formatOptionalNumber(destination.fixedTripPrice)"
          />

          <CardPreviewLabel
            :label="$t('form.contractorPricePerTon')"
            :content="formatOptionalNumber(destination.contractorPricePerTon)"
          />

          <CardPreviewLabel
            :label="$t('form.contractorFixedTripPrice')"
            :content="formatOptionalNumber(destination.contractorFixedTripPrice)"
          />
        </template>
      </CardPreview>

      <div class="mt-3 grid grid-cols-2 gap-4 space-y-6">
        <section>
          <h4 class="mb-2 text-base font-bold text-neutral-10">
            {{ $t('form.startLocation') }}
          </h4>

          <p class="mb-2 text-sm text-neutral-700">
            {{ destination.start?.addressName?.trim() ? destination.start.addressName : $t('content.notAvailable') }}
          </p>

          <ContractDestinationPointMap
            :latitude="destination.start?.latitude"
            :longitude="destination.start?.longitude"
            :map-label="$t('form.startLocation')"
          />
        </section>

        <section>
          <h4 class="mb-2 text-base font-bold text-neutral-10">
            {{ $t('form.endLocation') }}
          </h4>

          <p class="mb-2 text-sm text-neutral-700">
            {{ destination.end?.addressName?.trim() ? destination.end.addressName : $t('content.notAvailable') }}
          </p>

          <ContractDestinationPointMap
            :latitude="destination.end?.latitude"
            :longitude="destination.end?.longitude"
            :map-label="$t('form.endLocation')"
          />
        </section>
      </div>
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import type { ContractWizardDestination } from '@/types/request/contract/contract-destinations.types'
  import { getLocalizedValue } from '@/utils/i18n'
  import ContractDestinationPointMap from './ContractDestinationPointMap.vue'

  defineProps<{
    destination: ContractWizardDestination | null
  }>()

  const open = defineModel<boolean>({ required: true })
  const { t } = useI18n()

  function formatOptionalNumber(value: number | undefined): string {
    if (value == null || Number.isNaN(Number(value))) return t('content.notAvailable')

    return String(value)
  }
</script>

<style scoped>

</style>
