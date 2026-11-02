<template>
  <div
    ref="pdfSection"
    class="page-content"
  >
    <BaseLink
      :to="backRoute"
      class="
        mb-4 inline-flex items-center gap-2 text-sm font-medium text-neutral-800 transition-colors
        hover:text-primary
      "
      :class="{ 'hidden': isExportInAction }"
    >
      <IconArrow class="size-5 fill-current!" />
      {{ $t('button.back') }}
    </BaseLink>

    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
      :class="{ 'hidden': isExportInAction }"
    />

    <BaseAlert
      :model-value="!!loadError"
      :message="loadError"
      variant="error"
      class="mb-4"
    />

    <div class="flex items-center justify-between">
      <h1 class="mb-6 text-2xl font-bold text-neutral-10">
        {{ invoiceTitle }}
      </h1>

      <div class="flex items-center gap-3">
        <div :class="{ 'hidden': isExportInAction }">
          <CancelInvoiceAction />
        </div>

        <InvoiceExport
          :pdf-section
          :invoice-number="invoiceDetail?.invoiceNumber"
          @is-export-in-action="isExportInAction = $event"
        />
      </div>
    </div>

    <template v-if="invoiceDetail">
      <div class="mb-6 rounded-xl border border-neutral-500 p-4">
        <InvoiceHeader :invoice="invoiceDetail" />

        <InvoiceList :trips="invoiceDetail.trips" />

        <InvoiceFooter :invoice="invoiceDetail" />
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
  import { useInvoiceStore } from '@/stores/financial/invoice.ts'
  import { useLoading } from '@/composables/useLoading'
  import InvoiceHeader from './components/InvoiceHeader.vue'
  import InvoiceList from './components/InvoiceList.vue'
  import InvoiceFooter from './components/InvoiceFooter.vue'
  import CancelInvoiceAction from './components/CancelInvoiceAction.vue'
  import { InvoiceType } from '@/types/invoice.enums.ts'

  const InvoiceExport = defineAsyncComponent(() => import('./components/InvoiceExport.vue'))

  const route = useRoute()
  const { t } = useI18n()
  const { setLoading } = useLoading()

  const invoiceId = computed(() => String(route.params.id ?? ''))

  const invoiceStore = useInvoiceStore()
  const { invoiceDetail } = storeToRefs(invoiceStore)

  const pdfSection = ref<HTMLElement | null>(null)
  const loadError = ref('')
  const isExportInAction = ref(false)

  const invoiceTitle = computed(() => {
    const numberLabel = t('content.invoiceWithNumber', {
      number: invoiceDetail.value?.invoiceNumber,
    })
    const typeLabel = invoiceDetail.value?.invoiceType === InvoiceType.Invoice
      ? t('form.invoice')
      : t('form.claim')

    return `${numberLabel} - (${typeLabel})`
  })

  const backRoute = computed(() => {
    const snapshotId = route.query.snapshotId

    if (route.query.returnTo === 'FinancialMonthlyContractSlotsCharges' && snapshotId) {
      return {
        name: 'FinancialMonthlyContractSlotsCharges',
        params: { id: String(snapshotId) },
      }
    }

    return { name: 'FinancialPaymentRequests' }
  })

  const breadcrumbItems = computed(() => ([
    { label: t('title.financialManagement'), to: '/financial' },
    { label: t('title.paymentTracking'), to: '/financial/payment-requests' },
    { label: t('content.invoiceWithNumber', { number: invoiceDetail.value?.invoiceNumber || invoiceId.value }) },
  ]))

  async function fetchInvoice(): Promise<void> {
    const id = invoiceId.value
    if (!id) return

    loadError.value = ''
    setLoading(true)
    const { success, message } = await invoiceStore.getInvoiceById(id)
    setLoading(false)

    if (success) return
    loadError.value = message || t('error.networkError')
  }

  watch(invoiceId, async () => {
    await fetchInvoice()
  })

  onMounted(fetchInvoice)
</script>

<style scoped>

</style>
