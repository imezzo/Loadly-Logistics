<template>
  <ButtonExport
    :disabled="isExportLoading"
    @click="isExportPopupOpen = true"
  />

  <PopupExport
    v-model="isExportPopupOpen"
    :title="$t('title.exportVehicleReport')"
    :columns="exportKeys"
    :loading="isExportLoading"
    @export="exportVehicles"
  />

  <PopupError
    v-model="isErrorPopupOpen"
    :message="errorMessage"
  />
</template>

<script setup lang="ts">
  import { vehicleStatusFiltersToRequest, type VehicleStatusFilters } from '../composables/useVehicleStatusList'
  import { useVehicleStore } from '@/stores/admin-dashboard/vehicle'
  import { triggerDownload } from '@/utils/download'

  const props = defineProps<{
    filters: VehicleStatusFilters
  }>()

  const vehicleStore = useVehicleStore()
  const { vehiclesList } = storeToRefs(vehicleStore)

  const { t } = useI18n()

  const isExportLoading = ref(false)
  const isExportPopupOpen = ref(false)
  const errorMessage = ref<string | null>(null)
  const isErrorPopupOpen = ref(false)

  const exportKeys = computed(() => vehiclesList.value?.additional?.exportKeys ?? [])

  async function exportVehicles(columns: string[]): Promise<void> {
    if (isExportLoading.value || !columns.length) return

    isExportLoading.value = true

    const { success, message, blob } = await vehicleStore.exportExcel({
      ...vehicleStatusFiltersToRequest(props.filters),
      size: undefined,
      columns,
    })

    isExportLoading.value = false

    if (success) {
      isExportPopupOpen.value = false
      triggerDownload(blob as Blob, { prefix: 'truck-status-', extension: '.xlsx' })

      return
    }

    errorMessage.value = message || t('error.networkError')
    isErrorPopupOpen.value = true
  }
</script>

<style scoped>

</style>
