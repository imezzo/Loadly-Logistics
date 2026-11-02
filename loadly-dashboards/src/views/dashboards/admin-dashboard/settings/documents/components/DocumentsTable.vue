<template>
  <BaseTable
    :columns="tableColumns"
    :data="documents"
  >
    <template #cell-entityType="{ row }">
      {{ getEntityTypeLabel(row.entityId) }}
    </template>

    <template #cell-required="{ row }">
      {{ row.required ? $t('form.required') : $t('form.optional') }}
    </template>

    <template #cell-status="{ row }">
      <BaseChip
        :label="getDisplayName('DocumentStatus', Number(row.status))"
        :variant="documentStatusVariants[Number(row.status)]"
      />
    </template>

    <template #cell-action="{ row }">
      <div class="flex items-center gap-2">
        <button
          v-if="hasEditPermission"
          type="button"
          class="btn-action-success"
          @click="$emit('edit', row)"
        >
          <IconEdit class="size-4" />
        </button>

        <button
          v-if="hasDeletePermission"
          type="button"
          class="btn-action-error"
          @click="$emit('delete', row)"
        >
          <IconTrash class="size-4" />
        </button>
      </div>
    </template>
  </BaseTable>

  <BasePagination
    :pagination="pagination"
    @set-page="$emit('setPage', $event)"
    @next-page="$emit('nextPage')"
    @prev-page="$emit('prevPage')"
    @update-items-per-page="$emit('updateItemsPerPage', $event)"
  />
</template>

<script setup lang="ts">
  import type { TableColumn } from '@/types/ui'
  import type { Document } from '@/types/admin-dashboard/response/document'
  import type { Pagination } from '@/types/response/pagination'
  import { Document as DocumentPermission, Permission } from '@/types/response/permission-group'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { useEnum } from '@/composables/useEnum'
  import { documentStatusVariants } from '@/utils/statusChipVariants'

  defineProps<{
    documents: Document[]
    pagination: Pagination
    isLoading: boolean
  }>()

  defineEmits<{
    setPage: [page: number]
    nextPage: []
    prevPage: []
    updateItemsPerPage: [size: number]
    edit: [document: Document]
    delete: [document: Document]
  }>()

  const { enums, getDisplayName } = useEnum()
  const { t } = useI18n()
  const {
    hasPermission: hasEditPermission,
  } = usePermission(Permission.Document, DocumentPermission.Edit)

  const {
    hasPermission: hasDeletePermission,
  } = usePermission(Permission.Document, DocumentPermission.Delete)

  const tableColumns = ref<TableColumn[]>([
    { key: 'label', label: t('table.name'), cellClass: 'font-medium text-neutral-20' },
    { key: 'entityType', label: t('form.entityType') },
    { key: 'required', label: t('form.required') },
    { key: 'status', label: t('table.status') },
    { key: 'action', label: t('table.action') },
  ])

  function getEntityTypeLabel(entityId?: number): string {
    const option = enums.value?.DocumentEntityType?.find(option => +option.value === entityId)

    return option?.label ?? '-'
  }
</script>

<style scoped>

</style>
