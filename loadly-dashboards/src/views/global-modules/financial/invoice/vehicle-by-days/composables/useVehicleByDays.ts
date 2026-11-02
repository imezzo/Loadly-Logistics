import { useContractStore } from '@/stores/admin-dashboard/contract'
import { useInvoiceStore } from '@/stores/financial/invoice'
import { getLocalizedValue } from '@/utils/i18n'
import { InvoiceType } from '@/types/invoice.enums'

export function useVehicleByDays() {
  const route = useRoute()
  const { t } = useI18n()

  const contractStore = useContractStore()
  const { contract } = storeToRefs(contractStore)

  const invoiceStore = useInvoiceStore()
  const { invoiceDetail } = storeToRefs(invoiceStore)

  const contractId = computed(() => String(route.params.id ?? ''))
  const startDate = computed(() => String(route.query.startDate ?? ''))
  const endDate = computed(() => String(route.query.endDate ?? ''))

  const isEditMode = computed(() => route.query.returnTo === 'FinancialInvoiceEdit')

  const returnRouteName = computed(() => {
    const returnTo = route.query.returnTo

    return typeof returnTo === 'string' && returnTo
      ? returnTo
      : 'FinancialCompanyDetails'
  })

  const backRoute = computed(() => {
    if (returnRouteName.value === 'FinancialInvoiceEdit') {
      return {
        name: 'FinancialInvoiceEdit',
        params: { id: String(route.query.invoiceId ?? '') },
      }
    }

    if (returnRouteName.value === 'FinancialInvoiceWizard') {
      return {
        name: 'FinancialInvoiceWizard',
        params: {
          id: contractId.value,
          invoiceType: String(route.query.invoiceType ?? 'trip-wizard'),
        },
      }
    }

    return {
      name: 'FinancialCompanyDetails',
      params: { id: contractId.value },
    }
  })

  const pageTitle = computed(() => isEditMode.value
    ? t('button.editInvoice')
    : t('button.createInvoice'))

  const companyName = computed(() => contract.value?.company
    ? getLocalizedValue(contract.value.company.nameEn, contract.value.company.nameAr)
    : '-')

  const invoiceType = computed(() => {
    if (isEditMode.value && invoiceDetail.value) {
      return invoiceDetail.value.invoiceType === InvoiceType.Claim
        ? t('form.claim')
        : t('form.invoice')
    }

    return route.query.invoiceType === 'claim-wizard'
      ? t('form.claim')
      : t('form.invoice')
  })

  const breadcrumbItems = computed(() => [
    { label: t('title.financialManagement'), to: '/financial' },
    { label: t('content.daysByVehicles') },
  ])

  return {
    backRoute,
    breadcrumbItems,
    companyName,
    contractId,
    endDate,
    invoiceType,
    pageTitle,
    startDate,
  }
}
