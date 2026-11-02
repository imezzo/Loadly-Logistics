<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <h1 class="mb-6 text-xl font-bold text-neutral-800">
      {{ $t('sidebar.tripAuditing') }}
    </h1>

    <div class="mb-6 rounded-xl bg-neutral-30 p-4">
      <ContractConditionsPreview :contract-conditions="trip.contractConditions" />

      <div class="mb-2 flex gap-2">
        <p class="shrink-0 text-sm font-medium text-neutral-700">
          {{ $t('content.auditorName') }}:
        </p>

        <p class="text-sm text-neutral-600">
          {{ trip.auditorName || '—' }}
        </p>
      </div>

      <div class="flex gap-2">
        <p class="shrink-0 text-sm font-medium text-neutral-700">
          {{ $t('content.notes') }}:
        </p>

        <p class="text-sm whitespace-pre-wrap text-neutral-600">
          {{ trip.dispatcherNotes || '—' }}
        </p>
      </div>
    </div>

    <div class="mb-6 flex flex-col gap-2">
      <label
        for="proofOfDelivery"
        class="text-sm font-medium text-neutral-800"
      >
        {{ $t('table.pod') }}
        <span class="text-error-600">*</span>
      </label>

      <BaseInput
        id="proofOfDelivery"
        v-model="proofOfDelivery"
        :disabled="isTripDocumentStatusApproved"
        type="text"
      />
    </div>

    <div class="mb-6 flex flex-col gap-2">
      <label
        for="financialDate"
        class="text-sm font-medium text-neutral-800"
      >
        {{ $t('form.financialDate') }}
        <span class="text-error-600">*</span>
      </label>

      <p class="text-sm text-neutral-600">
        {{ $t('form.financialDateAccountingDay') }}
      </p>

      <BaseInput
        id="financialDate"
        v-model="financialDate"
        :disabled="isTripDocumentStatusApproved"
        type="date"
      />
    </div>

    <BaseCheckbox
      v-model="confirmFinanceApproval"
      :disabled="isTripDocumentStatusApproved"
      :label="$t('form.confirmFinanceApproval')"
    />

    <TripCharges
      v-model:charges="charges"
      :show-footer-actions="false"
      :disabled="isTripDocumentStatusApproved"
      class="mt-6"
    />

    <BaseActions
      container-class="mt-6 flex items-center justify-end gap-3"
      :prev-button-options="{ label: $t('button.cancel'), class: 'btn-secondary min-w-32' }"
      :next-button-options="isTripDocumentStatusApproved
        ? undefined
        : { label: $t('button.save'), class: 'btn-primary min-w-32' }"
      @prev="cancel"
      @next="requestSave"
    />

    <ConfirmAuditSavePopup
      v-model="isConfirmSavePopupOpen"
      @confirm="save"
    />

    <PopupError
      v-model="isErrorOpen"
      :message="saveError"
    />

    <PopupSuccess
      v-model="isSuccessPopupOpen"
      :message="$t('success.tripAudited')"
      @update:model-value="onSuccessModelUpdate"
    />
  </div>
</template>

<script setup lang="ts">
  import ConfirmAuditSavePopup from './components/ConfirmAuditSavePopup.vue'
  import ContractConditionsPreview from './components/ContractConditionsPreview.vue'
  import { useTripAuditingForm } from './composables/useTripAuditingForm'
  import { useTripStore } from '@/stores/admin-dashboard/trip'

  const { t } = useI18n()
  const {
    cancel,
    charges,
    confirmFinanceApproval,
    fetchTrip,
    financialDate,
    isConfirmSavePopupOpen,
    isSuccessPopupOpen,
    onSuccessClose,
    isTripDocumentStatusApproved,
    proofOfDelivery,
    requestSave,
    save,
    saveError,
    trip,
  } = useTripAuditingForm()

  const tripStore = useTripStore()

  const breadcrumbItems = computed(() => ([
    { label: t('sidebar.tripAuditing'), to: '/financial/auditing/company' },
    { label: t('sidebar.tripAuditing') },
  ]))

  const isErrorOpen = computed({
    get: () => saveError.value !== null,
    set: (open: boolean) => {
      if (!open) saveError.value = null
    },
  })

  function onSuccessModelUpdate(open: boolean): void {
    if (!open) onSuccessClose()
  }

  onMounted(fetchTrip)
  onUnmounted(tripStore.resetTrip)
</script>

<style scoped>

</style>
