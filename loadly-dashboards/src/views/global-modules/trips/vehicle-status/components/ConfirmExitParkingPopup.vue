<template>
  <BasePopup
    v-model="model"
    content-class="p-0.5!"
  >
    <div class="flex flex-col gap-3 px-6 py-4">
      <h2 class="text-xl font-bold text-neutral-10">
        {{ $t('content.confirmClearWaitingLoadingTitle') }}
      </h2>

      <p class="text-content-secondary">
        {{ $t('content.confirmClearWaitingLoadingDescription') }}
      </p>

      <BaseAlert
        v-model="hasErrorAlert"
        :message="errorMessage ?? ''"
        variant="error"
        dismissible
      />

      <div>
        <label class="mb-1 flex items-center font-medium text-neutral-10">
          {{ $t('content.reason') }}
        </label>

        <BaseTextarea
          v-model="reason"
          :placeholder="$t('content.reason')"
        />
      </div>

      <BaseActions
        container-class="mt-2 flex flex-wrap items-center justify-end gap-3"
        :prev-button-options="prevButtonOptions"
        :next-button-options="nextButtonOptions"
        @prev="model = false"
        @next="submit"
      />
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import type { Vehicle } from '@/types/admin-dashboard/response/vehicle/vehicle'
  import { useLoading } from '@/composables/useLoading'
  import { useVehicleStore } from '@/stores/admin-dashboard/vehicle'

  const props = defineProps<{
    vehicle: Vehicle
  }>()

  const emit = defineEmits<{
    fetchVehicles: []
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })

  const { t } = useI18n()
  const { isLoading, setLoading } = useLoading()
  const vehicleStore = useVehicleStore()

  const reason = ref('')
  const errorMessage = ref<string | null>(null)
  const hasErrorAlert = ref(false)

  const prevButtonOptions = computed(() => ({
    label: t('button.cancel'),
    disabled: isLoading.value,
  }))

  const nextButtonOptions = computed(() => ({
    label: t('button.confirm'),
    disabled: isLoading.value,
  }))

  async function submit(): Promise<void> {
    if (isLoading.value) return

    const vehicleFacilityId = Number(props.vehicle.assignedParkingFacility?.id)
    if (!vehicleFacilityId) {
      errorMessage.value = t('error.somethingWentWrong')
      hasErrorAlert.value = true

      return
    }

    setLoading(true)
    errorMessage.value = null

    const { success, message } = await vehicleStore.exitParkingFacilityVisit({
      eventDate: new Date().toISOString(),
      reason: reason.value.trim(),
      vehicleFacilityId,
      vehicleId: props.vehicle.id,
    })

    setLoading(false)

    if (success) {
      emit('fetchVehicles')
      model.value = false

      return
    }

    errorMessage.value = message || t('error.somethingWentWrong')
    hasErrorAlert.value = true
  }
</script>

<style scoped>

</style>
