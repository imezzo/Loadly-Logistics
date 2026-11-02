<template>
  <BaseTable
    :columns="tableColumns"
    :data="cities"
  >
    <template #cell-governerate="{ row }">
      {{ getGovernorateName(row.governerateId) }}
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
  import type { City } from '@/types/admin-dashboard/response/city'
  import type { Pagination } from '@/types/response/pagination'
  import { Address, Permission } from '@/types/response/permission-group'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { useLookups } from '@/composables/useLookups'

  defineProps<{
    cities: City[]
    pagination: Pagination
    isLoading: boolean
  }>()

  defineEmits<{
    setPage: [page: number]
    nextPage: []
    prevPage: []
    updateItemsPerPage: [size: number]
    edit: [city: City]
    delete: [city: City]
  }>()

  const {
    hasPermission: hasEditPermission,
  } = usePermission(Permission.Address, Address.EditCity)

  const {
    hasPermission: hasDeletePermission,
  } = usePermission(Permission.Address, Address.DeleteCity)

  const { t } = useI18n()
  const { governments } = useLookups()

  const tableColumns = ref<TableColumn[]>([
    { key: 'name', label: t('table.name'), cellClass: 'font-medium text-neutral-20' },
    { key: 'governerate', label: t('table.governorate') },
    { key: 'action', label: t('table.action') },
  ])

  function getGovernorateName(governerateId: number): string {
    const governorate = governments.value.find(gov => Number(gov.value) === governerateId)

    return governorate?.label || '-'
  }
</script>

<style scoped>

</style>
