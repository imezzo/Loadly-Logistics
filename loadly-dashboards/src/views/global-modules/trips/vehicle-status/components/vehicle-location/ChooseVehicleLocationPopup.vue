<template>
  <BasePopup v-model="model">
    <div class="flex w-full flex-col px-6 py-2">
      <h2 class="text-xl font-bold text-neutral-10">
        {{ $t('content.chooseVehicleLocationTitle') }}
      </h2>

      <p class="mt-2 text-content-secondary">
        {{ $t('content.chooseVehicleLocationDescription') }}
      </p>

      <div class="mt-6 flex flex-wrap gap-3">
        <button
          v-for="tab in tabs"
          :key="tab.value"
          type="button"
          class="rounded-xl border-2 px-5 py-2.5 text-sm font-semibold transition-colors"
          :class="activeTab === tab.value
            ? 'border-primary bg-brand-98 text-primary'
            : 'border-neutral-300 bg-white text-neutral-10'"
          @click="activeTab = tab.value"
        >
          {{ tab.label }}
        </button>
      </div>

      <div class="mt-3 min-h-24">
        <OutOfServiceLocationForm
          v-if="activeTab === VehicleLocationTab.OutOfService && vehicle"
          :vehicle="vehicle"
          @cancel="model = false"
          @success="model = false"
          @fetch-vehicles="$emit('fetchVehicles')"
        />

        <FactoryParkingLocationForm
          v-else-if="activeTab === VehicleLocationTab.FactoryParking && vehicle"
          :vehicle="vehicle"
          @cancel="model = false"
          @success="model = false"
          @fetch-vehicles="$emit('fetchVehicles')"
        />
      </div>
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import type { Vehicle } from '@/types/admin-dashboard/response/vehicle/vehicle'

  const OutOfServiceLocationForm = defineAsyncComponent(
    () => import('./OutOfServiceLocationForm.vue'),
  )
  const FactoryParkingLocationForm = defineAsyncComponent(
    () => import('./FactoryParkingLocationForm.vue'),
  )

  enum VehicleLocationTab {
    OutOfService,
    FactoryParking,
  }

  defineProps<{
    vehicle?: Vehicle | null
  }>()

  defineEmits<{
    fetchVehicles: []
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })

  const { t } = useI18n()

  const activeTab = ref<VehicleLocationTab>(VehicleLocationTab.OutOfService)

  const tabs = computed(() => [
    { label: t('content.outOfService'), value: VehicleLocationTab.OutOfService },
    { label: t('content.factoryParking'), value: VehicleLocationTab.FactoryParking },
  ])

  watch(model, async (isOpen) => {
    if (!isOpen) return

    activeTab.value = VehicleLocationTab.OutOfService
  })
</script>

<style scoped>

</style>
