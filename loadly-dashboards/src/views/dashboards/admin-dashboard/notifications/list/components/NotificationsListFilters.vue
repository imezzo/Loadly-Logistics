<template>
  <div
    class="
      mb-6 grid grid-cols-1 gap-4
      md:grid-cols-5
    "
  >
    <BaseInputSearch
      v-model="filters.search"
      :placeholder="$t('form.search')"
      wrapper-class="
        flex h-11 items-center rounded-lg border border-neutral-300 bg-white px-3
        md:flex-1
      "
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
  import { useList } from '../composables/useList'

  const {
    fetchNotifications,
    filters,
    resetFilters,
    setPage,
    updateFilters,
  } = useList()

  const hasActiveFilters = computed(() => !!filters.value.search)

  function applyFilters() {
    updateFilters({ search: filters.value.search })
    setPage(1)
    fetchNotifications()
  }

  function handleResetFilters() {
    resetFilters()
    setPage(1)
    fetchNotifications()
  }
</script>

<style scoped>

</style>
