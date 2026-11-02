<template>
  <h1 class="text-2xl font-bold text-neutral-10">
    {{ $t('content.invoiceWithNumber', { number: invoiceNumber }) }}
  </h1>

  <div
    class="
      my-6 flex flex-col gap-4
      sm:flex-row sm:items-center sm:justify-between
    "
  >
    <h2 class="text-2xl font-bold text-neutral-10">
      {{ $t('content.paymentRequestsForInvoice') }}
    </h2>

    <button
      type="button"
      class="btn-primary inline-flex items-center gap-2"
      @click="isCreatePopupOpen = true"
    >
      {{ $t('button.createPaymentRequest') }}
      <IconReceipt class="size-5" />
    </button>
  </div>

  <CreatePaymentRequestPopup
    v-model="isCreatePopupOpen"
    :invoice-id="Number(invoiceId)"
    @refresh="emit('refetchInvoice')"
  />
</template>

<script setup lang="ts">
  import CreatePaymentRequestPopup from './CreatePaymentRequestPopup.vue'

  defineProps<{
    invoiceNumber: string
  }>()

  const emit = defineEmits<{
    refetchInvoice: []
  }>()

  const route = useRoute()

  const isCreatePopupOpen = ref(false)
  const invoiceId = computed(() => String(route.params.id ?? ''))
</script>

<style scoped>

</style>
