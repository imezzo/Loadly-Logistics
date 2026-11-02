import type { Vehicle } from '@/types/admin-dashboard/response/vehicle/vehicle'
import { useVehicleStore } from '@/stores/admin-dashboard/vehicle'
import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'

export interface FleetFilters {
  companyId?: string
  contractId?: string
  contractorIds?: string[]
  sectorIds?: string[]
  search?: string
  vehicleUnitType?: string
  vehicleCategory?: string
  vehicleType?: string
  status?: string
}

export function useList() {
  const vehicleStore = useVehicleStore()
  const { vehiclesList, filters } = storeToRefs(vehicleStore)

  const { isLoading, setLoading } = useLoading()
  const {
    currentPage,
    itemsPerPage,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(computed(() => vehiclesList.value?.pagination || null))

  async function fetchVehicles() {
    setLoading(true)

    const { success, data } = await vehicleStore.getVehicles({
      page: currentPage.value,
      size: itemsPerPage.value,
      contractId: filters.value.contractId ? Number(filters.value.contractId) : undefined,
      contractorIds: filters.value.contractorIds?.length ? filters.value.contractorIds : undefined,
      search: filters.value.search || undefined,
      sectorIds: filters.value.sectorIds?.length ? filters.value.sectorIds : undefined,
      VehicleUnitType: filters.value.vehicleUnitType ? Number(filters.value.vehicleUnitType) : undefined,
      VehicleTypeCategory: filters.value.vehicleCategory ? Number(filters.value.vehicleCategory) : undefined,
      vehicleTypeIds: filters.value.vehicleType ? [String(filters.value.vehicleType)] : undefined,
      status: filters.value.status ? Number(filters.value.status) : undefined,
    })

    vehiclesList.value = success ? data : null
    setLoading(false)
  }

  function updateFilters(newFilters: Partial<FleetFilters>) {
    filters.value = { ...filters.value, ...newFilters }
    currentPage.value = 1
    fetchVehicles()
  }

  function resetFilters() {
    filters.value = {
      companyId: '',
      contractId: '',
      contractorIds: [],
      search: '',
      sectorIds: [],
      vehicleUnitType: '',
      vehicleCategory: '',
      vehicleType: '',
      status: '',
    }
    currentPage.value = 1
    fetchVehicles()
  }

  watch(
    [itemsPerPage],
    () => {
      currentPage.value = 1
      fetchVehicles()
    },
  )

  watch(currentPage, () => {
    fetchVehicles()
  })

  const vehicles = computed<Vehicle[]>(() => vehiclesList.value?.vehicles || [])

  return {
    vehicles,
    fetchVehicles,
    isLoading,
    filters,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
    updateFilters,
    resetFilters,
  }
}
