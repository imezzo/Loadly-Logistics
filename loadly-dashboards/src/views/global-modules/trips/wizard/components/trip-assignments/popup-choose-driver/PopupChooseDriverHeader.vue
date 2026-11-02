<template>
  <div class="flex shrink-0 flex-col gap-4">
    <header>
      <h2 class="mb-1 text-2xl font-bold text-neutral-800">
        {{ $t('content.chooseTheVehicle') }}
      </h2>
      <p class="text-sm text-neutral-600">
        {{ $t('content.chooseVehicleTypesDescription') }}
      </p>
    </header>

    <div
      v-if="canSelectOtherContractVehicles"
      class="
        flex h-11 items-center rounded-lg border border-neutral-300 bg-white px-4
        [&_label]:font-normal [&_label]:text-neutral-800 [&_label]:normal-case
      "
    >
      <BaseCheckbox
        v-model="includeOtherContractVehicles"
        :label="$t('content.selectVehiclesFromOtherContracts')"
      />
    </div>

    <div class="grid grid-cols-2 gap-3">
      <BaseInputSearch
        v-model="searchKeyword"
        wrapper-class="flex h-11 min-w-0 flex-1 items-center
        rounded-lg border border-neutral-500 bg-white px-3
        py-2 text-sm text-neutral-800 transition-colors
        focus-within:border-primary focus-within:ring-1 focus-within:ring-primary"
        :placeholder="$t('content.searchForVehicle')"
      />

      <BaseMultiSelect
        v-if="!isExternalTripMode"
        :model-value="vehicleTypes"
        :options="contractVehicleTypes?.items ?? []"
        :placeholder="$t('form.vehicleTypes')"
        @update:model-value="onUpdateVehicleTypes"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { useLookups } from '@/composables/useLookups'
  import { useContractStore } from '@/stores/admin-dashboard/contract'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { ConsumptionType } from '@/types/request/contract/consumption-types.enums'
  import { Permission, Trip } from '@/types/response/permission-group'
  import { useTripWizard } from '../../../composables/useTripWizard'

  const excludedConsumptionTypes = [ConsumptionType.Trips, ConsumptionType.Kilometers]

  const { trip } = storeToRefs(useTripStore())
  const { contractConsumptionType } = storeToRefs(useContractStore())

  const { isExternalTripMode } = useTripWizard()
  const { hasPermission: canPickOtherContractVehicle } = usePermission(
    Permission.Trip,
    Trip.PickOtherContractVehicle,
  )

  const searchKeyword = defineModel<string>('searchKeyword', { required: true })
  const vehicleTypes = defineModel<string[]>('vehicleTypes', { required: true })
  const includeOtherContractVehicles = defineModel<boolean>('includeOtherContractVehicles', { required: true })

  const canSelectOtherContractVehicles = computed(() => {
    if (!canPickOtherContractVehicle.value) return false

    const consumptionType = Number(
      contractConsumptionType.value ?? trip.value.contract?.consumptionType ?? 0
    )

    return !excludedConsumptionTypes.includes(consumptionType)
  })

  watch(canSelectOtherContractVehicles, (isVisible) => {
    if (!isVisible) includeOtherContractVehicles.value = false
  })

  const { contractVehicleTypes } = useLookups()

  function onUpdateVehicleTypes(value: string[]): void {
    if (value.length > 0) {
      vehicleTypes.value = value

      return
    }

    vehicleTypes.value = contractVehicleTypes.value?.items?.map(item => item.value) ?? []
  }
</script>

<style scoped>

</style>
