<template>
  <section class="mt-6">
    <h3 class="mb-6 text-lg font-bold text-neutral-10">
      {{ t('title.topVehicles') }}
    </h3>

    <TopVehiclesFilters
      v-model:filters="filters"
      @search="fetchVehicles"
    />

    <TopVehiclesTable
      :vehicles="vehiclesList?.vehicles ?? []"
      :is-loading="isLoading"
    />
  </section>
</template>

<script setup lang="ts">
  import TopVehiclesFilters from './TopVehiclesFilters.vue'
  import TopVehiclesTable from './TopVehiclesTable.vue'
  import { useLookups } from '@/composables/useLookups'
  import { useLoading } from '@/composables/useLoading'
  import { useVehicleStore } from '@/stores/admin-dashboard/vehicle'

  const { t } = useI18n()
  const { isLoading, setLoading } = useLoading()
  const { getVehicleBrandModels, getVehicleTypes } = useLookups()

  const vehicleStore = useVehicleStore()
  const { vehiclesList } = storeToRefs(vehicleStore)

  const filters = ref({
    search: '',
    vehicleTypeId: '',
    brandModelId: '',
  })

  async function fetchVehicles(): Promise<void> {
    setLoading(true)
    const { data } = await vehicleStore.getVehicles({
      size: 10,
      orderByCompletedTripsDescending: true,
      ...filters.value
    })
    setLoading(false)
    vehiclesList.value = data
  }

  onMounted(async () => {
    await Promise.all([
      getVehicleTypes(),
      getVehicleBrandModels(),
      fetchVehicles()
    ])
  })
</script>

<style scoped>

</style>
