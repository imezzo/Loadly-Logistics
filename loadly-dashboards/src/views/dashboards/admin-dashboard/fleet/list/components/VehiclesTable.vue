<template>
  <div class="flex flex-col gap-6">
    <div
      v-if="isLoading"
      class="flex items-center justify-center py-12"
    >
      <div class="text-neutral-400">
        {{ $t('loader.loading') }}
      </div>
    </div>

    <template v-else>
      <BaseTable
        :columns="tableColumns"
        :data="vehicles"
      >
        <template #cell-sector="{ row }">
          {{ row.sector?.name }}
        </template>

        <template #cell-contractor="{ row }">
          {{ row.contractorId
            ? $t('content.contractor')
            : $t('form.loadly')
          }}
        </template>

        <template #cell-contract="{ row }">
          <span class="flex max-w-45 flex-wrap">
            {{ row.contract?.name }} - {{ row.contract?.referenceNumber }}
          </span>
        </template>
        <template #cell-plateNumber="{ row }">
          <VehiclePlateNumber
            :plate-letters="row.plateLetters"
            :plate-no="row.plateNo"
            :plate-change-history="row.plateChangeHistory"
          />
        </template>

        <template #cell-headTail="{ row }">
          <div
            v-if="row.tailDisplayText"
            class="text-sm text-neutral-800"
          >
            <span class="font-medium">{{ $t('content.tail') }}</span>
            <span class="text-content-secondary"> · {{ row.tailDisplayText }}</span>
          </div>

          <div
            v-else-if="row.headDisplayText"
            class="text-sm text-neutral-800"
          >
            <span class="font-medium">{{ $t('content.head') }}</span>
            <span class="text-content-secondary"> · {{ row.headDisplayText }}</span>
          </div>

          <span
            v-else
            class="text-content-secondary"
            v-text="'—'"
          />
        </template>

        <template #cell-status="{ row }">
          <BaseChip
            :label="getDisplayName('VehicleStatus', row.status)"
            :variant="vehicleStatusVariants[Number(row.status)]"
          />
        </template>

        <template #cell-action="{ row }">
          <div class="flex items-center gap-2">
            <RouterLink
              v-if="hasViewPermission"
              :to="{ name: 'ViewVehicle', params: { id: row.id } }"
              class="btn-info-outline"
            >
              <IconEye
                visible
                class="size-4"
              />
            </RouterLink>

            <RouterLink
              v-if="hasEditPermission"
              :to="{ name: 'UpdateVehicle', params: { id: row.id } }"
              class="btn-action-success"
            >
              <IconEdit class="size-4" />
            </RouterLink>
          </div>
        </template>
      </BaseTable>
    </template>

    <BasePagination
      v-if="pagination && vehicles.length > 0"
      :pagination="pagination"
      @set-page="$emit('pageChange', $event)"
      @update-items-per-page="$emit('itemsPerPageChange', $event)"
    />
  </div>
</template>

<script setup lang="ts">
  import type { Vehicle } from '@/types/admin-dashboard/response/vehicle/vehicle'
  import type { Pagination } from '@/types/response/pagination'
  import type { TableColumn } from '@/types/ui'
  import { useEnum } from '@/composables/useEnum'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { Permission, Vehicle as VehiclePermission } from '@/types/response/permission-group'
  import { vehicleStatusVariants } from '@/utils/statusChipVariants'
  import VehiclePlateNumber from './VehiclePlateNumber.vue'

  defineProps<{
    vehicles: Vehicle[]
    pagination?: Pagination | null
    isLoading?: boolean
  }>()

  defineEmits<{
    pageChange: [page: number]
    itemsPerPageChange: [itemsPerPage: number]
  }>()

  const { t } = useI18n()
  const { getDisplayName } = useEnum()
  const { hasPermission: hasViewPermission } = usePermission(Permission.Vehicle, VehiclePermission.View)
  const { hasPermission: hasEditPermission } = usePermission(Permission.Vehicle, VehiclePermission.Edit)

  const tableColumns = computed<TableColumn[]>(() => [
    { key: 'contractor', label: t('table.owner') },
    { key: 'contract', label: t('table.contractName'), cellClass: 'break-all' },
    { key: 'sector', label: t('title.sectors') },
    { key: 'vehicleTypeName', label: t('table.vehicle'), cellClass: 'font-medium text-neutral-20' },
    { key: 'plateNumber', label: t('table.plateNumber') },
    { key: 'brandModelName', label: t('table.brandModel') },
    { key: 'driverName', label: t('table.driverName') },
    { key: 'headTail', label: t('table.headTail') },
    { key: 'status', label: t('table.status') },
    { key: 'action', label: t('table.action') },
  ])
</script>

<style scoped>

</style>
