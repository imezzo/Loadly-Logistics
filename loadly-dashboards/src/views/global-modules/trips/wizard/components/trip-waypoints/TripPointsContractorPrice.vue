<template>
  <div
    v-if="showContractorPrice"
    class="mb-4"
  >
    <label class="mb-1 inline-block">
      {{ $t('form.contractorPrice') }}
    </label>
    <BaseInput
      :model-value="trip.contractorPrice ?? ''"
      type="number"
      min="0"
      :disabled="isContractorPriceDisabled"
      @update:model-value="setContractorPrice"
    />
  </div>
</template>

<script setup lang="ts">
  import { DispatchingMode } from '@/types/admin-dashboard/response/trip/trip'
  import { useContractDestinationConsumption } from '../../composables/useContractDestinationConsumption'
  import { useTripContractorPriceCalculation } from '../../composables/useTripContractorPriceCalculation'
  import { useTripWizard } from '../../composables/useTripWizard'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { ConsumptionType } from '@/types/request/contract/consumption-types.enums'
  import { useApp } from '@/composables/useApp'

  const contractorPriceConsumptionTypes = [
    ConsumptionType.CargoWeight,
    ConsumptionType.DestinationFixedPrice,
    ConsumptionType.SpotPricing,
  ] as const

  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)

  const { contractConsumptionType } = useContractDestinationConsumption()
  useTripContractorPriceCalculation()
  const { isFormDisabledWithCompletedStatus, isExternalTripMode } = useTripWizard()
  const { isCompanyDashboard } = useApp()

  const consumptionType = computed(() => {
    return contractConsumptionType.value ?? trip.value.contract?.consumptionType ?? null
  })

  const isContractorsDispatchingMode = computed(() => {
    return trip.value.dispatchingMode === DispatchingMode.Contractors
  })

  const isExternalContractorsMode = computed(() => {
    return isExternalTripMode.value && isContractorsDispatchingMode.value
  })

  const showContractorPrice = computed(() => {
    if (isCompanyDashboard) return false

    return isContractorsDispatchingMode.value
      && (
        consumptionType.value != null
        && contractorPriceConsumptionTypes.includes(consumptionType.value)
      )
  })

  const isContractorPriceDisabled = computed(() => {
    if (isExternalContractorsMode.value) return false

    return isFormDisabledWithCompletedStatus.value
      || consumptionType.value !== ConsumptionType.SpotPricing
  })

  function setContractorPrice(value?: string | number | null): void {
    trip.value.contractorPrice = value == null ? '' : String(value)
  }
</script>

<style scoped>

</style>
