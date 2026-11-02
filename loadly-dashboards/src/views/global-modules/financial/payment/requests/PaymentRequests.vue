<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <ListHeader
      :invoice-number
      @refetch-invoice="fetchInvoice"
    />

    <BaseAlert
      :model-value="!!loadError"
      :message="loadError"
      variant="error"
    />

    <ListTable />
  </div>
</template>

<script setup lang="ts">
  import ListHeader from './components/ListHeader.vue'
  import ListTable from './components/ListTable.vue'
  import { useInvoiceStore } from '@/stores/financial/invoice.ts'
  import { useLoading } from '@/composables/useLoading'
  import { usePagination } from '@/composables/usePagination'

  const route = useRoute()
  const { t } = useI18n()
  const { setLoading } = useLoading()

  const invoiceStore = useInvoiceStore()
  const { invoicePaymentRequestsList } = storeToRefs(invoiceStore)
  const { currentPage, itemsPerPage } = usePagination(
    computed(() => invoicePaymentRequestsList.value?.pagination ?? null))

  const loadError = ref('')

  const invoiceId = computed(() => String(route.params.id ?? ''))
  const invoiceNumber = computed(() => invoicePaymentRequestsList.value?.data[0]?.invoiceNumber ?? '')
  const breadcrumbItems = computed(() => {
    return [
      { label: t('title.financialManagement'), to: '/financial' },
      { label: t('title.paymentTracking'), to: '/financial/payment-requests' },
      { label: t('content.invoiceWithNumber', { number: invoiceNumber.value }) },
    ]
  })

  async function fetchInvoice(): Promise<void> {
    const id = invoiceId.value
    if (!id) return

    loadError.value = ''
    setLoading(true)
    const { success, data, message } = await invoiceStore.getPaymentRequestsList({
      id,
      page: currentPage.value,
      size: itemsPerPage.value,
    })
    setLoading(false)
    invoicePaymentRequestsList.value = data

    if (!success) {
      loadError.value = message || t('error.networkError')
    }
  }

  watch([invoiceId, currentPage, itemsPerPage], async () => {
    await fetchInvoice()
  })

  onMounted(() => {
    fetchInvoice()
  })
</script>

<style scoped>

</style>
