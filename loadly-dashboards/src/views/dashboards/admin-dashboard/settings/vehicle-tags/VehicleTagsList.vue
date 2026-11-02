<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <div
      class="
        mb-4 flex flex-col gap-4
        sm:flex-row sm:items-center sm:justify-between
      "
    >
      <h1 class="text-xl font-bold text-neutral-800">
        {{ $t('title.vehicleTags') }}
      </h1>

      <button
        type="button"
        class="btn-primary"
        @click="openWizard()"
      >
        {{ $t('button.addNewTag') }}

        <IconPlus />
      </button>
    </div>

    <BaseInputSearch
      v-model="search"
      :placeholder="$t('form.searchByTag')"
      wrapper-class="
        mb-4 flex h-11 w-full max-w-80 items-center rounded-lg border border-neutral-300
        bg-white px-3
      "
    />

    <VehicleTagsTable
      :vehicle-tags="vehicleTags"
      :pagination="pagination"
      :is-loading="isLoading"
      @set-page="setPage"
      @next-page="nextPage"
      @prev-page="prevPage"
      @update-items-per-page="updateItemsPerPage"
      @edit="openWizard"
    />

    <VehicleTagWizard
      v-if="isWizardPopupOpen"
      v-model="isWizardPopupOpen"
      :vehicle-tag="selectedVehicleTag"
    />
  </div>
</template>

<script setup lang="ts">
  import type { VehicleTag } from '@/types/admin-dashboard/response/vehicle/vehicle-tag'
  import { useList } from './composables/useList'
  import VehicleTagsTable from './components/VehicleTagsTable.vue'

  const VehicleTagWizard = defineAsyncComponent(() => import('./components/VehicleTagWizard.vue'))

  const { t } = useI18n()

  const breadcrumbItems = [
    { label: t('title.settings'), to: '/settings' },
    { label: t('title.vehicleTags') },
  ]

  const {
    fetchVehicleTags,
    isLoading,
    nextPage,
    pagination,
    prevPage,
    search,
    setPage,
    updateItemsPerPage,
    vehicleTags,
  } = useList()

  const isWizardPopupOpen = ref(false)
  const selectedVehicleTag = ref<VehicleTag | null>(null)

  function openWizard(vehicleTag?: VehicleTag): void {
    selectedVehicleTag.value = vehicleTag ?? null
    isWizardPopupOpen.value = true
  }

  watch(isWizardPopupOpen, (isOpen) => {
    if (!isOpen) {
      selectedVehicleTag.value = null
    }
  })

  onMounted(fetchVehicleTags)
</script>

<style scoped>

</style>
