<template>
  <div
    class="
      mb-6 grid grid-cols-1 gap-4
      md:grid-cols-2
      lg:grid-cols-4
    "
  >
    <BaseInputSearch
      :model-value="filters.search"
      :placeholder="$t('form.searchByVehicleOrDriver')"
      wrapper-class="
        flex h-11 items-center rounded-lg border border-neutral-300 bg-white px-3
        md:flex-1 shrink-0
      "
      @update:model-value="filters.search = String($event ?? '')"
    />

    <BaseSelect
      v-model="filters.companyId"
      :options="companyOptions"
      searchable
      :on-scroll-to-end="hasMoreCompanies ? loadMoreCompanies : undefined"
      :on-search="(search) => getCompanies(1, search)"
      @update:model-value="onCompanyChange"
    />

    <BaseSelect
      v-model="filters.contractId"
      :options="contractOptions"
      :disabled="!filters.companyId"
      searchable
      :on-scroll-to-end="hasMoreContractReferenceNumbers ? loadMoreContractReferenceNumbers : undefined"
      :on-search="(search) => getContractReferenceNumbers(filters.companyId || undefined, 1, search)"
    />

    <BaseSelect
      v-model="filters.vehicleType"
      :options="vehicleTypeOptions"
      searchable
      :on-scroll-to-end="hasMoreVehicleTypes ? loadMoreVehicleTypes : undefined"
      :on-search="(search) => getVehicleTypes(1, search)"
    />

    <BaseSelect
      v-model="filters.sectorId"
      :options="sectorOptions"
      searchable
      :on-scroll-to-end="hasMoreSectors ? loadMoreSectors : undefined"
      :on-search="(search) => getSectors(1, search)"
    />

    <BaseSelect
      v-model="filters.facilityType"
      :options="facilityTypeOptions"
      @update:model-value="changeFacilityType"
    />

    <BaseSelect
      v-model="filters.facilityIds"
      :options="facilityOptions"
      searchable
      :disabled="!filters.facilityType"
      :on-scroll-to-end="hasMoreVehicleFacilities ? loadMoreVehicleFacilities : undefined"
      :on-search="(search) => getVehicleFacilities(1, search, +filters.facilityType)"
    />

    <div class="flex gap-2">
      <button
        type="button"
        class="btn-primary w-full"
        @click="$emit('search')"
      >
        {{ $t('button.search') }}
      </button>

      <button
        v-if="hasActiveFilters"
        type="button"
        class="btn-link shrink-0"
        @click="$emit('reset')"
      >
        {{ $t('button.clearFilters') }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
  import type { SelectOption } from '@/types/ui'
  import type { VehicleStatusFilters } from '../composables/useVehicleStatusList'
  import { useLookups } from '@/composables/useLookups'
  import { useEnum } from '@/composables/useEnum'

  const filters = defineModel<VehicleStatusFilters>('filters', { required: true })

  defineEmits<{
    search: []
    reset: []
  }>()

  const { t } = useI18n()
  const { enums } = useEnum()
  const {
    companies,
    contractReferenceNumbers,
    getCompanies,
    getContractReferenceNumbers,
    getSectors,
    getVehicleFacilities,
    getVehicleTypes,
    hasMoreCompanies,
    hasMoreContractReferenceNumbers,
    hasMoreSectors,
    hasMoreVehicleFacilities,
    hasMoreVehicleTypes,
    loadMoreCompanies,
    loadMoreContractReferenceNumbers,
    loadMoreSectors,
    loadMoreVehicleFacilities,
    loadMoreVehicleTypes,
    sectors,
    vehicleFacilities,
    vehicleTypes,
  } = useLookups()

  const companyOptions = computed<SelectOption[]>(() => [
    { label: t('content.company'), value: '' },
    ...(companies.value?.items ?? []),
  ])

  const contractOptions = computed<SelectOption[]>(() => [
    { label: t('form.contractRefNumber'), value: '' },
    ...(contractReferenceNumbers.value?.items ?? []),
  ])

  const vehicleTypeOptions = computed<SelectOption[]>(() => [
    { label: t('form.vehicleType'), value: '' },
    ...(vehicleTypes.value ?? []),
  ])

  const sectorOptions = computed<SelectOption[]>(() => [
    { label: t('form.sector'), value: '' },
    ...(sectors.value ?? []),
  ])

  const facilityTypeOptions = computed<SelectOption[]>(() => [
    { label: t('form.facilityType'), value: '' },
    ...(enums.value?.VehicleFacilityType ?? []),
  ])

  const facilityOptions = computed<SelectOption[]>(() => [
    { label: t('form.vehicleLocations'), value: '' },
    ...(vehicleFacilities.value ?? []),
  ])

  const hasActiveFilters = computed(() =>
    !!(
      filters.value.search
      || filters.value.companyId
      || filters.value.contractId
      || filters.value.vehicleType
      || filters.value.sectorId
      || filters.value.facilityType
      || filters.value.facilityIds
    ),
  )

  function onCompanyChange(value: string): void {
    filters.value.contractId = ''
    if (value) getContractReferenceNumbers(value, 1)
  }

  function changeFacilityType(value: string): void {
    filters.value.facilityIds = ''
    if (value) getVehicleFacilities(1, undefined, Number(value))
  }

  onMounted(() => {
    getVehicleTypes(1)
    getSectors(1)
    getCompanies(1)
  })
</script>

<style scoped>

</style>
