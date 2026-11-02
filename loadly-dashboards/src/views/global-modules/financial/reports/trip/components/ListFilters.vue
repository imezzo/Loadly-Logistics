<template>
  <div
    class="
      mb-6 grid grid-cols-1 gap-4
      md:grid-cols-3
      lg:grid-cols-5
    "
  >
    <div class="col-span-full flex items-center gap-2">
      <RouterLink
        :to="{ name: 'FinancialTripCompanyReports' }"
        class="tab-primary-outline shrink-0"
        :class="{ active: !isExternalTrip }"
      >
        {{ $t('content.company') }}
      </RouterLink>

      <RouterLink
        :to="{ name: 'FinancialTripExternalReports' }"
        class="tab-primary-outline shrink-0"
        :class="{ active: isExternalTrip }"
      >
        {{ $t('content.externalLead') }}
      </RouterLink>
    </div>

    <div class="flex flex-col gap-2">
      <label
        for="search"
        class="text-sm font-medium text-neutral-700"
      >
        {{ $t('form.searchByTripId') }}
      </label>
      <BaseInputSearch
        id="search"
        :model-value="filters.search"
        wrapper-class="input-container flex h-11 shrink-0 items-center rounded-xl border border-neutral-500 px-3"
        :placeholder="$t('form.searchByTripId')"
        @update:model-value="updateFilters({ search: $event ?? '' })"
      />
    </div>

    <dev
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
    </dev>

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
          {{ $t('table.contract') }}
        </label>
        <BaseSelect
          id="contractId"
          :model-value="filters.contractId"
          :options="contractOptions"
          :disabled="!filters.companyId"
          searchable
          :on-scroll-to-end="hasMoreContractReferenceNumbers ? loadMoreContractReferenceNumbers : undefined"
          @update:model-value="onContractChange"
        />
      </div>
    </template>

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
        :model-value="filters.financialDateTo ?? ''"
        type="date"
        :min="filters.financialDateFrom || undefined"
        :placeholder="$t('form.endDate')"
        @update:model-value="updateFilters({ financialDateTo: String($event ?? '') })"
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
        for="destinationIds"
        class="text-sm font-medium text-neutral-700"
      >
        {{ $t('table.destination') }}
      </label>
      <BaseMultiSelect
        v-model="filters.destinationIds"
        :options="destinationOptions"
        :placeholder="$t('table.destination')"
        searchable
        :disabled="!filters.contractId"
        :on-scroll-to-end="hasMoreContractDestinations ? loadMoreContractDestinations : undefined"
        :on-search="(search) => getContractDestinations(filters.contractId || '', 1, search)"
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
  import { useLookups } from '@/composables/useLookups'
  import { useTripReportsList } from '../composables/useTripReportsList'

  const { t } = useI18n()
  const {
    applyFilters,
    filters,
    hasActiveFilters,
    isExternalTrip,
    resetFilters,
    updateFilters,
  } = useTripReportsList()

  const {
    companies,
    contractDestinations,
    contractReferenceNumbers,
    getCompanies,
    getContractDestinations,
    getContractReferenceNumbers,
    getLeads,
    getVehicleTypes,
    hasMoreCompanies,
    hasMoreContractDestinations,
    hasMoreContractReferenceNumbers,
    hasMoreLeads,
    hasMoreVehicleTypes,
    leads,
    loadMoreCompanies,
    loadMoreContractDestinations,
    loadMoreContractReferenceNumbers,
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
    { value: '', label: t('table.contract') },
    ...(contractReferenceNumbers.value?.items ?? []),
  ])

  const sectorOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('title.sectors') },
    ...(sectors.value ?? []),
  ])

  const destinationOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('table.destination') },
    ...(contractDestinations.value?.items ?? []),
  ])

  const leadOptions = computed<SelectOption[]>(() => leads.value?.items ?? [])

  const vehicleTypeOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('table.vehicleType') },
    ...(vehicleTypes.value ?? []),
  ])

  function onCompanyChange(value: string | number): void {
    const companyId = String(value ?? '')
    updateFilters({ companyId, contractId: '', destinationIds: [] })
    getContractDestinations('', 1)
    if (companyId) {
      getContractReferenceNumbers(companyId, 1)
    }
  }

  function onContractChange(value: string | number): void {
    const contractId = String(value ?? '')
    updateFilters({ contractId, destinationIds: [] })
    getContractDestinations(contractId, 1)
  }

  onMounted(async () => {
    if (filters.value.contractId) {
      getContractDestinations(filters.value.contractId, 1)
    }

    getSectors()
    if (!companies.value?.items?.length) {
      await getCompanies(1)
    }
    getVehicleTypes()

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
