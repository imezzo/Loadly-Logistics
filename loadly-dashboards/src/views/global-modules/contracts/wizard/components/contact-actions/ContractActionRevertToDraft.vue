<template>
  <template v-if="hasContractSaveAsDraft">
    <button
      type="button"
      class="btn-secondary-outline"
      @click="showPopup = true"
    >
      {{ $t('button.revertToDraft') }}
    </button>

    <ActionPopupRevertToDraft
      v-model="showPopup"
      :contract-id="contractId"
      @success="emit('success')"
    />
  </template>
</template>

<script setup lang="ts">
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { ContractManagement, Permission } from '@/types/response/permission-group'

  const route = useRoute()

  const { hasModule: hasContractSaveAsDraft } = usePermission(
    Permission.ContractManagement,
    ContractManagement.PullBackToDraft
  )

  const emit = defineEmits<{ success: [] }>()

  const ActionPopupRevertToDraft = defineAsyncComponent(
    () => import('./action-popup/ActionPopupRevertToDraft.vue')
  )

  const contractId = computed(() => route.params.id as string)
  const showPopup = ref(false)
</script>

<style scoped>

</style>
