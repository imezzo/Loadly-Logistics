<template>
  <div class="flex w-full justify-end gap-10 border-t border-neutral-100 p-4">
    <div
      v-for="indicator in indicators"
      :key="indicator.label"
    >
      <p class="font-bold">
        {{ indicator.label }}
      </p>
      <p class="font-bold text-primary">
        {{ indicator.value.toLocaleString() }} {{ t('form.egp') }}
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
  import type { TripWizardTripCharge } from '@/types/admin-dashboard/response/trip/trip'

  const props = defineProps<{
    charges: TripWizardTripCharge[]
  }>()

  const { t } = useI18n()

  const totalAmount = computed(() => {
    return props.charges.reduce((acc, charge) => acc + charge.amount, 0)
  })

  const totalCompanyShare = computed(() => {
    return props.charges.reduce((acc, charge) => acc + (charge.companyShare ?? 0), 0)
  })
  const totalCustomerShare = computed(() => {
    return props.charges.reduce((acc, charge) => acc + charge.amount - (charge.companyShare ?? 0), 0)
  })

  const indicators = computed(() => {
    return [
      { label: t('content.totalAmount'), value: totalAmount.value },
      { label: t('content.totalCustomerShare'), value: totalCustomerShare.value },
      { label: t('content.totalCompanyShare'), value: totalCompanyShare.value },
    ]
  })
</script>

<style scoped>

</style>
