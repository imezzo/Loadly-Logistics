<template>
  <section class="mt-2">
    <CardPreview
      :title="$t('content.vehicleAssignment')"
      class="border-primary-500! border-2!"
    >
      <div
        class="
          flex flex-col gap-4 rounded-[0.625rem] border border-gray-50 bg-neutral-30 p-4
          sm:flex-row sm:flex-wrap sm:items-stretch
        "
      >
        <article
          class="
            flex min-w-0 flex-1 gap-3 rounded-lg border border-gray-50 bg-white p-4
            sm:max-w-full sm:min-w-[200px]
          "
        >
          <IconVehicle
            class="size-12 shrink-0 self-start"
            aria-hidden="true"
          />

          <div class="min-w-0 flex-1">
            <p class="text-sm font-bold text-neutral-800">
              {{ trip.vehicleType || $t('content.notAvailable') }}
            </p>

            <p class="mt-2 text-sm font-bold text-neutral-800">
              {{ $t('content.head') }}:  - {{ trip.vehicle?.plateNo }} - {{ trip.vehicle?.plateLetters }}
            </p>

            <p
              v-for="(tailVehicle, index) in trip.vehicle?.tailVehicles"
              :key="index"
              class="mt-2 text-sm font-bold text-neutral-800"
            >
              {{ $t('content.tail') }}: {{ tailVehicle.plateNo }} - {{ tailVehicle.plateLetters }}
            </p>

            <p class="mt-1 text-sm text-content-secondary">
              {{ $t('content.driver') }}: {{ trip.vehicle?.driverName || $t('content.notAvailable') }}
            </p>
          </div>
        </article>

        <p
          v-if="!trip.vehicle"
          class="w-full py-4 text-center text-sm text-content-secondary"
        >
          {{ $t('content.noData') }}
        </p>
      </div>
    </CardPreview>
  </section>
</template>

<script setup lang="ts">
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import CardPreview from '@/components/card/CardPreview.vue'

  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)
</script>

<style scoped>

</style>
