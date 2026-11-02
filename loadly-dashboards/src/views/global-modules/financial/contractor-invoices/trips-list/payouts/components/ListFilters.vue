<template>
  <div
    class="
      mb-6 grid grid-cols-1 gap-4
      md:grid-cols-3
    "
  >
    <div class="flex flex-col gap-2">
      <BaseInputSearch
        id="search"
        :model-value="filters.search"
        wrapper-class="
          input-container flex h-11 shrink-0 items-center rounded-xl border
          border-neutral-500 px-3
        "
        :placeholder="$t('form.searchByTripIdOrInvoice')"
        @update:model-value="updateFilters({ search: $event ?? '' })"
      />
    </div>

    <div class="flex flex-col gap-2">
      <BaseSelect
        id="status"
        :model-value="filters.status"
        :options="statusOptions"
        :placeholder="$t('form.statusField')"
        @update:model-value="updateFilters({ status: String($event ?? '') })"
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
  import { useContractorPayoutsList } from '../composables/useContractorPayoutsList'
  import { useEnum } from '@/composables/useEnum'

  const { t } = useI18n()
  const { enums } = useEnum()
  const {
    applyFilters,
    filters,
    hasActiveFilters,
    resetFilters,
    updateFilters,
  } = useContractorPayoutsList()

  const statusOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('form.statusField') },
    ...enums.value?.ContractorPayoutStatus ?? [],
  ])
</script>

<style scoped>

</style>
