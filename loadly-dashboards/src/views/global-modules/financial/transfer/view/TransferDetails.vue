<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-4"
    />

    <BaseAlert
      :model-value="!!loadError"
      :message="loadError"
      variant="error"
      class="mb-4"
    />

    <template v-if="transferDetail">
      <TransferDetailsSummary :transfer="transferDetail" />
      <TransferDetailsInvoicesList :invoice-payments="transferDetail.invoicePayments" />
    </template>
  </div>
</template>

<script setup lang="ts">
  import TransferDetailsSummary from './components/TransferDetailsSummary.vue'
  import TransferDetailsInvoicesList from './components/TransferDetailsInvoicesList.vue'
  import { useTransferDetails } from './composables/useTransferDetails'
  import { useBankTransferStore } from '@/stores/financial/bank-transfer'

  const bankTransferStore = useBankTransferStore()
  const {
    fetchTransferDetails,
    loadError,
    transferDetail,
    transferId,
  } = useTransferDetails()
  const { t } = useI18n()

  const breadcrumbItems = computed(() => [
    { label: t('title.transfer'), to: '/financial' },
    { label: t('title.viewTransfer'), to: '/financial/bank-transfers' },
  ])

  watch(transferId, fetchTransferDetails)

  onMounted(fetchTransferDetails)
  onUnmounted(bankTransferStore.resetTransferDetail)
</script>

<style scoped>

</style>
