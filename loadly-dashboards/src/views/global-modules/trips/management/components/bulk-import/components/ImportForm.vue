<template>
  <div
    class="
      grid gap-4
      md:grid-cols-2
    "
  >
    <div>
      <label class="text-sm font-medium text-neutral-800">
        {{ $t('form.contractRefNumber') }} <span class="ms-1 text-error-600">*</span>
      </label>

      <BaseSelect
        v-model="contractId"
        :options="contractsOptions"
        searchable
        @update:model-value="$emit('changeContract', $event)"
        @scroll-to-end="loadMoreContracts"
        @search="getContractReferenceNumbers(undefined, 1, $event)"
      />
    </div>

    <div>
      <label class="text-sm font-medium text-neutral-800">
        {{ $t('form.contractVehicleType') }} <span class="ms-1 text-error-600">*</span>
      </label>

      <BaseSelect
        v-model="vehicleTypeId"
        :options="vehicleTypesOptions"
        searchable
        :disabled="!contractId"
        @scroll-to-end="loadMoreVehicleTypes"
        @search="getContractVehicleTypes(selectedContractId ?? '', 1, $event)"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
  import { useLookups } from '@/composables/useLookups'
  import type { SelectOption } from '@/types/ui'

  defineProps<{
    selectedContractId: string | number
  }>()

  defineEmits<{
    changeContract: [value: string | number]
  }>()

  const contractId = defineModel<string | number>('contractId', { required: true })
  const vehicleTypeId = defineModel<string | number>('vehicleTypeId', { required: true })

  const { t } = useI18n()
  const {
    contractReferenceNumbers,
    contractVehicleTypes,
    getContractReferenceNumbers,
    getContractVehicleTypes,
    hasMoreContractReferenceNumbers,
    hasMoreContractVehicleTypes,
    loadMoreContractReferenceNumbers,
    loadMoreContractVehicleTypes,
  } = useLookups()

  const contractsOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('form.select') },
    ...(contractReferenceNumbers.value?.items ?? []),
  ])

  const vehicleTypesOptions = computed(() => [
    { value: '', label: t('form.select') },
    ...(contractVehicleTypes.value?.items ?? []),
  ])

  function loadMoreContracts(): void {
    if (hasMoreContractReferenceNumbers.value) {
      loadMoreContractReferenceNumbers()
    }
  }

  function loadMoreVehicleTypes(): void {
    if (hasMoreContractVehicleTypes.value) {
      loadMoreContractVehicleTypes()
    }
  }
</script>

<style scoped>

</style>
