import { useLoading } from '@/composables/useLoading'
import { usePagination } from '@/composables/usePagination'
import { useInvoiceStore } from '@/stores/financial/invoice'
import { useProfile } from '@/composables/admin-dashboard/useProfile'

export interface InvoiceReportsFilters {
  companyId: string
  contractId: string
  status: string
  search: string
  dateRange: string
}

export function useInvoiceReportsList() {
  const { setLoading } = useLoading()
  const invoiceStore = useInvoiceStore()
  const { invoicesList } = storeToRefs(invoiceStore)
  const { profile } = useProfile()

  const filters = ref<InvoiceReportsFilters>({
    companyId: '',
    contractId: '',
    status: '',
    search: '',
    dateRange: '',
  })

  const {
    currentPage,
    itemsPerPage,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(computed(() => invoicesList.value?.pagination ?? null))

  async function fetchInvoices(): Promise<void> {
    setLoading(true)
    const { data } = await invoiceStore.getList({
      page: currentPage.value,
      size: itemsPerPage.value,
      companyId: filters.value.companyId,
      contractId: filters.value.contractId,
      status: filters.value.status,
      search: filters.value.search,
    })
    invoicesList.value = data
    setLoading(false)
  }

  function updateFilters(partial: Partial<InvoiceReportsFilters>): void {
    filters.value = { ...filters.value, ...partial }
  }

  function applyFilters(): void {
    currentPage.value = 1
    fetchInvoices()
  }

  function resetFilters(): void {
    filters.value = {
      companyId: profile.value?.id?.toString() ?? '',
      contractId: '',
      status: '',
      search: '',
      dateRange: '',
    }
    applyFilters()
  }

  const hasActiveFilters = computed(() => {
    return !!(
      filters.value.contractId
      || filters.value.status
      || filters.value.search.trim()
    )
  })

  const rows = computed(() => invoicesList.value?.data ?? [])

  watch([currentPage, itemsPerPage], () => {
    fetchInvoices()
  })

  return {
    applyFilters,
    fetchInvoices,
    filters,
    hasActiveFilters,
    nextPage,
    pagination,
    prevPage,
    resetFilters,
    rows,
    setPage,
    updateFilters,
    updateItemsPerPage,
  }
}
