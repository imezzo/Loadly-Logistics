<template>
  <div
    class="
      mb-4 flex flex-col gap-4
      sm:flex-row sm:items-center sm:justify-between
    "
  >
    <h1 class="text-xl font-bold text-neutral-800">
      {{ $t('title.fleetManagement') }}
    </h1>

    <div class="flex flex-wrap gap-2">
      <button
        v-if="hasPermission"
        type="button"
        class="btn-secondary flex items-center gap-2"
        @click="isImportPopupOpen = true"
      >
        {{ $t('button.importVehicles') }}
      </button>

      <BaseLink
        v-if="hasPermission"
        to="/fleet/register"
        class="btn-primary"
      >
        {{ $t('button.registerNewVehicle') }}

        <IconPlus />
      </BaseLink>

      <FleetExport />
    </div>
  </div>

  <ImportVehiclesPopup
    v-model="isImportPopupOpen"
    @refresh="fetchVehicles"
  />
</template>

<script setup lang="ts">
  import FleetExport from './FleetExport.vue'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { Permission, Vehicle } from '@/types/response/permission-group'
  import ImportVehiclesPopup from './import-vehicles/ImportVehiclesPopup.vue'
  import { useList } from '../composables/useList'

  const { hasPermission } = usePermission(Permission.Vehicle, Vehicle.Create)
  const { fetchVehicles } = useList()

  const isImportPopupOpen = ref(false)
</script>

<style scoped>

</style>
