<template>
  <BasePopup
    v-model="model"
    content-class="max-w-3/4! md:max-w-200! w-full"
  >
    <div class="flex w-full flex-col gap-4 px-6 py-4">
      <PopupChooseDriverHeader
        v-model:include-other-contract-vehicles="includeOtherContractVehicles"
        v-model:search-keyword="searchKeyword"
        v-model:vehicle-types="vehicleTypes"
      />

      <div
        class="
          grid max-h-96 min-h-48 grid-cols-1 gap-4 overflow-y-auto px-px py-2
          md:grid-cols-2
        "
        @scroll="onScroll"
      >
        <PopupChooseDriverCard
          v-for="item in vehiclesList?.vehicles"
          :key="item.id"
          :item="item"
          @choose="onChooseCar"
        />
      </div>

      <p
        v-if="!vehiclesList?.vehicles.length"
        class="py-8 text-center text-sm text-content-secondary"
      >
        {{ $t('content.noData') }}
      </p>

      <button
        type="button"
        class="btn-secondary mx-auto w-1/4"
        @click="model = false"
      >
        {{ $t('button.cancel') }}
      </button>
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import PopupChooseDriverCard from './popup-choose-driver/PopupChooseDriverCard.vue'
  import PopupChooseDriverHeader from './popup-choose-driver/PopupChooseDriverHeader.vue'
  import { useVehicleStore } from '@/stores/admin-dashboard/vehicle'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import type { Vehicle } from '@/types/admin-dashboard/response/vehicle/vehicle'
  import { DispatchingMode } from '@/types/admin-dashboard/response/trip/trip'
  import { createScrollToEndHandler } from '@/composables/ui/useScrollToEnd'
  import type { VehiclesListResponse } from '@/types/admin-dashboard/response/vehicle/vehicle'
  import { useDebounceFn } from '@vueuse/core'
  import { useLookups } from '@/composables/useLookups'

  const model = defineModel<boolean>('modelValue', { required: true })

  const emit = defineEmits<{
    choose: [item: Vehicle]
  }>()

  const vehicleStore = useVehicleStore()
  const { vehiclesList } = storeToRefs(vehicleStore)
  const useTrip = useTripStore()
  const { trip } = storeToRefs(useTrip)

  const { contractVehicleTypes } = useLookups()

  const searchKeyword = ref('')
  const includeOtherContractVehicles = ref(false)
  const vehicleTypes = ref<string[]>(contractVehicleTypes.value?.items?.map(item => item.value) ?? [])
  const isLoading = ref(false)

  const onScroll = createScrollToEndHandler(() => {
    const pagination = vehiclesList.value?.pagination
    if (pagination?.pages === pagination?.page) return
    fetchVehicles((pagination?.page ?? 0) + 1, true)
  })

  function onChooseCar(item: Vehicle) {
    emit('choose', item)
    model.value = false
  }

  function assignFetchVehicles(data: VehiclesListResponse, append = false): void {
    vehicleStore.vehiclesList = append ? {
      ...vehicleStore.vehiclesList,
      vehicles: [...(vehicleStore.vehiclesList?.vehicles ?? []), ...data.vehicles],
      pagination: data.pagination,
    } : data
  }

  async function fetchVehicles(page = 1, append = false): Promise<void> {
    if (isLoading.value) return
    isLoading.value = true
    const vehicleTypeIds = trip.value.vehicleTypeId ? [trip.value.vehicleTypeId] : vehicleTypes.value
    const { success, data } = await vehicleStore.getVehicles({
      page: page,
      size: 20,
      pickVehiclesOrTheirHeads: true,
      ...(trip.value.contractId && !includeOtherContractVehicles.value
        ? { contractId: Number(trip.value.contractId) }
        : {}),
      hasDriver: true,
      contractorIds: trip.value.contractorId ? [trip.value.contractorId] : [],
      hasContractor: trip.value.dispatchingMode === DispatchingMode.Contractors,
      ...(searchKeyword.value ? { search: searchKeyword.value } : {}),
      ...(vehicleTypes.value.length ? { vehicleTypeIds } : {}),
    })
    isLoading.value = false

    if (success) {
      assignFetchVehicles(data, append)
    }
  }

  onMounted(fetchVehicles)

  const debouncedFetchVehicles = useDebounceFn(() => {
    fetchVehicles(1, false)
  }, 600)

  watch(searchKeyword, debouncedFetchVehicles)
  watch(vehicleTypes, () => fetchVehicles(1, false))
  watch(includeOtherContractVehicles, () => fetchVehicles(1, false))
</script>

<style scoped>

</style>
