<template>
  <div
    class="
      grid grid-cols-1 gap-4
      md:grid-cols-2
      lg:grid-cols-4
    "
  >
    <BaseInputSearch
      v-model="filters.search"
      :placeholder="$t('form.searchNameOrNumber')"
      wrapper-class="
        flex h-11 items-center rounded-lg border border-neutral-300 bg-white px-3
        md:flex-1 shrink-0
      "
    />

    <BaseSelect
      v-model="filters.licenseCategory"
      :options="licenseCategoryOptions"
      :label="$t('form.licenseCategory')"
    />

    <BaseSelect
      :model-value="filters.workingStatuses.toString()"
      :options="statusOptions"
      :label="$t('form.allStatus')"
      @update:model-value="filters.workingStatuses = Number($event)"
    />

    <BaseMultiSelect
      v-if="isAdminDashboard"
      v-model="filters.contractorIds"
      :options="contractors?.items ?? []"
      :placeholder="$t('form.contractors')"
      searchable
      :on-scroll-to-end="hasMoreContractors ? loadMoreContractors : undefined"
      :on-search="(search) => getContractors(1, ContractorStatus.ACTIVE, search)"
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
        class="btn-link shrink-0"
        @click="handleResetFilters"
      >
        {{ $t('button.clearFilters') }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { DriverStatus } from '@/types/admin-dashboard/driver'
  import { useList } from '../composables/useList'
  import type { SelectOption } from '@/types/ui'
  import { useEnum } from '@/composables/useEnum'
  import { useLookups } from '@/composables/useLookups'
  import { useApp } from '@/composables/useApp'
  import { ContractorStatus } from '@/types/admin-dashboard/response/contractor/contractor.enums'

  const { t } = useI18n()
  const { isAdminDashboard } = useApp()
  const {
    filters,
    updateFilters,
    resetFilters,
    fetchDrivers,
    setPage,
  } = useList()
  const { enums } = useEnum()
  const {
    contractors,
    getContractors,
    hasMoreContractors,
    loadMoreContractors,
  } = useLookups()

  const statusOptions = computed<SelectOption[]>(() => [
    { label: t('form.allStatus'), value: '' },
    ...(enums.value?.DriverWorkingStatus ?? []),
  ])

  const licenseCategoryOptions = computed<SelectOption[]>(() => [
    { label: t('form.allLicenseCategories'), value: '' },
    ...(enums.value?.LicenseCategory ?? []),
  ])

  const hasActiveFilters = computed(() => {
    return !!(
      filters.value.search ||
      filters.value.workingStatuses ||
      filters.value.licenseCategory ||
      filters.value.rate ||
      filters.value.contractorIds.length
    )
  })

  function applyFilters() {
    updateFilters({
      contractorIds: filters.value.contractorIds,
      search: filters.value.search,
      workingStatuses: filters.value.workingStatuses as DriverStatus,
      licenseCategory: filters.value.licenseCategory,
      rate: filters.value.rate,
    })
    setPage(1)
    fetchDrivers()
  }

  function handleResetFilters() {
    resetFilters()
    setPage(1)
    fetchDrivers()
  }

  onMounted(() => {
    if (isAdminDashboard) {
      getContractors(1)
    }
  })
</script>

<style scoped>

</style>
