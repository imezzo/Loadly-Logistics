<template>
  <ButtonExport
    :disabled="isExportDisabled"
    @click="exportSlotCharges"
  />

  <PopupError
    v-model="isErrorPopupOpen"
    :message="errorMessage"
  />
</template>

<script setup lang="ts">
  import { useContractStore } from '@/stores/admin-dashboard/contract'
  import { triggerDownload } from '@/utils/download'

  const props = defineProps<{
    contractId: string
    endDate: string
    startDate: string
  }>()

  const { t } = useI18n()
  const contractStore = useContractStore()

  const isExportLoading = ref(false)
  const errorMessage = ref<string | null>(null)
  const isErrorPopupOpen = ref(false)

  const isExportDisabled = computed(() =>
    isExportLoading.value
    || !props.contractId
    || !props.startDate
    || !props.endDate,
  )

  async function exportSlotCharges(): Promise<void> {
    if (isExportDisabled.value) return

    isExportLoading.value = true

    const { success, message, blob } = await contractStore.exportSlotCharges(
      props.contractId,
      {
        periodStart: props.startDate,
        periodEnd: props.endDate,
      },
    )

    isExportLoading.value = false

    if (success) {
      triggerDownload(blob as Blob, { prefix: 'slot-charges-', extension: '.xlsx' })

      return
    }

    errorMessage.value = message || t('error.networkError')
    isErrorPopupOpen.value = true
  }
</script>

<style scoped>

</style>
