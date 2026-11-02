<template>
  <div class="mb-6 flex flex-wrap items-center justify-between gap-4">
    <p class="text-sm text-neutral-800">
      {{ $t('content.chooseTripsAndCreateInvoice') }}
    </p>

    <button
      type="button"
      class="btn-primary"
      :disabled="!hasSelectedTrips || isCreatingInvoice"
      @click="createInvoice"
    >
      {{ $t('button.createInvoice') }}
      <IconReceipt class="size-4" />
    </button>
  </div>

  <PopupSuccess
    v-model="isSuccessPopupOpen"
    @update:model-value="onSuccessPopupUpdate"
  >
    <template #message>
      <p>{{ $t('success.contractorPayoutCreated') }}</p>
    </template>

    <template #actions>
      <button
        type="button"
        class="btn-primary mt-4 w-full"
        @click="onSuccessPopupUpdate(false)"
      >
        {{ $t('button.done') }}
      </button>
    </template>
  </PopupSuccess>

  <PopupError
    v-if="createInvoiceError"
    :model-value="createInvoiceError !== null"
    :message="createInvoiceError"
    @update:model-value="createInvoiceError = null"
  />
</template>

<script setup lang="ts">
  import { useContractorTripsList } from '../composables/useContractorTripsList.ts'

  const {
    createInvoiceError,
    createInvoice,
    hasSelectedTrips,
    isCreatingInvoice,
    isSuccessPopupOpen,
    onSuccessPopupUpdate,
  } = useContractorTripsList()
</script>

<style scoped>

</style>
