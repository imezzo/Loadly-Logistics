<template>
  <BaseAlert
    v-if="showError"
    :model-value="showError"
    :title="$t('content.addVehiclesType')"
    :message="$t('error.pleaseAddAtLeastOneVehicleType')"
    variant="error"
    dismissible
    class="mb-4 w-full"
    @update:model-value="showError = false"
  />

  <ContractAssignSwitcher />
  <ContractVehiclesHeader @add-vehicle-type="onAddVehicleType" />

  <div class="flex flex-col gap-6">
    <ContractVehiclesFallback v-if="!((formData.vehicleTypes as VehicleType[])?.length)" />

    <VehicleTypeSummaryCard
      v-for="(vehicleType, index) in formData?.vehicleTypes"
      :key="index"
      :vehicle-type="vehicleType"
      :type-id="Number(formData?.contractType)"
    />
  </div>

  <BaseActions
    :prev-button-options="{ hasScrollTop: true }"
    :next-button-options="{ label: saveButtonTitle, hasScrollTop: true }"
    @prev="$emit('prev')"
    @next="onSaveAndNext"
  />

  <AddVehicleTypesPopup
    v-model="isAddVehicleTypesPopupOpen"
    :existing-vehicle-types="(formData.vehicleTypes as VehicleType[])"
    :type-id="Number(formData?.contractType)"
    @add="onAddVehicles"
    @remove="onRemoveVehicle"
  />
</template>

<script setup lang="ts">
  import { CONTRACT_STATUS } from '@/types/admin-dashboard/contract'
  import type { VehicleType } from '@/types/admin-dashboard/response/contract/contract-details.ts'
  import { useContractWizard } from '../../composables/useContractWizard'
  import ContractAssignSwitcher from './contract-assign-switcher/ContractAssignSwitcher.vue'
  import ContractVehiclesHeader from './ContractVehiclesHeader.vue'

  const ContractVehiclesFallback = defineAsyncComponent(() => import('./ContractVehiclesFallback.vue'))
  const VehicleTypeSummaryCard = defineAsyncComponent(() => import('./VehicleTypeSummaryCard.vue'))
  const AddVehicleTypesPopup = defineAsyncComponent(() => import('./contract-vehicle/AddVehicleTypesPopup.vue'))

  const emit = defineEmits<{
    next: []
    prev: []
    update: [data: Record<string, unknown>]
  }>()

  const { t } = useI18n()
  const {
    contract,
    formData,
    isFormDisabled,
    isViewMode,
  } = useContractWizard()

  const isAddVehicleTypesPopupOpen = ref(false)
  const showError = ref(false)

  const saveButtonTitle = computed(() =>
    isViewMode.value || (contract.value?.status === CONTRACT_STATUS.ACTIVE && isViewMode.value)
    ? t('button.next')
    : t('button.saveAndNext'))

  function onAddVehicleType(): void {
    isAddVehicleTypesPopupOpen.value = true
  }

  function onAddVehicles(vehicles: VehicleType[]): void {
    const nextList: VehicleType[] = vehicles.map((vehicle) => {
      const existing = (formData.value.vehicleTypes as VehicleType[] ?? []).find((vt) => vt.vehicleTypeId === vehicle.id)

      return {
        name: vehicle.name ?? existing?.name ?? '',
        carCount: vehicle.carCount ?? existing?.carCount ?? 0,
        extraTrip: vehicle.extraTrip ?? existing?.extraTrip,
        ExtraWorkingDayFees: vehicle.ExtraWorkingDayFees ?? existing?.ExtraWorkingDayFees,
        consumptionRate: vehicle.consumptionRate ?? existing?.consumptionRate,
        tripsPerMonth: vehicle.tripsPerMonth ?? existing?.tripsPerMonth,
        maxKms: vehicle.maxKms ?? existing?.maxKms,
        OverWeightPricePerTon: vehicle.OverWeightPricePerTon ?? existing?.OverWeightPricePerTon,
        contractorOverWeightPricePerTon: vehicle.contractorOverWeightPricePerTon ?? existing?.contractorOverWeightPricePerTon,
        monthlyRental: vehicle.monthlyRental ?? existing?.monthlyRental,
        minBillableWeightTons: vehicle.minBillableWeightTons ?? existing?.minBillableWeightTons,
        maxPayloadTons: vehicle.maxPayloadTons ?? existing?.maxPayloadTons,
        vehicleTypeId: vehicle.id,
      }
    })

    formData.value.vehicleTypes = nextList
  }

  function onRemoveVehicle(vehicleTypeId: number): void {
    const index = (formData.value.vehicleTypes as VehicleType[])?.findIndex(
      vt => vt.vehicleTypeId === vehicleTypeId
    )
    if (index !== -1) {
      (formData.value.vehicleTypes as VehicleType[])?.splice(index, 1)
    }
  }


  function onSaveAndNext(): void {
    if (isFormDisabled.value) {
      emit('next')

      return
    }

    if (!((formData.value.vehicleTypes as VehicleType[])?.length)) {
      showError.value = true

      return
    }

    showError.value = false
    emit('update', {
      vehicleTypes: formData.value.vehicleTypes,
    })
    emit('next')
  }

  defineExpose({ submitStep: onSaveAndNext })
</script>

<style scoped>

</style>

