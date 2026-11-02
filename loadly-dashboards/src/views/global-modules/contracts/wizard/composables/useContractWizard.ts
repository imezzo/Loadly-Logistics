import { useContractStore } from '@/stores/admin-dashboard/contract'
import { CONTRACT_STATUS } from '@/types/admin-dashboard/contract'

export function useContractWizard() {
  const route = useRoute()
  const contractStore = useContractStore()
  const { contract, wizardFormData } = storeToRefs(contractStore)
  const { t } = useI18n()

  const formData = computed(() => wizardFormData.value || {})

  const isUpdateMode = computed(() => route.name === 'UpdateContract')
  const isViewMode = computed(() => route.name === 'ViewContract')
  const isRenewMode = computed(() => route.name === 'RenewContract')
  const isCreateMode = computed(() => route.name === 'CreateContract')

  const saveButtonTitle = computed(() =>
    isViewMode.value || contract.value?.status === CONTRACT_STATUS.ACTIVE
    ? t('button.next')
    : t('button.saveAndNext'))

  const hasContractInUpdateMode = computed(
    () => Boolean(isUpdateMode.value && contract.value)
  )

  const isFormDisabled = computed(() => {
    if (isViewMode.value) return true
    if (
      !hasContractInUpdateMode.value
      || [CONTRACT_STATUS.DRAFT, CONTRACT_STATUS.ACTIVE].includes(contract.value?.status as number)
    ) return false

    return true
  })

  const isPricingFormEnabled = computed(() => {
    if (isViewMode.value) return false
    if (hasContractInUpdateMode.value && !isViewMode.value) return true
    if (contract.value?.status === undefined) return false

    return [
      CONTRACT_STATUS.ACTIVE
    ].includes(contract.value?.status)
  })

  const allowSave = computed(() => {
    if (isViewMode.value || (contract.value?.status === undefined && isUpdateMode.value)) return false
    if (!hasContractInUpdateMode.value) return true

    return [
      CONTRACT_STATUS.DRAFT,
      CONTRACT_STATUS.ACTIVE
    ].includes(contract.value?.status as number)
  })

  return {
    allowSave,
    contract,
    formData,
    isCreateMode,
    isFormDisabled,
    isPricingFormEnabled,
    isRenewMode,
    isUpdateMode,
    isViewMode,
    saveButtonTitle,
    wizardFormData,
  }
}
