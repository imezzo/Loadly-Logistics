import { useLoading } from '@/composables/useLoading'
import { useTripStore } from '@/stores/admin-dashboard/trip'
import type { TripWizardTripCharge } from '@/types/admin-dashboard/response/trip/trip'
import { TripDocumentStatus } from '@/types/admin-dashboard/response/trip/trip-document-status.enums'

export function useTripAuditingForm() {
  const { t } = useI18n()
  const route = useRoute()
  const router = useRouter()
  const { setLoading } = useLoading()

  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)

  const proofOfDelivery = ref<string>('')
  const financialDate = ref<string>('')
  const confirmFinanceApproval = ref(false)
  const charges = ref<TripWizardTripCharge[]>([])
  const saveError = ref<string | null>(null)
  const isConfirmSavePopupOpen = ref(false)
  const isSuccessPopupOpen = ref(false)

  const tripId = computed(() => Number(route.params.id))

  const isTripDocumentStatusApproved = computed(() => trip.value.tripDocumentStatus === TripDocumentStatus.Approved)

  const listRouteName = computed(() => {
    return trip.value.isExternalTrip
      ? 'FinancialTripAuditingClients'
      : 'FinancialTripAuditingCompany'
  })

  async function fetchTrip(): Promise<void> {
    if (!Number.isInteger(tripId.value) || tripId.value <= 0) return

    setLoading(true)
    await tripStore.fetchTrip(tripId.value)
    charges.value = trip.value.tripCharges ?? []
    proofOfDelivery.value = trip.value.proofOfDelivery != null
      ? String(trip.value.proofOfDelivery)
      : ''
    financialDate.value = trip.value.financialDate ?? ''
    confirmFinanceApproval.value = isTripDocumentStatusApproved.value
    setLoading(false)
  }

  function cancel(): void {
    router.push({ name: listRouteName.value })
  }

  function requestSave(): void {
    if (!Number.isInteger(tripId.value) || tripId.value <= 0) return
    if (!proofOfDelivery.value.trim()) {
      saveError.value = t('validation.required', { field: t('table.pod') })

      return
    }

    if (!financialDate.value) {
      saveError.value = t('validation.required', { field: t('form.financialDate') })

      return
    }

    if (confirmFinanceApproval.value) {
      isConfirmSavePopupOpen.value = true

      return
    }

    void save()
  }

  async function save(): Promise<void> {
    if (!Number.isInteger(tripId.value) || tripId.value <= 0) return
    if (!proofOfDelivery.value.trim()) {
      saveError.value = t('validation.required', { field: t('table.pod') })

      return
    }

    if (!financialDate.value) {
      saveError.value = t('validation.required', { field: t('form.financialDate') })

      return
    }

    saveError.value = null
    setLoading(true)

    const { success, message } = await tripStore.auditTrip(tripId.value, {
      confirmFinanceApproval: confirmFinanceApproval.value,
      financialDate: financialDate.value,
      proofOfDelivery: proofOfDelivery.value,
      tripCharges: charges.value,
    })

    setLoading(false)

    if (success) {
      isSuccessPopupOpen.value = true

      return
    }

    saveError.value = message || t('error.somethingWentWrong')
  }

  function onSuccessClose(): void {
    isSuccessPopupOpen.value = false
    router.push({ name: listRouteName.value })
  }

  return {
    cancel,
    charges,
    confirmFinanceApproval,
    fetchTrip,
    financialDate,
    isConfirmSavePopupOpen,
    isSuccessPopupOpen,
    isTripDocumentStatusApproved,
    listRouteName,
    onSuccessClose,
    proofOfDelivery,
    requestSave,
    save,
    saveError,
    trip,
  }
}
