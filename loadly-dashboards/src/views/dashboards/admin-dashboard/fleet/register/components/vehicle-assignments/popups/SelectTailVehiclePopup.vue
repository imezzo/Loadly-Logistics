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
          {{ $t('content.chooseTailTitle') }}
        </h2>
        <p class="text-sm text-neutral-600">
          {{ $t('content.chooseTailDescription') }}
        </p>
      </div>

      <div class="min-w-0 flex-1 space-y-4">
        <div>
          <label class="mb-1 flex items-center gap-1.5 text-sm font-medium text-neutral-10">
            {{ $t('content.availableTailVehicles') }}
            <span class="text-error-600">*</span>
            <IconInfo
              variant="filled"
              class="size-5 text-neutral-40"
            />
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

        <div class="flex gap-3 pt-2">
          <button
            type="button"
            class="btn-secondary h-10 flex-1"
            @click="onBack"
          >
            <IconArrow class="size-5 rotate-180 fill-neutral-800!" />
            {{ $t('button.back') }}
          </button>

          <button
            type="button"
            class="btn-primary h-10 flex-1"
            :disabled="!selectedVehicle"
            @click="onAddTailVehicle"
          >
            {{ $t('content.addTailVehicle') }}
          </button>
        </div>
      </div>
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import { useVehicleStore } from '@/stores/admin-dashboard/vehicle'
import { VehicleStatus } from '@/types/admin-dashboard/request/vehicle/vehicle';
  import type { Vehicle } from '@/types/admin-dashboard/response/vehicle/vehicle'

  const props = defineProps<{
    contractorId?: number | null
    hasContractor?: boolean
    headVehicleId?: number | null
    tailVehicleIds?: number[]
    vehicleTypeId?: number | null
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })

  const emit = defineEmits<{
    select: [vehicle: { id: number; plateNo: string; plateLetters: string; displayText?: string; vehicleTypeName?: string }]
  }>()

  const vehicleStore = useVehicleStore()
  const selectedVehicleId = ref<string>('')

  const hasMoreVehicles = computed(() => {
    const vehiclePagination = vehicleStore.vehiclesList?.pagination
    if (!vehiclePagination) return false

    return vehiclePagination.page < vehiclePagination.pages
  })

  const excludedIds = computed(() => {
    const ids = new Set<number>()
    if (props.headVehicleId != null) {
      ids.add(props.headVehicleId)
    }
    ;(props.tailVehicleIds ?? []).forEach((id) => ids.add(id))

    return ids
  })

  const vehicles = computed(() => vehicleStore.vehiclesList?.vehicles ?? [])

  const vehicleOptions = computed(() =>
    vehicles.value
      .filter((v: Vehicle) => !excludedIds.value.has(v.id))
      .map((v: Vehicle) => ({
        value: String(v.id),
        label: (v.vehicleTypeName || v.displayText) + ' - ' + v.plateNo + ' ' + v.plateLetters,
      }))
  )

  const selectedVehicle = computed(() => {
    if (!selectedVehicleId.value) return null

    return vehicles.value.find((v: Vehicle) => String(v.id) === selectedVehicleId.value) ?? null
  })

  async function fetchVehicles(reset = true, search?: string): Promise<void> {
    if (!hasMoreVehicles.value && vehicleStore.vehiclesList?.pagination !== undefined && !reset) return

    const page = reset
      ? 1
      : (vehicleStore.vehiclesList?.pagination.page ?? 0) + 1

    const { success, data } = await vehicleStore.getVehicles({
      isAvailableTail: true,
      page,
      search: search ?? '',
      size: 100,
      status: VehicleStatus.Active,
      ...(props.contractorId && props.hasContractor ? { contractorIds: [String(props.contractorId)] } : {}),
      ...(props.hasContractor != undefined ? { hasContractor: props.hasContractor } : {}),
    })

    if (!success) return

    vehicleStore.vehiclesList = reset ? data : {
      vehicles: [...(vehicleStore.vehiclesList?.vehicles ?? []), ...data.vehicles],
      pagination: data.pagination,
    }
  }

  function onBack(): void {
    model.value = false
  }

  function onAddTailVehicle(): void {
    const vehicle = selectedVehicle.value
    if (!vehicle) return
    emit('select', {
      id: vehicle.id,
      plateNo: vehicle.plateNo,
      plateLetters: vehicle.plateLetters,
      displayText: vehicle.displayText,
      vehicleTypeName: vehicle.vehicleTypeName,
    })
    selectedVehicleId.value = ''
    model.value = false
  }

  watch(model, (isOpen) => {
    if (isOpen) {
      fetchVehicles(true)
      selectedVehicleId.value = ''
    }
  }, { immediate: true })
</script>

<style scoped>

</style>
