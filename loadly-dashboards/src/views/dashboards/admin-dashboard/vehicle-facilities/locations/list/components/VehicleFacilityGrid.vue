<template>
  <BaseInputSearch
    v-model="search"
    :placeholder="$t('form.searchByVehicleLocation')"
    wrapper-class="
      mb-4 flex h-11 w-1/4 items-center rounded-lg border border-neutral-300 bg-white px-3
    "
  />

  <div
    v-if="facilities.length"
    class="
      grid grid-cols-1 gap-4
      lg:grid-cols-2
    "
  >
    <VehicleFacilityCard
      v-for="facility in facilities"
      :key="facility.id"
      :facility="facility"
      @delete="openDeletePopup"
    />
  </div>

  <p
    v-else
    class="py-12 text-center text-content-secondary"
  >
    {{ $t('content.noVehicleLocationsFound') }}
  </p>

  <BasePagination
    :pagination="pagination"
    @set-page="setPage"
    @next-page="nextPage"
    @prev-page="prevPage"
    @update-items-per-page="updateItemsPerPage"
  />

  <PopupDelete
    v-if="isDeletePopupOpen"
    v-model="isDeletePopupOpen"
    :title="$t('error.deleteVehicleLocation')"
    :message="$t('error.deleteVehicleLocationConfirmation', { name: selectedFacility?.name })"
    :is-loading="isDeleting"
    @confirm="deleteFacility"
  />
</template>

<script setup lang="ts">
  import type { VehicleFacility } from '@/types/admin-dashboard/response/vehicle-facility'
  import { useLoading } from '@/composables/useLoading'
  import { useVehicleFacilityStore } from '@/stores/admin-dashboard/vehicle-facility'
  import { useList } from '../composables/useList.ts'
  import VehicleFacilityCard from './VehicleFacilityCard.vue'

  const PopupDelete = defineAsyncComponent(() => import('@/components/popup/PopupDelete.vue'))

  const {
    facilities,
    fetchFacilities,
    nextPage,
    pagination,
    prevPage,
    search,
    setPage,
    updateItemsPerPage,
  } = useList()

  const vehicleFacilityStore = useVehicleFacilityStore()
  const { isLoading: isDeleting, setLoading: setDeleting } = useLoading()

  const isDeletePopupOpen = ref(false)
  const selectedFacility = ref<VehicleFacility | null>(null)

  function openDeletePopup(facility: VehicleFacility): void {
    selectedFacility.value = facility
    isDeletePopupOpen.value = true
  }

  async function deleteFacility(): Promise<void> {
    if (!selectedFacility.value) return

    setDeleting(true)
    const { success } = await vehicleFacilityStore.deleteFacility(selectedFacility.value.id)
    setDeleting(false)

    if (success) {
      isDeletePopupOpen.value = false
      selectedFacility.value = null
      fetchFacilities()
    }
  }

  watch(isDeletePopupOpen, (isOpen) => {
    if (!isOpen) selectedFacility.value = null
  })

  onMounted(fetchFacilities)
</script>

<style scoped>

</style>
