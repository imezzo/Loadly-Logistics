<template>
  <article
    class="
      flex items-center justify-between gap-4 rounded-2xl border border-gray-50 bg-white px-4 py-5
    "
  >
    <div class="min-w-0">
      <p class="text-2xl font-bold text-neutral-10">
        {{ $t('content.prefixVsSuffix', { prefix: current, suffix: previous }) }}
      </p>

      <p class="mt-1 text-sm text-content-secondary">
        {{ t('content.vehiclesAssignTripsVsLastWeek') }}
      </p>
    </div>

    <div
      class="flex size-10 shrink-0 items-center justify-center rounded-full bg-green-100"
    >
      <IconArrow class="size-4 -rotate-60 fill-green-700!" />
    </div>
  </article>
</template>

<script setup lang="ts">
  import IconArrow from '@/components/icon/IconArrow.vue'
  import { useDashboardStore } from '@/stores/admin-dashboard/dashboard'
  import { useLoading } from '@/composables/useLoading'

  const { t } = useI18n()
  const dashboardStore = useDashboardStore()
  const { setLoading } = useLoading()

  const current = ref(0)
  const previous = ref(0)

  async function fetchComparison(): Promise<void> {
    setLoading(true)
    const { success, data } = await dashboardStore.fetchWeeklyVehicleTripsComparison()
    setLoading(false)

    if (!success) return

    current.value = data.currentWeekAssignedVehicleTripsCount
    previous.value = data.previousWeekAssignedVehicleTripsCount
  }

  fetchComparison()
</script>

<style scoped>

</style>
