<template>
  <div
    class="
      mb-6 grid grid-cols-1 items-end gap-4
      md:grid-cols-4
    "
  >
    <div>
      <label for="companyId">{{ $t('content.company') }}</label>
      <BaseSelect
        v-model="filters.companyId"
        :options="companyOptions"
        :on-scroll-to-end="hasMoreCompanies ? loadMoreCompanies : undefined"
        searchable
      />
    </div>

    <div>
      <label for="financialDateFrom">{{ $t('form.financialStartDate') }}</label>
      <BaseInput
        :model-value="filters.financialDateFrom ?? ''"
        type="date"
        @update:model-value="filters.financialDateFrom = $event?.toString()"
      />
    </div>

    <div>
      <label for="financialDateTo">{{ $t('form.financialEndDate') }}</label>
      <BaseInput
        :model-value="filters.financialDateTo ?? ''"
        type="date"
        :min="filters.financialDateFrom || undefined"
        @update:model-value="filters.financialDateTo = $event?.toString()"
      />
    </div>

    <div class="flex shrink-0 items-center gap-2">
      <button
        type="button"
        class="btn-primary flex-1"
        @click="$emit('search')"
      >
        {{ $t('button.search') }}
      </button>

      <button
        v-if="hasActiveFilters"
        type="button"
        class="btn-link shrink-0"
        @click="reset"
      >
        {{ $t('button.clearFilters') }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { useLookups } from '@/composables/useLookups'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import type { SelectOption } from '@/types/ui'

  const emit = defineEmits<{
    search: []
  }>()

  const {
    companies,
    getCompanies,
    hasMoreCompanies,
    loadMoreCompanies,
  } = useLookups()

  const tripStore = useTripStore()
  const { tripRequestsFilters: filters } = storeToRefs(tripStore)

  const companyOptions = computed<SelectOption[]>(() => companies.value?.items ?? [])

  const hasActiveFilters = computed(() => {
    return Boolean(filters.value.companyId)
      || Boolean(filters.value.financialDateFrom)
      || Boolean(filters.value.financialDateTo)
  })

  function reset(): void {
    filters.value = {
      companyId: '',
      financialDateFrom: '',
      financialDateTo: '',
    }
    emit('search')
  }

  onMounted(getCompanies)
</script>

<style scoped>

</style>
