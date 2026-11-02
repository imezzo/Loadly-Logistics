<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <BaseAlert
      :model-value="!!loadError"
      :message="loadError"
      variant="error"
      class="mb-4"
    />

    <div
      v-if="payoutDetail"
      class="mb-6 rounded-xl border border-neutral-500 p-4"
    >
      <InvoiceHeader
        :invoice="payoutDetail"
        :is-setting-paid="isSettingPaid"
        @set-paid="setPaid"
      />

      <InvoiceTrips :trips="payoutDetail.trips" />

      <InvoiceFooter :total-amount="payoutDetail.totalAmount" />
    </div>

    <PopupSuccess v-model="isSuccessPopupOpen">
      <template #message>
        <p>{{ $t('success.contractorPayoutMarkedPaid') }}</p>
      </template>

      <template #actions>
        <button
          type="button"
          class="btn-primary mt-4 w-full"
          @click="isSuccessPopupOpen = false"
        >
          {{ $t('button.done') }}
        </button>
      </template>
    </PopupSuccess>
  </div>
</template>

<script setup lang="ts">
  import InvoiceHeader from './components/InvoiceHeader.vue'
  import InvoiceTrips from './components/InvoiceTrips.vue'
  import InvoiceFooter from './components/InvoiceFooter.vue'
  import { useViewInvoice } from './composables/useViewInvoice'

  const {
    breadcrumbItems,
    fetchInvoice,
    isSettingPaid,
    isSuccessPopupOpen,
    loadError,
    payoutDetail,
    resetPayoutDetail,
    setPaid,
  } = useViewInvoice()

  onMounted(fetchInvoice)

  onUnmounted(resetPayoutDetail)
</script>

<style scoped>

</style>
