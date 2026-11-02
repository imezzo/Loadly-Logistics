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
        {{ $t('title.vehicleBrands') }}
      </h1>

      <button
        v-if="hasCreatePermission"
        type="button"
        class="btn-primary"
        @click="isCreatePopupOpen = true"
      >
        {{ $t('button.addVehicleBrand') }}

        <IconPlus />
      </button>
    </div>

    <VehicleBrandsTable
      :vehicle-brands="vehicleBrands"
      :pagination="pagination"
      :is-loading="isLoading"
      @set-page="setPage"
      @next-page="nextPage"
      @prev-page="prevPage"
      @update-items-per-page="updateItemsPerPage"
      @edit="editVehicleBrand"
      @delete="deleteVehicleBrand"
    />

    <EditVehicleBrandPopup
      v-model="isEditPopupOpen"
      :vehicle-brand="selectedVehicleBrand"
      @refresh="fetchVehicleBrands"
    />

    <DeleteVehicleBrandPopup
      v-model="isDeletePopupOpen"
      :vehicle-brand-name="vehicleBrandToDelete?.name || ''"
      :is-loading="isDeleting"
      @confirm="confirmDelete"
    />

    <CreateVehicleBrandPopup
      v-model="isCreatePopupOpen"
      @refresh="fetchVehicleBrands"
    />
  </div>
</template>

<script setup lang="ts">
  import type { VehicleBrand } from '@/types/admin-dashboard/response/vehicle/vehicle-brand'
  import {
    VehicleBrand as VehicleBrandPermission,
    Permission,
  } from '@/types/response/permission-group'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { useVehicleBrandSettingsStore } from '@/stores/admin-dashboard/vehicle-brand-settings'
  import { useLoading } from '@/composables/useLoading'
  import { useList } from './composables/useList'
  import VehicleBrandsTable from './components/VehicleBrandsTable.vue'
  import EditVehicleBrandPopup from './components/EditVehicleBrandPopup.vue'
  import DeleteVehicleBrandPopup from './components/DeleteVehicleBrandPopup.vue'
  import CreateVehicleBrandPopup from './components/CreateVehicleBrandPopup.vue'

  const { t } = useI18n()

  const {
    hasPermission: hasCreatePermission,
  } = usePermission(Permission.VehicleBrand, VehicleBrandPermission.Create)

  const breadcrumbItems = [
    { label: t('title.settings'), to: '/settings' },
    { label: t('title.vehicleBrands') },
  ]

  const {
    fetchVehicleBrands,
    vehicleBrands,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  } = useList()

  const vehicleBrandSettingsStore = useVehicleBrandSettingsStore()
  const { isLoading: isDeleting, setLoading: setDeleting } = useLoading()

  const isCreatePopupOpen = ref(false)
  const isEditPopupOpen = ref(false)
  const isDeletePopupOpen = ref(false)
  const selectedVehicleBrand = ref<VehicleBrand | null>(null)
  const vehicleBrandToDelete = ref<VehicleBrand | null>(null)

  function editVehicleBrand(vehicleBrand: VehicleBrand) {
    selectedVehicleBrand.value = vehicleBrand
    isEditPopupOpen.value = true
  }

  function deleteVehicleBrand(vehicleBrand: VehicleBrand) {
    vehicleBrandToDelete.value = vehicleBrand
    isDeletePopupOpen.value = true
  }

  async function confirmDelete() {
    if (!vehicleBrandToDelete.value) return

    setDeleting(true)
    const { success } = await vehicleBrandSettingsStore.deleteVehicleBrand(
      vehicleBrandToDelete.value.id
    )
    setDeleting(false)

    if (success) {
      isDeletePopupOpen.value = false
      vehicleBrandToDelete.value = null
      fetchVehicleBrands()
    }
  }

  watch(isEditPopupOpen, (newValue) => {
    if (!newValue) {
      selectedVehicleBrand.value = null
    }
  })

  watch(isDeletePopupOpen, (newValue) => {
    if (!newValue) {
      vehicleBrandToDelete.value = null
    }
  })

  onMounted(async () => {
    fetchVehicleBrands()
  })
</script>

<style scoped>

</style>
