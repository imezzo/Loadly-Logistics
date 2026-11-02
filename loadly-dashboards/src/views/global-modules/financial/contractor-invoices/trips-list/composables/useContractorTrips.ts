import { useContractorStore } from '@/stores/admin-dashboard/contractor'
import type { ContractorStatistics } from '@/types/response/financial/contractor-statistics'

export function useContractorTrips() {
  const route = useRoute()
  const contractorStore = useContractorStore()

  const statistics = ref<ContractorStatistics | null>(null)

  const contractorId = computed(() => String(route.params.contractorId ?? ''))

  const contractorName = computed(() => statistics.value?.contractorFullName ?? '')

  async function fetchStatistics(): Promise<void> {
    if (!contractorId.value) return

    const { success, data } = await contractorStore.getStatistics(contractorId.value)
    statistics.value = success ? data : null
  }

  watch(contractorId, fetchStatistics)

  return {
    contractorId,
    contractorName,
    fetchStatistics,
    statistics,
  }
}
