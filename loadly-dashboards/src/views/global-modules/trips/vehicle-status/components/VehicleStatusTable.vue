<template>
  <BaseTable
    :columns="tableColumns"
    :data="vehicles"
  >
    <template #cell-contract="{ row }">
      <span class="flex max-w-45 flex-wrap">
        {{ row.contract?.name }} - {{ row.contract?.referenceNumber }}
      </span>
    </template>

    <template #cell-sectorName="{ row }">
      <span class="w-full text-sm text-neutral-10">
        {{ row.sector?.name }}
      </span>
    </template>

    <template #cell-plateNumber="{ row }">
      <bdi class="me-1">
        {{ row.plateLetters }}
      </bdi>

      {{ row.plateNo }}
    </template>

    <template #cell-status="{ row }">
      <BaseChip
        :label="getDisplayName('VehicleStatus', row.status)"
        :variant="vehicleStatusVariants[Number(row.status)]"
      />

      <div
        v-if="row.status === VehicleStatus.Available && row.waitingForLoading"
        class="
          relative mt-1 flex items-center gap-1 ps-2 text-xs text-warning-500
          before:absolute before:start-0 before:top-1/2 before:size-1.5 before:-translate-y-1/2
          before:rounded-full before:bg-warning-500 before:content-['']
        "
      >
        {{ $t('content.parkingWaitingLoading') }}
      </div>
    </template>

    <template #cell-driverName="{ row }">
      <div class="flex flex-wrap items-center gap-2">
        <span
          v-if="row.driverName"
          class="text-sm text-neutral-10"
        >
          {{ row.driverName }}
        </span>

        <template
          v-if="row.status !== VehicleStatus.InRoute
            && hasAssignDriverPermission
            && Number(row.vehicleUnitType) !== VehicleUnitType.Tail"
        >
          <button
            v-if="row.driverName"
            type="button"
            class="rounded-md border border-primary bg-white p-0.5"
            :aria-label="$t('button.change')"
            @click="openChangeDriverPopup(row)"
          >
            <IconReload class="size-5 text-primary" />
          </button>

          <button
            v-else-if="row.status === VehicleStatus.Available"
            type="button"
            class="
              btn-success flex h-8 shrink-0 items-center gap-1 rounded-lg px-2 py-1 text-xs
              font-bold
            "
            :aria-label="$t('button.assignDriver')"
            @click="openChangeDriverPopup(row)"
          >
            <IconPlus class="size-5" />
            {{ $t('button.assignDriver') }}
          </button>

          <span v-else>-</span>
        </template>

        <span v-else>-</span>
      </div>
    </template>

    <template #cell-location="{ row }">
      <template v-if="row.status === VehicleStatus.Available">
        <span
          v-if="row.assignedParkingFacility"
          class="block max-w-40 text-sm wrap-break-word"
        >
          {{ row.assignedParkingFacility?.name }} - {{ row.assignedParkingFacility?.address }}
        </span>

        <button
          v-else-if="hasEnterPermission"
          type="button"
          class="btn-primary-outline rounded-xl underline"
          @click="openLocationPopup(row)"
        >
          {{ $t('table.location') }}
          <IconChevron class="[&_path]:stroke-primary" />
        </button>
      </template>

      <span
        v-if="row.status === VehicleStatus.OutOfService && row.currentServiceFacility"
        class="block max-w-40 text-sm wrap-break-word"
      >
        {{ row.currentServiceFacility.name }} - {{ row.currentServiceFacility.address }}
      </span>
    </template>

    <template #cell-action="{ row }">
      <div class="flex items-center gap-2">
        <button
          v-if="hasExitPermission
            && (row.status === VehicleStatus.OutOfService || row.waitingForLoading)"
          type="button"
          class="btn-action-error"
          :aria-label="$t('a11y.exit')"
          @click="exitFacility(row)"
        >
          <IconExit class="size-4" />
        </button>

        <button
          v-if="hasViewPermission"
          type="button"
          class="btn-info-outline ms-auto"
          :aria-label="$t('a11y.view')"
          @click="openActivityLogPopup(row)"
        >
          <IconEye
            visible
            class="size-4"
          />
        </button>
      </div>
    </template>
  </BaseTable>

  <BasePagination
    v-if="pagination && vehicles.length"
    :pagination="pagination"
    @set-page="$emit('pageChange', $event)"
    @update-items-per-page="$emit('itemsPerPageChange', $event)"
  />

  <ChooseVehicleLocationPopup
    v-if="isLocationPopupOpen"
    v-model="isLocationPopupOpen"
    :vehicle="selectedVehicle"
    @fetch-vehicles="$emit('fetchVehicles')"
  />

  <ExitFacilityVisitPopup
    v-if="isExitPopupOpen && selectedVehicle"
    v-model="isExitPopupOpen"
    :vehicle="selectedVehicle"
    @fetch-vehicles="$emit('fetchVehicles')"
  />

  <VehicleActivityLogPopup
    v-if="isActivityLogPopupOpen && selectedVehicle"
    v-model="isActivityLogPopupOpen"
    :visits="selectedVehicle.vehicleFacilityVisits"
  />

  <ChangeVehicleDriverPopup
    v-if="isChangeDriverPopupOpen && selectedVehicle"
    v-model="isChangeDriverPopupOpen"
    :vehicle="selectedVehicle"
    @fetch-vehicles="$emit('fetchVehicles')"
  />

  <ConfirmExitParkingPopup
    v-if="isExitParkingConfirmOpen && selectedVehicle"
    v-model="isExitParkingConfirmOpen"
    :vehicle="selectedVehicle"
    @fetch-vehicles="$emit('fetchVehicles')"
  />
</template>

<script setup lang="ts">
  import type { Vehicle } from '@/types/admin-dashboard/response/vehicle/vehicle'
  import type { Pagination } from '@/types/response/pagination'
  import type { TableColumn } from '@/types/ui'
  import { useEnum } from '@/composables/useEnum'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { Permission, VehicleFacilityVisit, Vehicle as VehiclePermission } from '@/types/response/permission-group'
  import { vehicleStatusVariants } from '@/utils/statusChipVariants'
  import { VehicleStatus, VehicleUnitType } from '@/types/admin-dashboard/response/vehicle/vehicle.enums'

  const ChooseVehicleLocationPopup = defineAsyncComponent(
    () => import('./vehicle-location/ChooseVehicleLocationPopup.vue'),
  )
  const ExitFacilityVisitPopup = defineAsyncComponent(
    () => import('./ExitFacilityVisitPopup.vue'),
  )
  const VehicleActivityLogPopup = defineAsyncComponent(
    () => import('./VehicleActivityLogPopup.vue'),
  )
  const ChangeVehicleDriverPopup = defineAsyncComponent(
    () => import('./ChangeVehicleDriverPopup.vue'),
  )
  const ConfirmExitParkingPopup = defineAsyncComponent(
    () => import('./ConfirmExitParkingPopup.vue'),
  )

  defineProps<{
    pagination?: Pagination | null
    vehicles: Vehicle[]
  }>()

  defineEmits<{
    pageChange: [page: number]
    itemsPerPageChange: [itemsPerPage: number]
    fetchVehicles: []
  }>()

  const { t } = useI18n()
  const { getDisplayName } = useEnum()
  const { hasPermission: hasViewPermission } = usePermission(Permission.Vehicle, VehiclePermission.View)
  const { hasPermission: hasAssignDriverPermission } = usePermission(Permission.Vehicle, VehiclePermission.AssignDriver)
  const { hasPermission: hasEnterPermission } = usePermission(Permission.VehicleFacilityVisit, VehicleFacilityVisit.Enter)
  const { hasPermission: hasExitPermission } = usePermission(Permission.VehicleFacilityVisit, VehicleFacilityVisit.Exit)

  const isLocationPopupOpen = ref(false)
  const selectedVehicle = ref<Vehicle | null>(null)
  const isExitPopupOpen = ref(false)
  const isActivityLogPopupOpen = ref(false)
  const isChangeDriverPopupOpen = ref(false)
  const isExitParkingConfirmOpen = ref(false)

  const tableColumns = computed<TableColumn[]>(() => [
    { key: 'contract', label: t('table.contractName'), cellClass: 'break-all' },
    { key: 'companyName', label: t('table.companyName'), cellClass: 'break-all' },
    { key: 'vehicleTypeName', label: t('table.vehicle') },
    { key: 'plateNumber', label: t('table.plateNumber') },
    { key: 'brandModelName', label: t('table.brandModel') },
    { key: 'sectorName', label: t('table.sector') },
    { key: 'driverName', label: t('table.driverName') },
    { key: 'status', label: t('table.status') },
    { key: 'location', label: t('table.location') },
    { key: 'action', label: t('table.action') },
  ])

  function openLocationPopup(vehicle: Vehicle): void {
    selectedVehicle.value = vehicle
    isLocationPopupOpen.value = true
  }

  function openChangeDriverPopup(vehicle: Vehicle): void {
    selectedVehicle.value = vehicle
    isChangeDriverPopupOpen.value = true
  }

  function openExitPopup(vehicle: Vehicle): void {
    selectedVehicle.value = vehicle
    isExitPopupOpen.value = true
  }

  function exitFacility(vehicle: Vehicle): void {
    if (vehicle.waitingForLoading) {
      selectedVehicle.value = vehicle
      isExitParkingConfirmOpen.value = true

      return
    }

    openExitPopup(vehicle)
  }

  function openActivityLogPopup(vehicle: Vehicle): void {
    selectedVehicle.value = vehicle
    isActivityLogPopupOpen.value = true
  }
</script>

<style scoped>

</style>
