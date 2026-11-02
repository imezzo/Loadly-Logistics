<template>
  <BaseTable
    :columns="tableColumns"
    :data="notifications"
  >
    <template #cell-action="{ row }">
      <RouterLink
        :to="{ name: 'ViewBroadcastNotification', params: { id: row.id } }"
        class="btn-info-outline"
      >
        <IconEye
          visible
          class="size-4"
        />
      </RouterLink>
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
  import { useList } from '../composables/useList'

  const { t } = useI18n()

  const {
    nextPage,
    notifications,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  } = useList()

  const tableColumns = ref<TableColumn[]>([
    { key: 'modelType', label: t('table.modelType') },
    { key: 'createdAt', label: t('table.dateSend') },
    { key: 'channel', label: t('table.channel') },
    { key: 'received', label: t('table.received') },
    { key: 'failed', label: t('table.failed') },
    { key: 'action', label: t('table.action') },
  ])
</script>

<style scoped>

</style>
