<template>
  <BasePopup
    v-model="model"
    :show-close-button="false"
    content-class="max-w-3/4! md:max-w-200!"
  >
    <div
      class="flex w-full flex-wrap gap-6 px-6 py-4"
    >
      <BaseAlert
        :model-value="!!errorMessage"
        :message="errorMessage ?? ''"
        variant="error"
        dismissible
        class="w-full"
        @update:model-value="errorMessage = null"
      />

      <div class="min-w-0 flex-1">
        <h2 class="mb-2 text-2xl font-bold text-neutral-800">
          {{ isViewMode
            ? $t('content.theDriver')
            : $t('button.assignDriver') }}
        </h2>

        <p class="text-sm text-neutral-600">
          {{
            isViewMode
              ? $t('content.removeDriverToAssign')
              : $t('content.assignDriverByMissions')
          }}
        </p>
      </div>

      <div class="min-w-0 flex-2 space-y-4">
        <CurrentVehicleDriver
          v-if="isViewMode"
          :driver-name="vehicle.driverName"
          :disabled="isLoading"
          @remove="removeDriver"
        />

        <template v-else>
          <div>
            <label class="mb-1 flex items-center gap-1.5 text-sm font-medium text-neutral-10">
              {{ $t('content.availableDrivers') }}
              <span class="text-error-600">*</span>
              <IconInfo
                variant="filled"
                class="size-5 text-neutral-40"
              />
            </label>

            <BaseSelect
              v-model="selectedDriverId"
              :options="drivers?.items ?? []"
              searchable
              :placeholder="$t('content.driver')"
              wrapper-class="[&>.select-button]:h-11 [&>.select-button]:rounded-lg w-full"
              :on-scroll-to-end="hasMoreDrivers ? loadMoreDrivers : undefined"
              @search="getDrivers({ page: 1, search: $event, status: DriverStatus.AVAILABLE })"
            />
          </div>
        </template>

        <BaseActions
          container-class="mt-2 flex flex-wrap items-center gap-3"
          :prev-button-options="{
            label: t('button.back'),
            hasIcon: true,
            disabled: isLoading,
            class: 'btn-secondary flex-1',
          }"
          :next-button-options="assignButtonOptions"
          @prev="close"
          @next="submitDriver"
        />
      </div>
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import type { Vehicle } from '@/types/admin-dashboard/response/vehicle/vehicle'
  import { useVehicleStore } from '@/stores/admin-dashboard/vehicle'
  import { useLoading } from '@/composables/useLoading'
  import CurrentVehicleDriver from './change-vehicle-driver/CurrentVehicleDriver.vue'
  import { useLookups } from '@/composables/useLookups.ts'
  import { DriverStatus } from '@/types/admin-dashboard/driver.ts'

  enum DriverPopupMode {
    View,
    Assign,
  }

  const props = defineProps<{
    vehicle: Vehicle
  }>()

  const emit = defineEmits<{
    fetchVehicles: []
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })

  const { t } = useI18n()
  const { isLoading, setLoading } = useLoading()
  const { drivers, hasMoreDrivers, loadMoreDrivers, getDrivers } = useLookups()

  const vehicleStore = useVehicleStore()

  const mode = ref(DriverPopupMode.Assign)
  const selectedDriverId = ref(props.vehicle.driverId?.toString() ?? '')
  const errorMessage = ref<string | null>(null)

  const isViewMode = computed(() => mode.value === DriverPopupMode.View)

  const assignButtonOptions = computed(() => ({
    label: t('button.save'),
    disabled: isLoading.value,
    class: 'btn-primary flex-1',
  }))

  function close(): void {
    model.value = false
  }

  function removeDriver(): void {
    mode.value = DriverPopupMode.Assign
    selectedDriverId.value = ''
    getDrivers({ page: 1, status: DriverStatus.AVAILABLE })
  }

  function resetState(): void {
    mode.value = props.vehicle.driverId
      ? DriverPopupMode.View
      : DriverPopupMode.Assign
    selectedDriverId.value = props.vehicle.driverId?.toString() ?? ''
    errorMessage.value = null
  }

  async function submitDriver(): Promise<boolean> {
    if (isLoading.value) return false

    setLoading(true)
    errorMessage.value = null

    const driverId = selectedDriverId.value ? Number(selectedDriverId.value) : null
    const { success, message } = await vehicleStore.assignVehicleDriver(
      props.vehicle.id,
      driverId,
    )

    setLoading(false)

    if (!success) {
      errorMessage.value = message || t('error.somethingWentWrong')

      return false
    }

    emit('fetchVehicles')
    close()

    return true
  }

  onMounted(() => {
    resetState()
    if (mode.value === DriverPopupMode.Assign) {
      getDrivers({ page: 1, status: DriverStatus.AVAILABLE })
    }
  })
</script>

<style scoped>

</style>
