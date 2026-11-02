<template>
  <ButtonExport
    :disabled="isExportLoading"
    @click="isExportPopupOpen = true"
  />

  <PopupExport
    v-model="isExportPopupOpen"
    :title="$t('title.exportDriverReport')"
    :columns="exportKeys"
    :loading="isExportLoading"
    @export="exportDrivers"
  />

  <PopupError
    v-model="isErrorPopupOpen"
    :message="errorMessage"
  />
</template>

<script setup lang="ts">
  import { useDriverStore } from '@/stores/admin-dashboard/driver'
  import { triggerDownload } from '@/utils/download'

  const driverStore = useDriverStore()
  const { filters, driversList } = storeToRefs(driverStore)

  const { t } = useI18n()

  const isExportLoading = ref(false)
  const isExportPopupOpen = ref(false)
  const errorMessage = ref<string | null>(null)
  const isErrorPopupOpen = ref(false)

  const exportKeys = computed(() => driversList.value?.additional?.exportKeys ?? [])

  async function exportDrivers(columns: string[]): Promise<void> {
    if (isExportLoading.value || !columns.length) return

    isExportLoading.value = true

    const { success, message, blob } = await driverStore.exportExcel({
      ...filters.value,
      workingStatuses: filters.value.workingStatuses ? [filters.value.workingStatuses] : undefined,
      pagination: { page: 1 },
      contractorIds: filters.value.contractorIds.map(Number),
      columns,
    })

    isExportLoading.value = false

    if (success) {
      isExportPopupOpen.value = false
      triggerDownload(blob as Blob, { prefix: 'drivers-', extension: '.xlsx' })

      return
    }

    errorMessage.value = message || t('error.networkError')
    isErrorPopupOpen.value = true
  }
</script>

<style scoped>

</style>
