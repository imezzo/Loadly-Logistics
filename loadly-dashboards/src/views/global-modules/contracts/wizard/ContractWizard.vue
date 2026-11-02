<template>
  <div class="mx-auto w-full space-y-6">
    <ContactActionHeader v-if="(isUpdateMode || isViewMode) && hasViewPermission" />

    <div class="rounded-2xl bg-white p-6">
      <ContractWizardHeader />

      <ContractForm ref="contractFormRef" />
    </div>
  </div>
</template>

<script setup lang="ts">
  import ContactActionHeader from './components/contact-actions/ContactActionHeader.vue'
  import ContractForm from './components/ContractForm.vue'
  import ContractWizardHeader from './components/ContractWizardHeader.vue'
  import { useContractWizard } from './composables/useContractWizard'
  import { useContractStore } from '@/stores/admin-dashboard/contract'
  import { useLoading } from '@/composables/useLoading'
  import { useLookups } from '@/composables/useLookups'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { CONTRACT_STATUS } from '@/types/admin-dashboard/contract'
  import { Contract, Permission } from '@/types/response/permission-group'
  import { useApp } from '@/composables/useApp'

  const route = useRoute()
  const router = useRouter()
  const { setLoading } = useLoading()
  const { getExtraFeeTypes, getAdmins } = useLookups()
  const { isUpdateMode, isViewMode, isCreateMode } = useContractWizard()
  const { hasPermission: hasViewPermission } = usePermission(Permission.Contract, Contract.View)
  const { isAdminDashboard } = useApp()

  const contractStore = useContractStore()
  const { contract, originalVehicleSlots, originalVehicleTypes, wizardFormData } = storeToRefs(contractStore)


  if (!isCreateMode.value && !route.params.id) {
    router.push({name: 'NotFound'})
  }

  const contractFormRef = useTemplateRef<InstanceType<typeof ContractForm>>('contractFormRef')

  async function getSalesReps(): Promise<void> {
    if (!isAdminDashboard) return
    if (isCreateMode.value
      || (isUpdateMode.value
      && contract.value?.status === CONTRACT_STATUS.DRAFT)) {
      await getAdmins()
    }
  }

  onMounted(async () => {
    if (!isCreateMode.value && route.params.id) {
      setLoading(true)
      const { success } = await contractStore.getContractById(route.params.id as string)
      setLoading(false)

      if (success) {
        wizardFormData.value = contract.value as Record<string, unknown>
      }
    }
    getSalesReps()
    await getExtraFeeTypes()
  })

  onUnmounted(() => {
    wizardFormData.value = {}
    contract.value = null
    originalVehicleSlots.value = []
    originalVehicleTypes.value = []
  })
</script>

<style scoped>

</style>
