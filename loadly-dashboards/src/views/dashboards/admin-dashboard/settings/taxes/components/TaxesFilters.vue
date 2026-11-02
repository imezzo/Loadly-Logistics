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
      :placeholder="$t('form.search')"
      wrapper-class="
        flex h-11 items-center rounded-lg border border-neutral-300 bg-white px-3
        md:flex-1 shrink-0
      "
    />

    <BaseSelect
      v-model="filters.type"
      :options="typeOptions"
      :label="$t('form.taxType')"
    />

    <BaseSelect
      v-model="filters.status"
      :options="statusOptions"
      :label="$t('form.allStatus')"
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
  import type { SelectOption } from '@/types/ui'
  import { useEnum } from '@/composables/useEnum'
  import { useList } from '../composables/useList'

  const { t } = useI18n()
  const { enums } = useEnum()
  const {
    filters,
    resetFilters,
    fetchTaxes,
    setPage,
  } = useList()

  const typeOptions = computed<SelectOption[]>(() => [
    { label: t('form.allTaxTypes'), value: '' },
    ...(enums.value?.TaxType ?? []),
  ])

  const statusOptions = computed<SelectOption[]>(() => [
    { label: t('form.allStatus'), value: '' },
    { label: t('form.disabled'), value: '0' },
    { label: t('form.active'), value: '1' },
    { label: t('form.suspended'), value: '2' },
  ])

  const hasActiveFilters = computed(() => !!(
    filters.value.search
    || filters.value.type
    || filters.value.status
  ))

  function applyFilters() {
    setPage(1)
    fetchTaxes()
  }

  function handleResetFilters() {
    resetFilters()
    setPage(1)
    fetchTaxes()
  }
</script>

<style scoped>

</style>
