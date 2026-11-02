<template>
  <div
    class="
      mb-6 flex flex-col gap-4
      sm:flex-row sm:items-center sm:justify-between
    "
  >
    <h1 class="text-xl font-bold text-neutral-800">
      {{ $t('title.financialManagement') }}
    </h1>
  </div>

  <div class="mb-8 flex items-start justify-between gap-4">
    <div>
      <h2 class="text-xl font-bold text-neutral-800">
        {{ companyName }}
      </h2>

      <p class="mt-4 text-sm text-neutral-800">
        {{ contract?.referenceNumber || '-' }}
      </p>
    </div>

    <template v-if="isAdminDashboard">
      <button
        class="btn-primary-outline ms-auto"
        :disabled="!hasManageTripAction"
        @click="hasManageTripAction && (isManageTripChargePopupOpen = true)"
      >
        {{ $t('button.manageTripCharge') }}
      </button>

      <BaseDropdown
        :options="invoiceOptions"
        :disabled="!hasCreateInvoiceAction || (!hasClaimInvoices && !hasTripInvoices)"
      >
        <template #icon>
          {{ $t('button.createInvoice') }}
          <IconReceipt class="size-4" />
        </template>
      </BaseDropdown>
    </template>
  </div>

  <ManageTripChargePopup
    v-if="isManageTripChargePopupOpen"
    v-model="isManageTripChargePopupOpen"
    :trip-ids="selectedTripsIds"
  />
</template>

<script setup lang="ts">
  import type { ContractDetails } from '@/types/admin-dashboard/response/contract/contract-details'
  import { getLocalizedValue } from '@/utils/i18n'
  import { useApp } from '@/composables/useApp'
  import { useTripStore } from '@/stores/admin-dashboard/trip'

  const ManageTripChargePopup = defineAsyncComponent(() => import('./ManageTripChargePopup.vue'))

  const props = defineProps<{
    contract: ContractDetails | null
    selectionSynced: boolean
  }>()

  const tripStore = useTripStore()
  const { tripInvoiceFilters: filters, selectedTripsIds, tripsInvoiceSummary } = storeToRefs(tripStore)

  const route = useRoute()
  const { t } = useI18n()
  const { isAdminDashboard } = useApp()

  const isManageTripChargePopupOpen = ref(false)

  const companyName = computed(() => props.contract?.company
    ? getLocalizedValue(props.contract.company.nameEn, props.contract.company.nameAr)
    : '-')

  const hasManageTripAction = computed(() => {
    return props.selectionSynced &&
      filters.value.financialDateFrom &&
      filters.value.financialDateTo &&
      selectedTripsIds.value.length &&
      !tripsInvoiceSummary.value?.claimInvoiceIds.length &&
      !tripsInvoiceSummary.value?.tripInvoiceIds.length
  })

  const hasCreateInvoiceAction = computed(() => {
    return props.selectionSynced &&
      filters.value.financialDateFrom &&
      filters.value.financialDateTo &&
      selectedTripsIds.value.length
  })

  const hasClaimInvoices = computed(() => {
    return !tripsInvoiceSummary.value?.claimInvoiceIds.length &&
      tripsInvoiceSummary.value?.claimInvoice.totalBillable
  })

  const hasTripInvoices = computed(() => {
    return !tripsInvoiceSummary.value?.tripInvoiceIds.length &&
      tripsInvoiceSummary.value?.tripInvoice.totalBillable
  })

  const invoiceOptions = computed(() => {
    return [
      ...(hasClaimInvoices.value ? [{
        label: t('button.createClaimInvoice'),
        to: {
          name: 'FinancialInvoiceWizard',
          params: {
            id: route.params.id,
            invoiceType: 'claim-wizard',
          }
        },
      }] : []),
      ...(hasTripInvoices.value ? [{
        label: t('button.createTripInvoice'),
        to: {
          name: 'FinancialInvoiceWizard',
          params: {
            id: route.params.id,
            invoiceType: 'trip-wizard',
          }
        },
      }] : []),
    ]
  })
</script>

<style scoped>

</style>
