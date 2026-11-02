<template>
  <nav
    class="
      mb-6 flex flex-wrap gap-1 overflow-x-auto border-b border-gray-50
      sm:gap-3
    "
    aria-label="Trip details sections"
  >
    <button
      v-for="tab in tabs"
      :key="tab.id"
      type="button"
      class="tab-item"
      :class="{ active: model === tab.id }"
      @click="model = tab.id"
    >
      {{ $t(tab.labelKey) }}
    </button>
  </nav>
</template>

<script setup lang="ts">
  import { useApp } from '@/composables/useApp'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { Permission, Trip as TripPermission } from '@/types/response/permission-group'

  const props = defineProps<{
    isTripCompleted?: boolean
  }>()

  export type TripDetailsTabId =
    | 'information'
    | 'vehicleType'
    | 'charge'
    | 'documents'
    | 'activity'

  interface TabItem {
    id: TripDetailsTabId
    labelKey: string
  }

  const { isAdminDashboard } = useApp()
  const { hasPermission: hasEditChargesPermission } = usePermission(
    Permission.Trip,
    TripPermission.EditCharges,
  )

  const chargeTab: TabItem = {
    id: 'charge',
    labelKey: 'content.tripCharge',
  }

  const hasChargesTab = computed(() => isAdminDashboard && props.isTripCompleted && hasEditChargesPermission.value)

  const tabs = computed<TabItem[]>(() => [
    { id: 'information', labelKey: 'content.tripInformation' },
    { id: 'vehicleType', labelKey: 'table.vehicleType' },
    ...(hasChargesTab.value ? [chargeTab] : []),
    { id: 'documents', labelKey: 'content.documents' },
    { id: 'activity', labelKey: 'content.tripActivity' },
  ])

  const model = defineModel<TripDetailsTabId>('modelValue', { required: true })
</script>

<style scoped>

</style>
