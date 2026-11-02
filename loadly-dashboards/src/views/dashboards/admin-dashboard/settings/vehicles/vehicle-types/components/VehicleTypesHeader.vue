<template>
  <BaseBreadcrumb :items="breadcrumbItems" />

  <div
    class="
      my-4 flex flex-col gap-4
      sm:flex-row sm:justify-between
    "
  >
    <h1 class="text-xl font-bold text-neutral-800">
      {{ $t('title.vehicleTypes') }}
    </h1>

    <button
      v-if="hasCreatePermission"
      type="button"
      class="btn-primary"
      @click="$emit('openWizard')"
    >
      {{ $t('button.addVehicleType') }}
      <IconPlus />
    </button>
  </div>
</template>

<script setup lang="ts">
  import { Permission, VehicleType as VehicleTypePermission } from '@/types/response/permission-group'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'

  defineEmits<{
    openWizard: []
  }>()

  const { t } = useI18n()

  const {
    hasPermission: hasCreatePermission,
  } = usePermission(Permission.VehicleType, VehicleTypePermission.Create)

  const breadcrumbItems = [
    { label: t('title.settings'), to: '/settings' },
    { label: t('title.vehicleTypes') },
  ]
</script>

<style scoped>

</style>
