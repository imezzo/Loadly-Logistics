<template>
  <div
    v-if="permissions.length"
    class="
      grid grid-cols-1 gap-4
      lg:grid-cols-2
    "
  >
    <PermissionCard
      v-for="module in permissions"
      :key="module.id"
      :module="module"
    />
  </div>

  <div
    v-else
    class="flex items-center justify-center py-12 text-neutral-600"
  >
    <p>{{ $t('content.noPermissionsFound') }}</p>
  </div>
</template>

<script setup lang="ts">
  import PermissionCard from './PermissionCard.vue'
  import type { PermissionModule } from '@/types/response/permission'

  defineProps<{
    permissions: PermissionModule[]
  }>()
</script>

<style scoped>

</style>

