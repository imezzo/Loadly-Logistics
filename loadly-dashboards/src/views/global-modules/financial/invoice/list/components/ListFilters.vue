<template>
  <div
    class="
      mb-6 grid grid-cols-1 gap-4
      md:grid-cols-3
    "
  >
    <BaseSelect
      v-if="isAdminDashboard"
      :model-value="filters.companyId"
      :options="companyOptions"
      searchable
      @update:model-value="onCompanyChange"
    />

    <BaseSelect
      :model-value="filters.contractId"
      :options="contractOptions"
      :disabled="!filters.companyId"
      searchable
      @update:model-value="updateFilters({ contractId: $event })"
    />

    <div class="flex gap-2">
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
        class="btn-link me-auto shrink-0"
        @click="resetFilters"
      >
        {{ $t('button.clearFilters') }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
  import type { SelectOption } from '@/types/ui'
  import { useInvoiceList } from '../composables/useInvoiceList'
  import { useLookups } from '@/composables/useLookups'
  import { useApp } from '@/composables/useApp'
  import { useProfile } from '@/composables/admin-dashboard/useProfile'

  const { t } = useI18n()
  const { isAdminDashboard } = useApp()
  const { profile } = useProfile()
  const {
    filters,
    hasActiveFilters,
    resetFilters,
    updateFilters,
    applyFilters,
  } = useInvoiceList()

  const {
    companies,
    contractReferenceNumbers,
    getCompanies,
    getContractReferenceNumbers,
  } = useLookups()

  const companyOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('content.company') },
    ...(companies.value?.items ?? []),
  ])

  const contractOptions = computed<SelectOption[]>(() => {
    return [
      { value: '', label: t('form.contractRefNumber') },
      ...(contractReferenceNumbers.value?.items ?? []),
    ]
  })

  function onCompanyChange(value: string): void {
    updateFilters({ companyId: value, contractId: '' })
    if (value) {
      getContractReferenceNumbers(value, 1)
    }
  }

  onMounted(async () => {
    if (!companies.value?.items?.length) {
      await getCompanies(1)
    }

    if (!isAdminDashboard) {
      onCompanyChange(profile.value?.id?.toString() ?? '')
    }
  })
</script>

<style scoped>

</style>
