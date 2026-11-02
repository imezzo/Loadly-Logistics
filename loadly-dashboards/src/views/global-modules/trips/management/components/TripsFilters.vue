<template>
  <div class="mb-6">
    <h2 class="mb-4 text-lg font-bold text-neutral-800">
      {{ pageTitle }}
    </h2>

    <div
      class="
        grid grid-cols-1 gap-4
        md:grid-cols-5
      "
    >
      <div>
        <label for="search">{{ $t('form.search') }}</label>
        <BaseInputSearch
          :model-value="filters.search"
          :placeholder="$t('form.search')"
          wrapper-class="
          flex h-11 items-center rounded-lg border border-neutral-300 bg-white px-3
          md:flex-1
        "
          @update:model-value="filters.search = $event?.toString()"
        />
      </div>

      <template v-if="!isExternalTrip">
        <div v-if="isAdminDashboard">
          <label for="companyId">{{ $t('content.company') }}</label>
          <BaseSelect
            v-model="filters.companyId"
            :options="companyOptions"
            :on-scroll-to-end="hasMoreCompanies ? loadMoreCompanies : undefined"
            searchable
          />
        </div>

        <div>
          <label for="contractId">{{ $t('content.contractReferenceNumber') }}</label>
          <BaseSelect
            v-model="filters.contractId"
            :options="contractReferenceNumbers?.items ?? []"
            :on-scroll-to-end="fetchMoreContractReferenceNumbers"
            searchable
          />
        </div>
      </template>

      <div v-else>
        <label for="leadIds">{{ $t('content.leads') }}</label>
        <BaseMultiSelect
          v-model="filters.leadIds"
          :options="leads?.items ?? []"
          :placeholder="$t('content.leads')"
          searchable
          :on-search="(search) => getLeads(1, search)"
          :on-scroll-to-end="hasMoreLeads ? loadMoreLeads : undefined"
        />
      </div>

      <div>
        <label for="vehicleTypeId">{{ $t('form.vehicleTypes') }}</label>
        <BaseSelect
          v-model="filters.vehicleTypeId"
          :options="vehicleTypes"
          :on-scroll-to-end="fetchVehicleTypes"
          searchable
        />
      </div>

      <div>
        <label for="driverId">{{ $t('content.driverList') }}</label>
        <BaseSelect
          v-model="filters.driverId"
          :options="drivers?.items ?? []"
          :disabled="!filters.contractId && !isExternalTrip"
          :on-scroll-to-end="fetchMoreDrivers"
          searchable
        />
      </div>

      <div>
        <label for="status">{{ $t('form.allStatus') }}</label>
        <BaseSelect
          v-model="filters.status"
          :options="statusOptions"
          @update:model-value="filters.currentDriverStatus = ''"
        />
      </div>

      <div>
        <label for="currentDriverStatus">{{ $t('content.currentDriverStatus') }}</label>
        <BaseSelect
          v-model="filters.currentDriverStatus"
          :options="driverStatusOptions"
          :disabled="Number(filters.status) !== TripStatus.IN_PROGRESS"
        />
      </div>

      <div>
        <label for="financialDateFrom">{{ $t('form.financialStartDate') }}</label>
        <BaseInput
          :model-value="filters.financialDateFrom ?? ''"
          type="date"
          @update:model-value="tripStore.filters.financialDateFrom = $event?.toString()"
        />
      </div>

      <div>
        <label for="financialDateTo">{{ $t('form.financialEndDate') }}</label>
        <BaseInput
          :model-value="filters.financialDateTo ?? ''"
          type="date"
          :min="filters.financialDateFrom || undefined"
          @update:model-value="tripStore.filters.financialDateTo = $event?.toString()"
        />
      </div>

      <template v-if="isAdminDashboard">
        <div>
          <label for="dispatchingMode">{{ $t('form.carrier') }}</label>
          <BaseSelect
            id="dispatchingMode"
            v-model="filters.dispatchingMode"
            :options="dispatchingModeOptions"
          />
        </div>

        <div>
          <label for="contractorIds">{{ $t('form.contractors') }}</label>
          <BaseMultiSelect
            v-model="filters.contractorIds"
            :options="contractors?.items ?? []"
            :placeholder="$t('form.contractors')"
            searchable
            :on-scroll-to-end="hasMoreContractors ? loadMoreContractors : undefined"
            :on-search="searchContractors"
            :disabled="Number(filters.dispatchingMode) !== DispatchingMode.Contractors"
          />
        </div>
      </template>

      <div v-if="isAdminDashboard">
        <label for="sectorIds">{{ $t('form.sectors') }}</label>
        <BaseMultiSelect
          v-model="filters.sectorIds"
          :options="sectors ?? []"
          :placeholder="$t('form.sectors')"
          searchable
          :on-scroll-to-end="hasMoreSectors ? loadMoreSectors : undefined"
          :on-search="(search) => getSectors(1, search)"
        />
      </div>

      <div>
        <label for="contractorId">{{ $t('content.overNight') }}</label>
        <BaseSelect
          v-model="filters.overNightCostBearer"
          :options="overNightOptions"
          searchable
        />
      </div>

      <div v-if="isAdminDashboard">
        <label for="lifecycleActor">{{ $t('table.doneBy') }}</label>
        <BaseSelect
          v-model="filters.lifecycleActor"
          :options="lifecycleActorOptions"
        />
      </div>

      <div
        class="
          ms-auto mt-auto flex w-auto gap-2
          md:w-full
        "
      >
        <button
          type="button"
          class="
            btn-primary
            md:w-full
          "
          @click="$emit('applyFilters')"
        >
          {{ $t('button.search') }}
        </button>

        <button
          v-if="hasActiveFilters"
          type="button"
          class="btn-link me-auto shrink-0"
          @click="reset"
        >
          {{ $t('button.clearFilters') }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { useApp } from '@/composables/useApp'
  import { useEnum } from '@/composables/useEnum'
  import { useLookups } from '@/composables/useLookups'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { DispatchingMode, TripStatus } from '@/types/admin-dashboard/response/trip/trip'
  import type { SelectOption } from '@/types/ui'
  import { ContractorStatus } from '@/types/admin-dashboard/response/contractor/contractor.enums'

  const emit = defineEmits<{
    applyFilters: []
  }>()

  const route = useRoute()
  const { t } = useI18n()
  const { enums } = useEnum()
  const { isAdminDashboard, isCompanyDashboard } = useApp()
  const { getVehicleTypes, loadMoreContractVehicleTypes, hasMoreContractVehicleTypes, vehicleTypes } = useLookups()

  const tripStore = useTripStore()
  const { filters, tripsList } = storeToRefs(tripStore)

  const {
    contractReferenceNumbers,
    contractors,
    drivers,
    getContractReferenceNumbers,
    getContractors,
    getDrivers,
    hasMoreContractReferenceNumbers,
    hasMoreContractors,
    hasMoreDrivers,
    loadMoreContractReferenceNumbers,
    loadMoreContractors,
    loadMoreDrivers,
    getCompanies,
    hasMoreCompanies,
    loadMoreCompanies,
    companies,
    leads,
    getLeads,
    hasMoreLeads,
    loadMoreLeads,
    sectors,
    getSectors,
    hasMoreSectors,
    loadMoreSectors,
  } = useLookups()

  const isExternalTrip = computed(() => route.path.includes('external'))
  const isTripsOnMap = computed(() => route.path.includes('map'))
  const pageTitle = computed(() => {
    if (isTripsOnMap.value) return t('content.liveMap')
    if (isExternalTrip.value) return t('sidebar.externalTrips')

    return t('sidebar.tripManagement')
  })

  const companyOptions = computed<SelectOption[]>(() => [
    ...(companies.value?.items ?? []),
  ])

  const statusOptions = computed<SelectOption[]>(() => [
    ...(isAdminDashboard
      ? enums.value?.TripStatus?.filter(status => Number(status.value) !== TripStatus.DRAFT) ?? []
      : enums.value?.TripStatus?.filter(status => Number(status.value) !== TripStatus.PENDING) ?? []
    ),
  ])

  const overNightOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('form.allOverNightCostBearers') },
    ...(enums.value?.OverNightCostBearer ?? []),
  ])

  const driverStatusOptions = computed<SelectOption[]>(() => [
    ...(enums.value?.OperationalDriverStatus ?? []),
  ])

  const lifecycleActorOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('form.select') },
    ...(enums.value?.TripLifecycleActor ?? []),
  ])

  const dispatchingModeOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('form.select') },
    ...(enums.value?.TripDispatchingMode ?? []),
  ])

  const hasActiveFilters = computed(() => {
    return Object.entries(filters.value).some(([, value]) => {
      if (Array.isArray(value)) return value.length > 0

      return value !== ''
    })
  })

  function reset() {
    tripStore.filters = {
      companyId: '',
      contractId: '',
      contractorIds: [],
      currentDriverStatus: '',
      dispatchingMode: '',
      driverId: '',
      leadIds: [],
      lifecycleActor: '',
      search: '',
      sectorIds: [],
      financialDateFrom: '',
      financialDateTo: '',
      status: '',
      tripDocumentStatuses: [],
      vehicleTypeId: '',
    }
    emit('applyFilters')
  }

  watch(
    () => filters.value.companyId,
    (companyId) => {
      tripStore.filters = { ...tripStore.filters, contractId: '', driverId: '' }
      if (companyId) {
        getContractReferenceNumbers(companyId)
      }
    }
  )

  watch(
    () => filters.value.contractId,
    (contractId) => {
      tripStore.filters = { ...tripStore.filters, driverId: '' }
      void getDrivers({ page: 1, contractId: contractId ?? '' })
    }
  )

  async function fetchMoreDrivers(): Promise<void> {
    if (hasMoreDrivers.value) {
      loadMoreDrivers()
    }
  }

  async function fetchMoreContractReferenceNumbers(): Promise<void> {
    if (hasMoreContractReferenceNumbers.value && filters.value.companyId) {
      loadMoreContractReferenceNumbers()
    }
  }

  async function fetchVehicleTypes(): Promise<void> {
    if (hasMoreContractVehicleTypes.value) {
      loadMoreContractVehicleTypes()
    }
  }

  watch(
    () => tripsList.value?.data,
    async (data) => {
      if (isCompanyDashboard
      && data && data.length
      && !filters.value.companyId) {
        filters.value.companyId = data[0]?.companyId
      }
    }
  )

  watch(isExternalTrip, (newValue) => {
    if (newValue) {
      getLeads()
    }
  })

  function searchContractors(search: string): void {
    getContractors(1, ContractorStatus.ACTIVE, search)
  }

  onMounted(async () => {
    if (isAdminDashboard) {
      getCompanies()
      getContractors(1)
      getSectors(1)
    }
    if (isCompanyDashboard) getContractReferenceNumbers()
    getVehicleTypes()
    if (isExternalTrip.value) {
      getDrivers()
      getLeads()
    }
  })
</script>

<style scoped>

</style>
