<template>
  <div
    class="flex flex-col gap-3 rounded-lg border border-gray-50 bg-neutral-30 p-1.5"
  >
    <div class="flex gap-3 rounded-xl border border-gray-50 bg-white p-1.5">
      <IconVehicle class="size-6" />

      <div class="min-w-0 flex-1">
        <p class="font-bold text-neutral-800">
          {{ item.vehicleTypeName }}
        </p>

        <p class="text-xs text-neutral-600">
          {{ $t('content.head') }}: {{ item.plateNo }} {{ item.plateLetters }}
        </p>

        <p class="text-xs text-neutral-600">
          {{ item.initialOdometerReading }} {{ item.capacityUnitId }}
        </p>
      </div>
    </div>

    <div class="flex items-center gap-3 border-t border-neutral-200 pt-3">
      <span
        class="
          flex size-10 shrink-0 items-center justify-center overflow-hidden rounded-full bg-theme
          text-white
        "
        aria-hidden
      >
        <IconUser class="size-5 text-black" />
      </span>

      <div class="min-w-0 flex-1">
        <p class="text-xs font-bold text-neutral-600">
          {{ $t('content.currentDriver') }}
        </p>

        <p class="text-sm text-neutral-800">
          {{ item.driverName }} – {{ item.driverPhone }}
        </p>
      </div>
    </div>

    <button
      type="button"
      class="btn-primary mt-auto w-full"
      @click="emit('choose', item)"
    >
      {{ $t('content.chooseThisCar') }}
    </button>
  </div>
</template>

<script setup lang="ts">
  import type { Vehicle } from '@/types/admin-dashboard/response/vehicle/vehicle'

  defineProps<{
    item: Vehicle
  }>()

  const emit = defineEmits<{
    choose: [item: Vehicle]
  }>()
</script>

<style scoped>

</style>
