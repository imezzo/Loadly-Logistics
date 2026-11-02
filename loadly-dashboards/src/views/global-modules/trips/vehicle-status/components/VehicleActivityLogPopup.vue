<template>
  <BasePopup v-model="model">
    <div class="flex w-full flex-col gap-4 px-6 py-2">
      <h2 class="text-xl font-bold text-neutral-10">
        {{ $t('content.activityLog') }}
      </h2>

      <div class="flex max-h-[60vh] flex-col gap-3 overflow-y-auto">
        <VehicleFacilityActivityLogItem
          v-for="visit in visits"
          :key="visit.id"
          :visit="visit"
        />

        <p
          v-if="!visits.length"
          class="py-8 text-center text-sm text-content-secondary"
        >
          {{ $t('content.noData') }}
        </p>
      </div>
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import type { VehicleFacilityVisit } from '@/types/admin-dashboard/response/vehicle-facility'

  defineProps<{
    visits: VehicleFacilityVisit[]
  }>()

  const model = defineModel<boolean>({ required: true })
</script>

<style scoped>

</style>
