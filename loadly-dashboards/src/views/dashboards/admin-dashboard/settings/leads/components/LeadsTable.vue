<template>
  <BaseTable
    :columns="tableColumns"
    :data="leads"
  >
    <template #cell-action="{ row }">
      <LeadsTableActions
        @edit="$emit('edit', row)"
        @delete="$emit('delete', row)"
      />
    </template>
  </BaseTable>

  <BasePagination
    :pagination
    @set-page="setPage"
    @next-page="nextPage"
    @prev-page="prevPage"
    @update-items-per-page="updateItemsPerPage"
  />
</template>

<script setup lang="ts">
  import type { Lead } from '@/types/admin-dashboard/response/lead/lead'
  import type { TableColumn } from '@/types/ui'
  import { useList } from '../composables/useList'
  import LeadsTableActions from './LeadsTableActions.vue'

  defineEmits<{
    delete: [lead: Lead]
    edit: [lead: Lead]
  }>()

  const { t } = useI18n()
  const {
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
    leads,
  } = useList()

  const tableColumns = ref<TableColumn[]>([
    { key: 'fullName', label: t('table.fullName') },
    { key: 'phone', label: t('table.phoneNumber') },
    { key: 'email', label: t('table.email') },
    { key: 'totalCompletedTrips', label: t('table.totalCompletedTrips') },
    { key: 'action', label: t('table.action') },
  ])
</script>

<style scoped>

</style>
