<template>
  <div class="mb-6">
    <div class="mb-4 flex items-center justify-between">
      <h1 class="mb-1 text-2xl font-bold text-neutral-10">
        {{ $t('title.drivers') }}
      </h1>

      <p class="ms-2 me-auto text-sm text-content-secondary">
        {{ $t('content.lastRefresh') }} {{ $t('content.minutesAgo', { minutes: 15 }) }}
      </p>

      <div class="flex gap-2">
        <button
          v-if="hasCreatePermission"
          type="button"
          class="btn-secondary flex items-center gap-2"
          @click="$emit('import-click')"
        >
          {{ $t('button.import') }} {{ $t('title.drivers') }}
        </button>
        <RouterLink
          v-if="hasCreatePermission"
          :to="{ name: 'DriverCreate' }"
          class="btn-primary flex items-center gap-2"
        >
          {{ $t('button.addNewDriver') }}
          <IconPlus class="size-4" />
        </RouterLink>

        <DriversExport />
      </div>
    </div>

    <div class="flex gap-4">
      <button
        type="button"
        class="tab-primary-outline"
        :class="{active: activeTab === 'list'}"
        @click="$emit('setTab', 'list')"
      >
        {{ $t('content.driverList') }}
      </button>

      <button
        type="button"
        class="tab-primary-outline"
        :class="{active: activeTab === 'map'}"
        @click="$emit('setTab', 'map')"
      >
        {{ $t('content.driversOnMap') }}
        <IconLocation class="size-4" />
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { Permission, Driver } from '@/types/response/permission-group'
  import DriversExport from './DriversExport.vue'

  defineProps<{
    activeTab?: 'list' | 'map'
  }>()

  defineEmits<{
    'import-click': []
    setTab: [tab: 'list' | 'map']
  }>()

  const {
    hasPermission: hasCreatePermission,
  } = usePermission(Permission.Driver, Driver.Create)
</script>

<style scoped>

</style>
