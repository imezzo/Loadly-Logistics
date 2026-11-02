import type { ContractsFilter } from "@/types/admin-dashboard/response/contract/contract-details"
import { useContractStore } from "@/stores/admin-dashboard/contract"

export function useListFilter() {
  const contractStore = useContractStore()
  const { filters } = storeToRefs(contractStore)

  function updateFilters(newFilters: Partial<ContractsFilter>) {
    filters.value = { ...filters.value, ...newFilters }
  }

  function resetFilters() {
    updateFilters({
      search: '',
      status: '',
      contractId: '',
      contractType: '',
      company: '',
      dateRange: '',
      salesRepresentative: '',
      valueRange: '',
    })
  }

  return {
    filters,
    updateFilters,
    resetFilters,
  }
}

