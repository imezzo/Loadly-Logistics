<template>
  <div class="mb-6 flex items-center justify-between">
    <h2 class="font-medium text-neutral-10">
      {{ $t('content.addVehiclesType') }}
    </h2>

    <button
      v-if="!isViewMode"
      type="button"
      :disabled="isFormDisabled"
      class="btn-primary py-2.5"
      @click="$emit('addVehicleType')"
    >
      {{ hasVehicleTypes ? $t('button.editVehicleTypes') : $t('button.addVehicleTypes') }}
    </button>
  </div>
</template>

<script setup lang="ts">
  import type { VehicleType } from '@/types/admin-dashboard/response/contract/contract-details'
  import { useContractWizard } from '../../composables/useContractWizard'

  defineEmits<{
    addVehicleType: []
  }>()

  const { formData, isFormDisabled, isViewMode } = useContractWizard()

  const hasVehicleTypes = computed(() =>
    Boolean((formData.value.vehicleTypes as VehicleType[])?.length)
  )
</script>

<style scoped>

</style>
