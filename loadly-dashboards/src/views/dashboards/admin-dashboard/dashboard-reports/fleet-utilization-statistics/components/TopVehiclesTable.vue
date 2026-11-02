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
  import type { Vehicle } from '@/types/admin-dashboard/response/vehicle/vehicle'
  import type { TableColumn } from '@/types/ui'

  const props = defineProps<{
    vehicles: Vehicle[]
    isLoading: boolean
  }>()

  const { t } = useI18n()

  const columns = computed<TableColumn[]>(() => [
    { key: 'rank', label: t('table.rate') },
    { key: 'type', label: t('table.vehicleType') },
    { key: 'plate', label: t('table.plateNumber') },
    { key: 'model', label: t('table.brandModel') },
    { key: 'consumption', label: t('form.consumptionRate') },
    { key: 'trips', label: t('title.trips') },
  ])

  const tableRows = computed(() =>
    props.vehicles.map(v => ({
      rank: '',
      type: v.vehicleTypeName || t('content.notAvailable'),
      plate: `${v.plateNo} - ${v.plateLetters}`,
      model: v.brandModelName || t('content.notAvailable'),
      consumption: t('content.notAvailable'),
      trips: t('table.countTimes', { count: v.completedTripsCount }),
    })),
  )
</script>

<style scoped>

</style>
