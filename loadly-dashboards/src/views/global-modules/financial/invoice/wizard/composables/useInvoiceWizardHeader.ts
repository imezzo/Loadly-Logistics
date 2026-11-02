import { useContractStore } from '@/stores/admin-dashboard/contract'
import { useInvoiceStore } from '@/stores/financial/invoice'
import { useTripStore } from '@/stores/admin-dashboard/trip'
import { getLocalizedValue } from '@/utils/i18n'
import { InvoiceType } from '@/types/invoice.enums'
import { ContractType } from '@/types/admin-dashboard/response/contract/contract-details'

export function useInvoiceWizardHeader(contractId: Ref<number | null>) {
  const route = useRoute()
  const { t } = useI18n()

  const contractStore = useContractStore()
  const { contract } = storeToRefs(contractStore)

  const invoiceStore = useInvoiceStore()
  const { invoiceDetail, invoice } = storeToRefs(invoiceStore)

  const tripStore = useTripStore()
  const { tripInvoiceFilters } = storeToRefs(tripStore)

  const isEditMode = computed(() => route.name === 'FinancialInvoiceEdit')
  const pageTitle = computed(() => isEditMode.value ? t('button.editInvoice') : t('button.createInvoice'))

  const companyName = computed(() => contract.value?.company
    ? getLocalizedValue(contract.value.company.nameEn, contract.value.company.nameAr)
    : '-')

  const invoiceType = computed(() => {
    if (isEditMode.value && invoiceDetail.value) {
      return invoiceDetail.value.invoiceType === InvoiceType.Claim
        ? t('form.claim')
        : t('form.invoice')
    }

    return route.params.invoiceType === 'claim-wizard'
      ? t('form.claim')
      : t('form.invoice')
  })

  const periodStartDate = computed(() =>
    invoice.value.startDate || tripInvoiceFilters.value.financialDateFrom || '',
  )

  const periodEndDate = computed(() =>
    invoice.value.endDate || tripInvoiceFilters.value.financialDateTo || '',
  )

  const hasVehicleByDaysPeriod = computed(() => Boolean(
    contractId.value
    && periodStartDate.value
    && periodEndDate.value
    && (contract.value?.contractType === ContractType.Included
      || contract.value?.contractType === ContractType.Excluded),
  ))

  const snapshotId = computed(() => {
    const id = Number(route.query.snapshotId)

    return Number.isFinite(id) && id > 0 ? String(id) : ''
  })

  const vehicleByDaysRoute = computed(() => {
    if (snapshotId.value) {
      return {
        name: 'FinancialMonthlyContractSlotsCharges',
        params: { id: snapshotId.value },
      }
    }

    return {
      name: 'FinancialVehicleByDays',
      params: { id: String(contractId.value ?? route.params.id) },
      query: {
        startDate: periodStartDate.value,
        endDate: periodEndDate.value,
        returnTo: isEditMode.value ? 'FinancialInvoiceEdit' : 'FinancialInvoiceWizard',
        ...(isEditMode.value ? { invoiceId: String(route.params.id) } : {}),
        ...(!isEditMode.value ? { invoiceType: String(route.params.invoiceType ?? 'trip-wizard') } : {}),
      },
    }
  })

  return {
    companyName,
    hasVehicleByDaysPeriod,
    invoiceType,
    pageTitle,
    vehicleByDaysRoute,
  }
}
