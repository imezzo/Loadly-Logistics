import type { ContractsListRequest } from "@/types/request/contract/contract"

export function useInvoiceListFilter() {
  const filters = ref<ContractsListRequest>({
    companyId: '',
    contractId: '',
  })

  function updateFilters(partial: Partial<ContractsListRequest>): void {
    filters.value = { ...filters.value, ...partial }
  }

  function resetFilters(): void {
    updateFilters({ companyId: '', contractId: '' })
  }

  const hasActiveFilters = computed(() =>
    filters.value.companyId !== '' || filters.value.contractId !== '')

  return {
    filters,
    updateFilters,
    resetFilters,
    hasActiveFilters,
  }
}
