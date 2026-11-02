<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <div
      class="
        flex flex-col gap-4
        sm:flex-row sm:items-center sm:justify-between
      "
    >
      <h1 class="text-xl font-bold text-neutral-10">
        {{ $t('title.vehiclesManage') }}
      </h1>

      <VehicleStatusExport :filters="filters" />
    </div>

    <article class="mt-4 rounded-2xl border border-neutral-500 bg-white p-1.5">
      <div
        class="
          flex flex-col gap-6
          lg:flex-row lg:items-stretch
        "
      >
        <section
          class="
            flex min-w-0 flex-col justify-center p-4
            lg:w-1/4
          "
        >
          <p class="font-bold text-neutral-10">
            {{ $t('content.totalVehicles') }}
          </p>

          <span class="mt-4 text-5xl font-bold text-neutral-10">
            {{ totalVehicles }}
          </span>
        </section>

        <div
          class="
            grid flex-1 grid-cols-1 gap-4
            sm:grid-cols-2
            lg:grid-cols-3
          "
        >
          <VehicleStatusCard
            v-for="card in statusCards"
            :key="card.label"
            :count="card.count"
            :label="card.label"
            :label-class="card.labelClass"
          />
        </div>
      </div>
    </article>

    <article class="mt-6">
      <VehicleStatusFilters
        v-model:filters="filters"
        @search="applyFilters"
        @reset="resetFilters"
      />

      <VehicleStatusTable
        :vehicles="vehicles"
        :pagination="pagination"
        @fetch-vehicles="refreshAfterAction"
        @page-change="setPage"
        @items-per-page-change="updateItemsPerPage"
      />
    </article>
  </div>
</template>

<script setup lang="ts">
  import VehicleStatusCard from './components/VehicleStatusCard.vue'
  import VehicleStatusExport from './components/VehicleStatusExport.vue'
  import VehicleStatusFilters from './components/VehicleStatusFilters.vue'
  import VehicleStatusTable from './components/VehicleStatusTable.vue'
  import { useVehicleStatus } from './composables/useVehicleStatus'
  import { useVehicleStatusList } from './composables/useVehicleStatusList'

  const { t } = useI18n()
  const { fetchVehicleStatus, statusCards, totalVehicles } = useVehicleStatus()
  const {
    applyFilters,
    fetchVehicles,
    filters,
    pagination,
    resetFilters,
    setPage,
    updateItemsPerPage,
    vehicles,
  } = useVehicleStatusList()

  const breadcrumbItems = computed(() => [
    { label: t('title.garages') },
    { label: t('title.vehiclesManage') },
  ])

  async function refreshAfterAction(): Promise<void> {
    await Promise.all([fetchVehicleStatus(), fetchVehicles()])
  }

  onMounted(() => {
    fetchVehicleStatus()
    fetchVehicles()
  })
</script>

<style scoped>

</style>

