<template>
  <BaseTable
    :columns
    :data="tableRows"
  >
    <template #cell-rank="{ index }">
      <span class="inline-flex items-center gap-1.5">
        {{ index + 1 }}

        <span
          v-if="index === 0"
          aria-hidden="true"
          class="text-base"
        >🏆</span>
      </span>
    </template>
  </BaseTable>
</template>

<script setup lang="ts">
  import type { Driver } from '@/types/admin-dashboard/response/driver/driver'
  import type { TableColumn } from '@/types/ui'

  const props = defineProps<{
    drivers: Driver[]
  }>()

  const { t } = useI18n()

  const columns = computed<TableColumn[]>(() => [
    { key: 'rank', label: t('table.rate') },
    { key: 'fullName', label: t('table.driverName'), cellClass: 'font-medium text-neutral-20' },
    { key: 'trips', label: t('table.tripsNumber') },
  ])

  const tableRows = computed(() =>
    props.drivers.map(d => ({
      rank: '',
      fullName: d.fullName,
      trips: t('table.countTimes', { count: d.completedTripsCount }),
    })),
  )
</script>

<style scoped>

</style>
