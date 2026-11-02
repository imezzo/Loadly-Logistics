<template>
  <CardPreview
    v-if="hasMultipleLocations"
    :title="$t('content.multipleLocations')"
  >
    <template #content>
      <div
        v-for="(location, index) in multipleLocations"
        :key="index"
        class="font-medium text-neutral-800"
      >
        {{ formatLocationAddress(location) }}
      </div>
    </template>
  </CardPreview>
</template>

<script setup lang="ts">
  import type { CompanyAddressFormData } from '@/types/admin-dashboard/response/company-detail'
  import type { LookupItem } from '@/types/response/lookup'

  const props = defineProps<{
    multipleLocations: (CompanyAddressFormData | {
      city?: string;
      governorate?: string;
      district?: string;
      street?: string;
      postalCode?: string
    })[]
    cities: LookupItem[]
    governments: LookupItem[]
  }>()

  const { t } = useI18n()

  const hasMultipleLocations = computed(() => props.multipleLocations.length > 0)

  function getLookupLabel(lookupItems: LookupItem[], value: string | undefined): string {
    if (!value) return ''

    return lookupItems.find(item => item.value === String(value))?.label || value
  }

  function formatLocationAddress(location: CompanyAddressFormData | { city?: string; governorate?: string; district?: string; street?: string; postalCode?: string }): string {
    const cityLabel = location.city ? getLookupLabel(props.cities, String(location.city)) : ''
    const governorateLabel = location.governorate ? getLookupLabel(props.governments, String(location.governorate)) : ''
    const streetName = 'streetName' in location ? location.streetName : (location as { street?: string }).street
    const parts = [
      streetName,
      location.district,
      cityLabel,
      governorateLabel,
      location.postalCode,
    ].filter(Boolean)

    return parts.length > 0 ? parts.join(', ') : t('content.notAvailable')
  }
</script>

<style scoped>

</style>
