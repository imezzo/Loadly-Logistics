<template>
  <div
    class="
      mb-6 grid grid-cols-1 gap-4
      md:grid-cols-3
      lg:grid-cols-5
    "
  >
    <div
      v-if="isAdminDashboard"
      class="col-span-full flex items-center gap-2"
    >
      <button
        type="button"
        class="tab-primary-outline shrink-0"
        :class="{ active: !filters.isLeadBilling }"
        @click="filters.isLeadBilling = false"
      >
        {{ $t('content.company') }}
      </button>

      <button
        type="button"
        class="tab-primary-outline shrink-0"
        :class="{ active: filters.isLeadBilling }"
        @click="filters.isLeadBilling = true"
      >
        {{ $t('content.externalLead') }}
      </button>
    </div>

    <BaseInputSearch
      :model-value="filters.search"
      wrapper-class="input-container flex items-center border shrink-0 border-neutral-500 rounded-xl px-3 h-11"
      :placeholder="$t('form.search')"
      @update:model-value="updateFilters({ search: $event ?? '' })"
    />

    <BaseSelect
      v-if="filters.isLeadBilling"
      :model-value="filters.leadId"
      :options="leadOptions"
      searchable
      @on-scroll-end="hasMoreLeads && loadMoreLeads"
      @on-search="getLeads(1, $event)"
      @update:model-value="updateFilters({ leadId: String($event ?? '') })"
    />
    <template v-else>
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
        @update:model-value="updateFilters({ contractId: String($event ?? '') })"
      />
    </template>

    <BaseSelect
      :model-value="filters.invoiceType"
      :options="InvoiceTypeOptions"
      searchable
      @update:model-value="updateFilters({ invoiceType: String($event ?? '') })"
    />

    <BaseSelect
      :model-value="filters.status"
      :options="statusOptions"
      @update:model-value="updateFilters({ status: String($event ?? '') })"
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
  import { useEnum } from '@/composables/useEnum'
  import { useLookups } from '@/composables/useLookups'
  import { usePaymentList } from '../composables/usePaymentList'
  import { useApp } from '@/composables/useApp'
  import { useProfile } from '@/composables/admin-dashboard/useProfile'
  import { InvoiceType } from '@/types/invoice.enums'

  const { t } = useI18n()
  const { isAdminDashboard } = useApp()
  const {
    applyFilters,
    filters,
    hasActiveFilters,
    resetFilters,
    updateFilters,
  } = usePaymentList()
  const {
    companies,
    contractReferenceNumbers,
    getCompanies,
    getContractReferenceNumbers,
    getLeads,
    hasMoreLeads,
    leads,
    loadMoreLeads,
  } = useLookups()
  const { enums } = useEnum()
  const { profile } = useProfile()

  const companyOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('content.company') },
    ...(companies.value?.items ?? []),
  ])

  const contractOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('form.contractRefNumber') },
    ...(contractReferenceNumbers.value?.items ?? []),
  ])

  const leadOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('form.lead') },
    ...(leads.value?.items ?? []),
  ])

  const InvoiceTypeOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('form.invoiceType') },
    { label: t('form.invoice'), value: String(InvoiceType.Invoice) },
    { label: t('form.claim'), value: String(InvoiceType.Claim) },
  ])

  const statusOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('table.status') },
    ...(enums.value?.InvoiceStatus ?? []),
  ])

  function onCompanyChange(value: string | number): void {
    const companyId = String(value ?? '')
    updateFilters({ companyId, contractId: '' })
    if (companyId) {
      getContractReferenceNumbers(companyId, 1)
    }
  }

  watch(() => filters.value.isLeadBilling, (newValue, oldValue) => {
    if (newValue !== oldValue) {
      applyFilters()

      if (newValue) getLeads(1)
    }
  })

  onMounted(async () => {
    if (isAdminDashboard && !companies.value?.items?.length) {
      await getCompanies(1)
    }

    if (!isAdminDashboard) {
      onCompanyChange(profile.value?.id?.toString() ?? '')
    }
  })
</script>

<style scoped>

</style>
