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
    @export="exportTripReports"
  />

  <PopupError
    v-model="isErrorPopupOpen"
    :message="errorMessage"
  />
</template>

<script setup lang="ts">
  import { triggerDownload } from '@/utils/download'
  import { useTripReportsList } from '../composables/useTripReportsList'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { TripStatus } from '@/types/admin-dashboard/response/trip/trip'

  const { t } = useI18n()
  const tripStore = useTripStore()
  const { tripsList, tripFinancialReportsFilters: filters } = storeToRefs(tripStore)

  const { isExternalTrip } = useTripReportsList()

  const isExportLoading = ref(false)
  const isExportPopupOpen = ref(false)
  const errorMessage = ref<string | null>(null)
  const isErrorPopupOpen = ref(false)

  const exportKeys = computed(() => tripsList.value?.additional?.exportKeys ?? [])

  async function exportTripReports(columns: string[]): Promise<void> {
    if (isExportLoading.value || !columns.length) return

    isExportLoading.value = true

    const { success, message, blob } = await tripStore.exportExcel({
      ...filters.value,
      columns,
      isExternalTrip: isExternalTrip.value,
      status: TripStatus.COMPLETED.toString(),
    })

    isExportLoading.value = false

    if (success) {
      isExportPopupOpen.value = false
      triggerDownload(blob as Blob, { prefix: 'Financial-trip-reports-', extension: '.xlsx' })

      return
    }

    errorMessage.value = message || t('error.networkError')
    isErrorPopupOpen.value = true
  }
</script>

<style scoped>

</style>
