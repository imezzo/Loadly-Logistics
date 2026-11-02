<template>
  <div
    class="
      mb-6 grid grid-cols-1 gap-4
      sm:grid-cols-2
      md:grid-cols-4
    "
  >
    <div>
      <label class="mb-1 block text-sm font-medium text-neutral-20">{{ $t('form.search') }}</label>
      <BaseInputSearch
        :model-value="filters.search"
        wrapper-class="input-container flex h-11 shrink-0 items-center rounded-xl border border-neutral-500 px-3"
        :placeholder="$t('form.searchByBillContract')"
        @update:model-value="emit('updateFilters', { search: $event ?? '' })"
      />
    </div>

    <div>
      <label class="mb-1 block text-sm font-medium text-neutral-20">{{ $t('content.contract') }}</label>
      <BaseSelect
        :model-value="filters.contractId"
        :options="contractOptions"
        searchable
        @update:model-value="emit('updateFilters', { contractId: String($event ?? '') })"
        @search="getContractReferenceNumbers(filters.companyId ?? '', 1, $event)"
        @on-scroll-to-end="fetchMoreContractReferenceNumbers"
      />
    </div>

    <div>
      <label class="mb-1 block text-sm font-medium text-neutral-20">{{ $t('table.status') }}</label>
      <BaseSelect
        :model-value="filters.status"
        :options="statusOptions"
        @update:model-value="emit('updateFilters', { status: String($event ?? '') })"
      />
    </div>

    <div class="flex items-end gap-2">
      <button
        type="button"
        class="btn-primary h-11 min-w-28 flex-1"
        @click="emit('apply')"
      >
        {{ $t('button.search') }}
      </button>

      <button
        v-if="hasActiveFilters"
        type="button"
        class="btn-link whitespace-nowrap"
        @click="emit('reset')"
      >
        {{ $t('button.clearFilters') }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
  import type { SelectOption } from '@/types/ui'
  import type { InvoiceReportsFilters } from '../composables/useInvoiceReportsList'
  import { useLookups } from '@/composables/useLookups'
  import { useEnum } from '@/composables/useEnum'

  defineProps<{
    filters: InvoiceReportsFilters
    hasActiveFilters: boolean
  }>()

  const emit = defineEmits<{
    updateFilters: [partial: Partial<InvoiceReportsFilters>]
    apply: []
    reset: []
  }>()

  const { contractReferenceNumbers, getContractReferenceNumbers, hasMoreContractReferenceNumbers, loadMoreContractReferenceNumbers } = useLookups()
  const { t } = useI18n()
  const { enums } = useEnum()

  const contractOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('form.contractRefNumber') },
    ...(contractReferenceNumbers.value?.items ?? []),
  ])

  const statusOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('form.allStatus') },
    ...(enums.value?.InvoiceStatus ?? []),
  ])

  function fetchMoreContractReferenceNumbers(): void {
    if (hasMoreContractReferenceNumbers.value) {
      loadMoreContractReferenceNumbers()
    }
  }
</script>

<style scoped>

</style>
