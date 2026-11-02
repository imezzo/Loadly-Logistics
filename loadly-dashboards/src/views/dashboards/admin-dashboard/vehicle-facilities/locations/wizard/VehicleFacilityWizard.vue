<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <h1 class="mb-6 text-xl font-bold text-neutral-800">
      {{ pageTitle }}
    </h1>

    <NavigationTabs
      v-if="!isCreateMode"
      :tabs
      :active-tab="activeTab"
      @update:active-tab="activeTab = $event"
    />

    <VehicleFacilityForm v-if="activeTab === ActiveTab.LocationInformation" />

    <VehicleFacilityActivityLog
      v-if="!isCreateMode && activeTab === ActiveTab.ActivityLog"
      :visits="facility?.vehicleFacilityVisits ?? []"
    />
  </div>
</template>

<script setup lang="ts">
  import type { BreadcrumbItem } from '@/types/ui'
  import VehicleFacilityForm from './components/VehicleFacilityForm.vue'
  import { useVehicleFacilityForm } from './composables/useVehicleFacilityForm'
  import { useLookups } from '@/composables/useLookups'
  import { useVehicleFacilityStore } from '@/stores/admin-dashboard/vehicle-facility'

  const VehicleFacilityActivityLog = defineAsyncComponent(() => import('./components/VehicleFacilityActivityLog.vue'))

  enum ActiveTab {
    LocationInformation,
    ActivityLog,
  }

  const { t } = useI18n()
  const { getCommunicationMethods, getCompanies } = useLookups()
  const { fetchFacility, isEditMode, isViewMode, isCreateMode } = useVehicleFacilityForm()

  const vehicleFacilityStore = useVehicleFacilityStore()
  const { facility } = storeToRefs(vehicleFacilityStore)

  const activeTab = ref(ActiveTab.LocationInformation)

  const pageTitle = computed(() => {
    if (isViewMode.value) return t('title.viewVehicleLocations')
    if (isEditMode.value) return t('title.editVehicleLocations')

    return t('title.addVehicleLocations')
  })

  const breadcrumbItems = computed<BreadcrumbItem[]>(() => [
    { label: t('title.settings'), to: '/settings' },
    { label: pageTitle.value },
  ])

  const tabs = computed(() => [
    { title: t('content.locationInformation') },
    { title: t('content.activityLog') },
  ])


  onMounted(async () => {
    await Promise.all([getCommunicationMethods(), getCompanies(1)])
    await fetchFacility()

    if (isCreateMode.value) {
      facility.value = null
    }
  })

  onUnmounted(() => {
    facility.value = null
  })
</script>

<style scoped>

</style>
