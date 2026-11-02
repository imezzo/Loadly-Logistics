<template>
  <template v-if="canCancel">
    <button
      type="button"
      class="btn-error-outline"
      :disabled="isLoading"
      @click="showCancelPopup = true"
    >
      {{ $t('button.cancelInvoice') }}
    </button>

    <CancelInvoicePopup
      v-model="showCancelPopup"
      :is-loading="isLoading"
      :error-message="cancelError"
      :error-form-errors="cancelFormErrors"
      @submit="onCancelInvoice"
    />
  </template>

  <LazyPopupSuccess
    v-if="isCancelSuccess"
    v-model="isCancelSuccess"
  >
    <template #message>
      {{ $t('success.invoiceCancelled') }}
    </template>
  </LazyPopupSuccess>
</template>

<script setup lang="ts">
  import { useInvoiceStore } from '@/stores/financial/invoice.ts'
  import { useLoading } from '@/composables/useLoading'
  import { useApp } from '@/composables/useApp'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { Invoice, Permission } from '@/types/response/permission-group'
  import { PaymentStatus } from '@/types/invoice.enums.ts'

  const LazyPopupSuccess = defineAsyncComponent(() => import('@/components/popup/PopupSuccess.vue'))
  const CancelInvoicePopup = defineAsyncComponent(() => import('./CancelInvoicePopup.vue'))

  const { t } = useI18n()
  const { isLoading, setLoading } = useLoading()
  const { isAdminDashboard } = useApp()
  const { hasPermission: hasEditPermission } = usePermission(Permission.Invoice, Invoice.Edit)

  const invoiceStore = useInvoiceStore()
  const { invoiceDetail } = storeToRefs(invoiceStore)

  const showCancelPopup = ref(false)
  const isCancelSuccess = ref(false)
  const cancelError = ref<string | null>(null)
  const cancelFormErrors = ref<Record<string, string[]>>({})

  const canCancel = computed(() => {
    if (!isAdminDashboard || !hasEditPermission.value || !invoiceDetail.value) return false

    return Number(invoiceDetail.value.status) === PaymentStatus.Unpaid
  })

  function resetCancelErrors(): void {
    cancelError.value = null
    cancelFormErrors.value = {}
  }

  async function onCancelInvoice(cancellationReason: string): Promise<void> {
    const id = invoiceDetail.value?.id
    if (!canCancel.value || id == null) return

    setLoading(true)
    resetCancelErrors()

    const { success, message, errors } = await invoiceStore.cancelInvoice(id, cancellationReason)

    if (success) {
      showCancelPopup.value = false
      await invoiceStore.getInvoiceById(id)
      isCancelSuccess.value = true
    } else {
      cancelError.value = message || t('error.somethingWentWrong')
      const reasonErrors = errors?.cancellationReason ?? errors?.CancellationReason
      cancelFormErrors.value = reasonErrors ? { cancellationReason: reasonErrors } : {}
    }

    setLoading(false)
  }

  watch(showCancelPopup, (visible) => {
    if (!visible) resetCancelErrors()
  })
</script>

<style scoped>

</style>
