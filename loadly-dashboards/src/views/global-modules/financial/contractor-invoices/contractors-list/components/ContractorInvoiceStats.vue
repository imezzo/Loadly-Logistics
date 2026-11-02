<template>
  <section
    class="
      mb-6 flex flex-col overflow-hidden rounded-2xl border border-neutral-500 bg-white
      md:flex-row
    "
  >
    <StatCard
      :label="$t('content.allTrips')"
      :value="statistics?.trips.allTime"
      :percent-change="statistics?.trips.vsLastMonth.percentChange"
      :direction="statistics?.trips.vsLastMonth.direction"
    >
      <template #icon>
        <IconTrip class="size-4 shrink-0" />
      </template>
    </StatCard>

    <div
      class="
        h-px w-full bg-neutral-500
        md:h-auto md:w-px
      "
    />

    <StatCard
      :label="$t('content.loadlyProfitMargin')"
      :value="statistics?.loadlyProfitMargin.allTime"
      :percent-change="statistics?.loadlyProfitMargin.vsLastMonth.percentChange"
      :direction="statistics?.loadlyProfitMargin.vsLastMonth.direction"
      show-currency
    >
      <template #icon>
        <IconMoneyBagBroken class="size-4 shrink-0" />
      </template>
    </StatCard>
  </section>
</template>

<script setup lang="ts">
  import StatCard from '../../components/StatCard.vue'
  import { useContractorInvoices } from '../composables/useContractorInvoices'

  const { statistics } = useContractorInvoices()
</script>

<style scoped>

</style>
