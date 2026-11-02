<template>
  <div class="mb-6 flex flex-col gap-4">
    <div
      class="
        flex flex-col gap-4
        sm:flex-row sm:items-center sm:justify-between
      "
    >
      <h1 class="text-xl font-bold text-neutral-800">
        {{ $t('title.invoicesList') }}
      </h1>

      <ButtonExport
        :disabled="isExportLoading"
        @click="exportInvoices"
      />
    </div>
  </div>

  <PopupError
    v-model="isErrorPopupOpen"
    :message="errorMessage"
  />
</template>

<script setup lang="ts">
  import { useInvoiceStore } from '@/stores/financial/invoice'
  import { triggerDownload } from '@/utils/download'
  import type { InvoicesListRequest } from '@/types/request/financial/invoice-list'

  const props = defineProps<{
    filters: InvoicesListRequest
  }>()

  const { t } = useI18n()
  const invoiceStore = useInvoiceStore()

  const isExportLoading = ref(false)
  const errorMessage = ref<string | null>(null)
  const isErrorPopupOpen = ref(false)

  async function exportInvoices(): Promise<void> {
    if (isExportLoading.value) return

    isExportLoading.value = true

    const { success, message, blob } = await invoiceStore.exportExcel({
      ...props.filters,
      columns: ['invoiceNumber', 'invoiceType', 'companyName', 'contractRef', 'issueDate', 'amount', 'statusName'],
    })

    isExportLoading.value = false

    if (success) {
      triggerDownload(blob as Blob, { prefix: 'invoices-', extension: '.xlsx' })

      return
    }

    errorMessage.value = message || t('error.networkError')
    isErrorPopupOpen.value = true
  }
</script>

<style scoped>

</style>
