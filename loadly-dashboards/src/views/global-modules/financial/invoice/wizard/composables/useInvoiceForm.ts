import { date, mixed, object, string } from 'yup'
import { useVeeValidate } from '@/composables/useVeeValidate'
import { useContractStore } from '@/stores/admin-dashboard/contract'
import { useMonthlyContractStore } from '@/stores/admin-dashboard/monthly-contract'
import { useInvoiceStore } from '@/stores/financial/invoice'
import type { Schema } from '@/types/form'
import { useTripStore } from '@/stores/admin-dashboard/trip'
import { useLookupStore } from '@/stores/lookup'
import { useLookups } from '@/composables/useLookups'
import { useInvoiceLinkedTransfers } from './useInvoiceLinkedTransfers'
import { InvoiceType } from '@/types/invoice.enums'
import type { InvoicePayment } from '@/types/request/financial/invoice'
import { ContractType } from '@/types/admin-dashboard/response/contract/contract-details'

export function useInvoiceForm(contractId: number) {
  useVeeValidate()

  const { t } = useI18n()
  const router = useRouter()
  const route = useRoute()

  const invoiceStore = useInvoiceStore()
  const { invoice, invoiceDetail } = storeToRefs(invoiceStore)

  const contractStore = useContractStore()
  const { contract } = storeToRefs(contractStore)

  const monthlyContractStore = useMonthlyContractStore()
  const { monthlyContract } = storeToRefs(monthlyContractStore)

  const invoiceId = computed(() => Number(route.params.id))
  const isEditMode = computed(() => route.name === 'FinancialInvoiceEdit')
  const isClaimWizard = computed(() => route.params.invoiceType === 'claim-wizard')
  const snapshotId = computed(() => Number(route.query.snapshotId))
  const isMonthlyContract = computed(() =>
    contract.value?.contractType === ContractType.Included
    || contract.value?.contractType === ContractType.Excluded,
  )
  const isIncludedContract = computed(() => contract.value?.contractType === ContractType.Included)

  const snapshotExtraKms = computed(() => monthlyContract.value?.calculation.totalExtraKmsCount ?? null)

  if (!isEditMode.value) {
    invoice.value.invoiceType = isClaimWizard.value
      ? InvoiceType.Claim
      : InvoiceType.Invoice
  }

  const lookupStore = useLookupStore()
  const { discounts, taxes } = storeToRefs(lookupStore)
  const {
    contractBillingSnapshots,
    getContractBillingSnapshots,
    hasMoreContractBillingSnapshots,
    loadMoreContractBillingSnapshots,
  } = useLookups()

  const tripStore = useTripStore()
  const { tripInvoiceFilters: filters, tripsList, selectedTripsIds } = storeToRefs(tripStore)

  const {
    fetchTransfers,
    initializeTransferRowsFromPayments,
    maxAmountForRow,
    resetTransferRows,
    transferRows,
    availableTransferOptions,
    newTransferId,
    onNewTransferSelect,
    removeTransferRow,
    updateTransferAmount,
  } = useInvoiceLinkedTransfers()

  const submitError = ref('')
  const submitErrors = ref<Record<string, string[]>>({})
  const successMessage = ref('')
  const isErrorPopupOpen = ref(false)
  const isSuccessPopupOpen = ref(false)

  invoice.value.contractId = contractId


  const invoiceStatusOptions = computed(() => [
    { label: t('content.paid'), value: 2 },
    { label: t('content.unpaid'), value: 0 },
  ])

  const validationSchema = computed(() => isEditMode.value ? object({}) : object({
    invoiceType: string().required(),
    startDate: string().optional(),
    endDate: string().optional(),
    issueDate: string().required(),
    dueDate: date()
      .typeError(() => t('validation.required', { field: t('form.dueDate') }))
      .required()
      .when('issueDate', (issueDate, schema) => {
        if (issueDate && issueDate.toString()) {
          return schema.min(
            new Date((issueDate + 'T00:00:00').toString()),
            () => t('validation.minDate', { min: t('form.issueDate') }))
        }

        return schema
      }),
    taxedInvoiceAttachment: isEditMode.value
      ? mixed<File>().optional().nullable()
      : mixed<File>().required(),
    status: invoice.value.linkedTransfer
      ? string().optional()
      : string().required(),
  }))

  const initialValues = ref({
    invoiceType: invoice.value.invoiceType,
    startDate: invoice.value.startDate || filters.value.financialDateFrom || '',
    endDate: invoice.value.endDate || filters.value.financialDateTo || '',
    dueDate: invoice.value.dueDate || '',
    issueDate: invoice.value.issueDate || '',
    discountId: invoice.value.discountId,
    taxId: invoice.value.taxId,
    taxedInvoiceAttachment: invoice.value.taxedInvoiceAttachment,
    contractBillingSnapshotId: invoice.value.contractBillingSnapshotId
      ? String(invoice.value.contractBillingSnapshotId)
      : '',
    linkedTransfer: invoice.value.linkedTransfer,
    excludeExtraKmsFromMainInvoice: invoice.value.excludeExtraKmsFromMainInvoice,
    status: invoice.value.status,
  })

  const fieldsSchema = computed<Schema[]>(() => {
    return [
      {
        name: 'issueDate',
        as: 'BaseInput',
        type: 'date',
        label: t('form.issueDate'),
        labelClass: 'mb-2 block font-medium text-neutral-800',
        labelRequired: true,
        disabled: isEditMode.value,
      },
      {
        name: 'dueDate',
        as: 'BaseInput',
        type: 'date',
        label: t('form.dueDate'),
        labelClass: 'mb-2 block font-medium text-neutral-800',
        labelRequired: true,
        disabled: isEditMode.value,
      },
      {
        name: 'startDate',
        as: 'BaseInput',
        type: 'date',
        label: t('form.startDate'),
        labelClass: 'mb-2 block font-medium text-neutral-800',
        disabled: true,
      },
      {
        name: 'endDate',
        as: 'BaseInput',
        type: 'date',
        label: t('form.endDate'),
        labelClass: 'mb-2 block font-medium text-neutral-800',
        min: invoice.value.startDate,
        disabled: true,
      },
      {
        name: 'discountId',
        as: 'BaseSelect',
        label: `${t('content.discount')} (%)`,
        labelClass: 'mb-2 block font-medium text-neutral-800',
        options: discounts.value?.items ?? [],
        disabled: isEditMode.value,
        onChange: (value: string | number) => {
          invoice.value.discountId = Number(value)
          resetTransferRows()
        },
      },
      {
        name: 'taxId',
        as: 'BaseSelect',
        label: `${t('content.tax')} (%)`,
        labelClass: 'mb-2 block font-medium text-neutral-800',
        options: taxes.value?.items ?? [],
        disabled: isEditMode.value,
        onChange: (value: string | number) => {
          invoice.value.taxId = Number(value)
          resetTransferRows()
        },
      },
      ...(isMonthlyContract.value ? [{
        name: 'contractBillingSnapshotId',
        as: 'BaseSelect',
        label: t('form.contractBillingSnapshotId'),
        labelClass: 'mb-2 block font-medium text-neutral-800',
        options: contractBillingSnapshots.value?.items ?? [],
        containerClass: 'col-span-full',
        disabled: isEditMode.value || !!snapshotId.value,
        onScrollToEnd: () => {
          if (hasMoreContractBillingSnapshots.value) loadMoreContractBillingSnapshots()
        },
        onChange: (value: string | number) => {
          invoice.value.contractBillingSnapshotId = value ? Number(value) : null
        },
      }] : []),
      {
        name: 'taxedInvoiceAttachment',
        as: 'BaseUploader',
        accept: '.pdf,.jpg,.jpeg,.png',
        label: t('content.uploadAttachment'),
        labelClass: 'mb-2 block font-medium text-neutral-800',
        labelRequired: !isEditMode.value,
        containerClass: 'mb-8 col-span-2',
        wrapperClass: 'w-full',
        disabled: isEditMode.value,
        showPreview: true,
        fileUrl: invoiceDetail.value?.taxedInvoiceAttachment,
        clearFileUrl: () => {
          if (!invoiceDetail.value) return

          invoiceDetail.value.taxedInvoiceAttachment = null
        },
      },
      {
        name: 'linkedTransfer',
        as: 'BaseCheckbox',
        label: t('form.linkedTransfer'),
        labelClass: 'mb-2 block font-medium text-neutral-800',
        containerClass: 'mb-8 col-span-2',
        wrapperClass: 'w-full',
        onChange: (value: Event) => {
          const checked = (value.target as HTMLInputElement).checked
          invoiceStore.$patch({ invoice: { linkedTransfer: checked } })

          if (checked) {
            void fetchTransfers()

            return
          }

          resetTransferRows()
          invoice.value.payments = []
        },
      },
      ...(isIncludedContract.value && !isClaimWizard.value ? [{
        name: 'excludeExtraKmsFromMainInvoice',
        as: 'BaseCheckbox',
        label: t('form.extraKm'),
        labelClass: 'mb-3.5 inline-block font-bold text-neutral-800',
        labelRequired: true,
        checkboxLabel: snapshotExtraKms.value === null
          ? t('form.excludeExtraKmsFromMainInvoice')
          : `${t('form.excludeExtraKmsFromMainInvoice')} ( ${snapshotExtraKms.value.toLocaleString()} ${t('form.kmUnit')} )`,
        containerClass: 'mb-4 col-span-2',
        wrapperClass: 'w-full',
        disabled: isEditMode.value,
        onChange: (value: Event) => {
          invoice.value.excludeExtraKmsFromMainInvoice = (value.target as HTMLInputElement).checked
          resetTransferRows()
        },
      }] : []),
      ...(!invoice.value.linkedTransfer ? [{
        name: 'status',
        as: 'BaseRadioGroup',
        options: invoiceStatusOptions.value,
        label: t('form.invoiceStatus'),
        labelClass: 'mb-3.5 inline-block font-bold text-neutral-800',
        labelRequired: true,
        wrapperClass: 'gap-40',
        containerClass: 'w-full col-span-2 mb-6',
        disabled: isEditMode.value,
        onChange: (value: string | number) => {
          invoice.value.status = Number(value)
        },
      }] : []),
    ]
  })

  const nextSubmitLabel = computed(() =>
    isEditMode.value ? t('button.editInvoice') : t('button.createInvoice'),
  )

  function buildPayments(): InvoicePayment[] {
    if (!invoice.value.linkedTransfer) return []

    return transferRows.value
      .filter(row => row.bankTransferId && row.amount !== '' && Number(row.amount) > 0)
      .map(row => ({
        bankTransferId: row.bankTransferId,
        amount: Number(row.amount),
        ...(row.invoicePaymentId != null ? { invoicePaymentId: row.invoicePaymentId } : {}),
      }))
  }

  function onFormSubmit(values: Record<string, unknown>): void {
    if (isEditMode.value) {
      void submit()

      return
    }

    invoice.value.startDate = String(values.startDate ?? '')
    invoice.value.invoiceType = Number(values.invoiceType)
    invoice.value.endDate = String(values.endDate ?? '')
    invoice.value.dueDate = String(values.dueDate ?? '')
    invoice.value.issueDate = String(values.issueDate ?? '')
    invoice.value.discountId = Number(values.discountId)
    invoice.value.taxId = Number(values.taxId)
    invoice.value.contractBillingSnapshotId = isMonthlyContract.value && values.contractBillingSnapshotId
      ? Number(values.contractBillingSnapshotId)
      : null
    invoice.value.taxedInvoiceAttachment = (values.taxedInvoiceAttachment as File | null) ?? null
    invoice.value.linkedTransfer = Boolean(values.linkedTransfer)
    invoice.value.excludeExtraKmsFromMainInvoice = isIncludedContract.value
      ? Boolean(values.excludeExtraKmsFromMainInvoice)
      : false
    invoice.value.status = Number(values.status)
    invoice.value.payments = buildPayments()
    void submit()
  }

  async function onSuccessPopupUpdate(value: boolean): Promise<void> {
    isSuccessPopupOpen.value = value
    if (value) return

    if (isEditMode.value) {
      await router.push({
        name: 'FinancialInvoiceView',
        params: { id: String(invoiceId.value) }
      })

      return
    }

    if (route.query.returnTo) {
      router.push({
        name: route.query.returnTo.toString(),
        params: { id: String(route.query.snapshotId ?? '') },
      })

      return
    }

    await router.push({
      name: 'FinancialCompanyDetails',
      params: { id: contractId },
    })
  }

  function setSelectedTripsIds(): void {
    if (isEditMode.value) return

    const filteredTrips = tripsList.value?.data.filter(trip =>
      (isClaimWizard.value && trip.requiresClaimInvoice) || (!isClaimWizard.value && trip.requiresMainInvoice)
    )
    invoice.value.tripIds = selectedTripsIds.value.map(id => Number(id)) ?? filteredTrips?.map(trip => Number(trip.id))
  }

  async function submit(): Promise<void> {
    submitError.value = ''
    submitErrors.value = {}
    successMessage.value = ''
    isErrorPopupOpen.value = false

    setSelectedTripsIds()

    const { success, message, errors } = isEditMode.value
      ? await invoiceStore.linkPayments(invoiceId.value, { payments: buildPayments() })
      : await invoiceStore.create()

    if (success) {
      successMessage.value = message || ''
      isSuccessPopupOpen.value = true

      return
    }

    submitError.value = message || t('error.networkError')
    submitErrors.value = (errors as Record<string, string[]>) ?? {}
    isErrorPopupOpen.value = true
  }

  watch(isMonthlyContract, (isMonthly) => {
    if (!isMonthly) return

    getContractBillingSnapshots(contractId)
  }, { immediate: true })

  onMounted(async () => {
    if (!isEditMode.value || !invoice.value.linkedTransfer) return

    await fetchTransfers()
    await initializeTransferRowsFromPayments(invoice.value.payments)
  })

  return {
    availableTransferOptions,
    fieldsSchema,
    initialValues,
    invoice,
    invoiceId,
    invoiceStore,
    isEditMode,
    isErrorPopupOpen,
    isSuccessPopupOpen,
    maxAmountForRow,
    newTransferId,
    nextSubmitLabel,
    onFormSubmit,
    onNewTransferSelect,
    onSuccessPopupUpdate,
    removeTransferRow,
    snapshotId,
    submitError,
    submitErrors,
    successMessage,
    transferRows,
    updateTransferAmount,
    validationSchema,
  }
}
