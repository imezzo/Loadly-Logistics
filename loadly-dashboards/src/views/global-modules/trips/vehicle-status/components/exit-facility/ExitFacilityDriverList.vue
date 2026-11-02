<template>
  <div
    class="
      flex flex-col gap-3 border-t border-neutral-500 pt-3
      sm:flex-row sm:items-center
    "
  >
    <BaseInputSearch
      v-model="searchKeyword"
      :placeholder="$t('form.searchHere')"
      wrapper-class="
        flex h-11 min-w-0 flex-1 items-center rounded-lg border border-neutral-500
        bg-white px-3
      "
      @keyup.enter="fetchDrivers(1)"
    />

    <button
      type="button"
      class="btn-primary shrink-0 gap-2"
      :disabled="isDriversLoading"
      @click="fetchDrivers(1)"
    >
      {{ $t('button.search') }}
      <IconSearch class="size-4" />
    </button>
  </div>

  <div
    class="
      grid max-h-40 min-h-48 grid-cols-1 gap-4 overflow-y-auto px-px py-1
      md:grid-cols-2
    "
    @scroll="onScroll"
  >
    <ExitFacilityDriverCard
      v-for="driver in drivers"
      :key="driver.id"
      :driver="driver"
      :selected="model === driver.id"
      @choose="chooseDriver"
    />

    <p
      v-if="!isDriversLoading && !drivers.length"
      class="col-span-full py-8 text-center text-sm text-content-secondary"
    >
      {{ $t('content.noData') }}
    </p>
  </div>

  <BaseValidation
    v-if="error"
    :message="error"
  />
</template>

<script setup lang="ts">
  import type { Driver, DriversListResponse } from '@/types/admin-dashboard/response/driver/driver'
  import { DriverStatus } from '@/types/admin-dashboard/driver'
  import { VehicleUnitType } from '@/types/admin-dashboard/response/vehicle/vehicle.enums'
  import { useDriverStore } from '@/stores/admin-dashboard/driver'
  import { createScrollToEndHandler } from '@/composables/ui/useScrollToEnd'
  import ExitFacilityDriverCard from './ExitFacilityDriverCard.vue'

  const props = defineProps<{
    error?: string
    vehicleUnitType?: string | number
  }>()

  const emit = defineEmits<{
    clearError: []
  }>()

  const model = defineModel<string | null>({ required: true })

  const driverStore = useDriverStore()

  const searchKeyword = ref('')
  const driversList = ref<DriversListResponse | null>(null)
  const isDriversLoading = ref(false)

  const drivers = computed(() => driversList.value?.drivers ?? [])
  const isTailVehicle = computed(
    () => Number(props.vehicleUnitType) === VehicleUnitType.Tail,
  )

  const onScroll = createScrollToEndHandler(() => {
    const pagination = driversList.value?.pagination
    if (!pagination || pagination.page >= pagination.pages) return

    fetchDrivers(pagination.page + 1, true)
  })

  async function fetchDrivers(page = 1, append = false): Promise<void> {
    if (isDriversLoading.value) return

    isDriversLoading.value = true

    const { success, data } = await driverStore.getList({
      ...(isTailVehicle.value ? {} : { isAvailable: true }),
      pagination: { page, size: 20 },
      search: searchKeyword.value,
      status: 1, // active driver status as a user status
      workingStatuses: [DriverStatus.AVAILABLE],
    })

    isDriversLoading.value = false

    if (!success || !data) return

    driversList.value = append
      ? {
          ...data,
          drivers: [...(driversList.value?.drivers ?? []), ...data.drivers],
        }
      : data
  }

  function chooseDriver(driver: Driver): void {
    model.value = driver.id
    emit('clearError')
  }

  onMounted(() => {
    fetchDrivers(1)
  })
</script>

<style scoped>

</style>
