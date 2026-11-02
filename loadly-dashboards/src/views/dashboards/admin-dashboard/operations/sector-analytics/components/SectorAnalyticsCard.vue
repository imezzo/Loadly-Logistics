<template>
  <article
    class="
      rounded-2xl border p-5
      4xl:border-3 4xl:p-10
    "
    :class="
      featured
        ? 'border-warning-300 bg-warning-50'
        : 'border-neutral-500 bg-white'
    "
  >
    <div class="mb-4 flex flex-wrap items-center gap-3">
      <div class="flex min-w-0 items-center gap-2">
        <IconStar
          v-if="featured"
          class="
            size-5 shrink-0 text-warning-300
            4xl:size-22.5
          "
        />
        <h2
          class="
            truncate text-lg font-bold text-neutral-10
            4xl:text-5xl
          "
        >
          {{ sector.name }}
        </h2>
      </div>

      <p
        class="
          shrink-0 pt-5 text-sm font-medium text-neutral-10
          4xl:text-4xl
        "
      >
        {{ $t('content.doneTrips') }}

        <BaseFlipNumber
          :value="sector.doneTrips"
          class="ms-1 font-bold text-success-500"
        />
      </p>
    </div>

    <div
      class="
        mb-5 flex flex-col gap-4
        sm:flex-row sm:items-center sm:justify-between
      "
    >
      <div>
        <p
          class="
            4xl:text-65xl
            text-4xl font-bold text-neutral-10
          "
        >
          <BaseFlipNumber :value="sector.totalVehicles" />
        </p>

        <div class="flex flex-wrap items-end gap-2">
          <span
            class="
              mt-1 text-sm text-content-secondary
              4xl:text-4xl
            "
          >
            {{ $t('content.totalVehicle') }}
          </span>

          <span
            class="
              mb-1 flex items-center gap-1 pt-3 text-sm font-medium text-success-500
              4xl:text-3xl
            "
          >
            <BaseFlipNumber :value="sector.activeVehicles" />
            {{ $t('content.activeVehicleCount') }}
          </span>
        </div>
      </div>

      <div
        class="
          w-full
          sm:max-w-56
          4xl:max-w-150
        "
      >
        <div class="mb-1 flex justify-end">
          <span
            class="
              flex place-items-center gap-1 text-sm font-bold text-neutral-10
              4xl:text-4xl
            "
          >
            <BaseFlipNumber :value="sector.overallProgress" /> %
          </span>
        </div>

        <div
          class="
            h-2.5 overflow-hidden rounded-full bg-neutral-200
            4xl:h-5
          "
        >
          <div
            class="h-full rounded-full transition-all duration-300"
            :class="progressBarClass"
            :style="{ width: `${Math.min(sector.overallProgress, 100)}%` }"
          />
        </div>
      </div>
    </div>

    <div
      class="grid gap-3 border-t border-neutral-200 pt-4"
      :class="featured ? `
        grid-cols-2
        sm:grid-cols-5
      ` : `
        grid-cols-2
        sm:grid-cols-3
        4xl:grid-cols-4 4xl:gap-y-14
      `"
    >
      <div
        v-for="status in statusItems"
        :key="status.label"
        class="flex flex-wrap items-center gap-2 text-sm text-neutral-10"
      >
        <span
          class="
            size-2.5 shrink-0 rounded-full
            4xl:size-5.75
          "
          :class="status.dotClass"
        />
        <span
          class="
            text-sm
            4xl:text-4xl
          "
        >{{ status.label }}</span>


        <BaseFlipNumber
          :value="status.count"
          class="
            w-full shrink-0 font-bold
            4xl:text-4xl
          "
        />
      </div>
    </div>
  </article>
</template>

<script setup lang="ts">
  import type { SectorAnalyticsItem } from '@/types/admin-dashboard/response/sector-analytics'

  const props = defineProps<{
    sector: SectorAnalyticsItem
    featured?: boolean
  }>()

  const { t } = useI18n()

  const progressBarClass = computed(() => {
    const progress = Math.min(props.sector.overallProgress, 100)
    if (progress >= 60) return 'bg-success-500'
    if (progress > 20) return 'bg-warning-300'
    if (progress > 0) return 'bg-error-600'

    return 'bg-neutral-200'
  })

  const statusItems = computed(() => [
    {
      label: t('content.inProgress'),
      count: props.sector.tripStatuses.inProgress,
      dotClass: 'bg-info-500',
    },
    {
      label: t('content.scheduled'),
      count: props.sector.tripStatuses.scheduled,
      dotClass: 'bg-neutral-500',
    },
    {
      label: t('content.pending'),
      count: props.sector.tripStatuses.pending,
      dotClass: 'bg-warning-500',
    },
    {
      label: t('content.completed'),
      count: props.sector.tripStatuses.completed,
      dotClass: 'bg-success-500',
    },
    {
      label: t('content.cancelled'),
      count: props.sector.tripStatuses.cancelled,
      dotClass: 'bg-error-600',
    },
  ])
</script>

<style scoped>

</style>
