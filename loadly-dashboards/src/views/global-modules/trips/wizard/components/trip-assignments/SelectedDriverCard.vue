<template>
  <section class="rounded-xl border border-info-300 bg-white p-4">
    <div class="mb-4 flex items-center gap-2">
      <span class="flex size-9 items-center justify-center rounded-full bg-neutral-30">
        <IconTruck class="size-5 text-primary" />
      </span>

      <span class="text-sm font-medium text-neutral-800">
        {{ $t('content.youChosenVehicle') }}
      </span>

      <button
        v-if="!isFormDisabled && !hasDisabledFields"
        type="button"
        class="btn-action-error ms-auto"
        aria-label="Remove driver"
        @click="emit('remove')"
      >
        <IconTrash class="size-5" />
      </button>
    </div>

    <div class="rounded-lg border border-neutral-200 bg-neutral-30 p-1">
      <div
        class="flex overflow-x-auto rounded-xl border border-neutral-200 bg-white p-3 pb-2"
      >
        <div class="flex min-w-1/3 flex-col gap-2">
          <IconTruck class="size-6 shrink-0 text-primary" />
          <div class="min-w-0 flex-1">
            <p class="font-medium text-neutral-800">
              {{ vehicle.vehicleTypeName }}
            </p>

            <p class="text-xs text-neutral-600">
              {{ $t('content.head') }}: {{ vehicle.plateNo }} {{ vehicle.plateLetters }}
            </p>

            <p class="text-xs text-neutral-600">
              {{ $t('content.tail') }}: {{ vehicle.tail.plateNo }} {{ vehicle.tail.plateLetters }}
            </p>
          </div>
        </div>
      </div>

      <div class="mt-4 flex items-center gap-3 border-t border-neutral-200 pt-4">
        <span
          class="
            flex size-10 shrink-0 items-center justify-center overflow-hidden rounded-full bg-theme
            text-white
          "
        >
          <IconUser class="size-5 text-black" />
        </span>

        <div class="min-w-0 flex-1">
          <p class="text-xs font-medium text-neutral-600">
            {{ $t('content.currentDriver') }}
          </p>

          <p class="text-sm text-neutral-800">
            {{ vehicle.driverName ?? '' }} – {{ vehicle.driverPhone ?? '' }}
          </p>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
  import type { Vehicle } from '@/types/admin-dashboard/response/vehicle/vehicle'
  import { useTripWizard } from '../../composables/useTripWizard'

  defineProps<{
    vehicle: Vehicle
  }>()

  const emit = defineEmits<{
    remove: []
  }>()

  const { isFormDisabled, hasDisabledFields } = useTripWizard()
</script>

<style scoped>

</style>
