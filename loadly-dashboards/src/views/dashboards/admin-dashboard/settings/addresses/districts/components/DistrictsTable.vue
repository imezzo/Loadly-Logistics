<template>
  <BaseTable
    :columns="tableColumns"
    :data="districts"
  >
    <template #cell-city="{ row }">
      {{ getCityName(row.cityId) }}
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
  import type { District } from '@/types/admin-dashboard/response/district'
  import type { Pagination } from '@/types/response/pagination'
  import { Address, Permission } from '@/types/response/permission-group'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { useLookups } from '@/composables/useLookups'

  defineProps<{
    districts: District[]
    pagination: Pagination
    isLoading: boolean
  }>()

  const {
    hasPermission: hasEditPermission,
  } = usePermission(Permission.Address, Address.EditDistrict)

  const {
    hasPermission: hasDeletePermission,
  } = usePermission(Permission.Address, Address.DeleteDistrict)

  defineEmits<{
    setPage: [page: number]
    nextPage: []
    prevPage: []
    updateItemsPerPage: [size: number]
    edit: [district: District]
    delete: [district: District]
  }>()

  const { t } = useI18n()
  const { cities } = useLookups()

  const tableColumns = ref<TableColumn[]>([
    { key: 'name', label: t('table.name'), cellClass: 'font-medium text-neutral-20' },
    { key: 'city', label: t('table.city') },
    { key: 'action', label: t('table.action') },
  ])

  function getCityName(cityId: number): string {
    const city = cities.value.find(c => Number(c.value) === cityId)

    return city?.label || '-'
  }
</script>

<style scoped>

</style>

