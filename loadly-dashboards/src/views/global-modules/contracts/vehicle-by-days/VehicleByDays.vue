<template>
  <div class="page-content">
    <BaseLink
      :to="contractRoute"
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
        sm:flex-row sm:items-center sm:justify-between
      "
    >
      <h1 class="text-2xl font-bold text-neutral-800">
        {{ pageTitle }}
      </h1>

      <SlotChargesExport
        :contract-id="contractId"
        :start-date="filterStartDate"
        :end-date="filterEndDate"
      />
    </div>

    <SlotCharges
      v-model:start-date="filterStartDate"
      v-model:end-date="filterEndDate"
      :contract-id="contractId"
      show-date-filters
      skip-contract-fetch
    />
  </div>
</template>

<script setup lang="ts">
  import SlotCharges from '@/views/global-modules/slot-charges/SlotCharges.vue'
  import SlotChargesExport from '@/views/global-modules/slot-charges/components/SlotChargesExport.vue'
  import { useVehicleByDays } from './composables/useVehicleByDays'

  const {
    breadcrumbItems,
    contractId,
    contractRoute,
    pageTitle,
    fetchContract,
    contract,
  } = useVehicleByDays()

  const today = new Date().toISOString().split('T')[0] ?? ''

  const filterStartDate = ref(today)
  const filterEndDate = ref(today)

  onMounted(fetchContract)

  onUnmounted(() => {
    contract.value = null
  })
</script>

<style scoped>

</style>
