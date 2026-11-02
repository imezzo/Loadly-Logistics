<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <div
      class="
        mb-4 flex flex-col gap-4
        sm:flex-row sm:items-center sm:justify-between
      "
    >
      <h1 class="text-xl font-bold text-neutral-800">
        {{ $t('title.vehicleCapacityUnits') }}
      </h1>

      <button
        v-if="hasCreatePermission"
        type="button"
        class="btn-primary"
        @click="isCreatePopupOpen = true"
      >
        {{ $t('button.addVehicleCapacityUnit') }}

        <IconPlus />
      </button>
    </div>

    <VehicleCapacityUnitsTable
      :vehicle-capacity-units="vehicleCapacityUnits"
      :pagination="pagination"
      :is-loading="isLoading"
      @set-page="setPage"
      @next-page="nextPage"
      @prev-page="prevPage"
      @update-items-per-page="updateItemsPerPage"
      @edit="editVehicleCapacityUnit"
      @delete="deleteVehicleCapacityUnit"
    />

    <EditVehicleCapacityUnitPopup
      v-model="isEditPopupOpen"
      :vehicle-capacity-unit="selectedVehicleCapacityUnit"
      @refresh="fetchVehicleCapacityUnits"
    />

    <DeleteVehicleCapacityUnitPopup
      v-model="isDeletePopupOpen"
      :vehicle-capacity-unit-name="vehicleCapacityUnitToDelete?.name || ''"
      :is-loading="isDeleting"
      @confirm="confirmDelete"
    />

    <CreateVehicleCapacityUnitPopup
      v-model="isCreatePopupOpen"
      @refresh="fetchVehicleCapacityUnits"
    />
  </div>
</template>

<script setup lang="ts">
  import type { VehicleCapacityUnit } from '@/types/admin-dashboard/response/vehicle/vehicle-capacity-unit'
  import {
    VehicleCapacityUnit as VehicleCapacityUnitPermission,
    Permission,
  } from '@/types/response/permission-group'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { useVehicleCapacityUnitSettingsStore } from '@/stores/admin-dashboard/vehicle-capacity-unit-settings'
  import { useLoading } from '@/composables/useLoading'
  import { useList } from './composables/useList'
  import VehicleCapacityUnitsTable from './components/VehicleCapacityUnitsTable.vue'
  import EditVehicleCapacityUnitPopup from './components/EditVehicleCapacityUnitPopup.vue'
  import DeleteVehicleCapacityUnitPopup from './components/DeleteVehicleCapacityUnitPopup.vue'
  import CreateVehicleCapacityUnitPopup from './components/CreateVehicleCapacityUnitPopup.vue'

  const { t } = useI18n()

  const {
    hasPermission: hasCreatePermission,
  } = usePermission(
    Permission.VehicleCapacityUnit,
    VehicleCapacityUnitPermission.Create
  )

  const breadcrumbItems = [
    { label: t('title.settings'), to: '/settings' },
    { label: t('title.vehicleCapacityUnits') },
  ]

  const {
    fetchVehicleCapacityUnits,
    vehicleCapacityUnits,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  } = useList()

  const vehicleCapacityUnitSettingsStore = useVehicleCapacityUnitSettingsStore()
  const { isLoading: isDeleting, setLoading: setDeleting } = useLoading()

  const isCreatePopupOpen = ref(false)
  const isEditPopupOpen = ref(false)
  const isDeletePopupOpen = ref(false)
  const selectedVehicleCapacityUnit = ref<VehicleCapacityUnit | null>(null)
  const vehicleCapacityUnitToDelete = ref<VehicleCapacityUnit | null>(null)

  function editVehicleCapacityUnit(vehicleCapacityUnit: VehicleCapacityUnit) {
    selectedVehicleCapacityUnit.value = vehicleCapacityUnit
    isEditPopupOpen.value = true
  }

  function deleteVehicleCapacityUnit(
    vehicleCapacityUnit: VehicleCapacityUnit
  ) {
    vehicleCapacityUnitToDelete.value = vehicleCapacityUnit
    isDeletePopupOpen.value = true
  }

  async function confirmDelete() {
    if (!vehicleCapacityUnitToDelete.value) return

    setDeleting(true)
    const { success } =
      await vehicleCapacityUnitSettingsStore.deleteVehicleCapacityUnit(
        vehicleCapacityUnitToDelete.value.id
      )
    setDeleting(false)

    if (success) {
      isDeletePopupOpen.value = false
      vehicleCapacityUnitToDelete.value = null
      fetchVehicleCapacityUnits()
    }
  }

  watch(isEditPopupOpen, (newValue) => {
    if (!newValue) {
      selectedVehicleCapacityUnit.value = null
    }
  })

  watch(isDeletePopupOpen, (newValue) => {
    if (!newValue) {
      vehicleCapacityUnitToDelete.value = null
    }
  })

  onMounted(async () => {
    fetchVehicleCapacityUnits()
  })
</script>

<style scoped>

</style>
