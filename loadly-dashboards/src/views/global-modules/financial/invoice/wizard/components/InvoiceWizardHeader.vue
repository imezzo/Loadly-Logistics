<template>
  <div class="mb-6 flex items-start justify-between gap-4">
    <div>
      <h1 class="text-2xl font-bold text-neutral-800">
        {{ pageTitle }}
      </h1>

      <p class="mt-2 text-content-secondary">
        <span class="rounded-lg bg-info-500 px-2 py-1 text-xs text-white">
          {{ invoiceType }}
        </span>

        {{ companyName }}
      </p>
    </div>

    <BaseLink
      v-if="hasVehicleByDaysPeriod"
      :to="vehicleByDaysRoute"
      class="
        inline-flex shrink-0 items-center gap-2 rounded-full bg-primary px-4 py-2 text-sm
        font-medium text-white transition-colors
        hover:bg-primary/90
      "
    >
      {{ $t('button.vehicleByDays') }}
      <IconTruck class="size-4" />
    </BaseLink>
  </div>
</template>

<script setup lang="ts">
  import { useInvoiceWizardHeader } from '../composables/useInvoiceWizardHeader'

  const props = defineProps<{
    contractId: number | null
  }>()

  const {
    companyName,
    hasVehicleByDaysPeriod,
    invoiceType,
    pageTitle,
    vehicleByDaysRoute,
  } = useInvoiceWizardHeader(toRef(props, 'contractId'))
</script>

<style scoped>

</style>
