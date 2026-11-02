<template>
  <BasePopup
    v-model="model"
    content-class="p-0.5!"
  >
    <div class="flex flex-col gap-3 px-6 py-4">
      <h2 class="text-xl font-bold text-neutral-10">
        {{ $t('content.outVehicleAssignDriverTitle') }}
      </h2>

      <BaseAlert
        v-model="hasErrorAlert"
        :message="errorMessage ?? ''"
        variant="error"
        dismissible
      />

      <div
        class="
          grid grid-cols-1 gap-4
          md:grid-cols-2
        "
      >
        <div>
          <label class="mb-1 flex items-center font-medium text-neutral-10">
            {{ $t('form.whenExitLocation') }}
            <span class="ms-1 text-error-600">*</span>
          </label>

          <BaseInput
            v-model="eventDate"
            type="datetime-local"
            :placeholder="$t('form.datePlaceholder')"
            :wrapper-class="errors.eventDate ? 'input-error' : ''"
          />

          <BaseValidation
            v-if="errors.eventDate"
            :message="errors.eventDate"
          />
        </div>

        <div>
          <label class="mb-1 flex items-center font-medium text-neutral-10">
            {{ $t('form.chooseSector') }}
          </label>

          <BaseSelect
            v-model="sectorId"
            :options="sectorOptions"
            searchable
            :placeholder="$t('form.chooseSector')"
            :on-scroll-to-end="hasMoreSectors ? loadMoreSectors : undefined"
            :on-search="(search) => getSectors(1, search)"
          />

          <BaseValidation
            v-if="errors.sectorId"
            :message="errors.sectorId"
          />
        </div>
      </div>

      <BaseCheckbox
        v-if="showIncludeTailCheckbox"
        v-model="includeHeadOrTail"
        :label="includedTailLabel"
      />

      <ExitFacilityDriverList
        v-model="selectedDriverId"
        :error="errors.driverId"
        :vehicle-unit-type="vehicle.vehicleUnitType"
        @clear-error="errors.driverId = ''"
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
        :prev-button-options="{ label: t('button.cancel') }"
        :next-button-options="confirmButtonOptions"
        @prev="close"
        @next="submit"
      />
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import type { SelectOption } from '@/types/ui'
  import type { Vehicle } from '@/types/admin-dashboard/response/vehicle/vehicle'
  import { useLookups } from '@/composables/useLookups'
  import { useLoading } from '@/composables/useLoading'
  import { useVehicleStore } from '@/stores/admin-dashboard/vehicle'
  import { formatDateToISO } from '@/utils/date'
  import ExitFacilityDriverList from './exit-facility/ExitFacilityDriverList.vue'

  const props = defineProps<{
    vehicle: Vehicle
  }>()

  const emit = defineEmits<{
    fetchVehicles: []
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })

  const { t } = useI18n()
  const vehicleStore = useVehicleStore()
  const { isLoading, setLoading } = useLoading()
  const {
    getSectors,
    hasMoreSectors,
    loadMoreSectors,
    sectors,
  } = useLookups()

  const eventDate = ref('')
  const sectorId = ref(props.vehicle.sector?.id ? String(props.vehicle.sector.id) : '')
  const reason = ref('')
  const selectedDriverId = ref<string | null>(null)
  const errorMessage = ref<string | null>(null)
  const hasErrorAlert = ref(false)
  const errors = ref<Record<string, string>>({})

  const includeHeadOrTail = ref(true)

  const showIncludeTailCheckbox = computed(() =>
    Number(props.vehicle.vehicleUnitType) === 1 && Boolean(props.vehicle.tailVehicleTypeId),
  )

  const includedTailLabel = computed(() => {
    const plate = [props.vehicle.tail.plateNo, props.vehicle.tail.plateLetters]
      .filter(Boolean)
      .join(' ')
    const details = [plate, props.vehicle.tail.vehicleType].filter(Boolean).join(' - ')

    return details
      ? `${t('form.includedTail')} (${details})`
      : t('form.includedTail')
  })

  const confirmButtonOptions = computed(() => ({
    label: t('button.submit'),
    disabled: isLoading.value,
  }))

  const sectorOptions = computed<SelectOption[]>(() => [
    { label: t('form.chooseSector'), value: '' },
    ...(sectors.value ?? []),
  ])

  function validate(): boolean {
    const nextErrors: Record<string, string> = {}

    if (!eventDate.value) {
      nextErrors.eventDate = t('validation.required', { field: t('form.whenGoToLocation') })
    }

    if (!selectedDriverId.value) {
      nextErrors.driverId = t('validation.required', { field: t('form.selectDriver') })
    }

    if (!sectorId.value) {
      nextErrors.sectorId = t('validation.required', { field: t('form.sector') })
    }

    if (!props.vehicle.assignedParkingFacility?.id && !props.vehicle.currentServiceFacility?.id) {
      errorMessage.value = t('error.somethingWentWrong')
      hasErrorAlert.value = true

      return false
    }

    errors.value = nextErrors

    return !Object.keys(nextErrors).length
  }

  function close(): void {
    model.value = false
  }

  async function submit(): Promise<void> {
    if (isLoading.value || !validate()) return

    setLoading(true)
    errorMessage.value = null

    const vehicleFacilityId = (
      Number(props.vehicle.assignedParkingFacility?.id)
      || Number(props.vehicle.currentServiceFacility?.id)
    )

    const { success, message } = await vehicleStore.exitFacilityVisit({
      driverId: Number(selectedDriverId.value),
      eventDate: formatDateToISO(String(eventDate.value), true),
      includeHeadOrTail: showIncludeTailCheckbox.value
        ? includeHeadOrTail.value
        : true,
      reason: reason.value.trim(),
      sectorId: Number(sectorId.value),
      vehicleFacilityId,
      vehicleId: props.vehicle.id,
    })

    setLoading(false)

    if (success) {
      emit('fetchVehicles')
      close()

      return
    }

    errorMessage.value = message || t('error.somethingWentWrong')
    hasErrorAlert.value = true
  }

  onMounted(() => {
    getSectors(1)
  })
</script>

<style scoped>

</style>
