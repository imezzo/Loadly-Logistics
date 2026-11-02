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
        {{ $t('title.vehicleBrandModels') }}
      </h1>

      <button
        v-if="hasCreatePermission"
        type="button"
        class="btn-primary"
        @click="isCreatePopupOpen = true"
      >
        {{ $t('button.addVehicleBrandModel') }}

        <IconPlus />
      </button>
    </div>

    <VehicleBrandModelsTable
      :vehicle-brand-models="vehicleBrandModels"
      :vehicle-brands="vehicleBrands"
      :pagination="pagination"
      :is-loading="isLoading"
      @set-page="setPage"
      @next-page="nextPage"
      @prev-page="prevPage"
      @update-items-per-page="updateItemsPerPage"
      @edit="editVehicleBrandModel"
      @delete="deleteVehicleBrandModel"
    />

    <EditVehicleBrandModelPopup
      v-model="isEditPopupOpen"
      :vehicle-brand-model="selectedVehicleBrandModel"
      :vehicle-brands="vehicleBrands"
      @refresh="fetchVehicleBrandModels"
    />

    <DeleteVehicleBrandModelPopup
      v-model="isDeletePopupOpen"
      :vehicle-brand-model-name="vehicleBrandModelToDelete?.name || ''"
      :is-loading="isDeleting"
      @confirm="confirmDelete"
    />

    <CreateVehicleBrandModelPopup
      v-model="isCreatePopupOpen"
      :vehicle-brands="vehicleBrands"
      @refresh="fetchVehicleBrandModels"
    />
  </div>
</template>

<script setup lang="ts">
  import type { VehicleBrandModel } from '@/types/admin-dashboard/response/vehicle/vehicle-brand-model'
  import type { VehicleBrand } from '@/types/admin-dashboard/response/vehicle/vehicle-brand'
  import {
    VehicleBrandModel as VehicleBrandModelPermission,
    Permission,
  } from '@/types/response/permission-group'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { useVehicleBrandModelSettingsStore } from '@/stores/admin-dashboard/vehicle-brand-model-settings'
  import { useVehicleBrandSettingsStore } from '@/stores/admin-dashboard/vehicle-brand-settings'
  import { useLoading } from '@/composables/useLoading'
  import { useList } from './composables/useList'
  import VehicleBrandModelsTable from './components/VehicleBrandModelsTable.vue'
  import EditVehicleBrandModelPopup from './components/EditVehicleBrandModelPopup.vue'
  import DeleteVehicleBrandModelPopup from './components/DeleteVehicleBrandModelPopup.vue'
  import CreateVehicleBrandModelPopup from './components/CreateVehicleBrandModelPopup.vue'

  const { t } = useI18n()

  const {
    hasPermission: hasCreatePermission,
  } = usePermission(Permission.VehicleBrandModel, VehicleBrandModelPermission.Create)

  const breadcrumbItems = [
    { label: t('title.settings'), to: '/settings' },
    { label: t('title.vehicleBrandModels') },
  ]

  const {
    fetchVehicleBrandModels,
    vehicleBrandModels,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  } = useList()

  const vehicleBrandModelSettingsStore = useVehicleBrandModelSettingsStore()
  const vehicleBrandSettingsStore = useVehicleBrandSettingsStore()
  const { vehicleBrandsList } = storeToRefs(vehicleBrandSettingsStore)
  const vehicleBrands = computed<VehicleBrand[]>(
    () => vehicleBrandsList.value?.vehicleBrands ?? []
  )

  const { isLoading: isDeleting, setLoading: setDeleting } = useLoading()

  const isCreatePopupOpen = ref(false)
  const isEditPopupOpen = ref(false)
  const isDeletePopupOpen = ref(false)
  const selectedVehicleBrandModel = ref<VehicleBrandModel | null>(null)
  const vehicleBrandModelToDelete = ref<VehicleBrandModel | null>(null)

  function editVehicleBrandModel(vehicleBrandModel: VehicleBrandModel) {
    selectedVehicleBrandModel.value = vehicleBrandModel
    isEditPopupOpen.value = true
  }

  function deleteVehicleBrandModel(vehicleBrandModel: VehicleBrandModel) {
    vehicleBrandModelToDelete.value = vehicleBrandModel
    isDeletePopupOpen.value = true
  }

  async function confirmDelete() {
    if (!vehicleBrandModelToDelete.value) return

    setDeleting(true)
    const { success } = await vehicleBrandModelSettingsStore.deleteVehicleBrandModel(
      vehicleBrandModelToDelete.value.id
    )
    setDeleting(false)

    if (success) {
      isDeletePopupOpen.value = false
      vehicleBrandModelToDelete.value = null
      fetchVehicleBrandModels()
    }
  }

  watch(isEditPopupOpen, (newValue) => {
    if (!newValue) {
      selectedVehicleBrandModel.value = null
    }
  })

  watch(isDeletePopupOpen, (newValue) => {
    if (!newValue) {
      vehicleBrandModelToDelete.value = null
    }
  })

  onMounted(async () => {
    await vehicleBrandSettingsStore.getList({
      pagination: { page: 1, size: 100 },
    })
    fetchVehicleBrandModels()
  })
</script>

<style scoped>

</style>
