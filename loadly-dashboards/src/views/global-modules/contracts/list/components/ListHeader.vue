<template>
  <div
    class="
      mb-6 flex flex-col gap-4
      sm:flex-row sm:items-center sm:justify-between
    "
  >
    <h1 class="text-xl font-bold text-neutral-800">
      {{ $t('title.clientContracts') }}
    </h1>

    <div class="flex items-center gap-3">
      <BaseLink
        v-if="hasCreatePermission"
        to="/contracts/create"
        class="btn-primary"
      >
        {{ $t('button.addContract') }}

        <IconPlus />
      </BaseLink>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { usePermission } from '@/composables/admin-dashboard/usePermission';
  import { Contract, Permission } from '@/types/response/permission-group';

    const {
    hasPermission: hasCreatePermission
  } = usePermission(Permission.Contract, Contract.Create)
</script>

<style scoped>

</style>
