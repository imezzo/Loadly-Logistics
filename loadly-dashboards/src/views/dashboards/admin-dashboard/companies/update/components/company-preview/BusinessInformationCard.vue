<template>
  <CardPreview :title="$t('content.businessInformation')">
    <template #content>
      <CardPreviewLabel
        :label="$t('form.industryType')"
        :content="getLookupLabel(industryTypes, formData.industryType?.toString())
          || $t('content.notAvailable')"
        :is-edited="isFieldEdited('industryType', formData.industryType)"
      />

      <CardPreviewLabel
        :label="$t('form.companySize')"
        :content="companySizeLabel || $t('content.notAvailable')"
        :is-edited="isFieldEdited('companySize', formData.companySize)"
      />

      <CardPreviewLabel
        :label="$t('form.serviceRequirements')"
        :content="getLookupLabel(serviceRequirements, formData.serviceRequirements?.toString())
          || $t('content.notAvailable')"
        :is-edited="isServiceRequirementsEdited()"
      />

      <CardPreviewLabel
        :label="$t('form.operatingHours')"
        :content="getLookupLabel(operatingHours, formData.operatingHours?.toString())
          || $t('content.notAvailable')"
        :is-edited="isFieldEdited('operatingHours', formData.operatingHours)"
      />

      <CardPreviewLabel
        :label="$t('form.preferredCommunicationMethods')"
        :content="getLookupLabel(communicationMethods, formData.preferredCommunicationMethods?.toString())
          || $t('content.notAvailable')"
        :is-edited="isFieldEdited('preferredCommunicationMethods', formData.preferredCommunicationMethods)"
      />
    </template>
  </CardPreview>
</template>

<script setup lang="ts">
  import type { LookupItem } from '@/types/response/lookup'

  const props = defineProps<{
    formData: Record<string, unknown>
    originalFormData?: Record<string, unknown>
    industryTypes: LookupItem[]
    serviceRequirements: LookupItem[]
    communicationMethods: LookupItem[]
    operatingHours: LookupItem[]
  }>()

  function getLookupLabel(lookupItems: LookupItem[], value: string | undefined): string {
    if (!value) return ''

    return lookupItems.find(item => item.value === String(value))?.label || value
  }

  const { t } = useI18n()

  const companySizeLabel = computed(() => {
    const size = props.formData.companySize as string | undefined
    if (!size) return ''
    const sizeMap: Record<string, string> = {
      small: t('form.companySizeSmall'),
      medium: t('form.companySizeMedium'),
      large: t('form.companySizeLarge'),
    }

    return sizeMap[size] || size
  })

  function isFieldEdited(fieldName: string, currentValue: unknown): boolean {
    if (!props.originalFormData) return false
    const originalValue = props.originalFormData[fieldName]

    return String(currentValue || '') !== String(originalValue || '')
  }

  function isServiceRequirementsEdited(): boolean {
    if (!props.originalFormData) return false
    const current = props.formData.serviceRequirements as string | undefined
    const original = props.originalFormData.serviceRequirements as string | undefined
    const compareFn = (a: string, b: string) => a.localeCompare(b)
    const currentIds = current
      ? current.split(',').map(id => id.trim()).sort(compareFn).join(',')
      : ''
    const originalIds = original
      ? original.split(',').map(id => id.trim()).sort(compareFn).join(',')
      : ''

    return currentIds !== originalIds
  }
</script>

<style scoped>

</style>
