<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <ListHeader
      :active-tab="activeTab"
      @import-click="isImportPopupOpen = true"
      @set-tab="activeTab = $event"
    />

    <div
      v-if="activeTab === 'list'"
      class="space-y-6"
    >
      <DriverStatusCounters :status-counts="statusCounts" />

      <ListFilters />

      <DriversTable />
    </div>

    <div
      v-else
      class="py-12 text-center text-content-secondary"
    >
      {{ $t('content.noData') }}
    </div>

    <ImportDriversPopup
      v-model="isImportPopupOpen"
      @refresh="onImportRefresh"
    />
  </div>
</template>

<script setup lang="ts">
  import { useList } from './composables/useList'
  import ListHeader from './components/ListHeader.vue'
  import ListFilters from './components/ListFilters.vue'
  import DriversTable from './components/DriversTable.vue'
  import DriverStatusCounters from './components/DriverStatusCounters.vue'
  import ImportDriversPopup from './components/ImportDriversPopup.vue'

  const { t } = useI18n()
  const route = useRoute()

  const breadcrumbItems = [
    { label: t('title.drivers'), to: '/drivers' },
    { label: t('title.driverList') },
  ]

  const {
    fetchDrivers,
    resetFilters,
    statusCounts,
  } = useList()

  const activeTab = ref<'list' | 'map'>('list')
  const isImportPopupOpen = ref(false)

  function onImportRefresh(): void {
    fetchDrivers()
  }

  onMounted(() => {
    fetchDrivers()
  })

  onUnmounted(() => {
    if (!route.path.includes('/driver')) {
      resetFilters()
    }
  })
</script>

<style scoped>

</style>
