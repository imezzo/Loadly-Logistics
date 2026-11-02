<template>
  <template v-if="hasContractDeactivate">
    <button
      type="button"
      class="btn-secondary"
      @click="showPopup = true"
    >
      {{ $t('button.deactivateContract') }}
    </button>

    <ActionPopupDeactivate
      v-model="showPopup"
      :contract-id="contractId"
      @discard="showPopup = false"
      @success="emit('success')"
    />
  </template>
</template>

<script setup lang="ts">
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { ContractManagement, Permission } from '@/types/response/permission-group'

  const route = useRoute()

  const { hasModule: hasContractDeactivate } = usePermission(
    Permission.ContractManagement,
    ContractManagement.Deactivate
  )

  const emit = defineEmits<{ success: [] }>()

  const ActionPopupDeactivate = defineAsyncComponent(
    () => import('./action-popup/ActionPopupDeactivate.vue')
  )

  const contractId = computed(() => route.params.id as string)
  const showPopup = ref(false)
</script>

<style scoped>

</style>
