<template>
  <div class="flex items-center gap-4 rounded-2xl border border-neutral-200 bg-white p-6">
    <IconVehicle class="size-11" />

    <div class="min-w-0 flex-1">
      <h3 class="text-lg font-semibold text-neutral-10">
        {{ vehicleType.name }}
      </h3>

      <div class="mt-2 flex flex-wrap gap-x-5 gap-y-2 text-sm text-neutral-600">
        <p
          v-for="option in vehicleTypeOptions.filter(option => option.visible)"
          :key="option.label"
          class="text-neutral-10"
        >
          <span class="font-semibold">{{ option.label }}:</span> {{ option.value }}
        </p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { ContractType } from '@/types/admin-dashboard/response/contract/contract-details'
  import type { VehicleType } from '@/types/admin-dashboard/response/contract/contract-details'
  import { useContractWizard } from '../../composables/useContractWizard'
  import { ConsumptionType } from '@/types/request/contract/consumption-types.enums'

  const props = defineProps<{
    vehicleType: VehicleType
    typeId?: number
  }>()

  const { formData } = useContractWizard()

  const { t } = useI18n()

  const isOnDemand = computed(() => props.typeId === ContractType.OnDemand)
  const isIncluded = computed(() => props.typeId === ContractType.Included)
  const isSeasonal = computed(() => props.typeId === ContractType.Seasonal)
  const isExcluded = computed(() => props.typeId === ContractType.Excluded)

  const isKilometersConsumption = computed(() =>
    isOnDemand.value && Number(formData.value?.consumptionType) === ConsumptionType.Kilometers)
  const isCargoWeightConsumption = computed(() =>
    isOnDemand.value && Number(formData.value?.consumptionType) === ConsumptionType.CargoWeight)

  const vehicleTypeOptions = computed(() => [
    {
      label: t('form.howManyCars'),
      value: Number(props.vehicleType.carCount),
      visible: !isOnDemand.value || (isOnDemand.value && !formData.value.autoAssignMode),
    },
    {
      label: isOnDemand.value
        ? t('form.fixedCarRent')
        : t('form.monthlyFeesPerVehicle'),
      value: Number(props.vehicleType.monthlyRental),
      visible: !isOnDemand.value || isKilometersConsumption.value,
    },
    {
      label: t('form.consumptionRate'),
      value: Number(props.vehicleType.consumptionRate),
      visible: isSeasonal.value || isExcluded.value || isKilometersConsumption.value,
    },
    {
      label: isIncluded.value || isOnDemand.value
          ? t('form.kmFees')
          : t('form.extraTrip'),
      value: Number(props.vehicleType.extraTrip),
      visible: !isOnDemand.value || isKilometersConsumption.value,
    },
    {
      label: t('form.extraWorkingDaysFees'),
      value: Number(props.vehicleType.ExtraWorkingDayFees),
      visible: isIncluded.value || isExcluded.value,
    },
    {
      label: t('form.OverWeightPricePerTon'),
      value: Number(props.vehicleType.OverWeightPricePerTon),
      visible: isCargoWeightConsumption.value,
    },
    {
      label: t('form.contractorOverweightPricePerTon'),
      value: Number(props.vehicleType.contractorOverWeightPricePerTon ?? 0),
      visible: isCargoWeightConsumption.value,
    },
    {
      label: t('form.kilometersPerMonth'),
      value: Number(props.vehicleType.maxKms),
      visible: isIncluded.value,
    },
    {
      label: isSeasonal.value
        ? t('form.totalTripsPerContractPeriod')
        : t('form.tripsPerMonthPerVehicle'),
      value: Number(props.vehicleType.tripsPerMonth),
      visible: isSeasonal.value || isExcluded.value,
    },
    {
      label: t('form.minBillableWeightTons'),
      value: Number(props.vehicleType.minBillableWeightTons ?? 0),
      visible: isOnDemand.value && isCargoWeightConsumption.value,
    },
    {
      label: t('form.maxPayloadTons'),
      value: Number(props.vehicleType.maxPayloadTons ?? 0),
      visible: isOnDemand.value && isCargoWeightConsumption.value,
    },
  ])
</script>

<style scoped>

</style>
