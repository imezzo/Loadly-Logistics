import type { LookupItem } from '@/types/response/lookup'
import type { LookupRequest } from '@/types/request/lookup'
import { useLookupStore } from '@/stores/lookup'
import { useLoadMore } from '@/composables/useLoadMore'
import { ContractorStatus } from '@/types/admin-dashboard/response/contractor/contractor.enums'
import { MonthlyContractStatus } from '@/types/admin-dashboard/response/contract/monthly-contract/monthly-contract.enums'

export function useLookups() {
  const lookupStore = useLookupStore()
  const {
    admins,
    cities,
    companies,
    communicationMethods,
    contractBillingSnapshots,
    contractDestinations,
    contractReferenceNumbers,
    contractors,
    contractVehicleTypes,
    drivers,
    extraFeeTypes,
    fuelTypes,
    governments,
    industryTypes,
    leads,
    operatingHours,
    roles,
    sectors,
    serviceRequirements,
    tags,
    vehicleBrandModels,
    vehicleBrands,
    vehicleCapacityUnits,
    vehicleFacilities,
    vehicleInsurances,
    vehicleTypes,
  } = storeToRefs(lookupStore)

  const currentGovernorateId = ref<number>()
  const currentCompanyId = ref<number | string>()
  const currentBillingSnapshotContractId = ref<number | string>()
  const currentContractId = ref<number | string>()
  const currentContractDestinationsContractId = ref<number | string>()
  const currentDriverSearch = ref<string>()
  const currentDriverStatus = ref<number>()
  const selectedBrandId = ref<number | string | undefined>()
  const selectedVehicleTypeCategory = ref<number | undefined>()
  const selectedFacilityType = ref<number | undefined>()
  const selectedFacilityCompanyIds = ref<number[] | undefined>()


  const { hasMore: hasMoreAdmins, loadMore: loadMoreAdmins } = useLoadMore<LookupItem, 'items'>({
    listRef: admins,
    itemsKey: 'items',
    fetchPage: (page) => lookupStore.getAdmins({ page }),
  })

  const { hasMore: hasMoreGovernments, loadMore: loadMoreGovernments } = useLoadMore<
    LookupItem,
    'items'
  >({
    listRef: governments,
    itemsKey: 'items',
    fetchPage: (page) => lookupStore.getGovernments({ page }),
  })

  const { hasMore: hasMoreCities, loadMore: loadMoreCities } = useLoadMore<LookupItem, 'items'>({
    listRef: cities,
    itemsKey: 'items',
    fetchPage: (page) => lookupStore.getCities({ page, id: currentGovernorateId.value }),
  })

  const { hasMore: hasMoreCompanies, loadMore: loadMoreCompanies } = useLoadMore<
    LookupItem,
    'items'
  >({
    listRef: companies,
    itemsKey: 'items',
    fetchPage: (page) => lookupStore.getCompanies({ page }),
  })

  const { hasMore: hasMoreContractors, loadMore: loadMoreContractors } = useLoadMore<
    LookupItem,
    'items'
  >({
    listRef: contractors,
    itemsKey: 'items',
    fetchPage: (page) => lookupStore.getContractors({ page, status: ContractorStatus.ACTIVE }),
  })

  const { hasMore: hasMoreLeads, loadMore: loadMoreLeads } = useLoadMore<LookupItem, 'items'>({
    listRef: leads,
    itemsKey: 'items',
    fetchPage: (page) => lookupStore.getLeads({ page }),
  })

  const { hasMore: hasMoreContractReferenceNumbers, loadMore: loadMoreContractReferenceNumbers } = useLoadMore<
    LookupItem,
    'items'
  >({
    listRef: contractReferenceNumbers,
    itemsKey: 'items',
    fetchPage: (page) => lookupStore.getContractReferenceNumbers({ companyId: currentCompanyId.value ?? '', page }),
  })

  const { hasMore: hasMoreContractVehicleTypes, loadMore: loadMoreContractVehicleTypes } = useLoadMore<
    LookupItem,
    'items'
  >({
    listRef: contractVehicleTypes,
    itemsKey: 'items',
    fetchPage: (page) => lookupStore.getVehicleTypes({ contractId: currentContractId.value ?? '', page }),
  })

  const { hasMore: hasMoreContractBillingSnapshots, loadMore: loadMoreContractBillingSnapshots } = useLoadMore<
    LookupItem,
    'items'
  >({
    listRef: contractBillingSnapshots,
    itemsKey: 'items',
    fetchPage: (page) => lookupStore.getContractBillingSnapshots({
      contractId: currentBillingSnapshotContractId.value ?? '',
      page,
      status: MonthlyContractStatus.Draft,
    }),
  })

  const { hasMore: hasMoreContractDestinations, loadMore: loadMoreContractDestinations } = useLoadMore<
    LookupItem,
    'items'
  >({
    listRef: contractDestinations,
    itemsKey: 'items',
    fetchPage: async (page) => {
      const result = await lookupStore.getContractDestinations({
        contractId: currentContractDestinationsContractId.value ?? '',
        page,
      })

      return { success: Boolean(result.success), data: result.data ?? null }
    },
  })

  const { hasMore: hasMoreDrivers, loadMore: loadMoreDrivers } = useLoadMore<
    LookupItem,
    'items'
  >({
    listRef: drivers,
    itemsKey: 'items',
    fetchPage: (page) => lookupStore.getDrivers({
      page,
      contractId: currentContractId.value,
      search: currentDriverSearch.value,
      status: currentDriverStatus.value,
    }),
  })

  const { hasMore: hasMoreFuelTypes, loadMore: loadMoreFuelTypes } = useLoadMore<
    LookupItem,
    'items'
  >({
    listRef: fuelTypes,
    itemsKey: 'items',
    fetchPage: (page) => lookupStore.getFuelTypes({ page }),
  })

  const { hasMore: hasMoreRoles, loadMore: loadMoreRoles } = useLoadMore<LookupItem, 'items'>({
    listRef: roles,
    itemsKey: 'items',
    fetchPage: (page) => lookupStore.getRoles({ page, guardName: 'admin' }),
  })

  const { hasMore: hasMoreSectors, loadMore: loadMoreSectors } = useLoadMore<
    LookupItem,
    'items'
  >({
    listRef: sectors,
    itemsKey: 'items',
    fetchPage: (page) => lookupStore.getSectors({ page }),
  })

  const { hasMore: hasMoreVehicleBrands, loadMore: loadMoreVehicleBrands } = useLoadMore<
    LookupItem,
    'items'
  >({
    listRef: vehicleBrands,
    itemsKey: 'items',
    fetchPage: (page) => lookupStore.getVehicleBrands({ page }),
  })

  const { hasMore: hasMoreVehicleBrandModels, loadMore: loadMoreVehicleBrandModels } = useLoadMore<
    LookupItem,
    'items'
  >({
    listRef: vehicleBrandModels,
    itemsKey: 'items',
    fetchPage: (page) => lookupStore.getVehicleBrandModels({ page, brandId: selectedBrandId.value }),
  })

  const { hasMore: hasMoreVehicleCapacityUnits, loadMore: loadMoreVehicleCapacityUnits } = useLoadMore<
    LookupItem,
    'items'
  >({
    listRef: vehicleCapacityUnits,
    itemsKey: 'items',
    fetchPage: (page) => lookupStore.getVehicleCapacityUnits({ page }),
  })

  const { hasMore: hasMoreVehicleFacilities, loadMore: loadMoreVehicleFacilities } = useLoadMore<
    LookupItem,
    'items'
  >({
    listRef: vehicleFacilities,
    itemsKey: 'items',
    fetchPage: (page) => lookupStore.getVehicleFacilities({
      page,
      facilityType: selectedFacilityType.value,
      companyIds: selectedFacilityCompanyIds.value,
    }),
  })

  const { hasMore: hasMoreVehicleInsurances, loadMore: loadMoreVehicleInsurances } = useLoadMore<
    LookupItem,
    'items'
  >({
    listRef: vehicleInsurances,
    itemsKey: 'items',
    fetchPage: (page) => lookupStore.getVehicleInsurances({ page }),
  })

  const { hasMore: hasMoreVehicleTypes, loadMore: loadMoreVehicleTypes } = useLoadMore<
    LookupItem,
    'items'
  >({
    listRef: vehicleTypes,
    itemsKey: 'items',
    fetchPage: (page) => lookupStore.getVehicleTypes({
      page,
      category: selectedVehicleTypeCategory.value,
    }),
  })

  const citiesItems = computed(() => cities.value?.items ?? [])
  const communicationMethodsItems = computed(() => communicationMethods.value?.items ?? [])
  const fuelTypesItems = computed(() => fuelTypes.value?.items ?? [])
  const governmentsItems = computed(() => governments.value?.items ?? [])
  const industryTypesItems = computed(() => industryTypes.value?.items ?? [])
  const operatingHoursItems = computed(() => operatingHours.value?.items ?? [])
  const rolesItems = computed(() => roles.value?.items ?? [])
  const sectorsItems = computed(() => sectors.value?.items ?? [])
  const serviceRequirementsItems = computed(() => serviceRequirements.value?.items ?? [])
  const vehicleBrandsItems = computed(() => vehicleBrands.value?.items ?? [])
  const vehicleBrandModelsItems = computed(() => vehicleBrandModels.value?.items ?? [])
  const vehicleCapacityUnitsItems = computed(() => vehicleCapacityUnits.value?.items ?? [])
  const vehicleFacilitiesItems = computed(() => vehicleFacilities.value?.items ?? [])
  const vehicleInsurancesItems = computed(() => vehicleInsurances.value?.items ?? [])
  const vehicleTypesItems = computed(() => vehicleTypes.value?.items ?? [])

  async function getGovernments(page?: number): Promise<void> {
    const { success, data } = await lookupStore.getGovernments({ page })

    if (!success || !data) return

    governments.value = { items: data.items, pagination: data.pagination }
  }

  async function getCities(id?: number, page?: number): Promise<void> {
    const { success, data } = await lookupStore.getCities({ page, id })

    if (!success || !data) return

    if (id !== undefined) {
      currentGovernorateId.value = id
    }

    cities.value = { items: data.items, pagination: data.pagination }
  }

  async function getIndustryTypes(page?: number): Promise<void> {
    const { data } = await lookupStore.getIndustryTypes({ page })
    industryTypes.value = data
  }

  async function getCommunicationMethods(page?: number): Promise<void> {
    const { data } = await lookupStore.getCommunicationMethods({ page })
    communicationMethods.value = data
  }

  async function getServiceRequirements(page?: number): Promise<void> {
    const { data } = await lookupStore.getServiceRequirements({ page })
    serviceRequirements.value = data
  }

  async function getOperatingHours(page?: number): Promise<void> {
    const { data } = await lookupStore.getOperatingHours({ page })
    operatingHours.value = data
  }

  async function getRoles(page = 1, search?: string, guardName?: string): Promise<void> {
    const { success, data } = await lookupStore.getRoles({ page, search, guardName })

    if (!success || !data) return

    roles.value = data
  }

  async function getSectors(page = 1, search?: string): Promise<void> {
    const { success, data } = await lookupStore.getSectors({ page, search })

    if (!success || !data) return

    sectors.value = data
  }

  async function getCompanies(page?: number, search?: string): Promise<void> {
    const { success, data } = await lookupStore.getCompanies({ page, search })

    if (!success || !data) return

    companies.value = { items: data.items, pagination: data.pagination }
  }

  async function getContractors(page = 1, status?: number, search?: string): Promise<void> {
    const { success, data } = await lookupStore.getContractors({ page, status, search })

    if (!success || !data) return

    contractors.value = data
  }

  async function getLeads(page = 1, search?: string): Promise<void> {
    const { success, data } = await lookupStore.getLeads({ page, search })

    if (!success || !data) return

    leads.value = { items: data.items, pagination: data.pagination }
  }

  async function getExtraFeeTypes(): Promise<void> {
    const { data } = await lookupStore.getExtraFeeTypes()
    extraFeeTypes.value = data
  }

  async function getFuelTypes(page = 1, search?: string): Promise<void> {
    const { data } = await lookupStore.getFuelTypes({ page, search })

    fuelTypes.value = data
  }

  async function getContractReferenceNumbers(companyId?: string | number, page = 1, search?: string): Promise<void> {
    const { data } = await lookupStore.getContractReferenceNumbers({ companyId, page, search })
    currentCompanyId.value = companyId
    contractReferenceNumbers.value = data
  }

  async function getContractVehicleTypes(contractId: string | number, page = 1, search?: string): Promise<void> {
    currentContractId.value = contractId
    const { data } = await lookupStore.getVehicleTypes({ contractId, page, search })
    contractVehicleTypes.value = data
  }

  async function getContractBillingSnapshots(contractId: string | number, page = 1): Promise<void> {
    currentBillingSnapshotContractId.value = contractId
    const { success, data } = await lookupStore.getContractBillingSnapshots({
      contractId,
      page,
      status: MonthlyContractStatus.Draft,
    })

    if (!success || !data) return

    contractBillingSnapshots.value = data
  }

  async function getContractDestinations(contractId: string | number, page = 1, search?: string): Promise<void> {
    currentContractDestinationsContractId.value = contractId
    const { data } = await lookupStore.getContractDestinations({
      contractId,
      page,
      search,
    })
    contractDestinations.value = data
  }

  async function getDrivers(params: LookupRequest = {}): Promise<void> {
    currentDriverSearch.value = params.search
    currentDriverStatus.value = params.status
    const { data } = await lookupStore.getDrivers({
      ...params,
      page: params.page ?? 1,
    })
    drivers.value = data
  }

  async function getVehicleBrands(page = 1, search?: string): Promise<void> {
    const { success, data } = await lookupStore.getVehicleBrands({ page, search })

    if (!success || !data) return

    vehicleBrands.value = data
  }

  async function getVehicleBrandModels(page = 1, brandId?: number | string, search?: string): Promise<void> {
    selectedBrandId.value = brandId
    const { success, data } = await lookupStore.getVehicleBrandModels({ page, brandId, search })

    if (!success || !data) return

    vehicleBrandModels.value = data
  }

  async function getVehicleCapacityUnits(page = 1, search?: string): Promise<void> {
    const { success, data } = await lookupStore.getVehicleCapacityUnits({ page, search })

    if (!success || !data) return

    vehicleCapacityUnits.value = data
  }

  async function getVehicleFacilities(
    page = 1,
    search?: string,
    facilityType?: number,
    companyIds?: number[],
  ): Promise<void> {
    selectedFacilityType.value = facilityType
    selectedFacilityCompanyIds.value = companyIds

    const { success, data } = await lookupStore.getVehicleFacilities({
      page,
      search,
      facilityType,
      companyIds,
    })

    if (!success || !data) return

    vehicleFacilities.value = data
  }

  async function getVehicleInsurances(page = 1, search?: string): Promise<void> {
    const { success, data } = await lookupStore.getVehicleInsurances({ page, search })

    if (!success || !data) return

    vehicleInsurances.value = data
  }

  async function getVehicleTypes(page = 1, search?: string, category?: number): Promise<void> {
    if (category !== undefined) {
      selectedVehicleTypeCategory.value = category
    }

    const { data } = await lookupStore.getVehicleTypes({
      page,
      search,
      category: selectedVehicleTypeCategory.value,
    })
    vehicleTypes.value = data
  }

  async function getTags(page = 1, isPricing?: boolean): Promise<void> {
    const { success, data } = await lookupStore.getTags({ page, isPricing })

    if (!success || !data) return

    tags.value = data
  }

  async function getAdmins(page?: number, search?: string, adminIds?: number[]): Promise<void> {
    const { data } = await lookupStore.getAdmins({ page, search, adminIds })
    admins.value = data
  }

  return {
    admins,
    cities: citiesItems,
    companies,
    communicationMethods: communicationMethodsItems,
    contractors,
    contractBillingSnapshots,
    contractDestinations,
    contractReferenceNumbers,
    contractVehicleTypes,
    currentCompanyId,
    drivers,
    extraFeeTypes,
    fuelTypes: fuelTypesItems,
    getAdmins,
    getCities,
    getCompanies,
    getCommunicationMethods,
    getContractBillingSnapshots,
    getContractDestinations,
    getContractReferenceNumbers,
    getContractVehicleTypes,
    getContractors,
    getDrivers,
    getExtraFeeTypes,
    getFuelTypes,
    getGovernments,
    getIndustryTypes,
    getLeads,
    getOperatingHours,
    getRoles,
    getSectors,
    getServiceRequirements,
    getTags,
    getVehicleBrands,
    getVehicleBrandModels,
    getVehicleCapacityUnits,
    getVehicleFacilities,
    getVehicleInsurances,
    getVehicleTypes,
    governments: governmentsItems,
    hasMoreAdmins,
    hasMoreCities,
    hasMoreCompanies,
    hasMoreContractBillingSnapshots,
    hasMoreContractDestinations,
    hasMoreContractReferenceNumbers,
    hasMoreContractVehicleTypes,
    hasMoreContractors,
    hasMoreDrivers,
    hasMoreFuelTypes,
    hasMoreGovernments,
    hasMoreLeads,
    hasMoreRoles,
    hasMoreSectors,
    hasMoreVehicleBrandModels,
    hasMoreVehicleBrands,
    hasMoreVehicleCapacityUnits,
    hasMoreVehicleFacilities,
    hasMoreVehicleInsurances,
    hasMoreVehicleTypes,
    industryTypes: industryTypesItems,
    leads,
    loadMoreAdmins,
    loadMoreCities,
    loadMoreCompanies,
    loadMoreContractBillingSnapshots,
    loadMoreContractDestinations,
    loadMoreContractReferenceNumbers,
    loadMoreContractVehicleTypes,
    loadMoreContractors,
    loadMoreDrivers,
    loadMoreFuelTypes,
    loadMoreGovernments,
    loadMoreLeads,
    loadMoreRoles,
    loadMoreSectors,
    loadMoreVehicleBrandModels,
    loadMoreVehicleBrands,
    loadMoreVehicleCapacityUnits,
    loadMoreVehicleFacilities,
    loadMoreVehicleInsurances,
    loadMoreVehicleTypes,
    operatingHours: operatingHoursItems,
    roles: rolesItems,
    sectors: sectorsItems,
    serviceRequirements: serviceRequirementsItems,
    tags,
    vehicleBrands: vehicleBrandsItems,
    vehicleBrandModels: vehicleBrandModelsItems,
    vehicleCapacityUnits: vehicleCapacityUnitsItems,
    vehicleFacilities: vehicleFacilitiesItems,
    vehicleInsurances: vehicleInsurancesItems,
    vehicleTypes: vehicleTypesItems,
  }
}
