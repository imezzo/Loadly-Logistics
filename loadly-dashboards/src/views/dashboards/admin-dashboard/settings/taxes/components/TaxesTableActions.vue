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
  import { Permission, Tax as TaxPermission } from '@/types/response/permission-group'

  defineEmits<{
    edit: []
    delete: []
  }>()

  const {
    hasPermission: hasEditPermission,
  } = usePermission(Permission.Tax, TaxPermission.Edit)

  const {
    hasPermission: hasDeletePermission,
  } = usePermission(Permission.Tax, TaxPermission.Delete)
</script>

<style scoped>

</style>
