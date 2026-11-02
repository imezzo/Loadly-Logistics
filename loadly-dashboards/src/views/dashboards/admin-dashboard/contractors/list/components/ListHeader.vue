<template>
  <div
    class="
      mb-6 flex flex-col gap-4
      sm:flex-row sm:items-center sm:justify-between
    "
  >
    <h1 class="text-xl font-bold text-neutral-800">
      {{ $t('title.contractors') }}
    </h1>

    <RouterLink
      v-if="hasCreatePermission"
      :to="{ name: 'CreateContractor' }"
      class="btn-primary"
    >
      {{ $t('button.addContractor') }}
      <IconPlus />
    </RouterLink>
  </div>
</template>

<script setup lang="ts">
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { Contractor, Permission } from '@/types/response/permission-group'

  const { hasPermission: hasCreatePermission } = usePermission(Permission.Contractor, Contractor.Create)
</script>

<style scoped>

</style>
