import { contractsListRequestAdapter, createContractRequestAdapter } from "@/types/request/contract/contract"
import type { ContractsListRequest, CreateContractRequest } from "@/types/request/contract/contract"
import { updateContractRequestAdapter } from "@/types/request/contract/contract-update"
import type { UpdateContractRequest } from "@/types/request/contract/contract-update"
import { contractListAsLookupResponseAdapter, contractsListResponseAdapter } from "@/types/admin-dashboard/response/contract/contract"
import type { ContractsListResponse } from "@/types/admin-dashboard/response/contract/contract"
import { contractDestinationDetailResponseAdapter } from "@/types/admin-dashboard/response/contract/contract-destination-detail"
import { contractDetailResponseAdapter } from "@/types/admin-dashboard/response/contract/contract-details"
import type { ContractDetails, ContractsFilter } from "@/types/admin-dashboard/response/contract/contract-details"
import type { ContractDestinationsCalculateRequest } from "@/types/admin-dashboard/contract-destinations-calculate"
import { calculateSlotChargesResponseAdapter } from "@/types/admin-dashboard/response/contract/included-slot-charges-calculate"
import type { CalculateSlotChargesResponse } from "@/types/admin-dashboard/response/contract/included-slot-charges-calculate"
import type { CalculateSlotChargesRequest } from "@/types/request/contract/included-slot-charges-calculate"
import { calculateSlotChargesRequestAdapter } from "@/types/request/contract/included-slot-charges-calculate"
import type { ContractVehicleSlot } from "@/types/admin-dashboard/response/contract/contract-slots"
import type { VehicleType } from "@/types/request/contract/contract"
import type { LookupResponse } from "@/types/response/lookup"
import { api } from "@/utils/api"

export const useContractStore = defineStore('contract', () => {
  const contractsList = ref<ContractsListResponse | null>(null)
  const contractsListAsLookup = ref<LookupResponse | null>(null)
  const contract = ref<ContractDetails | null>(null)
  const originalVehicleSlots = ref<ContractVehicleSlot[]>([])
  const originalVehicleTypes = ref<VehicleType[]>([])
  const wizardFormData = ref<Record<string, unknown>>({})
  const filters = ref<ContractsFilter>({
    search: '',
    status: '',
    contractId: '',
    contractType: '',
    company: '',
    dateRange: '',
    salesRepresentative: '',
    valueRange: '',
  })
  const contractConsumptionType = ref<number | null>(null)
  const slotCharges = ref<CalculateSlotChargesResponse | null>(null)

  async function getList(data: ContractsListRequest) {
    return api({
      url: '/contracts',
      method: 'GET',
      params: contractsListRequestAdapter(data),
      responseAdapter: contractsListResponseAdapter,
    })
  }

  async function getListAsLookup(data: ContractsListRequest) {
    return api({
      url: '/contracts',
      method: 'GET',
      params: contractsListRequestAdapter(data),
      responseAdapter: contractListAsLookupResponseAdapter,
    })
  }

  async function create(data: CreateContractRequest) {
    return api({
      url: '/contracts',
      method: 'POST',
      data: createContractRequestAdapter(data),
    })
  }

  async function calculateDestinations(data: ContractDestinationsCalculateRequest) {
    return api({
      url: '/contracts/destinations/calculate',
      method: 'POST',
      data,
    })
  }

  async function getContractById(id: string | number) {
    const result = await api({
      url: `/contracts/${id}`,
      method: 'GET',
      responseAdapter: contractDetailResponseAdapter,
    })

    if (result.success && result.data) {
      contract.value = result.data
      originalVehicleSlots.value = structuredClone(result.data.vehicleSlots ?? [])
      originalVehicleTypes.value = structuredClone(result.data.vehicleTypes ?? [])
    }

    return result
  }

  async function getContractDestinationById(destinationId: string | number) {
    return api({
      url: `/contracts/destinations/${destinationId}`,
      method: 'GET',
      responseAdapter: contractDestinationDetailResponseAdapter,
    })
  }

  async function calculateSlotCharges(
    contractId: string | number,
    data: CalculateSlotChargesRequest,
  ) {
    const result = await api({
      url: `/contracts/${contractId}/slot-charges/calculate`,
      method: 'POST',
      data: calculateSlotChargesRequestAdapter(data),
      responseAdapter: calculateSlotChargesResponseAdapter,
    })

    if (result.success && result.data) {
      slotCharges.value = result.data
    }

    return result
  }

  async function exportSlotCharges(
    contractId: string | number,
    data: CalculateSlotChargesRequest,
  ) {
    return api({
      url: `/contracts/${contractId}/slot-charges/excel`,
      method: 'GET',
      responseType: 'blob',
      params: {
        ...calculateSlotChargesRequestAdapter(data),
        contractId, // TODO: remove this once the endpoint is updated
      },
      headers: {
        Accept: '*/*',
      },
    })
  }

  async function getContractConsumptionType(contractId: string | number) {
    return api({
      url: `/contracts/${contractId}/consumption-type`,
      method: 'GET',
      responseAdapter: (data: { consumptionType: number }) => {
        contractConsumptionType.value = data.consumptionType

        return data
      },
    })
  }

  async function update(id: string | number, data: UpdateContractRequest) {
    return api({
      url: `/contracts/${id}`,
      method: 'PUT',
      data: updateContractRequestAdapter(data, {
        originalVehicleSlots: originalVehicleSlots.value,
        originalVehicleTypes: originalVehicleTypes.value,
      }),
    })
  }

  async function submitDecision(
    id: string | number,
    data: { Action: 1 | 2; Comments: string }
  ) {
    return api({
      url: `/contracts/${id}/approvals/submit-decision`,
      method: 'POST',
      data,
    })
  }

  async function saveAsDraft(id: string | number) {
    return api({
      url: `/contracts/${id}/pull-back`,
      method: 'POST',
    })
  }

  async function deactivate(
    id: string | number,
    data: { Reason: number; EffectiveDate: string; Notes: string }
  ) {
    return api({
      url: `/contracts/${id}/deactivate`,
      method: 'POST',
      data,
    })
  }

  async function terminate(id: string | number, data: { Notes: string }) {
    return api({
      url: `/contracts/${id}/terminate`,
      method: 'POST',
      data,
    })
  }

  async function reactivate(id: string | number, data: { Notes: string }) {
    return api({
      url: `/contracts/${id}/reactivate`,
      method: 'POST',
      data,
    })
  }

  return {
    calculateDestinations,
    calculateSlotCharges,
    exportSlotCharges,
    contract,
    contractConsumptionType,
    contractsList,
    contractsListAsLookup,
    create,
    deactivate,
    filters,
    getContractById,
    getContractConsumptionType,
    getContractDestinationById,
    getList,
    getListAsLookup,
    originalVehicleSlots,
    originalVehicleTypes,
    reactivate,
    saveAsDraft,
    slotCharges,
    submitDecision,
    terminate,
    update,
    wizardFormData,
  }
})
