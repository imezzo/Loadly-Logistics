import type { VehicleListRequest } from '@/types/admin-dashboard/request/vehicle/vehicle'
import type { Vehicle } from '@/types/admin-dashboard/response/vehicle/vehicle'
import { useVehicleStore } from '@/stores/admin-dashboard/vehicle'
import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'

export interface VehicleStatusFilters {
  search: string
  companyId: string
  contractId: string
  sectorId: string
  facilityType: string
  facilityIds: string
  vehicleType: string
}

export function vehicleStatusFiltersToRequest(filters: VehicleStatusFilters): VehicleListRequest {
  return {
    search: filters.search,
    sectorIds: filters.sectorId ? [filters.sectorId] : undefined,
    facilityTypes: filters.facilityType ? [Number(filters.facilityType)] : undefined,
    facilityIds: filters.facilityIds ? Number(filters.facilityIds) : undefined,
    vehicleTypeIds: filters.vehicleType ? [filters.vehicleType] : undefined,
  }
}

const emptyFilters = () => ({
  search: '',
  companyId: '',
  contractId: '',
  sectorId: '',
  facilityType: '',
  facilityIds: '',
  vehicleType: '',
})

export function useVehicleStatusList() {
  const vehicleStore = useVehicleStore()
  const { vehiclesList} = storeToRefs(vehicleStore)

  const filters = ref<VehicleStatusFilters>(emptyFilters())

  const { setLoading } = useLoading()
  const {
    currentPage,
    itemsPerPage,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(computed(() => vehiclesList.value?.pagination || null))

  async function fetchVehicles(): Promise<void> {
    setLoading(true)

    const { success, data } = await vehicleStore.getVehicles({
      ...vehicleStatusFiltersToRequest(filters.value),
      page: currentPage.value,
      size: itemsPerPage.value,
      search: filters.value.search,
      contractId: filters.value.contractId
        ? Number(filters.value.contractId)
        : undefined,
      sectorIds: filters.value.sectorId
        ? [filters.value.sectorId]
        : undefined,
      facilityTypes: filters.value.facilityType
        ? [Number(filters.value.facilityType)]
        : undefined,
      facilityIds: filters.value.facilityIds
        ? Number(filters.value.facilityIds)
        : undefined,
      vehicleTypeIds: filters.value.vehicleType
        ? [filters.value.vehicleType]
        : undefined,
    })

    vehiclesList.value = success ? data : null
    setLoading(false)
  }

  function applyFilters(): void {
    currentPage.value = 1
    fetchVehicles()
  }

  function resetFilters(): void {
    filters.value = emptyFilters()
    currentPage.value = 1
    fetchVehicles()
  }

  watch(itemsPerPage, () => {
    currentPage.value = 1
    fetchVehicles()
  })

  watch(currentPage, () => {
    fetchVehicles()
  })

  const vehicles = computed<Vehicle[]>(() => vehiclesList.value?.vehicles || [])

  return {
    applyFilters,
    fetchVehicles,
    filters,
    nextPage,
    pagination,
    prevPage,
    resetFilters,
    setPage,
    updateItemsPerPage,
    vehicles,
  }
}
