<template>
  <div
    class="
      mb-6 grid grid-cols-1 items-end gap-4
      sm:grid-cols-2
      md:grid-cols-4
    "
  >
    <div>
      <label for="search">{{ $t('form.search') }}</label>
      <BaseInput
        :model-value="filters.search ?? ''"
        :placeholder="$t('form.searchByTripOrDriver')"
        @update:model-value="filters.search = $event?.toString()"
      />
    </div>

    <div>
      <label for="contractId">{{ $t('content.contractReferenceNumber') }}</label>
      <BaseSelect
        v-model="filters.contractId"
        :options="contractReferenceNumbers?.items ?? []"
        searchable
        @on-scroll-to-end="fetchMoreContractReferenceNumbers"
        @search="getContractReferenceNumbers(filters.companyId ?? '', 1, $event)"
      />
    </div>

    <div class="flex items-center gap-2">
      <button
        class="btn-primary"
        @click="$emit('search')"
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
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { useLookups } from '@/composables/useLookups'
  import { useProfile } from '@/composables/admin-dashboard/useProfile'

  const emit = defineEmits<{
    search: []
  }>()

  const tripStore = useTripStore()
  const { filters } = storeToRefs(tripStore)
  const { profile } = useProfile()

  const {
    contractReferenceNumbers,
    getContractReferenceNumbers,
    hasMoreContractReferenceNumbers,
    loadMoreContractReferenceNumbers,
  } = useLookups()

  const hasActiveFilters = computed(() => {
    return Object.values(filters.value).some(value => value !== '')
  })

  async function fetchMoreContractReferenceNumbers(): Promise<void> {
    if (hasMoreContractReferenceNumbers.value && filters.value.companyId) {
      loadMoreContractReferenceNumbers()
    }
  }

  getContractReferenceNumbers(profile.value?.id?.toString() ?? '')

  function resetFilters(): void {
    filters.value = {
      search: '',
      contractId: '',
    }
    emit('search')
  }
</script>

<style scoped>

</style>
