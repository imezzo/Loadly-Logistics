<template>
  <div
    class="
      mb-6 grid grid-cols-1 gap-4
      md:grid-cols-2
      lg:grid-cols-4
    "
  >
    <div class="flex flex-col gap-2">
      <label
        for="search"
        class="text-sm font-medium text-neutral-700"
      >
        {{ $t('form.search') }}
      </label>

      <BaseInputSearch
        id="search"
        v-model="filters.search"
        :placeholder="$t('form.searchByVehicleOrDriver')"
        wrapper-class="
          flex h-11 items-center rounded-lg border border-neutral-300 bg-white px-3
          md:flex-1 shrink-0
        "
      />
    </div>

    <div class="flex flex-col gap-2">
      <label
        for="vehicleUnitType"
        class="text-sm font-medium text-neutral-700"
      >
        {{ $t('content.unitType') }}
      </label>

      <BaseSelect
        id="vehicleUnitType"
        v-model="filters.vehicleUnitType"
        :options="unitTypeOptions"
      />
    </div>

    <div class="flex flex-col gap-2">
      <label
        for="vehicleCategory"
        class="text-sm font-medium text-neutral-700"
      >
        {{ $t('content.vehicleCategory') }}
      </label>

      <BaseSelect
        id="vehicleCategory"
        :model-value="filters.vehicleCategory"
        :options="vehicleCategoryOptions"
        @update:model-value="changeVehicleCategory"
      />
    </div>

    <div class="flex flex-col gap-2">
      <label
        for="vehicleType"
        class="text-sm font-medium text-neutral-700"
      >
        {{ $t('form.vehicleTypes') }}
      </label>

      <BaseSelect
        id="vehicleType"
        v-model="filters.vehicleType"
        :options="vehicleTypeOptions"
        :disabled="!filters.vehicleCategory"
        searchable
        :on-scroll-to-end="hasMoreVehicleTypes ? loadMoreVehicleTypes : undefined"
        :on-search="onVehicleTypeSearch"
      />
    </div>

    <div class="flex flex-col gap-2">
      <label
        for="status"
        class="text-sm font-medium text-neutral-700"
      >
        {{ $t('table.status') }}
      </label>

      <BaseSelect
        id="status"
        v-model="filters.status"
        :options="statusOptions"
      />
    </div>

    <div
      v-if="isAdminDashboard"
      class="flex flex-col gap-2"
    >
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
        :on-search="(search) => getCompanies(1, search)"
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
        :on-search="(search) => filters.companyId && getContractReferenceNumbers(filters.companyId, 1, search)"
        @update:model-value="filters.contractId = String($event ?? '')"
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
        :options="contractors?.items ?? []"
        :placeholder="$t('form.contractors')"
        searchable
        :on-scroll-to-end="hasMoreContractors ? loadMoreContractors : undefined"
        :on-search="(search) => getContractors(1, ContractorStatus.ACTIVE, search)"
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
        :options="sectors ?? []"
        :placeholder="$t('form.sectors')"
        searchable
        :on-scroll-to-end="hasMoreSectors ? loadMoreSectors : undefined"
        :on-search="(search) => getSectors(1, search)"
      />
    </div>

    <div class="flex items-end gap-2">
      <button
        type="button"
        class="btn-primary w-full"
        @click="applyFilters"
      >
        {{ $t('button.search') }}
      </button>

      <button
        v-if="hasActiveFilters"
        type="button"
        class="btn-link shrink-0"
        @click="resetFilters"
      >
        {{ $t('button.clearFilters') }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
  import type { SelectOption } from '@/types/ui'
  import { useList } from '../composables/useList'
  import { useEnum } from '@/composables/useEnum'
  import { useLookups } from '@/composables/useLookups'
  import { useApp } from '@/composables/useApp'
  import { ContractorStatus } from '@/types/admin-dashboard/response/contractor/contractor.enums'

  const { t } = useI18n()
  const { isAdminDashboard } = useApp()
  const {
    filters,
    updateFilters,
    resetFilters,
  } = useList()
  const { enums } = useEnum()
  const {
    companies,
    contractReferenceNumbers,
    contractors,
    getCompanies,
    getContractReferenceNumbers,
    getContractors,
    getSectors,
    getVehicleTypes,
    hasMoreCompanies,
    hasMoreContractors,
    hasMoreContractReferenceNumbers,
    hasMoreSectors,
    hasMoreVehicleTypes,
    loadMoreCompanies,
    loadMoreContractors,
    loadMoreContractReferenceNumbers,
    loadMoreSectors,
    loadMoreVehicleTypes,
    sectors,
    vehicleTypes,
  } = useLookups()

  onMounted(() => {
    if (isAdminDashboard) {
      getCompanies(1)
      getContractors(1)
    }
    getSectors(1)

    if (filters.value.vehicleCategory) {
      getVehicleTypes(1, undefined, Number(filters.value.vehicleCategory))
    }
  })

  const companyOptions = computed<SelectOption[]>(() => [
    { label: t('content.company'), value: '' },
    ...(companies.value?.items ?? []),
  ])

  const contractOptions = computed<SelectOption[]>(() => [
    { label: t('form.contractRefNumber'), value: '' },
    ...(contractReferenceNumbers.value?.items ?? []),
  ])

  const unitTypeOptions = computed<SelectOption[]>(() => [
    { label: t('form.allVehicleUnitTypes'), value: '' },
    ...(enums.value?.VehicleUnitType ?? []),
  ])

  const vehicleCategoryOptions = computed<SelectOption[]>(() => [
    { label: t('form.allVehicleCategories'), value: '' },
    ...(enums.value?.VehicleTypeCategory ?? []),
  ])

  const vehicleTypeOptions = computed<SelectOption[]>(() => [
    { label: t('form.allVehicleTypes'), value: '' },
    ...(vehicleTypes.value ?? []),
  ])

  const statusOptions = computed<SelectOption[]>(() => [
    { label: t('form.allStatus'), value: '' },
    ...(enums.value?.VehicleStatus ?? []),
  ])

  const hasActiveFilters = computed(() => {
    return !!(
      filters.value.search ||
      filters.value.companyId ||
      filters.value.contractId ||
      filters.value.vehicleUnitType ||
      filters.value.vehicleCategory ||
      filters.value.vehicleType ||
      filters.value.status ||
      filters.value.contractorIds?.length ||
      filters.value.sectorIds?.length
    )
  })

  function onCompanyChange(value: string): void {
    filters.value.companyId = value
    filters.value.contractId = ''

    if (value) {
      getContractReferenceNumbers(value, 1)
    }
  }

  function changeVehicleCategory(value: string): void {
    filters.value.vehicleCategory = value
    filters.value.vehicleType = ''

    if (value) {
      getVehicleTypes(1, undefined, Number(value))
    }
  }

  function onVehicleTypeSearch(search: string): void {
    if (!filters.value.vehicleCategory) return

    getVehicleTypes(1, search, Number(filters.value.vehicleCategory))
  }

  function applyFilters() {
    updateFilters({
      companyId: filters.value.companyId,
      contractId: filters.value.contractId,
      contractorIds: filters.value.contractorIds,
      sectorIds: filters.value.sectorIds,
      search: filters.value.search,
      vehicleUnitType: filters.value.vehicleUnitType,
      vehicleCategory: filters.value.vehicleCategory,
      vehicleType: filters.value.vehicleType,
      status: filters.value.status,
    })
  }
</script>

<style scoped>

</style>
