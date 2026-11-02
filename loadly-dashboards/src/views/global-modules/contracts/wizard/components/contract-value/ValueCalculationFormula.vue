<template>
  <div class="w-full rounded-xl border border-gray-50 px-1 py-2">
    <p class="mb-2 px-2 font-bold text-neutral-10">
      {{ $t('form.valueCalculation') }}
    </p>

    <p class="mb-2 rounded-xl border border-gray-50 bg-neutral-30 px-3 py-2">
      {{ $t('form.valueCalculationFormula', {
        tripsLabel: t('form.monthlyFeesPerVehicle'),
        price: price ?? '',
        currency: currency ?? ''
      }) }}
    </p>

    <p class="px-2 font-medium text-neutral-10">
      {{ $t('form.valueCalculation') }}
    </p>
  </div>
</template>

<script setup lang="ts">
  const { t } = useI18n()

  defineProps<{
    price?: string
    currency?: string
    contractType?: number
  }>()
</script>

<style scoped>

</style>
