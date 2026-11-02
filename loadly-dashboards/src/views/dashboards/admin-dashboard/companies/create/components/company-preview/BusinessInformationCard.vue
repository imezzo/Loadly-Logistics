<template>
  <CardPreview :title="$t('content.businessInformation')">
    <template #content>
      <CardPreviewLabel
        :label="$t('form.industryType')"
        :content="getLookupLabel(props.industryTypes, formData.industryType?.toString())
          || $t('content.notAvailable')"
      />

      <CardPreviewLabel
        :label="$t('form.companySize')"
        :content="companySizeLabel || $t('content.notAvailable')"
      />

      <CardPreviewLabel
        :label="$t('form.serviceRequirements')"
        :content="serviceRequirementsLabels || $t('content.notAvailable')"
      />

      <CardPreviewLabel
        :label="$t('form.operatingHours')"
        :content="getLookupLabel(operatingHours, formData.operatingHours?.toString())
          || $t('content.notAvailable')"
      />

      <CardPreviewLabel
        :label="$t('form.preferredCommunicationMethods')"
        :content="getLookupLabel(communicationMethods, formData.preferredCommunicationMethods?.toString())
          || $t('content.notAvailable')"
      />
    </template>
  </CardPreview>
</template>

<script setup lang="ts">
  import type { LookupItem } from '@/types/response/lookup'

  const props = defineProps<{
    formData: Record<string, unknown>
    industryTypes: LookupItem[]
    serviceRequirements: LookupItem[]
    communicationMethods: LookupItem[]
    operatingHours: LookupItem[]
  }>()

  function getLookupLabel(lookupItems: LookupItem[], value?: string): string {
    if (!value) return ''

    return lookupItems.find(item => item.value === String(value))?.label || value
  }

  const { t } = useI18n()

  const companySizeLabel = computed(() => {
    const value = props.formData.companySize as string | undefined
    const companySizes: LookupItem[] = [
      { value: 'small', label: t('form.companySizeSmall') },
      { value: 'medium', label: t('form.companySizeMedium') },
      { value: 'large', label: t('form.companySizeLarge') },
    ]

    return getLookupLabel(companySizes, value)
  })

  const serviceRequirementsLabels = computed(() => {
    const value = props.formData.serviceRequirements as string | undefined
    if (!value) return ''
    const ids = value.split(',').filter(Boolean)
    const labels = ids.map(id => getLookupLabel(props.serviceRequirements, id.trim())).filter(Boolean)

    return labels.join(', ') || value
  })
</script>

<style scoped>

</style>

