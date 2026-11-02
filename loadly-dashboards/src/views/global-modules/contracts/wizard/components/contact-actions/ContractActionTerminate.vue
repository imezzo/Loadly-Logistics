<template>
  <template v-if="hasContractTerminate">
    <button
      type="button"
      class="btn-error"
      @click="showPopup = true"
    >
      {{ $t('button.terminateContract') }}
    </button>

    <ActionPopupTerminate
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

  const { hasModule: hasContractTerminate } = usePermission(
    Permission.ContractManagement,
    ContractManagement.Terminate
  )

  const emit = defineEmits<{ success: [] }>()

  const ActionPopupTerminate = defineAsyncComponent(
    () => import('./action-popup/ActionPopupTerminate.vue')
  )

  const contractId = computed(() => route.params.id as string)
  const showPopup = ref(false)
</script>

<style scoped>

</style>
