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
        v-model="localFilters.search"
        :placeholder="$t('form.searchNameOrEmail')"
        wrapper-class="
          flex h-11 items-center rounded-lg border border-neutral-300 bg-white px-3
          md:flex-1
        "
      />
    </div>

    <div class="flex flex-col gap-2">
      <label
        for="contractorStatus"
        class="text-sm font-medium text-neutral-700"
      >
        {{ $t('form.contractorStatus') }}
      </label>

      <BaseSelect
        id="contractorStatus"
        v-model="localFilters.contractorStatus"
        :options="statusOptions"
      />
    </div>

    <div class="flex flex-col gap-2">
      <label
        for="status"
        class="text-sm font-medium text-neutral-700"
      >
        {{ $t('form.statusField') }}
      </label>

      <BaseSelect
        id="status"
        v-model="localFilters.status"
        :options="userStatusOptions"
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
        @click="handleResetFilters"
      >
        {{ $t('button.clearFilters') }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
  import type { SelectOption } from '@/types/ui'
  import { useList } from '../composables/useList'
  import { useEnum } from '@/composables/useEnum'

  const { t } = useI18n()
  const {
    filters,
    updateFilters,
    resetFilters,
    fetchContractors,
    setPage,
  } = useList()
  const { enums, userStatus } = useEnum()

  const localFilters = ref({
    search: filters.value.search || '',
    contractorStatus: filters.value.contractorStatus || '',
    status: filters.value.status || '',
  })

  const statusOptions = computed<SelectOption[]>(() => [
    { label: t('form.allStatus'), value: '' },
    ...(enums.value?.ContractorStatus ?? []).map(item => ({
      label: item.label,
      value: item.value,
    })),
  ])

  const userStatusOptions = computed<SelectOption[]>(() => [
    { label: t('form.allStatus'), value: '' },
    ...(userStatus.value ?? []),
  ])

  const hasActiveFilters = computed(() => {
    return !!(localFilters.value.search || localFilters.value.contractorStatus || localFilters.value.status)
  })

  function applyFilters() {
    updateFilters({
      search: localFilters.value.search,
      contractorStatus: localFilters.value.contractorStatus,
      status: localFilters.value.status,
    })
    setPage(1)
    fetchContractors()
  }

  function handleResetFilters() {
    resetFilters()
    localFilters.value = {
      search: '',
      contractorStatus: '',
      status: '',
    }
    setPage(1)
    fetchContractors()
  }

  watch(() => filters.value, (newFilters) => {
    localFilters.value = {
      search: newFilters.search,
      contractorStatus: newFilters.contractorStatus,
      status: newFilters.status,
    }
  }, { deep: true })
</script>

<style scoped>

</style>
