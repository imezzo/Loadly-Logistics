import type {
  VehicleInsuranceCreateRequest,
  VehicleInsuranceUpdateRequest,
  VehicleInsurancesListRequest,
} from '@/types/admin-dashboard/request/vehicle-insurance-settings'
import { vehicleInsurancesListRequestAdapter } from '@/types/admin-dashboard/request/vehicle-insurance-settings'
import { vehicleInsuranceAdapter } from '@/types/admin-dashboard/response/vehicle/vehicle-insurance'
import { vehicleInsurancesListResponseAdapter } from '@/types/admin-dashboard/response/vehicle/vehicle-insurance'
import type { ApiVehicleInsurance } from '@/types/admin-dashboard/response/vehicle/vehicle-insurance'
import type { VehicleInsurancesListResponse } from '@/types/admin-dashboard/response/vehicle/vehicle-insurance'
import { api } from '@/utils/api'

export const useVehicleInsuranceSettingsStore = defineStore(
  'vehicle-insurance-settings',
  () => {
    const vehicleInsurancesList = ref<VehicleInsurancesListResponse | null>(
      null
    )

    async function getList(data: VehicleInsurancesListRequest) {
      return api({
        url: '/settings/vehicle-insurances',
        method: 'GET',
        params: vehicleInsurancesListRequestAdapter(data),
        responseAdapter: vehicleInsurancesListResponseAdapter,
      })
    }

    async function getById(id: number) {
      return api({
        url: `/settings/vehicle-insurances/${id}`,
        method: 'GET',
        responseAdapter: (data: ApiVehicleInsurance) =>
          vehicleInsuranceAdapter(data),
      })
    }

    async function create(data: VehicleInsuranceCreateRequest) {
      return api({
        url: '/settings/vehicle-insurances',
        method: 'POST',
        data,
      })
    }

    async function update(id: number, data: VehicleInsuranceUpdateRequest) {
      return api({
        url: `/settings/vehicle-insurances/${id}`,
        method: 'PUT',
        data,
      })
    }

    async function deleteVehicleInsurance(id: number) {
      return api({
        url: `/settings/vehicle-insurances/${id}`,
        method: 'DELETE',
      })
    }

    return {
      vehicleInsurancesList,
      getList,
      getById,
      create,
      update,
      deleteVehicleInsurance,
    }
  }
)
