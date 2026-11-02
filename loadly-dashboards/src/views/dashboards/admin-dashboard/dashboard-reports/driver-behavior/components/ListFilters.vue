<template>
  <div
    class="
      mb-6 grid grid-cols-1 gap-4
      md:grid-cols-2
      lg:grid-cols-4 lg:items-end
    "
  >
    <div>
      <label
        for="driverId"
        class="mb-2 block text-sm font-medium text-neutral-800"
      >
        {{ $t('content.driverList') }}
      </label>
      <BaseSelect
        id="driverId"
        :model-value="filters.driverId"
        :options="driverOptions"
        searchable
        :on-scroll-to-end="hasMoreDrivers ? loadMoreDrivers : undefined"
        @update:model-value="updateFilters({ driverId: String($event ?? '') })"
      />
    </div>

    <div>
      <label
        for="fromDate"
        class="mb-2 block text-sm font-medium text-neutral-800"
      >
        {{ $t('form.startDate') }}
      </label>
      <BaseInput
        id="fromDate"
        :model-value="filters.fromDate"
        type="date"
        @update:model-value="updateFilters({ fromDate: String($event ?? '') })"
      />
    </div>

    <div>
      <label
        for="toDate"
        class="mb-2 block text-sm font-medium text-neutral-800"
      >
        {{ $t('form.endDate') }}
      </label>
      <BaseInput
        id="toDate"
        :model-value="filters.toDate"
        type="date"
        :min="filters.fromDate || undefined"
        @update:model-value="updateFilters({ toDate: String($event ?? '') })"
      />
    </div>

    <div class="flex flex-wrap items-center gap-3">
      <button
        type="button"
        class="btn-primary h-11 min-w-40"
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
  import { useDriverBehaviorList } from '../composables/useDriverBehaviorList'

  const { t } = useI18n()
  const {
    applyFilters,
    filters,
    hasActiveFilters,
    resetFilters,
    updateFilters,
  } = useDriverBehaviorList()

  const {
    drivers,
    getDrivers,
    hasMoreDrivers,
    loadMoreDrivers,
  } = useLookups()

  const driverOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('form.select') },
    ...(drivers.value?.items ?? []),
  ])

  onMounted(getDrivers)
</script>

<style scoped>

</style>
