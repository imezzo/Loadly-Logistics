import { useLoading } from '@/composables/useLoading'
import { useBankTransferStore } from '@/stores/financial/bank-transfer'

export function useTransferDetails() {
  const route = useRoute()
  const { t } = useI18n()
  const { setLoading } = useLoading()

  const bankTransferStore = useBankTransferStore()
  const { transferDetail } = storeToRefs(bankTransferStore)

  const transferId = computed(() => Number(route.params.id))

  const loadError = ref('')

  async function fetchTransferDetails(): Promise<void> {
    const id = transferId.value
    if (Number.isNaN(id)) return

    loadError.value = ''
    setLoading(true)
    const { success, message } = await bankTransferStore.getById(id)
    setLoading(false)

    if (!success) {
      loadError.value = message || t('error.networkError')
    }
  }

  return {
    fetchTransferDetails,
    loadError,
    transferDetail,
    transferId,
  }
}
