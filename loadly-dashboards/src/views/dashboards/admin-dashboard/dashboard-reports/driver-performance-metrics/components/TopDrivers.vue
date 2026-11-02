<template>
  <section class="mt-2">
    <h3 class="mb-6 text-lg font-bold text-neutral-10">
      {{ t('title.topDrivers') }}
    </h3>

    <TopDriversFilters
      v-model:search="search"
      @search="fetchDrivers"
    />

    <TopDriversTable :drivers />

    <TripsDriversMetrics />
  </section>
</template>

<script setup lang="ts">
  import type { Driver } from '@/types/admin-dashboard/response/driver/driver'
  import TopDriversFilters from './TopDriversFilters.vue'
  import TopDriversTable from './TopDriversTable.vue'
  import TripsDriversMetrics from './TripsDriversMetrics.vue'
  import { useDriverStore } from '@/stores/admin-dashboard/driver'
  import { useLoading } from '@/composables/useLoading'

  const { t } = useI18n()
  const { setLoading } = useLoading()
  const driverStore = useDriverStore()

  const search = ref('')

  const drivers = ref<Driver[]>([])

  async function fetchDrivers(): Promise<void> {
    setLoading(true)

    const { data } = await driverStore.getList({
      search: search.value,
      orderByCompletedTripsDescending: true,
      pagination: { size: 10 },
    })

    drivers.value = data.drivers
    setLoading(false)
  }

  onMounted(fetchDrivers)
</script>

<style scoped>

</style>
