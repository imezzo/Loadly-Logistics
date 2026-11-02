<template>
  <ButtonExport
    :disabled="isExportDisabled"
    @click="exportSnapshot"
  />

  <PopupError
    v-model="isErrorPopupOpen"
    :message="errorMessage"
  />
</template>

<script setup lang="ts">
  import { useMonthlyContractStore } from '@/stores/admin-dashboard/monthly-contract'
  import { triggerDownload } from '@/utils/download'

  const props = defineProps<{
    snapshotId: string
  }>()

  const { t } = useI18n()
  const monthlyContractStore = useMonthlyContractStore()

  const isExportLoading = ref(false)
  const errorMessage = ref<string | null>(null)
  const isErrorPopupOpen = ref(false)

  const isExportDisabled = computed(() => isExportLoading.value || !props.snapshotId)

  async function exportSnapshot(): Promise<void> {
    if (isExportDisabled.value) return

    isExportLoading.value = true

    const { success, message, blob } = await monthlyContractStore.exportExcel(props.snapshotId)

    isExportLoading.value = false

    if (success) {
      triggerDownload(blob as Blob, { prefix: 'monthly-contract-', extension: '.xlsx' })

      return
    }

    errorMessage.value = message || t('error.networkError')
    isErrorPopupOpen.value = true
  }
</script>

<style scoped>

</style>
