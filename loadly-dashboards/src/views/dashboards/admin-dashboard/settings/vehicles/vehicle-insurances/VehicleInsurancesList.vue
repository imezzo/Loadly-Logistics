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
        {{ $t('title.vehicleInsurances') }}
      </h1>

      <button
        v-if="hasCreatePermission"
        type="button"
        class="btn-primary"
        @click="isCreatePopupOpen = true"
      >
        {{ $t('button.addVehicleInsurance') }}

        <IconPlus />
      </button>
    </div>

    <VehicleInsurancesTable
      :vehicle-insurances="vehicleInsurances"
      :pagination="pagination"
      :is-loading="isLoading"
      @set-page="setPage"
      @next-page="nextPage"
      @prev-page="prevPage"
      @update-items-per-page="updateItemsPerPage"
      @edit="editVehicleInsurance"
      @delete="deleteVehicleInsurance"
    />

    <EditVehicleInsurancePopup
      v-model="isEditPopupOpen"
      :vehicle-insurance="selectedVehicleInsurance"
      @refresh="fetchVehicleInsurances"
    />

    <DeleteVehicleInsurancePopup
      v-model="isDeletePopupOpen"
      :vehicle-insurance-name="vehicleInsuranceToDelete?.name || ''"
      :is-loading="isDeleting"
      @confirm="confirmDelete"
    />

    <CreateVehicleInsurancePopup
      v-model="isCreatePopupOpen"
      @refresh="fetchVehicleInsurances"
    />
  </div>
</template>

<script setup lang="ts">
  import type { VehicleInsurance } from '@/types/admin-dashboard/response/vehicle/vehicle-insurance'
  import {
    VehicleInsurance as VehicleInsurancePermission,
    Permission,
  } from '@/types/response/permission-group'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { useVehicleInsuranceSettingsStore } from '@/stores/admin-dashboard/vehicle-insurance-settings'
  import { useLoading } from '@/composables/useLoading'
  import { useList } from './composables/useList'
  import VehicleInsurancesTable from './components/VehicleInsurancesTable.vue'
  import EditVehicleInsurancePopup from './components/EditVehicleInsurancePopup.vue'
  import DeleteVehicleInsurancePopup from './components/DeleteVehicleInsurancePopup.vue'
  import CreateVehicleInsurancePopup from './components/CreateVehicleInsurancePopup.vue'

  const { t } = useI18n()

  const {
    hasPermission: hasCreatePermission,
  } = usePermission(
    Permission.VehicleInsurance,
    VehicleInsurancePermission.Create
  )

  const breadcrumbItems = [
    { label: t('title.settings'), to: '/settings' },
    { label: t('title.vehicleInsurances') },
  ]

  const {
    fetchVehicleInsurances,
    vehicleInsurances,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  } = useList()

  const vehicleInsuranceSettingsStore = useVehicleInsuranceSettingsStore()
  const { isLoading: isDeleting, setLoading: setDeleting } = useLoading()

  const isCreatePopupOpen = ref(false)
  const isEditPopupOpen = ref(false)
  const isDeletePopupOpen = ref(false)
  const selectedVehicleInsurance = ref<VehicleInsurance | null>(null)
  const vehicleInsuranceToDelete = ref<VehicleInsurance | null>(null)

  function editVehicleInsurance(vehicleInsurance: VehicleInsurance) {
    selectedVehicleInsurance.value = vehicleInsurance
    isEditPopupOpen.value = true
  }

  function deleteVehicleInsurance(vehicleInsurance: VehicleInsurance) {
    vehicleInsuranceToDelete.value = vehicleInsurance
    isDeletePopupOpen.value = true
  }

  async function confirmDelete() {
    if (!vehicleInsuranceToDelete.value) return

    setDeleting(true)
    const { success } =
      await vehicleInsuranceSettingsStore.deleteVehicleInsurance(
        vehicleInsuranceToDelete.value.id
      )
    setDeleting(false)

    if (success) {
      isDeletePopupOpen.value = false
      vehicleInsuranceToDelete.value = null
      fetchVehicleInsurances()
    }
  }

  watch(isEditPopupOpen, (newValue) => {
    if (!newValue) {
      selectedVehicleInsurance.value = null
    }
  })

  watch(isDeletePopupOpen, (newValue) => {
    if (!newValue) {
      vehicleInsuranceToDelete.value = null
    }
  })

  onMounted(async () => {
    fetchVehicleInsurances()
  })
</script>

<style scoped>

</style>
