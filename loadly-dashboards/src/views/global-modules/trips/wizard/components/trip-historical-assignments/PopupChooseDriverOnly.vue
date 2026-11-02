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
          {{ $t('content.chooseDriverTitle') }}
        </h2>
        <p class="text-sm text-neutral-600">
          {{ $t('content.chooseDriverDescription') }}
        </p>
      </div>

      <div class="min-w-0 flex-1 space-y-4">
        <div>
          <label class="mb-1 flex items-center gap-1.5 text-sm font-medium text-neutral-10">
            {{ $t('content.availableDrivers') }}
            <span class="text-error-600">*</span>
          </label>

          <BaseSelect
            v-model="selectedDriverId"
            :options="driverOptions"
            searchable
            :placeholder="$t('content.searchByDriver')"
            wrapper-class="[&>.select-button]:h-11 [&>.select-button]:rounded-lg w-full"
            :on-scroll-to-end="loadMoreDrivers"
            @search="fetchDrivers(1, $event)"
          />
        </div>

        <div class="flex gap-3 pt-2">
          <button
            type="button"
            class="btn-secondary h-10 flex-1"
            @click="model = false"
          >
            <IconArrow class="size-5 rotate-180 fill-neutral-800!" />
            {{ $t('button.back') }}
          </button>
          <button
            type="button"
            class="btn-primary h-10 flex-1"
            :disabled="!selectedDriver"
            @click="onAssignDriver"
          >
            {{ $t('button.addDriver') }}
          </button>
        </div>
      </div>
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import { useDriverStore } from '@/stores/admin-dashboard/driver'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import type { Driver } from '@/types/admin-dashboard/response/driver/driver'
  import { DispatchingMode } from '@/types/admin-dashboard/response/trip/trip'

  export interface ChosenDriver {
    fullName: string
    id: number
    phone?: string
  }

  const emit = defineEmits<{
    select: [driver: ChosenDriver]
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })

  const driverStore = useDriverStore()
  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)

  const selectedDriverId = ref('')
  const currentPage = ref(1)
  const isLoadingMore = ref(false)
  const drivers = ref<Driver[]>([])

  const driverOptions = computed(() =>
    drivers.value.map((driver: Driver) => ({
      value: driver.id,
      label: driver.fullName || driver.userName || driver.email,
    })),
  )

  const selectedDriver = computed(() => {
    if (!selectedDriverId.value) return null

    return drivers.value.find((driver: Driver) => driver.id === selectedDriverId.value) ?? null
  })

  const hasMorePages = computed(() => {
    const pagination = driverStore.driversList?.pagination
    if (!pagination) return false

    return currentPage.value < pagination.pages
  })

  async function fetchDrivers(page = 1, search?: string): Promise<void> {
    const { success, data } = await driverStore.getList({
      hasContractor: trip.value.dispatchingMode === DispatchingMode.Contractors,
      pagination: { page, size: 100 },
      search: search ?? '',
      ...(trip.value.contractorId ? { contractorIds: [Number(trip.value.contractorId)] } : {}),
    })

    if (!success) return

    driverStore.driversList = data
    drivers.value = page === 1 ? data.drivers : [...drivers.value, ...data.drivers]
    currentPage.value = page
  }

  async function loadMoreDrivers(): Promise<void> {
    if (isLoadingMore.value || !hasMorePages.value) return
    isLoadingMore.value = true
    await fetchDrivers(currentPage.value + 1)
    isLoadingMore.value = false
  }

  function onAssignDriver(): void {
    const driver = selectedDriver.value
    if (!driver) return

    const fullName = driver.fullName || driver.userName || driver.email
    emit('select', {
      fullName,
      id: Number(driver.id),
      phone: driver.phone,
    })
    selectedDriverId.value = ''
    model.value = false
  }

  watch(model, (isOpen) => {
    if (!isOpen) return

    drivers.value = []
    currentPage.value = 1
    selectedDriverId.value = ''
    fetchDrivers(1)
  }, { immediate: true })
</script>

<style scoped>

</style>
