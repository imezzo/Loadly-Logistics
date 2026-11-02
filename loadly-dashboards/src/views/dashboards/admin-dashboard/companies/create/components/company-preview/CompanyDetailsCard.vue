<template>
  <CardPreview :title="$t('content.companyDetails')">
    <template #content>
      <CardPreviewLabel
        :label="$t('form.companyName')"
        :content="companyName || $t('content.notAvailable')"
      />

      <CardPreviewLabel
        :label="$t('form.registrationNumber')"
        :content="String(formData.registrationNumber || $t('content.notAvailable'))"
      />

      <CardPreviewLabel
        :label="$t('form.taxId')"
        :content="String(formData.taxId || $t('content.notAvailable'))"
      />

      <CardPreviewLabel
        :label="$t('form.websiteUrl')"
        :content="String(formData.websiteUrl || $t('content.notAvailable'))"
      />

      <CardPreviewLabel
        :label="$t('form.contactPersonName')"
        :content="String(formData.contactPersonName || $t('content.notAvailable'))"
      />

      <CardPreviewLabel
        :label="$t('form.userName')"
        :content="String(formData.userName || $t('content.notAvailable'))"
      />

      <CardPreviewLabel
        :label="$t('content.emailAddress')"
        :content="String(formData.email || $t('content.notAvailable'))"
      />

      <CardPreviewLabel
        :label="$t('content.phoneNumber')"
        :content="String(formData.phone || $t('content.notAvailable'))"
      />

      <CardPreviewLabel
        :label="$t('form.postalCode')"
        :content="String(formData.postalCode || $t('content.notAvailable'))"
      />

      <CardPreviewLabel
        :label="$t('content.address')"
        :content="formatAddress()"
      />
    </template>
  </CardPreview>
</template>

<script setup lang="ts">
  import type { LookupItem } from '@/types/response/lookup'

  const props = defineProps<{
    formData: Record<string, unknown>
    companyName: string
    cities: LookupItem[]
    governments: LookupItem[]
  }>()

  const { t } = useI18n()

  function getLookupLabel(lookupItems: LookupItem[], value: string | undefined): string {
    if (!value) return ''

    return lookupItems.find(item => item.value === String(value))?.label || value
  }

  function formatAddress(): string {
    const street = props.formData.street as string | undefined
    const district = props.formData.district as string | undefined
    const cityId = props.formData.city as string | undefined
    const governorateId = props.formData.governorate as string | undefined
    const buildingNumber = props.formData.buildingNumber as string | undefined

    const cityLabel = cityId ? getLookupLabel(props.cities, cityId) : ''
    const governorateLabel = governorateId ? getLookupLabel(props.governments, governorateId) : ''
    const districtLabel = district || ''

    const parts = [
      street,
      buildingNumber,
      districtLabel,
      cityLabel,
      governorateLabel,
    ].filter(Boolean)

    return parts.length > 0 ? parts.join(', ') : t('content.notAvailable')
  }
</script>

<style scoped>

</style>

