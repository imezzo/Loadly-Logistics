<template>
  <div
    class="
      mb-6 grid grid-cols-1 gap-4
      md:grid-cols-3
      lg:grid-cols-4
    "
  >
    <ListTabs />

    <div class="flex flex-col gap-2">
      <label
        for="search"
        class="text-sm font-medium text-neutral-700"
      >
        {{ $t('form.search') }}
      </label>

      <BaseInputSearch
        id="search"
        :model-value="filters.search"
        wrapper-class="input-container flex h-11 shrink-0 items-center rounded-xl border border-neutral-500 px-3"
        :placeholder="$t('form.searchByTripId')"
        @update:model-value="updateFilters({ search: $event ?? '' })"
      />
    </div>

    <div
      v-if="isExternalTrip"
      class="flex flex-col gap-2"
    >
      <label
        for="leadIds"
        class="text-sm font-medium text-neutral-700"
      >
        {{ $t('content.leads') }}
      </label>

      <BaseMultiSelect
        id="leadIds"
        :model-value="filters.leadIds"
        :options="leadOptions"
        :placeholder="$t('content.leads')"
        searchable
        :on-scroll-to-end="hasMoreLeads ? loadMoreLeads : undefined"
        :on-search="(search) => getLeads(1, search)"
        @update:model-value="updateFilters({ leadIds: $event ?? [] })"
      />
    </div>

    <template v-else>
      <div class="flex flex-col gap-2">
        <label
          for="companyId"
          class="text-sm font-medium text-neutral-700"
        >
          {{ $t('content.company') }}
        </label>

        <BaseSelect
          id="companyId"
          :model-value="filters.companyId"
          :options="companyOptions"
          searchable
          :on-scroll-to-end="hasMoreCompanies ? loadMoreCompanies : undefined"
          @update:model-value="onCompanyChange"
        />
      </div>

      <div class="flex flex-col gap-2">
        <label
          for="contractId"
          class="text-sm font-medium text-neutral-700"
        >
          {{ $t('content.contractReferenceNumber') }}
        </label>

        <BaseSelect
          id="contractId"
          :model-value="filters.contractId"
          :options="contractOptions"
          :disabled="!filters.companyId"
          searchable
          :on-scroll-to-end="hasMoreContractReferenceNumbers ? loadMoreContractReferenceNumbers : undefined"
          @update:model-value="updateFilters({ contractId: String($event ?? '') })"
        />
      </div>
    </template>

    <div class="flex flex-col gap-2">
      <label
        for="driverId"
        class="text-sm font-medium text-neutral-700"
      >
        {{ $t('table.driverName') }}
      </label>

      <BaseSelect
        id="driverId"
        :model-value="filters.driverId"
        :options="driverOptions"
        searchable
        :on-scroll-to-end="hasMoreDrivers ? loadMoreDrivers : undefined"
        @update:model-value="updateFilters({ driverId: String($event ?? '') })"
      />
    </div>

    <div class="flex flex-col gap-2">
      <label
        for="vehicleTypeId"
        class="text-sm font-medium text-neutral-700"
      >
        {{ $t('table.vehicleType') }}
      </label>

      <BaseSelect
        id="vehicleTypeId"
        :model-value="filters.vehicleTypeId"
        :options="vehicleTypeOptions"
        searchable
        :on-scroll-to-end="hasMoreVehicleTypes ? loadMoreVehicleTypes : undefined"
        @update:model-value="updateFilters({ vehicleTypeId: String($event ?? '') })"
      />
    </div>

    <div class="flex flex-col gap-2">
      <label
        for="financialDateFrom"
        class="text-sm font-medium text-neutral-700"
      >
        {{ $t('form.financialStartDate') }}
      </label>

      <BaseInput
        id="financialDateFrom"
        :model-value="filters.financialDateFrom ?? ''"
        type="date"
        :placeholder="$t('form.startDate')"
        @update:model-value="updateFilters({ financialDateFrom: String($event ?? '') })"
      />
    </div>

    <div class="flex flex-col gap-2">
      <label
        for="financialDateTo"
        class="text-sm font-medium text-neutral-700"
      >
        {{ $t('form.financialEndDate') }}
      </label>

      <BaseInput
        id="financialDateTo"
        :model-value="filters.financialDateTo ?? ''"
        type="date"
        :min="filters.financialDateFrom || undefined"
        :placeholder="$t('form.endDate')"
        @update:model-value="updateFilters({ financialDateTo: String($event ?? '') })"
      />
    </div>

    <div
      v-if="isAdminDashboard"
      class="flex flex-col gap-2"
    >
      <label
        for="contractorIds"
        class="text-sm font-medium text-neutral-700"
      >
        {{ $t('form.contractors') }}
      </label>

      <BaseMultiSelect
        id="contractorIds"
        v-model="filters.contractorIds"
        :options="contractorOptions"
        :placeholder="$t('form.contractors')"
        searchable
        :on-scroll-to-end="hasMoreContractors ? loadMoreContractors : undefined"
        :on-search="searchContractors"
      />
    </div>

    <div class="flex flex-col gap-2">
      <label
        for="sectorIds"
        class="text-sm font-medium text-neutral-700"
      >
        {{ $t('form.sectors') }}
      </label>

      <BaseMultiSelect
        id="sectorIds"
        v-model="filters.sectorIds"
        :options="sectorOptions"
        :placeholder="$t('form.sectors')"
        searchable
        :on-scroll-to-end="hasMoreSectors ? loadMoreSectors : undefined"
        :on-search="(search) => getSectors(1, search)"
      />
    </div>

    <div class="flex flex-col gap-2">
      <label
        for="tripDocumentStatuses"
        class="text-sm font-medium text-neutral-700"
      >
        {{ $t('content.tripDocumentStatus') }}
      </label>

      <BaseMultiSelect
        id="tripDocumentStatuses"
        v-model="filters.tripDocumentStatuses"
        :options="tripDocumentStatusOptions"
        :placeholder="$t('content.tripDocumentStatus')"
      />
    </div>

    <div class="flex items-center gap-2">
      <button
        type="button"
        class="btn-primary w-full min-w-32"
        @click="applyFilters"
      >
        {{ $t('button.search') }}
      </button>

      <button
        v-if="hasActiveFilters"
        type="button"
        class="btn-link"
        @click="resetFilters"
      >
        {{ $t('button.clearFilters') }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
  import type { SelectOption } from '@/types/ui'
  import { useApp } from '@/composables/useApp'
  import { useEnum } from '@/composables/useEnum'
  import { useLookups } from '@/composables/useLookups'
  import { ContractorStatus } from '@/types/admin-dashboard/response/contractor/contractor.enums'
  import { useTripAuditingList } from '../composables/useTripAuditingList'
  import ListTabs from './ListTabs.vue'

  const { t } = useI18n()
  const { isAdminDashboard } = useApp()
  const {
    applyFilters,
    filters,
    hasActiveFilters,
    isExternalTrip,
    resetFilters,
    updateFilters,
  } = useTripAuditingList()
  const { enums } = useEnum()

  const {
    companies,
    contractReferenceNumbers,
    contractors,
    drivers,
    getCompanies,
    getContractors,
    getContractReferenceNumbers,
    getDrivers,
    getLeads,
    getVehicleTypes,
    hasMoreCompanies,
    hasMoreContractors,
    hasMoreContractReferenceNumbers,
    hasMoreDrivers,
    hasMoreLeads,
    hasMoreVehicleTypes,
    leads,
    loadMoreCompanies,
    loadMoreContractors,
    loadMoreContractReferenceNumbers,
    loadMoreDrivers,
    loadMoreLeads,
    loadMoreVehicleTypes,
    vehicleTypes,
    sectors,
    getSectors,
    hasMoreSectors,
    loadMoreSectors,
  } = useLookups()

  const companyOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('content.company') },
    ...(companies.value?.items ?? []),
  ])

  const contractOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('content.contractReferenceNumber') },
    ...(contractReferenceNumbers.value?.items ?? []),
  ])

  const contractorOptions = computed<SelectOption[]>(() => contractors.value?.items ?? [])

  const driverOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('table.driverName') },
    ...(drivers.value?.items ?? []),
  ])

  const sectorOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('title.sectors') },
    ...(sectors.value ?? []),
  ])

  const leadOptions = computed<SelectOption[]>(() => leads.value?.items ?? [])

  const tripDocumentStatusOptions = computed<SelectOption[]>(() =>
    enums.value?.TripDocumentStatus ?? [],
  )

  const vehicleTypeOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('table.vehicleType') },
    ...(vehicleTypes.value ?? []),
  ])

  function onCompanyChange(value: string | number): void {
    const companyId = String(value ?? '')
    updateFilters({ companyId, contractId: '' })
    if (companyId) {
      getContractReferenceNumbers(companyId, 1)
    }
  }

  function searchContractors(search: string): void {
    void getContractors(1, ContractorStatus.ACTIVE, search)
  }

  onMounted(async () => {
    getSectors()
    getDrivers({ page: 1 })
    getVehicleTypes()

    if (isAdminDashboard) {
      getContractors(1, ContractorStatus.ACTIVE)
    }

    if (!companies.value?.items?.length) {
      await getCompanies(1)
    }

    if (filters.value.companyId) {
      getContractReferenceNumbers(filters.value.companyId, 1)
    }

    if (isExternalTrip.value) {
      getLeads(1)
    }
  })

  watch(isExternalTrip, (external) => {
    if (external) getLeads(1)
  })
</script>

<style scoped>

</style>
