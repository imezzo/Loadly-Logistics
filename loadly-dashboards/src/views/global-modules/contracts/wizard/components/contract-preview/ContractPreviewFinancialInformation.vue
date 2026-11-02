<template>
  <CardPreview :title="$t('content.financialInformation')">
    <template #content>
      <CardPreviewLabel
        :label="$t('form.monthlyContractValue')"
        :content="formData.price
          ? formData.price + ' ' + currency
          : $t('content.notAvailable')"
      />

      <CardPreviewLabel
        :label="$t('content.totalContractValue')"
        :content="formData.price
          ? String((+formData.price * getContractMonthsCount).toFixed(0)) + ' ' + currency
          : $t('content.notAvailable')"
      />

      <CardPreviewLabel
        :label="$t('form.currency')"
        :content="currency.toString()"
      />

      <CardPreviewLabel
        :label="$t('form.paymentTerms')"
        :content="$t('form.N_days', { days: formData.paymentDueDays })"
      />

      <CardPreviewLabel
        v-if="hasTripsPerMonth"
        :label="tripsPerMonthFieldLabel"
        :content="formData.tripsPerMonth?.toString() ?? ''"
      />
    </template>
  </CardPreview>
</template>

<script setup lang="ts">
  import { useContractWizard } from '../../composables/useContractWizard'
  import { useContractValue } from '../../composables/useContractValue'

  const { formData } = useContractWizard()
  const { tripsPerMonthFieldLabel, hasTripsPerMonth } = useContractValue()

  const currency = computed(() => {
    return formData.value?.currency || 'EGP'
  })

  const getContractMonthsCount = computed(() => {
    const startDate = new Date(formData.value?.startDate as string)
    const endDate = new Date(formData.value?.endDate as string)

    return Math.floor((endDate.getTime() - startDate.getTime()) / (1000 * 60 * 60 * 24 * 30))
  })
</script>

<style scoped>

</style>

