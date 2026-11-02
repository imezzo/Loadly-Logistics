<template>
  <div
    class="
      grid grid-cols-1 gap-4
      xs:grid-cols-2
      md:grid-cols-6
    "
  >
    <CardAnalyticsCounter
      :count="statusCounts.totalDrivers"
      :label="$t('content.totalDrivers')"
      variant="default"
    />
    <CardAnalyticsCounter
      :count="statusCounts.available"
      :label="$t('content.availableDriver')"
      variant="success"
    />
    <CardAnalyticsCounter
      :count="statusCounts.busy"
      :label="$t('content.busyDrivers')"
      variant="info"
    />
    <CardAnalyticsCounter
      :count="statusCounts.onBreak"
      :label="$t('content.onBreakDrivers')"
      variant="warning"
    />
    <CardAnalyticsCounter
      :count="statusCounts.offline"
      :label="$t('content.offlineDrivers')"
      variant="default"
    />
    <CardAnalyticsCounter
      :count="statusCounts.offDuty"
      :label="$t('content.offDutyDrivers')"
      variant="error"
    />
  </div>
</template>

<script setup lang="ts">
  defineProps<{
    statusCounts: {
      totalDrivers: number
      available: number
      busy: number
      onBreak: number
      offline: number
      offDuty: number
    }
  }>()
</script>

<style scoped>

</style>
