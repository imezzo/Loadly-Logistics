<template>
  <div
    class="
      mb-6 grid grid-cols-1 items-end gap-4
      md:grid-cols-5
    "
  >
    <div>
      <label for="search">{{ $t('form.search') }}</label>
      <BaseInputSearch
        :model-value="filters.search"
        :placeholder="$t('form.searchByTripId')"
        wrapper-class="
          flex h-11 items-center rounded-lg border border-neutral-300 bg-white px-3
        "
        @update:model-value="filters.search = $event?.toString()"
      />
    </div>

    <div>
      <label for="vehicleTypeId">{{ $t('form.vehicleType') }}</label>
      <BaseSelect
        v-model="filters.vehicleTypeId"
        :options="vehicleTypes"
        :on-scroll-to-end="hasMoreVehicleTypes ? loadMoreVehicleTypes : undefined"
        searchable
      />
    </div>

    <div>
      <label for="companyId">{{ $t('form.company') }}</label>
      <BaseSelect
        v-model="filters.companyId"
        :options="companyOptions"
        :on-scroll-to-end="hasMoreCompanies ? loadMoreCompanies : undefined"
        searchable
      />
    </div>

    <div>
      <label for="contractorIds">{{ $t('form.contractorName') }}</label>
      <BaseMultiSelect
        v-model="filters.contractorIds"
        :options="contractors?.items ?? []"
        :placeholder="$t('form.contractorName')"
        searchable
        :on-search="(search) => getContractors(1, ContractorStatus.ACTIVE, search)"
        :on-scroll-to-end="hasMoreContractors ? loadMoreContractors : undefined"
      />
    </div>

    <div class="flex shrink-0 items-center gap-2">
      <button
        type="button"
        class="btn-primary flex-1"
        @click="$emit('search')"
      >
        {{ $t('button.search') }}
      </button>

      <button
        v-if="hasActiveFilters"
        type="button"
        class="btn-link shrink-0"
        @click="reset"
      >
        {{ $t('button.clearFilters') }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { useLookups } from '@/composables/useLookups'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { ContractorStatus } from '@/types/admin-dashboard/response/contractor/contractor.enums'
  import type { SelectOption } from '@/types/ui'

  const emit = defineEmits<{
    search: []
  }>()

  const {
    companies,
    contractors,
    getCompanies,
    getContractors,
    getVehicleTypes,
    hasMoreCompanies,
    hasMoreContractors,
    hasMoreVehicleTypes,
    loadMoreCompanies,
    loadMoreContractors,
    loadMoreVehicleTypes,
    vehicleTypes,
  } = useLookups()

  const tripStore = useTripStore()
  const { contractorsRequestsFilters: filters } = storeToRefs(tripStore)

  const companyOptions = computed<SelectOption[]>(() => companies.value?.items ?? [])

  const hasActiveFilters = computed(() => {
    return Boolean(filters.value.search)
      || Boolean(filters.value.vehicleTypeId)
      || Boolean(filters.value.companyId)
      || Boolean(filters.value.contractorIds?.length)
  })

  function reset(): void {
    filters.value = {
      companyId: '',
      contractorIds: [],
      search: '',
      vehicleTypeId: '',
    }
    emit('search')
  }

  onMounted(() => {
    getCompanies()
    getContractors(1, ContractorStatus.ACTIVE)
    getVehicleTypes()
  })
</script>

<style scoped>

</style>
