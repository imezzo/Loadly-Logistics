<template>
  <div class="flex flex-wrap items-start gap-4 rounded-lg border border-neutral-200 bg-white p-4">
    <IconVehicle class="mt-0.5 size-8 shrink-0 text-neutral-600" />

    <p class="min-w-0 flex-1 text-sm font-semibold text-neutral-10">
      {{ vehicleTypeName }}
    </p>

    <button
      v-if="!isFormDisabled"
      type="button"
      class="btn-action-error"
      :aria-label="$t('button.remove')"
      @click="emit('remove')"
    >
      <IconTrash class="size-4" />
    </button>

    <div class="w-full shrink-0">
      <div class="grid grid-cols-3 gap-4">
        <div v-if="hasFixedCarRent">
          <label class="mb-1 block text-xs font-medium text-neutral-600">
            {{ $t('form.fixedCarRent') }}
          </label>
          <BaseInput
            :model-value="config.monthlyRental ?? ''"
            type="number"
            min="0"
            @update:model-value="($event) => emit('updateConfig', { field: 'monthlyRental', value: $event })"
          />
        </div>

        <div v-if="!isOnDemand || (isOnDemand && !formData?.autoAssignMode)">
          <label class="mb-1 block text-xs font-medium text-neutral-600">
            {{ $t('form.howManyCars') }}
          </label>
          <BaseInput
            :model-value="config.carCount ?? ''"
            type="number"
            min="0"
            :max="vehicleCount"
            :disabled="isFormDisabled"
            @update:model-value="updateCarCount"
          />

          <BaseValidation :message="carCountError" />
        </div>

        <div v-if="hasConsumptionRate">
          <label class="mb-1 block text-xs font-medium text-neutral-600">
            {{ $t('form.consumptionRate') }}
          </label>
          <BaseInput
            :model-value="config.consumptionRate ?? ''"
            type="number"
            min="0"
            @update:model-value="($event) => emit('updateConfig', { field: 'consumptionRate', value: $event })"
          />
        </div>

        <div v-if="hasExtraFees">
          <label class="mb-1 block text-xs font-medium text-neutral-600">
            {{ isIncluded || isOnDemand
              ? $t('form.kmFees')
              : $t('form.extraTrip') }}
          </label>
          <BaseInput
            :model-value="config.extraTrip ?? ''"
            type="number"
            min="0"
            @update:model-value="($event) => emit('updateConfig', { field: 'extraTrip', value: $event })"
          />
        </div>

        <div v-if="isIncluded || isExcluded">
          <label class="mb-1 block text-xs font-medium text-neutral-600">
            {{ $t('form.extraWorkingDaysFees') }}
          </label>
          <BaseInput
            :model-value="config.ExtraWorkingDayFees ?? ''"
            type="number"
            min="0"
            @update:model-value="($event) => emit('updateConfig', { field: 'ExtraWorkingDayFees', value: $event })"
          />
        </div>

        <div v-if="isCargo">
          <label class="mb-1 block text-xs font-medium text-neutral-600">
            {{ $t('form.OverWeightPricePerTon') }}
          </label>
          <BaseInput
            :model-value="config.OverWeightPricePerTon ?? ''"
            type="number"
            min="0"
            @update:model-value="($event) => emit('updateConfig', { field: 'OverWeightPricePerTon', value: $event })"
          />
        </div>

        <template v-if="!isOnDemand">
          <div>
            <label class="mb-1 block text-xs font-medium text-neutral-600">
              {{ $t('form.monthlyFeesPerVehicle') }}
            </label>
            <BaseInput
              :model-value="config.monthlyRental ?? ''"
              type="number"
              min="0"
              @update:model-value="($event) => emit('updateConfig', { field: 'monthlyRental', value: $event })"
            />
          </div>

          <div>
            <label class="mb-1 block text-xs font-medium text-neutral-600">
              {{ isIncluded
                ? $t('form.kilometersPerMonth')
                : isSeasonal
                  ? $t('form.totalTripsPerContractPeriod')
                  : $t('form.tripsPerMonthPerVehicle')
              }}
            </label>

            <BaseInput
              v-if="isExcluded || isSeasonal"
              :model-value="config.tripsPerMonth ?? ''"
              type="number"
              min="0"
              :disabled="isFormDisabled"
              @update:model-value="($event) => emit('updateConfig', { field: 'tripsPerMonth', value: $event })"
            />

            <BaseInput
              v-else-if="isIncluded"
              :model-value="config.maxKms ?? ''"
              type="number"
              min="0"
              :disabled="isFormDisabled"
              @update:model-value="($event) => emit('updateConfig', { field: 'maxKms', value: $event })"
            />
          </div>
        </template>

        <template v-if="isCargo">
          <div>
            <label class="mb-1 block text-xs font-medium text-neutral-600">
              {{ $t('form.minBillableWeightTons') }}
            </label>
            <BaseInput
              :model-value="config.minBillableWeightTons ?? ''"
              type="number"
              min="0"
              :disabled="isFormDisabled"
              @update:model-value="($event) => emit('updateConfig', { field: 'minBillableWeightTons', value: $event })"
            />
          </div>

          <div>
            <label class="mb-1 block text-xs font-medium text-neutral-600">
              {{ $t('form.maxPayloadTons') }}
            </label>
            <BaseInput
              :model-value="config.maxPayloadTons ?? ''"
              type="number"
              min="0"
              :disabled="isFormDisabled"
              @update:model-value="($event) => emit('updateConfig', { field: 'maxPayloadTons', value: $event })"
            />
          </div>

          <div>
            <label class="mb-1 block text-xs font-medium text-neutral-600">
              {{ $t('form.contractorOverweightPricePerTon') }}
            </label>
            <BaseInput
              :model-value="config.contractorOverWeightPricePerTon ?? ''"
              type="number"
              min="0"
              :disabled="isFormDisabled"
              @update:model-value="($event) => emit('updateConfig', { field: 'contractorOverWeightPricePerTon', value: $event })"
            />
          </div>
        </template>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { ContractType } from '@/types/admin-dashboard/response/contract/contract-details'
  import { useContractWizard } from '../../../composables/useContractWizard'
  import { ConsumptionType } from '@/types/request/contract/consumption-types.enums'

  interface VehicleConfig {
    carCount?: number
    consumptionRate?: number
    extraTrip?: number
    ExtraWorkingDayFees?: number
    OverWeightPricePerTon?: number
    contractorOverWeightPricePerTon?: number
    monthlyRental?: number
    tripsPerMonth?: number
    maxKms?: number
    minBillableWeightTons?: number
    maxPayloadTons?: number
  }

  const props = defineProps<{
    vehicleTypeId: number
    vehicleTypeName: string
    config: VehicleConfig
    vehicleConfigs: Record<number, VehicleConfig>
    typeId?: number
  }>()

  const emit = defineEmits<{
    remove: []
    updateConfig: [payload: { field: string, value?: string | number | null }]
  }>()

  const { isFormDisabled, formData } = useContractWizard()
  const { t } = useI18n()

  const isOnDemand = computed(() => props.typeId === ContractType.OnDemand)
  const isIncluded = computed(() => props.typeId === ContractType.Included)
  const isExcluded = computed(() => props.typeId === ContractType.Excluded)
  const isSeasonal = computed(() => props.typeId === ContractType.Seasonal)

  const isKilometersConsumption = computed(() => Number(formData.value?.consumptionType) === ConsumptionType.Kilometers)
  const isCargoWeight = computed(() => Number(formData.value?.consumptionType) === ConsumptionType.CargoWeight)

  const hasFixedCarRent = computed(() => isOnDemand.value && isKilometersConsumption.value)
  const hasConsumptionRate = computed(() =>
    (!isOnDemand.value && !isIncluded.value) || (isOnDemand.value && isKilometersConsumption.value))
  const hasExtraFees = computed(() => !isOnDemand.value || (isOnDemand.value && isKilometersConsumption.value))
  const isCargo = computed(() => isOnDemand.value && (isCargoWeight.value))

  const vehicleCount = computed<number>(() => Number(formData.value?.vehiclesCount ?? 0))
  const vehicleTypesQuantity = computed(() => {
    return Object.keys(props.vehicleConfigs)
      .filter((type: string) => Number(type) !== props.vehicleTypeId)
      .reduce((acc: number, type: string) => acc + (props.vehicleConfigs[Number(type)]?.carCount ?? 0), 0)
  })

  const carCountError = ref<string | null>(null)

  function updateCarCount(value?: string | number | null): void {
    if ((Number(value) + vehicleTypesQuantity.value) <= vehicleCount.value) {
      carCountError.value = null
      emit('updateConfig', { field: 'carCount', value })

      return
    }

    emit('updateConfig', { field: 'carCount', value: null })
    carCountError.value = t('validation.maxNum', { max: vehicleCount.value })
  }
</script>

<style scoped>

</style>
