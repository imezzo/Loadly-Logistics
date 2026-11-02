<template>
  <div
    class="
      mb-6 grid grid-cols-1 gap-4
      md:grid-cols-4
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
        :model-value="filters.search"
        :placeholder="$t('form.searchCommercialNumberNameTax')"
        wrapper-class="
          flex h-11 items-center rounded-lg border border-neutral-300 bg-white px-3
          md:flex-1
        "
        @update:model-value="updateFilters({ search: $event })"
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
        :model-value="filters.status"
        :options="statusOptions"
        @update:model-value="updateFilters({ status: $event })"
      />
    </div>

    <div class="flex flex-col gap-2">
      <label
        for="contractType"
        class="text-sm font-medium text-neutral-700"
      >
        {{ $t('form.contractType') }}
      </label>

      <BaseSelect
        id="contractType"
        :model-value="filters.contractType"
        :options="contractTypeOptions"
        @update:model-value="updateFilters({ contractType: $event })"
      />
    </div>

    <div
      v-if="!isCompanyDashboard"
      class="flex flex-col gap-2"
    >
      <label
        for="company"
        class="text-sm font-medium text-neutral-700"
      >
        {{ $t('content.company') }}
      </label>

      <BaseSelect
        id="company"
        :model-value="filters.company"
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
        :disabled="!isCompanyDashboard && !filters.company"
        searchable
        :on-scroll-to-end="hasMoreContractReferenceNumbers ? loadMoreContractReferenceNumbers : undefined"
        :on-search="(search) => getContractReferenceNumbers(
          filters.company || profile?.id?.toString() || undefined,
          1,
          search,
        )"
        @update:model-value="updateFilters({ contractId: String($event ?? '') })"
      />
    </div>

    <div
      v-if="!isCompanyDashboard"
      class="flex flex-col gap-2"
    >
      <label
        for="salesRepresentative"
        class="text-sm font-medium text-neutral-700"
      >
        {{ $t('form.salesRepresentative') }}
      </label>

      <BaseSelect
        id="salesRepresentative"
        :model-value="filters.salesRepresentative"
        :options="salesRepresentativeOptions"
        searchable
        :on-scroll-to-end="hasMoreAdmins ? loadMoreAdmins : undefined"
        :on-search="(search) => getAdmins(1, search)"
        @update:model-value="updateFilters({ salesRepresentative: $event })"
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
  import { useContractList } from '../composables/useContractList'
  import { useLookups } from '@/composables/useLookups'
  import { useEnum } from '@/composables/useEnum'
  import { useApp } from '@/composables/useApp'
  import { useProfile } from '@/composables/admin-dashboard/useProfile'

  const { t } = useI18n()
  const { enums } = useEnum()
  const { isCompanyDashboard } = useApp()
  const { profile } = useProfile()
  const {
    applyFilters,
    filters,
    resetFilters,
    updateFilters,
  } = useContractList()
  const {
    admins,
    companies,
    contractReferenceNumbers,
    getAdmins,
    getCompanies,
    getContractReferenceNumbers,
    hasMoreAdmins,
    hasMoreCompanies,
    hasMoreContractReferenceNumbers,
    loadMoreAdmins,
    loadMoreCompanies,
    loadMoreContractReferenceNumbers,
  } = useLookups()

  const hasActiveFilters = computed(() => Object.values(filters.value).some(value => value !== ''))

  const contractTypeOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('form.contractType') },
    ...enums.value?.ContractType ?? [],
  ])

  const companyOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('content.company') },
    ...(companies.value?.items ?? []),
  ])

  const contractOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('form.contractRefNumber') },
    ...(contractReferenceNumbers.value?.items ?? []),
  ])

  const salesRepresentativeOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('form.salesRepresentative') },
    ...(admins.value?.items ?? []),
  ])

  const statusOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('form.allStatus') },
    ...(enums.value?.ContractStatus ?? []),
  ])

  function onCompanyChange(value: string): void {
    updateFilters({ company: value, contractId: '' })

    if (value) {
      getContractReferenceNumbers(value, 1)
    }
  }

  onMounted(async () => {
    if (!isCompanyDashboard) {
      if (!companies.value?.items?.length) {
        await getCompanies(1)
      }

      if (!admins.value) {
        getAdmins(1)
      }

      return
    }

    getContractReferenceNumbers(profile.value?.id?.toString())
  })
</script>

<style scoped>

</style>
