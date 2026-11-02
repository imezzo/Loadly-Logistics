import { useLoading } from '@/composables/useLoading'
import { useContractorPayoutStore } from '@/stores/financial/contractor-payout'
import { ContractorPayoutStatus } from '@/types/response/financial/contractor-payout.enums'

export function useViewInvoice() {
  const route = useRoute()
  const { t } = useI18n()
  const { setLoading } = useLoading()

  const contractorPayoutStore = useContractorPayoutStore()
  const { payoutDetail } = storeToRefs(contractorPayoutStore)

  const loadError = ref('')
  const isSettingPaid = ref(false)
  const isSuccessPopupOpen = ref(false)

  const invoiceId = computed(() => String(route.params.invoiceId ?? ''))
  const contractorId = computed(() => String(route.params.contractorId ?? ''))

  const payoutStatus = computed(() => payoutDetail.value?.status)

  const breadcrumbItems = computed(() => [
    { label: t('title.financialManagement'), to: '/financial' },
    { label: t('title.contractorInvoices'), to: '/financial/contractor-invoices' },
    {
      label: t('content.payouts'),
      to: {
        name: 'FinancialContractorPayouts',
        params: { contractorId: contractorId.value },
      },
    },
    {
      label: t('content.invoiceWithNumber', {
        number: payoutDetail.value?.payoutNumber || invoiceId.value,
      }),
    },
  ])

  async function fetchInvoice(): Promise<void> {
    if (!invoiceId.value) return

    loadError.value = ''
    setLoading(true)

    const { success, message } = await contractorPayoutStore.getById(invoiceId.value)

    setLoading(false)

    if (success) return

    loadError.value = message || t('error.networkError')
  }

  async function setPaid(): Promise<void> {
    if (
      !invoiceId.value
      || payoutStatus.value !== ContractorPayoutStatus.Pending
      || isSettingPaid.value
    ) return

    isSettingPaid.value = true
    setLoading(true)

    const { success, message } = await contractorPayoutStore.setPaid(invoiceId.value)

    isSettingPaid.value = false

    if (success) {
      isSuccessPopupOpen.value = true
      await fetchInvoice()

      return
    }

    setLoading(false)
    loadError.value = message || t('error.networkError')
  }

  watch(invoiceId, fetchInvoice)

  return {
    breadcrumbItems,
    fetchInvoice,
    isSettingPaid,
    isSuccessPopupOpen,
    loadError,
    payoutDetail,
    resetPayoutDetail: contractorPayoutStore.resetPayoutDetail,
    setPaid,
  }
}
