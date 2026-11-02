<template>
  <BasePopup
    v-model="model"
    :show-close-button="false"
    content-class="max-w-3/4! md:max-w-200!"
  >
    <div class="w-full min-w-0 space-y-6 p-6">
      <h2 class="text-2xl font-bold text-neutral-800">
        {{ $t('content.assignVehicle') }}
      </h2>

      <div
        v-if="selectedVehicle"
        class="flex items-start gap-3 rounded-lg border border-neutral-200 bg-white p-4"
      >
        <BaseVehiclePlate
          :plate-letters="selectedVehicle.plateLetters"
          :plate-no="selectedVehicle.plateNo"
          class="max-w-fit"
        />

        <div class="flex flex-col gap-1">
          <p class="text-xs text-neutral-600">
            {{ $t('content.tail') }}: {{ selectedTailLabel || $t('content.notAvailable') }}
          </p>

          <p class="text-xs text-neutral-600">
            {{ $t('content.currentDriver') }}:
            {{ selectedVehicle.driverName || $t('content.notAvailable') }}
          </p>
        </div>

        <button
          class="btn-action-error ms-auto"
          @click="selectedVehicleId = ''"
        >
          <IconTrash class="size-4" />
        </button>
      </div>

      <div>
        <label class="mb-1.5 inline-block text-sm font-medium text-neutral-10">
          {{ $t('content.availableVehicles') }}
          <span class="text-error-600">*</span>
        </label>

        <BaseSelect
          v-model="selectedVehicleId"
          :options="vehicleOptions"
          searchable
          :placeholder="$t('content.searchForVehicle')"
          :on-scroll-to-end="() => fetchVehicles(false)"
          @search="fetchVehicles(true, $event)"
          @focus="search ? fetchVehicles(true, '') : undefined"
        />
        <span
          v-if="isLoading"
          class="loader"
        />
      </div>

      <BaseActions
        container-class="flex flex-wrap items-center gap-3"
        :prev-button-options="{
          label: $t('button.cancel'),
          class: 'btn-secondary',
        }"
        :next-button-options="{
          label: $t('button.assignVehicle'),
          disabled: !selectedVehicle,
          class: 'btn-primary',
        }"
        @prev="model = false"
        @next="confirm"
      />
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import { useVehicleStore } from '@/stores/admin-dashboard/vehicle'
  import type { Vehicle } from '@/types/admin-dashboard/response/vehicle/vehicle'
  import type { Pagination } from '@/types/response/pagination'

  const props = defineProps<{
    excludeIds?: number[]
    vehicleTypeId: number
  }>()

  const emit = defineEmits<{
    select: [vehicle: Vehicle]
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })

  const vehicleStore = useVehicleStore()

  const isLoading = ref(false)
  const search = ref('')
  const selectedVehicleId = ref('')
  const vehicles = ref<Vehicle[]>([])
  const pagination = ref<Pagination | null>(null)

  const excludedIds = computed(() => new Set(props.excludeIds ?? []))

  const vehicleOptions = computed(() =>
    vehicles.value
      .filter((vehicle) => !excludedIds.value.has(vehicle.id))
      .map((vehicle) => ({
        value: String(vehicle.id),
        label: `${vehicle.plateNo} ${vehicle.plateLetters}`.trim() || vehicle.displayText,
      })),
  )

  const selectedVehicle = computed(() => {
    if (!selectedVehicleId.value) return null

    return vehicles.value.find((vehicle) => String(vehicle.id) === selectedVehicleId.value) ?? null
  })

  const selectedTailLabel = computed(() => {
    const vehicle = selectedVehicle.value
    if (!vehicle) return ''

    return vehicle.tailDisplayText
      || [vehicle.tail?.plateNo, vehicle.tail?.plateLetters].filter(Boolean).join(' - ')
  })

  const hasMoreVehicles = computed(() => {
    if (!pagination.value) return false

    return pagination.value.page < pagination.value.pages
  })

  async function fetchVehicles(reset = true, searchKeyword?: string): Promise<void> {
    if (!props.vehicleTypeId || (!reset && !hasMoreVehicles.value)) return
    search.value = searchKeyword ?? ''

    isLoading.value = true
    const { success, data } = await vehicleStore.getVehicles({
      page: reset ? 1 : (pagination.value?.page ?? 0) + 1,
      search: searchKeyword ?? '',
      size: 100,
      vehicleTypeIds: [String(props.vehicleTypeId)],
      pickVehiclesOrTheirHeads: true,
      hasDriver: true,
      isAssignedToContract: false,
    })

    isLoading.value = false
    if (!success || !data) return

    vehicles.value = reset
      ? data.vehicles ?? []
      : [...vehicles.value, ...(data.vehicles ?? [])]
    pagination.value = data.pagination
  }

  function confirm(): void {
    if (!selectedVehicle.value) return

    emit('select', selectedVehicle.value)
    selectedVehicleId.value = ''
    model.value = false
  }

  watch(model, (isOpen) => {
    if (!isOpen) {
      selectedVehicleId.value = ''
      vehicles.value = []
      pagination.value = null

      return
    }

    fetchVehicles(true)
  })
</script>

<style scoped>

</style>
