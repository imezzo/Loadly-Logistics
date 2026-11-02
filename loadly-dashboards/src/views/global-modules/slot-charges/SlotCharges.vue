<template>
  <section
    class="
      rounded-xl bg-neutral-400 p-3
      md:p-2.5
    "
  >
    <SlotChargesHeader :start-date="filterStartDate" />

    <SlotChargesSummary
      v-if="slotCharges"
      :charges="slotCharges"
      :consumption-type="contract?.consumptionType"
      :km-pool="slotCharges.kmPool"
    />

    <SlotChargesFilters
      v-if="showDateFilters"
      v-model:start-date="filterStartDate"
      v-model:end-date="filterEndDate"
      @search="initCalculatedSlot"
    />

    <BaseAlert
      :model-value="!!errorMessage"
      :message="errorMessage"
      variant="error"
      class="mb-4"
    />

    <SlotChargesTable
      :columns="tableColumns"
      :data="tableRows"
      :has-clickable-days="hasClickableDays"
    />
  </section>
</template>

<script setup lang="ts">
  import SlotChargesHeader from './components/SlotChargesHeader.vue'
  import SlotChargesSummary from './components/SlotChargesSummary.vue'
  import SlotChargesTable from './components/slots-charges-table/SlotChargesTable.vue'
  import { useSlotsCharges } from './composables/useSlotsCharges.ts'

  const SlotChargesFilters = defineAsyncComponent(() => import('./components/SlotChargesFilters.vue'))

  const props = defineProps<{
    contractId: string
    showDateFilters?: boolean
    skipContractFetch?: boolean
    hasClickableDays?: boolean
  }>()

  const filterStartDate = defineModel<string>('startDate', { default: '' })
  const filterEndDate = defineModel<string>('endDate', { default: '' })

  const {
    contract,
    errorMessage,
    initCalculatedSlot,
    slotCharges,
    tableColumns,
    tableRows,
  } = useSlotsCharges({
    contractId: toRef(() => props.contractId),
    startDate: filterStartDate,
    endDate: filterEndDate,
    skipContractFetch: props.skipContractFetch ?? props.showDateFilters ?? false,
  })

  onMounted(() => {
    if (!props.showDateFilters || (filterStartDate.value && filterEndDate.value)) {
      initCalculatedSlot()
    }
  })

  onUnmounted(() => {
    slotCharges.value = null
  })
</script>

<style scoped>

</style>
