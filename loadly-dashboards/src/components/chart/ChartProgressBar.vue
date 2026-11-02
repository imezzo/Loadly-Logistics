<template>
  <div
    v-for="(row, index) in rows"
    :key="index"
    class="
      mb-4 flex flex-col gap-2
      sm:flex-row sm:items-center sm:gap-4
    "
  >
    <span
      class="
        w-full shrink-0 font-medium text-neutral-10
        sm:w-24
      "
    >
      {{ row.label }}
    </span>

    <div
      :ref="index === 0
        ? setBarsContainerRef
        : undefined"
      class="flex w-full flex-1 gap-1.25"
    >
      <span
        v-for="count in segmentCount"
        :key="count"
        class="h-8 w-0.75 shrink-0 rounded-sm transition-colors duration-300"
        :class="count <= getActiveSegments(row)
          ? 'bg-primary'
          : 'bg-gray-200'"
      />
    </div>

    <span
      class="
        flex w-full shrink-0 font-medium text-neutral-10
        sm:ms-2 sm:w-28
      "
    >
      {{ row.activeCount }} {{ $t('title.trips') }}
    </span>
  </div>
</template>

<script setup lang="ts">
  import type { ChartProgressBarRow } from '@/types/chart'

  defineProps<{
    rows: ChartProgressBarRow[]
  }>()

  const SEGMENT_WIDTH_PX = 3
  const SEGMENT_GAP_PX = 5
  const CONTAINER_PADDING_PX = 20

  const barsContainer = ref<HTMLElement | null>(null)
  const segmentCount = ref(1)
  const resizeObserver = ref<ResizeObserver | null>(null)

  function setBarsContainerRef(refEl: Element | ComponentPublicInstance | null): void {
    barsContainer.value = refEl instanceof HTMLElement ? refEl : null
  }

  function updateSegmentCount(): void {
    const width = barsContainer.value?.clientWidth ?? 0
    if (!width) return

    const count = Math.floor((width - CONTAINER_PADDING_PX) / (SEGMENT_WIDTH_PX + SEGMENT_GAP_PX))
    segmentCount.value = Math.max(1, count)
  }

  function getPercentage(row: ChartProgressBarRow): number {
    if (!row.total) return 0

    return Math.round((row.activeCount / row.total) * 100)
  }

  function getActiveSegments(row: ChartProgressBarRow): number {
    return Math.round((getPercentage(row) / 100) * segmentCount.value)
  }

  onMounted(() => {
    updateSegmentCount()

    resizeObserver.value = new ResizeObserver(() => {
      updateSegmentCount()
    })

    if (barsContainer.value) {
      resizeObserver.value.observe(barsContainer.value)
    }
  })

  onBeforeUnmount(() => {
    resizeObserver.value?.disconnect()
  })
</script>

<style scoped>

</style>
