<template>
  <section class="rounded-xl border border-neutral-200 bg-white p-6">
    <h2 class="mb-4 text-lg font-semibold text-neutral-800">
      {{ $t('content.vehicleAssignments') }}
    </h2>

    <div
      v-if="selectedDriverVehicle"
      class="rounded-lg border border-neutral-200 bg-neutral-30 p-4"
    >
      <div class="flex flex-wrap items-start gap-4">
        <div class="flex min-w-0 flex-1 flex-col gap-1">
          <div class="flex items-center gap-2">
            <span
              class="flex size-9 shrink-0 items-center justify-center rounded-full bg-primary/10"
            >
              <IconTruck class="size-5 text-primary" />
            </span>

            <span class="font-medium text-neutral-800">
              {{ selectedDriverVehicle.vehicleTypeName }}
            </span>
          </div>

          <p class="text-xs text-neutral-600">
            {{ $t('content.head') }}: {{ selectedDriverVehicle.plateNo }} {{ selectedDriverVehicle.plateLetters }}
          </p>

          <p
            v-if="selectedDriverVehicle.tail?.plateNo || selectedDriverVehicle.tail?.plateLetters"
            class="text-xs text-neutral-600"
          >
            {{ $t('content.tail') }}: {{ selectedDriverVehicle.tail.plateNo }} {{ selectedDriverVehicle.tail.plateLetters }}
          </p>

          <p class="text-xs text-neutral-600">
            {{ selectedDriverVehicle.displayText }}
          </p>
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
            {{ selectedDriverVehicle.driverName ?? '—' }} – {{ selectedDriverVehicle.driverPhone ?? '—' }}
          </p>
        </div>
      </div>
    </div>

    <p
      v-else
      class="text-sm text-neutral-500"
    >
      {{ $t('content.noDriverYet') }}
    </p>
  </section>
</template>

<script setup lang="ts">
  import { useTripStore } from '@/stores/admin-dashboard/trip'

  const tripStore = useTripStore()
  const { selectedDriverVehicle } = storeToRefs(tripStore)
</script>

<style scoped>

</style>
