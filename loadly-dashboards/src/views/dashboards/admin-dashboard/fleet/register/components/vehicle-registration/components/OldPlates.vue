<template>
  <details
    v-if="oldPlates.length"
    class="group w-full rounded-lg border border-neutral-200"
  >
    <summary
      class="
        flex cursor-pointer list-none items-center gap-2 p-3 text-start font-medium text-neutral-10
        [&::-webkit-details-marker]:hidden
      "
    >
      <IconChevron
        class="
          size-5 shrink-0 rotate-90 transition-transform
          group-open:rotate-270
        "
      />
      <span class="flex-1">{{ $t('content.oldPlates') }}</span>
      <span
        class="
          flex size-6 items-center justify-center rounded-full bg-neutral-200 text-sm font-normal
          text-neutral-10
        "
      >{{ oldPlates.length }}</span>
    </summary>

    <div class="border-t border-neutral-200 p-1">
      <BaseTable
        :columns="oldPlateColumns"
        :data="oldPlates"
      >
        <template #cell-supersededAt="{ row }">
          {{ formatSupersededAt(row.supersededAt) }}
        </template>
      </BaseTable>
    </div>
  </details>
</template>

<script setup lang="ts">
  import type { ApiPlateChangeHistory } from '@/types/admin-dashboard/response/vehicle/vehicle-detail'
  import type { TableColumn } from '@/types/ui'
  import { formatDate, formatTime } from '@/utils/time'

  const props = defineProps<{
    history?: ApiPlateChangeHistory[]
  }>()

  const { t, locale } = useI18n()

  const oldPlates = computed<ApiPlateChangeHistory[]>(() => {
    if (!Array.isArray(props.history)) return []

    return props.history as ApiPlateChangeHistory[]
  })

  const oldPlateColumns = computed<TableColumn[]>(() => [
    { key: 'plateLetters', label: t('form.plateLetters') },
    { key: 'plateNo', label: t('form.plateNumbers') },
    { key: 'supersededAt', label: t('table.supersededAt') },
  ])

  function formatSupersededAt(value: string): string {
    const date = new Date(value)
    if (Number.isNaN(date.getTime())) return value

    return `${formatDate(date, locale.value)} ${t('content.at')} ${formatTime(date, locale.value)}`
  }
</script>

<style scoped>

</style>
