<template>
  <div class="page-content">
    <VehicleTypesHeader @open-wizard="openWizard" />

    <VehicleTypesTable
      @edit="openWizard"
      @delete="openDeletePopup"
    />

    <VehicleTypesWizard
      v-if="isWizardPopupOpen"
      v-model="isWizardPopupOpen"
      :vehicle-type="selectedVehicleType"
    />

    <PopupDelete
      v-if="isDeletePopupOpen"
      v-model="isDeletePopupOpen"
      :title="$t('error.deleteVehicleType')"
      :message="$t('error.deleteVehicleTypeConfirmation', { name: selectedVehicleType?.name })"
      @confirm="deleteVehicleType"
    />
  </div>
</template>

<script setup lang="ts">
  import { useLoading } from '@/composables/useLoading'
  import { useVehicleTypeSettingsStore } from '@/stores/admin-dashboard/vehicle-type-settings'
  import type { VehicleType } from '@/types/admin-dashboard/response/vehicle/vehicle-type'
  import VehicleTypesHeader from './components/VehicleTypesHeader.vue'
  import VehicleTypesTable from './components/VehicleTypesTable.vue'
  import { useList } from './composables/useList'

  const VehicleTypesWizard = defineAsyncComponent(() => import('./components/VehicleTypesWizard.vue'))
  const PopupDelete = defineAsyncComponent(() => import('@/components/popup/PopupDelete.vue'))

  const { fetchVehicleTypes } = useList()
  const { setLoading: setDeleting } = useLoading()

  const vehicleTypeSettingsStore = useVehicleTypeSettingsStore()

  const isWizardPopupOpen = ref(false)
  const isDeletePopupOpen = ref(false)
  const selectedVehicleType = ref<VehicleType | null>(null)

  function resetVehicleType(): void {
    selectedVehicleType.value = null
  }

  function openWizard(vehicleType?: VehicleType): void {
    selectedVehicleType.value = vehicleType ?? null
    isWizardPopupOpen.value = true
  }

  function openDeletePopup(vehicleType: VehicleType): void {
    selectedVehicleType.value = vehicleType
    isDeletePopupOpen.value = true
  }

  async function deleteVehicleType(): Promise<void> {
    if (!selectedVehicleType.value) return

    setDeleting(true)
    const { success } = await vehicleTypeSettingsStore.deleteVehicleType(selectedVehicleType.value.id)
    setDeleting(false)

    if (success) {
      isDeletePopupOpen.value = false
      resetVehicleType()
      fetchVehicleTypes()
    }
  }

  watch(isWizardPopupOpen, (newValue) => {
    if (!newValue) resetVehicleType()
  })

  watch(isDeletePopupOpen, (newValue) => {
    if (!newValue) resetVehicleType()
  })

  onMounted(fetchVehicleTypes)
</script>

<style scoped>

</style>
