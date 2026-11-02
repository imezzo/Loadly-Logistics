<template>
  <div
    class="
      mb-6 grid grid-cols-1 gap-4
      lg:grid-cols-4
    "
  >
    <BaseInputSearch
      v-model="filters.search"
      :placeholder="t('form.searchByPlateNumber')"
      wrapper-class="
        flex h-11 shrink-0 items-center rounded-lg border border-neutral-300
        bg-white px-3 lg:flex-1
      "
      class="w-full"
    />

    <BaseSelect
      v-model="filters.vehicleTypeId"
      :options="vehicleTypeOptions"
      wrapper-class="w-full"
    />

    <BaseSelect
      v-model="filters.brandModelId"
      :options="brandModelOptions"
      wrapper-class="w-full"
    />

    <button
      type="button"
      class="btn-primary h-11 shrink-0 px-6"
      @click="emit('search')"
    >
      {{ t('button.search') }}
    </button>
  </div>
</template>

<script setup lang="ts">
  import { useLookups } from '@/composables/useLookups'
  import type { SelectOption } from '@/types/ui'

  const emit = defineEmits<{
    search: []
  }>()

  const filters = defineModel<{
    search: string
    vehicleTypeId: string
    brandModelId: string
  }>('filters', { required: true })

  const { t } = useI18n()
  const { vehicleBrandModels, vehicleTypes } = useLookups()

  const vehicleTypeOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('form.allVehicleTypes') },
    ...vehicleTypes.value,
  ])
  const brandModelOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('form.allVehicleModels') },
    ...vehicleBrandModels.value,
  ])
</script>

<style scoped>

</style>
