import { api } from "@/utils/api"
import { extraFeeTypesApiResponseAdapter } from "@/types/admin-dashboard/response/contract/extra-fee-type"
import type { ExtraFeeType } from "@/types/admin-dashboard/response/contract/extra-fee-type"
import { lookupResponseAdapter } from "@/types/response/lookup"
import type { LookupResponse } from "@/types/response/lookup"
import type { LookupRequest } from "@/types/request/lookup"
import { lookupRequestAdapter } from "@/types/request/lookup"

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL + '/lookups'

export const useLookupStore = defineStore('lookup', () => {
  const admins = ref<LookupResponse | null>(null)
  const cities = ref<LookupResponse>({} as LookupResponse)
  const companies = ref<LookupResponse>({} as LookupResponse)
  const communicationMethods = ref<LookupResponse>({} as LookupResponse)
  const contractors = ref<LookupResponse | null>(null)
  const contractBillingSnapshots = ref<LookupResponse | null>(null)
  const contractDestinations = ref<LookupResponse | null>(null)
  const contractReferenceNumbers = ref<LookupResponse | null>(null)
  const contractTypes = ref<LookupResponse | null>(null)
  const contractVehicleTypes = ref<LookupResponse | null>(null)
  const drivers = ref<LookupResponse | null>(null)
  const extraFeeTypes = ref<ExtraFeeType[]>([])
  const discounts = ref<LookupResponse | null>(null)
  const fuelTypes = ref<LookupResponse | null>(null)
  const governments = ref<LookupResponse>({} as LookupResponse)
  const industryTypes = ref<LookupResponse>({} as LookupResponse)
  const leads = ref<LookupResponse | null>(null)
  const operatingHours = ref<LookupResponse>({} as LookupResponse)
  const roles = ref<LookupResponse | null>(null)
  const sectors = ref<LookupResponse | null>(null)
  const serviceRequirements = ref<LookupResponse>({} as LookupResponse)
  const tags = ref<LookupResponse | null>(null)
  const taxes = ref<LookupResponse | null>(null)
  const vehicleBrands = ref<LookupResponse | null>(null)
  const vehicleBrandModels = ref<LookupResponse | null>(null)
  const vehicleCapacityUnits = ref<LookupResponse | null>(null)
  const vehicleFacilities = ref<LookupResponse | null>(null)
  const vehicleInsurances = ref<LookupResponse | null>(null)
  const vehicleTypes = ref<LookupResponse | null>(null)

  function getRoles(params: LookupRequest) {
    return api({
      url: API_BASE_URL + '/roles/default',
      method: 'GET',
      params: lookupRequestAdapter(params),
      responseAdapter: lookupResponseAdapter,
    })
  }

  function getCities(params: LookupRequest) {
    return api({
      url: API_BASE_URL + '/cities/default',
      method: 'GET',
      params: lookupRequestAdapter(params),
      responseAdapter: lookupResponseAdapter,
    })
  }

  function getCommunicationMethods(params: LookupRequest) {
    return api({
      url: API_BASE_URL + '/communicationmethods/default',
      method: 'GET',
      params: lookupRequestAdapter(params),
      responseAdapter: lookupResponseAdapter,
    })
  }

  function getGovernments(params: LookupRequest) {
    return api({
      url: API_BASE_URL + '/governerates/default',
      method: 'GET',
      params: lookupRequestAdapter(params),
      responseAdapter: lookupResponseAdapter,
    })
  }

  function getIndustryTypes(params: LookupRequest) {
    return api({
      url: API_BASE_URL + '/industrytypes/default',
      method: 'GET',
      params: lookupRequestAdapter(params),
      responseAdapter: lookupResponseAdapter,
    })
  }

  function getServiceRequirements(params: LookupRequest) {
    return api({
      url: API_BASE_URL + '/servicerequirements/default',
      method: 'GET',
      params: lookupRequestAdapter(params),
      responseAdapter: lookupResponseAdapter,
    })
  }

  function getOperatingHours(params: LookupRequest) {
    return api({
      url: API_BASE_URL + '/OperatingHours/default',
      method: 'GET',
      params: lookupRequestAdapter(params),
      responseAdapter: lookupResponseAdapter,
    })
  }

  function getCompanies(params: LookupRequest) {
    return api({
      url: API_BASE_URL + '/companies/default',
      method: 'GET',
      params: lookupRequestAdapter(params),
      responseAdapter: lookupResponseAdapter,
    })
  }

  function getContractors(params: LookupRequest) {
    return api({
      url: API_BASE_URL + '/contractors/default',
      method: 'GET',
      params: lookupRequestAdapter(params),
      responseAdapter: lookupResponseAdapter,
    })
  }

  function getLeads(params: LookupRequest) {
    return api({
      url: API_BASE_URL + '/leads/default',
      method: 'GET',
      params: lookupRequestAdapter(params),
      responseAdapter: lookupResponseAdapter,
    })
  }

  async function getContractReferenceNumbers(params: LookupRequest) {
    return api({
      url: API_BASE_URL + '/contractreferencenumbers/default',
      method: 'GET',
      params: lookupRequestAdapter(params),
      responseAdapter: lookupResponseAdapter,
    })
  }

  async function getContractVehicleTypes(params: LookupRequest) {
    return api({
      url: API_BASE_URL + '/contractvehicletypes/default',
      method: 'GET',
      params: lookupRequestAdapter(params),
      responseAdapter: lookupResponseAdapter,
    })
  }

  async function getContractBillingSnapshots(params: LookupRequest) {
    return api({
      url: API_BASE_URL + '/contractbillingsnapshots/default',
      method: 'GET',
      params: lookupRequestAdapter(params),
      responseAdapter: lookupResponseAdapter,
    })
  }

  async function getContractDestinations(params: LookupRequest) {
    return api({
      url: API_BASE_URL + '/contractdestinations/default',
      method: 'GET',
      params: lookupRequestAdapter(params),
      responseAdapter: lookupResponseAdapter,
    })
  }

  async function getVehicleTypes(params: LookupRequest) {
    return api({
      url: API_BASE_URL + '/vehicletypes/default',
      method: 'GET',
      params: lookupRequestAdapter(params),
      responseAdapter: lookupResponseAdapter,
    })
  }

  async function getVehicleBrands(params: LookupRequest) {
    return api({
      url: API_BASE_URL + '/vehiclebrands/default',
      method: 'GET',
      params: lookupRequestAdapter(params),
      responseAdapter: lookupResponseAdapter,
    })
  }

  async function getVehicleBrandModels(params: LookupRequest) {
    return api({
      url: API_BASE_URL + '/vehiclebrandmodels/default',
      method: 'GET',
      params: lookupRequestAdapter(params),
      responseAdapter: lookupResponseAdapter,
    })
  }

  async function getVehicleCapacityUnits(params: LookupRequest) {
    return api({
      url: API_BASE_URL + '/vehiclecapacityunits/default',
      method: 'GET',
      params: lookupRequestAdapter(params),
      responseAdapter: lookupResponseAdapter,
    })
  }

  async function getVehicleInsurances(params: LookupRequest) {
    return api({
      url: API_BASE_URL + '/vehicleinsurances/default',
      method: 'GET',
      params: lookupRequestAdapter(params),
      responseAdapter: lookupResponseAdapter,
    })
  }

  async function getDrivers(params: LookupRequest) {
    return api({
      url: API_BASE_URL + '/drivers/default',
      method: 'GET',
      params: lookupRequestAdapter(params),
      responseAdapter: lookupResponseAdapter,
    })
  }

  function getAdmins(params: LookupRequest) {
    return api({
      url: API_BASE_URL + '/admins/default',
      method: 'GET',
      params: lookupRequestAdapter(params),
      responseAdapter: lookupResponseAdapter,
    })
  }

  async function getExtraFeeTypes() {
    const result = await api({
      url: API_BASE_URL + '/extra-fee-types',
      method: 'GET',
      responseAdapter: extraFeeTypesApiResponseAdapter,
    })

    if (result.success && result.data) {
      extraFeeTypes.value = result.data as ExtraFeeType[]
    }

    return result
  }

  function getDiscounts(params: LookupRequest) {
    return api({
      url: API_BASE_URL + '/discounts/default',
      method: 'GET',
      params: lookupRequestAdapter(params),
      responseAdapter: lookupResponseAdapter,
    })
  }

  async function getFuelTypes(params: LookupRequest) {
    return api({
      url: API_BASE_URL + '/fueltypes/default',
      method: 'GET',
      params: lookupRequestAdapter(params),
      responseAdapter: lookupResponseAdapter,
    })
  }

  function getTags(params: LookupRequest) {
    return api({
      url: API_BASE_URL + '/tags/default',
      method: 'GET',
      params: lookupRequestAdapter(params),
      responseAdapter: lookupResponseAdapter,
    })
  }

  function getTaxes(params: LookupRequest) {
    return api({
      url: API_BASE_URL + '/taxes/default',
      method: 'GET',
      params: lookupRequestAdapter(params),
      responseAdapter: lookupResponseAdapter,
    })
  }

  async function getSectors(params: LookupRequest) {
    return api({
      url: API_BASE_URL + '/sectors/default',
      method: 'GET',
      params: lookupRequestAdapter(params),
      responseAdapter: lookupResponseAdapter,
    })
  }

  function getVehicleFacilities(params: LookupRequest) {
    return api({
      url: API_BASE_URL + '/vehiclefacilities/default',
      method: 'GET',
      params: lookupRequestAdapter(params),
      responseAdapter: lookupResponseAdapter,
    })
  }

  return {
    admins,
    cities,
    companies,
    communicationMethods,
    contractors,
    contractBillingSnapshots,
    contractDestinations,
    contractReferenceNumbers,
    contractTypes,
    contractVehicleTypes,
    discounts,
    drivers,
    fuelTypes,
    extraFeeTypes,
    getAdmins,
    getCities,
    getCompanies,
    getCommunicationMethods,
    getContractors,
    getContractBillingSnapshots,
    getContractDestinations,
    getContractReferenceNumbers,
    getContractVehicleTypes,
    getDiscounts,
    getDrivers,
    getFuelTypes,
    getExtraFeeTypes,
    getGovernments,
    getIndustryTypes,
    getLeads,
    getOperatingHours,
    getRoles,
    getSectors,
    getServiceRequirements,
    getTags,
    getTaxes,
    getVehicleBrands,
    getVehicleBrandModels,
    getVehicleCapacityUnits,
    getVehicleFacilities,
    getVehicleInsurances,
    getVehicleTypes,
    governments,
    industryTypes,
    leads,
    operatingHours,
    roles,
    sectors,
    serviceRequirements,
    tags,
    taxes,
    vehicleBrands,
    vehicleBrandModels,
    vehicleCapacityUnits,
    vehicleFacilities,
    vehicleInsurances,
    vehicleTypes,
  }
})
