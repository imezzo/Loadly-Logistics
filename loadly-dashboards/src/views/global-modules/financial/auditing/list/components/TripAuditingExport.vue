<template>
  <ButtonExport
    :disabled="isExportLoading"
    @click="isExportPopupOpen = true"
  />

  <PopupExport
    v-model="isExportPopupOpen"
    :title="$t('title.exportTripAuditing')"
    :columns="exportKeys"
    :loading="isExportLoading"
    @export="exportTripAuditing"
  />

  <PopupError
    v-model="isErrorPopupOpen"
    :message="errorMessage"
  />
</template>

<script setup lang="ts">
  import { triggerDownload } from '@/utils/download'
  import { useTripAuditingList } from '../composables/useTripAuditingList'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { TripStatus } from '@/types/admin-dashboard/response/trip/trip'

  const { t } = useI18n()
  const tripStore = useTripStore()
  const { tripsList, tripAuditingFilters: filters } = storeToRefs(tripStore)

  const { isExternalTrip } = useTripAuditingList()

  const isExportLoading = ref(false)
  const isExportPopupOpen = ref(false)
  const errorMessage = ref<string | null>(null)
  const isErrorPopupOpen = ref(false)

  const exportKeys = computed(() => tripsList.value?.additional?.exportKeys ?? [])

  async function exportTripAuditing(columns: string[]): Promise<void> {
    if (isExportLoading.value || !columns.length) return

    isExportLoading.value = true

    const { success, message, blob } = await tripStore.exportExcel({
      columns,
      isExternalTrip: isExternalTrip.value,
      ...filters.value,
      status: TripStatus.COMPLETED.toString(),
    })

    isExportLoading.value = false

    if (success) {
      isExportPopupOpen.value = false
      triggerDownload(blob as Blob, { prefix: 'Trip-auditing-', extension: '.xlsx' })

      return
    }

    errorMessage.value = message || t('error.networkError')
    isErrorPopupOpen.value = true
  }
</script>

<style scoped>

</style>
