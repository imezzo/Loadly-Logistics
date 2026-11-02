import { createSharedComposable } from '@vueuse/core'
import { useLoading } from '@/composables/useLoading'
import { usePagination } from '@/composables/usePagination'
import { useInvoiceStore } from '@/stores/financial/invoice'
import type { InvoicesListRequest } from '@/types/request/financial/invoice-list'

function usePaymentListBase() {
  const { setLoading } = useLoading()
  const invoiceStore = useInvoiceStore()
  const { invoicesList, invoicesListFilters: filters } = storeToRefs(invoiceStore)

  const {
    currentPage,
    itemsPerPage,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  } = usePagination(computed(() => invoicesList.value?.pagination ?? null))

  async function fetchInvoices(): Promise<void> {
    setLoading(true)
    const { data } = await invoiceStore.getList({
      ...filters.value,
      page: currentPage.value,
      size: itemsPerPage.value,
    })
    invoicesList.value = data

    setLoading(false)
  }

  function updateFilters(partial: Partial<InvoicesListRequest>): void {
    filters.value = { ...filters.value, ...partial }
  }

  function applyFilters(): void {
    currentPage.value = 1
    fetchInvoices()
  }

  function resetFilters(): void {
    filters.value = {
      companyId: '',
      contractId: '',
      invoiceType: '',
      isLeadBilling: false,
      leadId: '',
      search: '',
      status: '',
    }
    applyFilters()
  }

  const hasActiveFilters = computed(() => {
    return !!(
      filters.value.companyId
      || filters.value.contractId
      || filters.value.invoiceType
      || filters.value.isLeadBilling
      || filters.value.leadId
      || filters.value.search?.trim()
      || filters.value.status
    )
  })

  const rows = computed(() => invoicesList.value?.data ?? [])

  watch([currentPage, itemsPerPage], fetchInvoices)

  return {
    applyFilters,
    currentPage,
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

export const usePaymentList = createSharedComposable(usePaymentListBase)
