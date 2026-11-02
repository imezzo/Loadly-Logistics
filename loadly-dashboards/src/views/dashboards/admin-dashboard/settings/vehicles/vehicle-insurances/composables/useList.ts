import type { VehicleInsurance } from '@/types/admin-dashboard/response/vehicle/vehicle-insurance'
import type { Pagination } from '@/types/response/pagination'
import { useVehicleInsuranceSettingsStore } from '@/stores/admin-dashboard/vehicle-insurance-settings'
import { usePagination } from '@/composables/usePagination'
import { useLoading } from '@/composables/useLoading'

export function useList() {
  const vehicleInsuranceSettingsStore = useVehicleInsuranceSettingsStore()
  const { vehicleInsurancesList } = storeToRefs(vehicleInsuranceSettingsStore)

  const { isLoading, setLoading } = useLoading()

  const insurancePagination = computed<Pagination | null>(
    () => vehicleInsurancesList.value?.pagination || null
  )

  const {
    currentPage,
    itemsPerPage,
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
  } = usePagination(insurancePagination)

  async function fetchVehicleInsurances() {
    setLoading(true)

    const { success, data } = await vehicleInsuranceSettingsStore.getList({
      pagination: {
        page: currentPage.value,
        size: itemsPerPage.value,
      },
    })

    setLoading(false)
    vehicleInsurancesList.value = success ? data : null
  }

  watch(itemsPerPage, () => {
    currentPage.value = 1
    fetchVehicleInsurances()
  })

  watch(currentPage, () => {
    fetchVehicleInsurances()
  })

  const vehicleInsurances = computed<VehicleInsurance[]>(
    () => vehicleInsurancesList.value?.vehicleInsurances || []
  )

  return {
    vehicleInsurances,
    fetchVehicleInsurances,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    setPage,
    updateItemsPerPage,
  }
}
