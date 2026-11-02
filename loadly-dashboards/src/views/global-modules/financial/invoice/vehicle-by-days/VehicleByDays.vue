<template>
  <div class="page-content">
    <BaseLink
      :to="backRoute"
      class="
        mb-4 inline-flex items-center gap-2 text-sm font-medium text-neutral-800 transition-colors
        hover:text-primary
      "
    >
      <IconArrow class="size-5 rotate-180 fill-current!" />
      {{ $t('button.back') }}
    </BaseLink>

    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <div
      class="
        mb-6 flex flex-col gap-4
        sm:flex-row sm:items-start sm:justify-between
      "
    >
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

      <SlotChargesExport
        :contract-id="contractId"
        :start-date="startDate"
        :end-date="endDate"
      />
    </div>

    <SlotCharges
      :contract-id="contractId"
      :start-date="startDate"
      :end-date="endDate"
    />
  </div>
</template>

<script setup lang="ts">
  import SlotCharges from '@/views/global-modules/slot-charges/SlotCharges.vue'
  import SlotChargesExport from '@/views/global-modules/slot-charges/components/SlotChargesExport.vue'
  import { useVehicleByDays } from './composables/useVehicleByDays'

  const {
    backRoute,
    breadcrumbItems,
    companyName,
    contractId,
    endDate,
    invoiceType,
    pageTitle,
    startDate,
  } = useVehicleByDays()
</script>

<style scoped>

</style>
