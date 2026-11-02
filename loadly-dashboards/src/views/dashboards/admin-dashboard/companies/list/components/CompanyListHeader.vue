<template>
  <div
    class="
      mb-4 flex flex-col gap-4
      sm:flex-row sm:items-center sm:justify-between
    "
  >
    <h1 class="text-xl font-bold text-neutral-800">
      {{ $t('title.allCompaniesClients') }}
    </h1>

    <div class="flex gap-2">
      <button
        v-if="hasPermission"
        type="button"
        class="btn-secondary flex items-center gap-2"
        @click="emit('import-click')"
      >
        {{ $t('button.import') }} {{ $t('title.companies') }}
      </button>
      <BaseLink
        v-if="hasPermission"
        to="/companies/create"
        class="btn-primary flex items-center gap-2"
      >
        {{ $t('button.addCompany') }}
        <IconPlus />
      </BaseLink>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { Permission, Company } from '@/types/response/permission-group'

  const { hasPermission } = usePermission(Permission.Company, Company.Create)
  const emit = defineEmits<{ 'import-click': [] }>()
</script>

<style scoped>

</style>

