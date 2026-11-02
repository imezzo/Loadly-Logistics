<template>
  <div class="page-content">
    <div
      class="
        mb-2 flex flex-col gap-3
        sm:flex-row sm:items-center sm:justify-between
      "
    >
      <BaseBreadcrumb :items="breadcrumbItems" />

      <SlotsChargesExport :snapshot-id="snapshotId" />
    </div>

    <BaseAlert
      :model-value="!!errorMessage"
      :message="errorMessage"
      variant="error"
      class="mb-4"
    />

    <section
      v-if="monthlyContract"
      class="
        rounded-xl p-3
        md:p-2.5
      "
    >
      <div
        class="
          mb-6 flex flex-col gap-3
          sm:flex-row sm:items-center sm:justify-between
        "
      >
        <div class="min-w-0">
          <h1 class="text-lg font-bold text-neutral-800">
            {{ monthlyContract.name }}
          </h1>

          <p class="mt-1 text-sm text-content-secondary">
            {{ formatDateLocal(monthlyContract.periodStart) }}
            -
            {{ formatDateLocal(monthlyContract.periodEnd) }}
          </p>
        </div>

        <button
          v-if="hasEditActions"
          type="button"
          class="btn-primary-outline me-auto shrink-0"
          :disabled="isLoading"
          @click="isChargeTermsPopupOpen = true"
        >
          {{ $t('button.editChargeTerms') }}
        </button>

        <button
          v-if="canManageTripCharge"
          type="button"
          class="btn-primary-outline ms-auto shrink-0"
          :disabled="isLoading"
          @click="isManageTripChargePopupOpen = true"
        >
          {{ $t('button.manageTripCharge') }}
        </button>

        <BaseDropdown
          v-if="hasCreateInvoicePermission && canCreateInvoice"
          :options="invoiceOptions"
          :class="{'ms-auto': !hasEditActions && !canManageTripCharge}"
        >
          <template #icon>
            {{ $t('button.createInvoice') }}
            <IconReceipt class="size-4" />
          </template>
        </BaseDropdown>

        <BaseChip
          :label="monthlyContract.statusName"
          :variant="monthlyContractStatusVariants[monthlyContract.status]"
        />
      </div>

      <div
        v-if="tripsInvoiceId || claimsInvoiceId"
        class="
          mb-4 flex items-center gap-2 rounded-md border border-success-400 bg-success-100 p-2
          text-sm text-content-secondary
        "
      >
        {{ $t('content.reviewHasBeenInvoiced') }}

        <RouterLink
          v-if="tripsInvoiceId"
          :to="{ name: 'FinancialInvoiceView', params: { id: tripsInvoiceId }, query: invoiceViewQuery }"
          class="btn-link shrink-0"
        >
          {{ $t('button.viewInvoice') }}
        </RouterLink>

        <span v-if="tripsInvoiceId && claimsInvoiceId">|</span>

        <RouterLink
          v-if="claimsInvoiceId"
          :to="{ name: 'FinancialInvoiceView', params: { id: claimsInvoiceId }, query: invoiceViewQuery }"
          class="btn-link shrink-0"
        >
          {{ $t('button.viewClaim') }}
        </RouterLink>
      </div>

      <SlotChargesSummary
        v-if="slotCharges"
        :charges="slotCharges"
        :consumption-type="monthlyContract?.consumptionType"
        :km-pool="slotCharges.kmPool"
      />

      <SlotsChargesFilters
        v-model="vehicleId"
        :vehicle-options="vehicleOptions"
      />

      <div
        v-if="hasEditActions"
        class="mb-4 flex justify-end"
      >
        <button
          type="button"
          class="btn-primary"
          :disabled="isLoading"
          @click="updateReview"
        >
          {{ $t('button.updateReview') }}
        </button>
      </div>

      <SlotChargesTable
        :columns="tableColumns"
        :data="tableRows"
        :has-clickable-days="hasEditActions"
        @row-action="openEditPopup"
      />
    </section>

    <EditCalendarDayPopup
      v-if="monthlyContract"
      v-model="isEditPopupOpen"
      :day="selectedDay"
      :snapshot-id="snapshotId"
      :vehicle-label="selectedVehicleLabel"
      :company-id="monthlyContract.companyId"
      @refresh="onCalendarDayRefresh"
    />

    <EditChargeTermsPopup
      v-if="monthlyContract"
      v-model="isChargeTermsPopupOpen"
      :monthly-contract="monthlyContract"
      :seat-options="seatOptions"
      :snapshot-id="snapshotId"
      @refresh="fetchMonthlyContract"
    />

    <ManageTripChargePopup
      v-if="isManageTripChargePopupOpen"
      v-model="isManageTripChargePopupOpen"
      :trip-ids="tripIds"
      @refresh="getInvoiceSummary"
    />
  </div>
</template>

<script setup lang="ts">
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { useLoading } from '@/composables/useLoading'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import { useInvoiceStore } from '@/stores/financial/invoice.ts'
  import { ContractBillingSnapshot, Invoice, Permission } from '@/types/response/permission-group'
  import SlotChargesTable from '@/views/global-modules/slot-charges/components/slots-charges-table/SlotChargesTable.vue'
  import SlotsChargesExport from './components/SlotsChargesExport.vue'
  import { monthlyContractStatusVariants } from '@/utils/statusChipVariants'
  import { extractISODate, formatDateLocal, isBeforeToday } from '@/utils/date'
  import { useMonthlyContractSlotsCharges } from './composables/useSlotsCharges'
  import SlotChargesSummary from '@/views/global-modules/slot-charges/components/SlotChargesSummary.vue'
  import SlotsChargesFilters from './components/SlotsChargesFilters.vue'

  const EditCalendarDayPopup = defineAsyncComponent(() => import('./components/EditCalendarDayPopup.vue'))
  const EditChargeTermsPopup = defineAsyncComponent(() => import('./components/EditChargeTermsPopup.vue'))
  const ManageTripChargePopup = defineAsyncComponent(() => import('@/views/global-modules/financial/invoice/company/components/ManageTripChargePopup.vue'))

  const { t } = useI18n()
  const { setLoading } = useLoading()

  const { hasPermission: hasEditPermission } = usePermission(
    Permission.ContractBillingSnapshot,
    ContractBillingSnapshot.Edit,
  )
  const { hasPermission: hasCreateInvoicePermission } = usePermission(
    Permission.Invoice,
    Invoice.Create,
  )
  const {
    errorMessage,
    fetchMonthlyContract,
    isEditPopupOpen,
    isLoading,
    monthlyContract,
    openEditPopup,
    seatOptions,
    selectedDay,
    selectedVehicleLabel,
    slotCharges,
    snapshotId,
    tableColumns,
    tableRows,
    updateReview,
    vehicleId,
    vehicleOptions,
  } = useMonthlyContractSlotsCharges()

  const tripStore = useTripStore()
  const { selectedTripsIds, tripInvoiceFilters, tripsInvoiceSummary } = storeToRefs(tripStore)
  const invoiceStore = useInvoiceStore()
  const { invoice } = storeToRefs(invoiceStore)

  const isChargeTermsPopupOpen = ref(false)
  const isManageTripChargePopupOpen = ref(false)

  const hasEditActions = computed(() => hasEditPermission.value && !monthlyContract.value?.invoiceId)

  const tripsInvoiceId = computed(() => monthlyContract.value?.invoicing.tripsInvoiceId)
  const claimsInvoiceId = computed(() => monthlyContract.value?.invoicing.claimsInvoiceId)

  const invoiceViewQuery = computed(() => ({
    snapshotId: snapshotId.value,
    returnTo: 'FinancialMonthlyContractSlotsCharges',
  }))

  const tripIds = computed(() => {
    const ids = new Set<string>()

    monthlyContract.value?.calculation.calendar.forEach(slot => {
      slot.days.forEach(day => {
        day.tripIds?.map(id => String(id).replace(/\D/g, ''))
          .forEach(id => ids.add(id))
      })
    })

    return [...ids]
  })

  const canManageTripCharge = computed(() =>
    hasEditPermission.value
    && selectedTripsIds.value.length
    && !tripsInvoiceSummary.value?.claimInvoiceIds.length
    && !tripsInvoiceSummary.value?.tripInvoiceIds.length
  )

  const hasCreateInvoiceAction = computed(() =>
    Boolean(
      tripInvoiceFilters.value.financialDateFrom
      && tripInvoiceFilters.value.financialDateTo
      && selectedTripsIds.value.length,
    ),
  )

  const hasClaimInvoices = computed(() => !tripsInvoiceSummary.value?.claimInvoiceIds.length)

  const hasTripInvoices = computed(() => !tripsInvoiceSummary.value?.tripInvoiceIds.length)

  const canCreateInvoice = computed(() =>
    !isLoading.value
    && selectedTripsIds.value.length
    && hasCreateInvoiceAction.value
    && (hasClaimInvoices.value || hasTripInvoices.value),
  )

  const invoiceOptions = computed(() => [
    ...(hasClaimInvoices.value ? [{
      label: t('button.createClaimInvoice'),
      to: {
        name: 'FinancialInvoiceWizard',
        params: {
          id: monthlyContract.value?.contractId,
          invoiceType: 'claim-wizard',
        },
        query: {
          snapshotId: monthlyContract.value?.snapshotId,
          returnTo: 'FinancialMonthlyContractSlotsCharges',
        },
      },
    }] : []),
    ...(hasTripInvoices.value ? [{
      label: t('button.createTripInvoice'),
      to: {
        name: 'FinancialInvoiceWizard',
        params: {
          id: monthlyContract.value?.contractId,
          invoiceType: 'trip-wizard',
        },
        query: {
          snapshotId: monthlyContract.value?.snapshotId,
          returnTo: 'FinancialMonthlyContractSlotsCharges',
        },
      },
    }] : []),
  ])

  function onCalendarDayRefresh(): void {
    if (hasEditActions.value) {
      void updateReview()

      return
    }

    void fetchMonthlyContract()
  }

  async function getInvoiceSummary(): Promise<void> {
    if (!monthlyContract.value || !tripIds.value.length) return

    selectedTripsIds.value = [...tripIds.value]
    tripInvoiceFilters.value.financialDateFrom = extractISODate(monthlyContract.value.periodStart) ?? ''
    tripInvoiceFilters.value.financialDateTo = extractISODate(monthlyContract.value.periodEnd) ?? ''

    invoice.value.startDate = extractISODate(monthlyContract.value.periodStart) ?? ''
    invoice.value.endDate = extractISODate(monthlyContract.value.periodEnd) ?? ''
    invoice.value.contractBillingSnapshotId = monthlyContract.value.snapshotId

    setLoading(true)
    const { data, message, success } = await tripStore.getTripsInvoiceSummary(tripIds.value)
    setLoading(false)

    if (!success || !data) {
      errorMessage.value = message || t('error.networkError')

      return
    }

    tripsInvoiceSummary.value = data
  }

  onMounted(() => {
    void getInvoiceSummary()

    if (
      hasEditActions.value
      && monthlyContract.value?.periodEnd
      && !isBeforeToday(monthlyContract.value.periodEnd)
    ) {
      void updateReview()
    }
  })

  watch(monthlyContract, getInvoiceSummary)

  const breadcrumbItems = computed(() => [
    { label: t('sidebar.financialReports') },
    { label: t('title.reviewMonthlyContracts'), to: '/financial/reports/monthly-contracts' },
    { label: monthlyContract.value?.name || t('title.reviewMonthlyContracts') },
  ])
</script>

<style scoped>

</style>
