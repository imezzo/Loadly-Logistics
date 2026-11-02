<template>
  <section class="flex flex-wrap items-center justify-between gap-4 rounded-2xl bg-white p-6">
    <h2 class="text-xl font-bold text-neutral-800">
      {{ $t('content.contactAction') }}
    </h2>

    <div class="flex flex-wrap items-center gap-3">
      <template v-if="canApproveContract">
        <ContractActionApprove @success="onActionSuccess($t('button.approveContract'))" />
        <ContractActionReject @success="onActionSuccess($t('button.rejectContract'))" />
      </template>

      <ContractActionRevertToDraft
        v-if="isPendingContract"
        @success="onActionSuccess($t('button.revertToDraft'))"
      />

      <template v-if="isActiveContract">
        <ContractActionDeactivate @success="onActionSuccess($t('button.deactivateContract'))" />
        <ContractActionTerminate @success="onActionSuccess($t('button.terminateContract'))" />
      </template>

      <ContractActionReactivate
        v-if="canReactivateContract"
        @success="onActionSuccess($t('button.reactivate'))"
      />

      <RouterLink
        v-if="canRenewContract"
        :to="{ name: 'RenewContract', params: { id: route.params.id } }"
        class="
          rounded-2xl bg-primary px-4 py-3 text-sm font-medium text-white transition-colors
          hover:bg-primary/90
        "
      >
        {{ $t('button.renewContract') }}
      </RouterLink>
    </div>

    <ActionPopupSuccess
      v-model="showSuccessPopup"
      :message="$t('success.contractActionSuccess', { action: successAction })"
    />
  </section>
</template>

<script setup lang="ts">
  import ContractActionApprove from './ContractActionApprove.vue'
  import ContractActionReject from './ContractActionReject.vue'
  import ContractActionDeactivate from './ContractActionDeactivate.vue'
  import ContractActionReactivate from './ContractActionReactivate.vue'
  import ContractActionRevertToDraft from './ContractActionRevertToDraft.vue'
  import ContractActionTerminate from './ContractActionTerminate.vue'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { useContractStore } from '@/stores/admin-dashboard/contract'
  import { CONTRACT_STATUS } from '@/types/admin-dashboard/contract'
  import { Contract, ContractApprovals, Permission } from '@/types/response/permission-group'
  import { useContractWizard } from '../../composables/useContractWizard'

  const ActionPopupSuccess = defineAsyncComponent(() => import('./action-popup/ActionPopupSuccess.vue'))

  const contractStore = useContractStore()
  const { contract } = storeToRefs(contractStore)

  const route = useRoute()
  const { isRenewMode } = useContractWizard()

  const { hasModule: canFinanceApproveContract } = usePermission(
    Permission.ContractApprovals,
    ContractApprovals.Finance
  )

  const { hasModule: canOperationsApproveContract } = usePermission(
    Permission.ContractApprovals,
    ContractApprovals.Operations
  )

  const { hasModule: canSalesApproveContract } = usePermission(
    Permission.ContractApprovals,
    ContractApprovals.Sales
  )

  const { hasModule: canCreateContract } = usePermission(
    Permission.Contract,
    Contract.Create
  )

  const { hasModule: canEditContract } = usePermission(
    Permission.Contract,
    Contract.Edit
  )

  const showSuccessPopup = ref(false)
  const successAction = ref<string | null>(null)

  const isPendingContract = computed(() => Number(contract.value?.status) === CONTRACT_STATUS.PENDING)
  const isActiveContract = computed(() => Number(contract.value?.status) === CONTRACT_STATUS.ACTIVE)

  const canApproveContract = computed(() => isPendingContract.value
    && (canFinanceApproveContract.value || canOperationsApproveContract.value || canSalesApproveContract.value))

  const canReactivateContract = computed(() => ( Number(contract.value?.status) === CONTRACT_STATUS.INACTIVE
    && canCreateContract.value
    && canEditContract.value))

  const canRenewContract = computed(() => contract.value
    && !isRenewMode.value
    && canCreateContract.value
    && canEditContract.value
    && [
      CONTRACT_STATUS.EXPIRED,
      CONTRACT_STATUS.ACTIVE,
    ].includes(Number(contract.value?.status))
  )

  async function refreshContract(): Promise<void> {
    if (!route.params.id) return
    await contractStore.getContractById(route.params.id as string)
  }

  async function onActionSuccess(action: string): Promise<void> {
    showSuccessPopup.value = true
    successAction.value = action

    await refreshContract()
  }
</script>

<style scoped>

</style>
