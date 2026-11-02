<template>
  <div class="mt-6">
    <TripsListFilter @search="updateFilters" />

    <BaseTable
      :columns="tableColumns"
      :data="tripsList?.data ?? []"
    />

    <BasePagination
      :pagination="pagination"
      @set-page="setPage"
      @next-page="nextPage"
      @prev-page="prevPage"
      @update-items-per-page="updateItemsPerPage"
    />
  </div>
</template>

<script setup lang="ts">
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import type { TableColumn } from '@/types/ui'
  import { usePagination } from '@/composables/usePagination'
  import { useLoading } from '@/composables/useLoading'
  import TripsListFilter from './TripsListFilter.vue'

  const tripStore = useTripStore()
  const { filters, tripsList } = storeToRefs(tripStore)
  const { t } = useI18n()
  const { setLoading } = useLoading()

  const {
    pagination,
    setPage,
    nextPage,
    prevPage,
    updateItemsPerPage,
    currentPage,
    itemsPerPage,
  } = usePagination(computed(() => tripsList.value?.pagination || null))

  const tableColumns = computed<TableColumn[]>(() => ([
    { key: 'id', label: t('content.tripId') },
    { key: 'contractRef', label: t('table.contractRef') },
    { key: 'actualDistanceKm', label: t('table.km') },
    { key: 'startDate', label: t('table.dateOfTrip') },
    { key: 'fuelCost', label: t('table.fuelCost') },
    { key: 'totalTripCost', label: t('table.totalTripCost') },
    { key: 'totalAddonsCost', label: t('table.tripsAddonsCost') },
  ]))

  async function fetchTrips(): Promise<void> {
    setLoading(true)
    await tripStore.fetchTrips({
      page: currentPage.value,
      size: itemsPerPage.value,
      ...filters.value,
    })
    setLoading(false)
  }

  function updateFilters(): void {
    currentPage.value = 1
    fetchTrips()
  }

  watch([currentPage, itemsPerPage], () => fetchTrips())

  onMounted(fetchTrips)
</script>

<style scoped>

</style>
