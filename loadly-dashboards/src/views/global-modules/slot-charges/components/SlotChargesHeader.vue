<template>
  <div
    class="
      mb-6 flex flex-col gap-4
      md:flex-row md:items-end md:justify-between
    "
  >
    <div>
      <h2 class="text-lg font-bold text-neutral-800">
        {{ $t('content.vehicleFleetStatus', { period }) }}
      </h2>

      <p class="mt-1 text-sm text-content-secondary">
        {{ $t('content.vehicleFleetStatusDescription') }}
      </p>
    </div>

    <SlotChargesLegend />
  </div>
</template>

<script setup lang="ts">
  import SlotChargesLegend from './SlotChargesLegend.vue'

  const props = defineProps<{
    startDate: string
  }>()

  const period = computed(() => {
    if (!props.startDate) return ''

    const date = new Date(`${props.startDate}T00:00:00`)

    return date.toLocaleDateString('en-US', { month: 'long', year: 'numeric' })
  })
</script>

<style scoped>

</style>
