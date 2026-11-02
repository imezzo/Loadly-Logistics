<template>
  <div class="flex items-center gap-2">
    <button
      v-if="hasEditPermission"
      type="button"
      class="btn-action-success"
      @click="$emit('edit')"
    >
      <IconEdit class="size-4" />
    </button>

    <button
      v-if="hasDeletePermission"
      type="button"
      class="btn-action-error"
      @click="$emit('delete')"
    >
      <IconTrash class="size-4" />
    </button>
  </div>
</template>

<script setup lang="ts">
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { Permission, Lead as LeadPermission } from '@/types/response/permission-group'

  defineEmits<{
    delete: []
    edit: []
  }>()

  const {
    hasPermission: hasEditPermission,
  } = usePermission(Permission.Lead, LeadPermission.Edit)

  const {
    hasPermission: hasDeletePermission,
  } = usePermission(Permission.Lead, LeadPermission.Delete)
</script>

<style scoped>

</style>
