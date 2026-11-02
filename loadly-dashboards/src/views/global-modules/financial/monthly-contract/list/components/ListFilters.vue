<template>
  <div class="mb-6 flex flex-col gap-4">
    <div
      class="
        grid grid-cols-1 gap-4
        sm:grid-cols-2
        md:grid-cols-3
        lg:grid-cols-4
      "
    >
      <div class="flex flex-col gap-2">
        <label
          for="search"
          class="text-sm font-medium text-neutral-700"
        >
          {{ $t('form.searchByInvoice') }}
        </label>

        <BaseInputSearch
          id="search"
          :model-value="filters.search"
          wrapper-class="
            input-container flex h-11 shrink-0 items-center rounded-xl border
            border-neutral-500 px-3
          "
          :placeholder="$t('form.searchByInvoice')"
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
          @update:model-value="changeCompany"
        />
      </div>

      <div class="flex flex-col gap-2">
        <label
          for="contractId"
          class="text-sm font-medium text-neutral-700"
        >
          {{ $t('content.contract') }}
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

      <div class="flex items-end">
        <button
          type="button"
          class="
            btn-primary w-full min-w-32
            sm:w-auto
          "
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
  </div>
</template>

<script setup lang="ts">
  import type { SelectOption } from '@/types/ui'
  import { useLookups } from '@/composables/useLookups'
  import { useBillingSnapshotsList } from '../composables/useBillingSnapshotsList'

  const { t } = useI18n()
  const {
    applyFilters,
    filters,
    hasActiveFilters,
    resetFilters,
    updateFilters,
  } = useBillingSnapshotsList()

  const {
    companies,
    contractReferenceNumbers,
    getCompanies,
    getContractReferenceNumbers,
    hasMoreCompanies,
    hasMoreContractReferenceNumbers,
    loadMoreCompanies,
    loadMoreContractReferenceNumbers,
  } = useLookups()

  const companyOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('content.company') },
    ...(companies.value?.items ?? []),
  ])

  const contractOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('content.contract') },
    ...(contractReferenceNumbers.value?.items ?? []),
  ])

  function changeCompany(value: string | number): void {
    const companyId = String(value ?? '')
    updateFilters({ companyId, contractId: '' })

    if (companyId) {
      getContractReferenceNumbers(companyId, 1)
    }
  }

  onMounted(async () => {
    if (!companies.value?.items?.length) {
      await getCompanies(1)
    }

    if (filters.value.companyId) {
      getContractReferenceNumbers(filters.value.companyId, 1)
    }
  })
</script>

<style>

</style>
