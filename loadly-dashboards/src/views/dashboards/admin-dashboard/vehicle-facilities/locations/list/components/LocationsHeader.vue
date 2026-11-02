<template>
  <div
    class="
      mb-4 flex flex-col gap-4
      sm:flex-row sm:items-center sm:justify-between
    "
  >
    <h1 class="text-xl font-bold text-neutral-800">
      {{ $t('title.vehicleLocations') }}
    </h1>

    <RouterLink
      v-if="hasCreatePermission"
      :to="{ name: 'CreateVehicleFacility' }"
      class="btn-primary"
    >
      {{ $t('button.addVehicleLocations') }}

      <IconPlus />
    </RouterLink>
  </div>
</template>

<script setup lang="ts">
  import {
    Permission,
    VehicleFacility as VehicleFacilityPermission,
  } from '@/types/response/permission-group'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'

  const {
    hasPermission: hasCreatePermission,
  } = usePermission(Permission.VehicleFacility, VehicleFacilityPermission.Create)
</script>

<style scoped>

</style>
