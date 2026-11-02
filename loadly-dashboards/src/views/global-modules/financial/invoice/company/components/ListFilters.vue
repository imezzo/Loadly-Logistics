<template>
  <div
    class="
      mb-6 grid grid-cols-1 items-end gap-4
      md:grid-cols-2
      lg:grid-cols-4
    "
  >
    <div>
      <label class="mb-2 block text-sm font-medium text-neutral-800">
        {{ $t('form.financialStartDate') }}
      </label>

      <BaseInput
        :model-value="filters.financialDateFrom ?? ''"
        type="date"
        @update:model-value="filters.financialDateFrom = $event?.toString()"
      />
    </div>

    <div>
      <label class="mb-2 block text-sm font-medium text-neutral-800">
        {{ $t('form.financialEndDate') }}
      </label>

      <BaseInput
        :model-value="filters.financialDateTo ?? ''"
        type="date"
        :min="filters.financialDateFrom"
        @update:model-value="filters.financialDateTo = $event?.toString()"
      />
    </div>

    <div>
      <label
        for="destinationIds"
        class="mb-2 block text-sm font-medium text-neutral-800"
      >
        {{ $t('table.destination') }}
      </label>
      <BaseMultiSelect
        id="destinationIds"
        v-model="filters.destinationIds"
        :options="destinationOptions"
        :placeholder="$t('table.destination')"
        searchable
        :on-scroll-to-end="hasMoreContractDestinations ? loadMoreContractDestinations : undefined"
        :on-search="(search) => getContractDestinations(contractId, 1, search)"
      />
    </div>

    <div class="flex items-center gap-2">
      <button
        type="button"
        class="btn-primary min-w-40"
        @click="emit('search')"
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
  import { useLookups } from '@/composables/useLookups'
  import { useTripStore } from '@/stores/admin-dashboard/trip'

  const emit = defineEmits<{
    search: []
  }>()

  const route = useRoute()
  const { t } = useI18n()
  const tripStore = useTripStore()
  const { tripInvoiceFilters: filters } = storeToRefs(tripStore)

  const contractId = computed(() => String(route.params.id ?? ''))

  const {
    contractDestinations,
    getContractDestinations,
    hasMoreContractDestinations,
    loadMoreContractDestinations,
  } = useLookups()

  const destinationOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('table.destination') },
    ...(contractDestinations.value?.items ?? []),
  ])

  const hasActiveFilters = computed(() =>
    filters.value.financialDateFrom
    || filters.value.financialDateTo
    || filters.value.destinationIds?.length
  )

  function resetFilters(): void {
    filters.value.financialDateFrom = ''
    filters.value.financialDateTo = ''
    filters.value.destinationIds = []
    emit('search')
  }

  onMounted(() => {
    if (contractId.value) getContractDestinations(contractId.value, 1)
  })
</script>

<style scoped>

</style>
