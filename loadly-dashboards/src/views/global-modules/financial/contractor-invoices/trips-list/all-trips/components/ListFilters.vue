<template>
  <div
    class="
      mb-6 grid grid-cols-1 gap-4
      md:grid-cols-3
      lg:grid-cols-5
    "
  >
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
        wrapper-class="
          input-container flex h-11 shrink-0 items-center rounded-xl border
          border-neutral-500 px-3
        "
        :placeholder="$t('form.searchByTripId')"
        @update:model-value="updateFilters({ search: $event ?? '' })"
      />
    </div>

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
        @update:model-value="updateFilters({ companyId: String($event ?? '') })"
        @on-search="getCompanies(1, $event)"
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

    <div class="flex items-end gap-2">
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
  import { useLookups } from '@/composables/useLookups'
  import { useContractorTripsList } from '../composables/useContractorTripsList'

  const { t } = useI18n()
  const {
    applyFilters,
    filters,
    hasActiveFilters,
    resetFilters,
    updateFilters,
  } = useContractorTripsList()

  const {
    companies,
    getCompanies,
    hasMoreCompanies,
    loadMoreCompanies,
  } = useLookups()

  const companyOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('content.company') },
    ...(companies.value?.items ?? []),
  ])

  onMounted(() => {
    if (!companies.value?.items?.length) {
      getCompanies(1)
    }
  })
</script>

<style scoped>

</style>
