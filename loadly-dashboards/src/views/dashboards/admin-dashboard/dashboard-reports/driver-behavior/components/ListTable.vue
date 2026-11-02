<template>
  <BaseTable
    :columns="tableColumns"
    :data="rows"
  >
    <template #cell-id="{ row }">
      #{{ row.id }}
    </template>
  </BaseTable>

  <BasePagination
    :pagination="pagination"
    @set-page="setPage"
    @next-page="nextPage"
    @prev-page="prevPage"
    @update-items-per-page="updateItemsPerPage"
  />
</template>

<script setup lang="ts">
  import type { TableColumn } from '@/types/ui'
  import { useDriverBehaviorList } from '../composables/useDriverBehaviorList'

  const { t } = useI18n()

  const {
    fetchDriverBehavior,
    nextPage,
    pagination,
    prevPage,
    rows,
    setPage,
    updateItemsPerPage,
  } = useDriverBehaviorList()

  const tableColumns = computed<TableColumn[]>(() => [
    { key: 'id', label: t('table.id') },
    { key: 'name', label: t('table.name') },
    { key: 'phone', label: t('table.phoneNumber') },
    { key: 'driver', label: t('table.driverTrips') },
    { key: 'dispatcher', label: t('table.dispatcherTrips') },
    { key: 'assisted', label: t('table.assistedTrips') },
    { key: 'total', label: t('form.total') },
  ])

  onMounted(fetchDriverBehavior)
</script>

<style scoped>

</style>
