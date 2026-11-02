<template>
  <div
    class="
      mb-4 flex flex-col flex-wrap gap-4
      sm:flex-row sm:items-center sm:justify-between
    "
  >
    <h1 class="text-xl font-bold text-neutral-800">
      {{ $t('sidebar.tripManagement') }}
    </h1>

    <div class="flex flex-wrap items-center gap-2">
      <TripsExport v-if="isAdminDashboard" />

      <button
        v-if="hasAddPermission && isCompanyDashboard"
        type="button"
        class="btn-secondary shrink-0"
        @click="isBulkImportPopupOpen = true"
      >
        <IconDocumentUpload class="size-4" />
        {{ $t('button.importTrips') }}
      </button>

      <BaseLink
        v-if="hasAddPermission"
        :to="createPath"
        class="
          flex items-center justify-center gap-2 rounded-full bg-neutral-30 p-4 text-neutral-10
          hover:bg-neutral-200
        "
      >
        {{ $t('button.createTrip') }}
        <IconPlus class="size-4" />
      </BaseLink>

      <TripsRequestsButton v-if="isAdminDashboard" />
    </div>
  </div>

  <BulkImportTripsPopup v-model="isBulkImportPopupOpen" />

  <div
    v-if="!isTripsOnMap"
    class="mb-6 flex flex-wrap gap-4"
  >
    <RouterLink
      :to="{ name: 'AllTrips' }"
      class="tab-primary-outline shrink-0"
      :class="{ active: activeTab === TripsTab.List }"
      @click="resetFilters"
    >
      <IconContracts class="size-4" />
      {{ $t('sidebar.contractTrips') }}
    </RouterLink>

    <RouterLink
      v-if="isAdminDashboard"
      :to="{ name: 'ExternalTrips' }"
      class="tab-primary-outline shrink-0"
      :class="{ active: activeTab === TripsTab.External }"
      @click="resetFilters"
    >
      <IconTruck class="size-4" />
      {{ $t('sidebar.externalTrips') }}
    </RouterLink>
  </div>
</template>

<script setup lang="ts">
  import TripsExport from './TripsExport.vue'
  import TripsRequestsButton from './TripsRequestsButton.vue'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { Permission, Trip } from '@/types/response/permission-group'
  import { useApp } from '@/composables/useApp'
  import { useTripStore } from '@/stores/admin-dashboard/trip'

  const BulkImportTripsPopup = defineAsyncComponent(() => import('./bulk-import/BulkImportTripsPopup.vue'))

  enum TripsTab {
    List,
    External,
  }

  const { isAdminDashboard, isCompanyDashboard } = useApp()
  const route = useRoute()

  const tripStore = useTripStore()

  const isBulkImportPopupOpen = ref(false)
  const isExternalTrips = computed(() => route.path.includes('external'))
  const isTripsOnMap = computed(() => route.path.includes('map'))

  const activeTab = computed(() => {
    if (isExternalTrips.value) return TripsTab.External

    return TripsTab.List
  })

  const createPath = computed(() => {
    if (isExternalTrips.value) return { name: 'CreateExternalTrip' }

    return { name: 'CreateTrip' }
  })

  const { hasPermission: hasAddPermission } = usePermission(Permission.Trip, Trip.Create)

  function resetFilters() {
    tripStore.resetFilters()
  }
</script>

<style scoped>

</style>
