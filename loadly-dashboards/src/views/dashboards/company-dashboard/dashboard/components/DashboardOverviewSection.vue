<template>
  <article class="rounded-2xl border border-neutral-500 bg-white p-6">
    <div
      class="
        flex flex-col gap-6
        lg:flex-row lg:items-stretch
      "
    >
      <section
        class="
          min-w-0
          lg:w-2/5
        "
      >
        <div class="flex items-center gap-2">
          <IconTruck class="size-5" />
          <p class="text-lg font-bold text-primary">
            {{ $t('content.totalTripsLabel') }}
          </p>
        </div>

        <div class="my-4 flex flex-wrap items-end gap-2">
          <span class="text-5xl font-bold text-neutral-10">
            {{ totalTrips }}
          </span>
          <span class="text-5xl font-bold text-neutral-10 capitalize">
            {{ $t('content.tripsUnit') }}
          </span>
          <div
            class="
              mb-2 flex size-9.5 shrink-0 items-center justify-center rounded-full bg-green-100
            "
          >
            <IconArrow class="size-5 -rotate-60 fill-green-700!" />
          </div>
        </div>

        <p
          class="text-lg font-bold capitalize"
          :class="tripsComparison.variant === 'success'
            ? 'text-success-500'
            : tripsComparison.variant === 'error'
              ? 'text-error-600'
              : 'text-neutral-10'"
        >
          {{ tripsComparison.text }}
        </p>
      </section>

      <div
        class="grid flex-1 grid-cols-2 gap-4"
      >
        <DashboardMetricStatCard
          :value="spendingValue"
          :label="$t('content.totalSpending')"
          :comparison="spendingComparison"
        >
          <template #icon>
            <IconMoneyBagBroken class="size-4" />
          </template>
        </DashboardMetricStatCard>

        <DashboardMetricStatCard
          :value="kmValue"
          :label="$t('content.totalKm')"
          :comparison="kmComparison"
        >
          <template #icon>
            <IconLocation class="size-4 fill-content-secondary" />
          </template>
        </DashboardMetricStatCard>

        <DashboardMetricStatCard
          :value="onTimeDeliveryValue"
          :label="$t('content.onTimeDelivery')"
          :comparison="onTimeDeliveryComparison"
        />
      </div>
    </div>
  </article>
</template>

<script setup lang="ts">
  import DashboardMetricStatCard from './DashboardMetricStatCard.vue'
  import type { ComparisonDisplay } from '../composables/useCompanyDashboard'

  defineProps<{
    totalTrips: number
    tripsComparison: ComparisonDisplay
    spendingValue: string
    spendingComparison: ComparisonDisplay
    kmValue: string
    kmComparison: ComparisonDisplay
    onTimeDeliveryValue: string
    onTimeDeliveryComparison: ComparisonDisplay
  }>()
</script>

<style scoped>

</style>
