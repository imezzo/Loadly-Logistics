<template>
  <div
    class="
      mb-6 grid grid-cols-1 gap-4
      md:grid-cols-3
      lg:grid-cols-4
    "
  >
    <BaseInputSearch
      :model-value="filters.search"
      wrapper-class="
        input-container flex h-11 shrink-0 items-center rounded-xl border
        border-neutral-500 px-3
      "
      :placeholder="$t('form.searchByContractorName')"
      @update:model-value="updateFilters({ search: $event ?? '' })"
    />

    <BaseInput
      :model-value="filters.startDateFrom"
      type="date"
      :placeholder="$t('form.dateRange')"
      @update:model-value="updateFilters({ startDateFrom: String($event ?? '') })"
    />

    <BaseInput
      :model-value="filters.startDateTo"
      type="date"
      :min="filters.startDateFrom || undefined"
      :placeholder="$t('form.endDate')"
      @update:model-value="updateFilters({ startDateTo: String($event ?? '') })"
    />

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
        class="btn-link shrink-0"
        @click="resetFilters"
      >
        {{ $t('button.clearFilters') }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { useContractorInvoices } from '../composables/useContractorInvoices'

  const {
    applyFilters,
    filters,
    hasActiveFilters,
    resetFilters,
    updateFilters,
  } = useContractorInvoices()
</script>

<style scoped>

</style>
