import { useContractStore } from '@/stores/admin-dashboard/contract'
import { useLoading } from '@/composables/useLoading'
import { getLocalizedValue } from '@/utils/i18n'
import { ContractType } from '@/types/admin-dashboard/response/contract/contract-details'

export function useVehicleByDays() {
  const route = useRoute()
  const router = useRouter()
  const { t } = useI18n()
  const { setLoading } = useLoading()

  const contractStore = useContractStore()
  const { contract } = storeToRefs(contractStore)

  const contractId = computed(() => String(route.params.id ?? ''))

  const returnRouteName = computed(() => {
    const returnTo = route.query.returnTo

    return typeof returnTo === 'string' && returnTo
      ? returnTo
      : 'ViewContract'
  })

  const contractRoute = computed(() => ({
    name: returnRouteName.value,
    params: { id: contractId.value },
  }))

  const pageTitle = computed(() => contract.value?.nameEn
    ? getLocalizedValue(contract.value.nameEn, contract.value.nameAr ?? '')
    : t('content.daysByVehicles'))

  const breadcrumbItems = computed(() => [
    { label: t('title.contracts'), to: '/contracts' },
    {
      label: contract.value?.nameEn
        ? getLocalizedValue(contract.value.nameEn, contract.value.nameAr ?? '')
        : t('title.viewContract'),
      to: contractRoute.value,
    },
    { label: t('content.daysByVehicles') },
  ])

  async function fetchContract(): Promise<void> {
    if (!contractId.value) {
      router.push({ name: 'NotFound' })

      return
    }

    setLoading(true)
    const { success } = await contractStore.getContractById(contractId.value)
    setLoading(false)

    if (!success || (contract.value?.contractType !== ContractType.Included
      && contract.value?.contractType !== ContractType.Excluded)) {
      router.push(contractRoute.value)
    }
  }

  return {
    breadcrumbItems,
    contract,
    contractId,
    contractRoute,
    fetchContract,
    pageTitle,
  }
}
