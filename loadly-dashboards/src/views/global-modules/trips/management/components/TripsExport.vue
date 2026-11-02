<template>
  <ButtonExport
    :disabled="isExportLoading"
    @click="isExportPopupOpen = true"
  />

  <PopupExport
    v-model="isExportPopupOpen"
    :title="$t('title.exportTripReport')"
    :columns="exportKeys"
    :loading="isExportLoading"
    @export="exportTrips"
  />

  <PopupError
    v-model="isErrorPopupOpen"
    :message="errorMessage"
  />
</template>

<script setup lang="ts">
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { triggerDownload } from '@/utils/download'

  const tripStore = useTripStore()
  const { filters, tripsList } = storeToRefs(tripStore)

  const { t } = useI18n()
  const route = useRoute()

  const excludedColumns = [
    'cargoPrice', 'spotPrice', 'totalTripCost', 'totalAddonsCost',
    'tripCostWithoutAddon', 'customsTotalAmount', 'totalFuelAmount'
  ]

  const isExportLoading = ref(false)
  const isExportPopupOpen = ref(false)
  const errorMessage = ref<string | null>(null)
  const isErrorPopupOpen = ref(false)

  const isExternalTrip = computed(() => route.name === 'ExternalTrips')
  const exportKeys = computed(() =>
    (tripsList.value?.additional?.exportKeys ?? []).filter(column => !excludedColumns.includes(column.key))
  )

  async function exportTrips(columns: string[]): Promise<void> {
    if (isExportLoading.value || !columns.length) return

    isExportLoading.value = true

    const { success, message, blob } = await tripStore.exportExcel({
      ...filters.value,
      isExternalTrip: isExternalTrip.value,
      columns,
    })

    isExportLoading.value = false

    if (success) {
      isExportPopupOpen.value = false
      triggerDownload(blob as Blob, { prefix: 'trips-', extension: '.xlsx' })

      return
    }

    errorMessage.value = message || t('error.networkError')
    isErrorPopupOpen.value = true
  }
</script>

<style scoped>

</style>
