<template>
  <CardPreview :title="$t('content.companyDetails')">
    <template #content>
      <CardPreviewLabel
        :label="$t('form.companyName')"
        :content="companyName || $t('content.notAvailable')"
        :is-edited="isCompanyNameEdited()"
      />

      <CardPreviewLabel
        :label="$t('form.registrationNumber')"
        :content="String(formData.registrationNumber || $t('content.notAvailable'))"
        :is-edited="isFieldEdited('registrationNumber', formData.registrationNumber)"
      />

      <CardPreviewLabel
        :label="$t('form.taxId')"
        :content="String(formData.taxId || $t('content.notAvailable'))"
        :is-edited="isFieldEdited('taxId', formData.taxId)"
      />

      <CardPreviewLabel
        :label="$t('form.websiteUrl')"
        :content="String(formData.websiteUrl || $t('content.notAvailable'))"
        :is-edited="isFieldEdited('websiteUrl', formData.websiteUrl)"
      />

      <CardPreviewLabel
        :label="$t('form.contactPersonName')"
        :content="String(formData.contactPersonName || $t('content.notAvailable'))"
        :is-edited="isFieldEdited('contactPersonName', formData.contactPersonName)"
      />

      <CardPreviewLabel
        :label="$t('form.userName')"
        :content="String(formData.userName || $t('content.notAvailable'))"
        :is-edited="isFieldEdited('userName', formData.userName)"
      />

      <CardPreviewLabel
        :label="$t('content.emailAddress')"
        :content="String(formData.email || $t('content.notAvailable'))"
        :is-edited="isFieldEdited('email', formData.email)"
      />

      <CardPreviewLabel
        :label="$t('content.phoneNumber')"
        :content="String(formData.phone || $t('content.notAvailable'))"
        :is-edited="isFieldEdited('phone', formData.phone)"
      />

      <CardPreviewLabel
        :label="$t('form.postalCode')"
        :content="String(formData.postalCode || $t('content.notAvailable'))"
        :is-edited="isFieldEdited('postalCode', formData.postalCode)"
      />

      <CardPreviewLabel
        :label="$t('content.address')"
        :content="formatAddress()"
        :is-edited="isAddressEdited()"
      />
    </template>
  </CardPreview>
</template>

<script setup lang="ts">
  import type { LookupItem } from '@/types/response/lookup'

  const props = defineProps<{
    formData: Record<string, unknown>
    originalFormData?: Record<string, unknown>
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

  function isFieldEdited(fieldName: string, currentValue: unknown): boolean {
    if (!props.originalFormData) return false
    const originalValue = props.originalFormData[fieldName]

    return String(currentValue || '') !== String(originalValue || '')
  }

  function isCompanyNameEdited(): boolean {
    if (!props.originalFormData) return false

    return isFieldEdited('companyName', props.formData.companyName) ||
           isFieldEdited('nameEn', props.formData.nameEn) ||
           isFieldEdited('nameAr', props.formData.nameAr)
  }

  function isAddressEdited(): boolean {
    if (!props.originalFormData) return false
    const addressFields = ['street', 'district', 'city', 'governorate', 'buildingNumber']

    return addressFields.some(field => isFieldEdited(field, props.formData[field]))
  }
</script>

<style scoped>

</style>

