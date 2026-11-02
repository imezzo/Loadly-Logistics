<template>
  <BasePopup
    v-model="model"
    :show-close-button="false"
    content-class="max-w-3/4! md:max-w-200!"
  >
    <div
      class="
        flex w-full flex-wrap gap-6 px-6 py-4
        md:flex-nowrap
      "
    >
      <div class="min-w-0 flex-1">
        <h2 class="mb-2 text-2xl font-bold text-neutral-800">
          {{ $t(copy.titleKey) }}
        </h2>
        <p class="text-sm text-neutral-600">
          {{ $t(copy.descriptionKey) }}
        </p>
      </div>

      <div class="min-w-0 flex-1 space-y-4">
        <div>
          <label class="mb-1 flex items-center gap-1.5 text-sm font-medium text-neutral-10">
            {{ $t(copy.labelKey) }}
            <span class="text-error-600">*</span>
          </label>

          <BaseSelect
            v-model="selectedVehicleId"
            :options="vehicleOptions"
            searchable
            :placeholder="$t('content.searchForVehicle')"
            wrapper-class="[&>.select-button]:h-11 [&>.select-button]:rounded-lg w-full"
            :on-scroll-to-end="() => fetchVehicles(false)"
            @search="fetchVehicles(true, $event)"
          />
        </div>

        <BaseActions
          container-class="mt-2 flex flex-wrap items-center gap-3"
          :prev-button-options="{
            label: $t('button.back'),
            class: 'btn-secondary flex-1',
          }"
          :next-button-options="{
            label: $t(copy.confirmKey),
            disabled: !selectedVehicle,
            class: 'btn-primary flex-1',
          }"
          @prev="model = false"
          @next="onConfirm"
        />
      </div>
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { useVehicleStore } from '@/stores/admin-dashboard/vehicle'
  import type { Vehicle } from '@/types/admin-dashboard/response/vehicle/vehicle'
  import { DispatchingMode } from '@/types/admin-dashboard/response/trip/trip'
  import { VehicleUnitType } from '@/types/admin-dashboard/response/vehicle/vehicle.enums'

  const COPY = {
    head: {
      confirmKey: 'content.addHeadVehicle',
      descriptionKey: 'content.chooseHeadRigidDescription',
      labelKey: 'content.availableHeadRigidVehicles',
      titleKey: 'content.chooseHeadRigidTitle',
    },
    tail: {
      confirmKey: 'content.addTailVehicle',
      descriptionKey: 'content.chooseTailDescription',
      labelKey: 'content.availableTailVehicles',
      titleKey: 'content.chooseTailTitle',
    },
  } as const

  const props = defineProps<{
    excludeIds?: number[]
    mode: 'head' | 'tail'
  }>()

  const emit = defineEmits<{
    select: [vehicle: Vehicle]
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })

  const tripStore = useTripStore()
  const vehicleStore = useVehicleStore()
  const { trip } = storeToRefs(tripStore)

  const selectedVehicleId = ref('')
  const isLoading = ref(false)

  const copy = computed(() => COPY[props.mode])

  const excludedIds = computed(() => new Set(props.excludeIds ?? []))

  const vehicles = computed(() => vehicleStore.vehiclesList?.vehicles ?? [])

  const vehicleOptions = computed(() =>
    vehicles.value
      .filter((vehicle: Vehicle) => !excludedIds.value.has(vehicle.id))
      .map((vehicle: Vehicle) => ({
        value: String(vehicle.id),
        label: `${vehicle.vehicleTypeName || vehicle.displayText} - ${vehicle.plateNo} ${vehicle.plateLetters}`,
      })),
  )

  const selectedVehicle = computed(() => {
    if (!selectedVehicleId.value) return null

    return vehicles.value.find((vehicle: Vehicle) => String(vehicle.id) === selectedVehicleId.value) ?? null
  })

  const hasMoreVehicles = computed(() => {
    const pagination = vehicleStore.vehiclesList?.pagination
    if (!pagination) return false

    return pagination.page < pagination.pages
  })

  async function fetchVehicles(reset = true, search?: string): Promise<void> {
    if (!reset && (!hasMoreVehicles.value || isLoading.value)) return

    isLoading.value = true

    const page = reset ? 1 : (vehicleStore.vehiclesList?.pagination.page ?? 0) + 1

    const { success, data } = await vehicleStore.getVehicles({
      page,
      search: search ?? '',
      size: 100,
      ...(props.mode === 'head'
        ? { pickVehiclesOrTheirHeads: false }
        : { VehicleUnitType: VehicleUnitType.Tail }),
      ...(trip.value.contractorId ? { contractorIds: [trip.value.contractorId] } : {}),
      ...(trip.value.dispatchingMode != null
        ? { hasContractor: trip.value.dispatchingMode === DispatchingMode.Contractors }
        : {}),
    })

    if (!success) return

    vehicleStore.vehiclesList = reset
      ? data
      : {
          vehicles: [...(vehicleStore.vehiclesList?.vehicles ?? []), ...data.vehicles],
          pagination: data.pagination,
        }

    isLoading.value = false
  }

  function onConfirm(): void {
    const vehicle = selectedVehicle.value
    if (!vehicle) return

    emit('select', vehicle)
    selectedVehicleId.value = ''
    model.value = false
  }

  watch(model, (isOpen) => {
    if (!isOpen) return

    selectedVehicleId.value = ''
    fetchVehicles(true)
  }, { immediate: true })
</script>

<style scoped>

</style>
