<template>
  <section
    class="
      grid grid-cols-1 gap-4
      md:grid-cols-2
      lg:grid-cols-4
    "
  >
    <div
      v-for="item in statistics"
      :key="item.label"
      class="rounded-2xl border border-neutral-500 p-4 text-center"
    >
      <span class="text-2xl font-extrabold text-neutral-800">{{ item.value }}</span>
      <p class="mt-1.5 font-medium text-neutral-10">
        {{ item.label }}
      </p>
    </div>
  </section>
</template>

<script setup lang="ts">
  import { useDashboardStore } from '@/stores/company-dashboard/dashboard'
  import type { SelectOption } from '@/types/ui'

  const dashboardStore = useDashboardStore()
  const { t } = useI18n()

  const statistics = ref<SelectOption[]>([])

  async function fetchInvoicesStatistics(): Promise<void> {
    const { success, data } = await dashboardStore.fetchInvoicesStatistics()
    if (!success || !data) return

    statistics.value = [
      {
        label: t('content.invoiceBillingPaidVsUnpaid'),
        value: `${data.paidInvoicesCount} vs ${data.unpaidInvoicesCount}`,
      },
      {
        label: t('content.invoiceBillingTotalAmount'),
        value: data.totalInvoicesAmount,
      },
      {
        label: t('content.invoiceBillingTotalCollected'),
        value: data.totalPaidAmount,
      },
      {
        label: t('content.invoiceBillingRemainingBalance'),
        value: data.totalRemainingAmount,
      },
    ]
  }

  onMounted(fetchInvoicesStatistics)
</script>

<style scoped>

</style>
