<template>
  <BaseTable
    :columns="tableColumns"
    :data="rows"
  >
    <template #cell-invoiceId="{ row }">
      {{ getInvoiceNumber(row.invoiceId) }}
    </template>

    <template #cell-periodStart="{ row }">
      {{ formatDateLocal(row.periodStart) }}
    </template>

    <template #cell-periodEnd="{ row }">
      {{ formatDateLocal(row.periodEnd) }}
    </template>

    <template #cell-statusName="{ row }">
      <BaseChip
        :label="row.statusName"
        :variant="monthlyContractStatusVariants[row.status]"
      />
    </template>

    <template #cell-action="{ row }">
      <div class="flex items-center justify-end gap-2">
        <BaseLink
          v-if="row.snapshotId && hasViewPermission"
          :to="{ name: 'FinancialMonthlyContractSlotsCharges', params: { id: row.snapshotId } }"
          class="btn-info-outline"
          :aria-label="$t('a11y.view')"
        >
          <IconEye
            visible
            class="size-4"
          />
        </BaseLink>

        <template v-if="!row.invoicing.tripsInvoiceId && !row.invoicing.claimsInvoiceId">
          <button
            v-if="hasEditPermission"
            type="button"
            class="btn-info-outline"
            :aria-label="$t('a11y.edit')"
            @click="openReviewWizard(row)"
          >
            <IconEdit class="size-4" />
          </button>

          <button
            v-if="hasDeletePermission"
            type="button"
            class="btn-action-error"
            :aria-label="$t('a11y.delete')"
            @click="openDeletePopup(row)"
          >
            <IconTrash class="size-4" />
          </button>
        </template>
      </div>
    </template>
  </BaseTable>

  <BasePagination
    :pagination="pagination"
    @set-page="setPage"
    @next-page="nextPage"
    @prev-page="prevPage"
    @update-items-per-page="updateItemsPerPage"
  />

  <ReviewWizardPopup
    v-model="isReviewWizardPopupOpen"
    :review="selectedReview"
    @refresh="fetchMonthlyContracts"
  />

  <PopupDelete
    v-if="isDeletePopupOpen"
    v-model="isDeletePopupOpen"
    :is-loading="isLoading"
    :title="$t('error.deleteMonthlyContractReview')"
    :message="deletePopupMessage"
    @confirm="deleteMonthlyContract"
  />
</template>

<script setup lang="ts">
  import type { MonthlyContract } from '@/types/admin-dashboard/response/contract/monthly-contract/monthly-contract'
  import type { TableColumn } from '@/types/ui'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { useLoading } from '@/composables/useLoading'
  import { ContractBillingSnapshot, Permission } from '@/types/response/permission-group'
  import { useMonthlyContractStore } from '@/stores/admin-dashboard/monthly-contract'
  import { formatDateLocal } from '@/utils/date'
  import { useBillingSnapshotsList } from '../composables/useBillingSnapshotsList'
  import { monthlyContractStatusVariants } from '@/utils/statusChipVariants'

  const ReviewWizardPopup = defineAsyncComponent(() => import('./ReviewWizardPopup.vue'))

  const { t } = useI18n()
  const { isLoading, setLoading } = useLoading()
  const { hasPermission: hasViewPermission } = usePermission(
    Permission.ContractBillingSnapshot,
    ContractBillingSnapshot.View,
  )
  const { hasPermission: hasEditPermission } = usePermission(
    Permission.ContractBillingSnapshot,
    ContractBillingSnapshot.Edit,
  )
  const { hasPermission: hasDeletePermission } = usePermission(
    Permission.ContractBillingSnapshot,
    ContractBillingSnapshot.Delete,
  )
  const monthlyContractStore = useMonthlyContractStore()
  const {
    fetchMonthlyContracts,
    nextPage,
    pagination,
    prevPage,
    rows,
    setPage,
    updateItemsPerPage,
  } = useBillingSnapshotsList()

  const isReviewWizardPopupOpen = ref(false)
  const isDeletePopupOpen = ref(false)
  const selectedReview = ref<MonthlyContract | null>(null)
  const reviewToDelete = ref<MonthlyContract | null>(null)
  const deleteErrorMessage = ref('')

  const deletePopupMessage = computed(() => {
    const confirmation = t('error.deleteMonthlyContractReviewConfirmation', {
      name: reviewToDelete.value?.name ?? '',
    })

    return deleteErrorMessage.value
      ? `${confirmation} ${deleteErrorMessage.value}`
      : confirmation
  })

  const tableColumns = computed<TableColumn[]>(() => [
    { key: 'companyName', label: t('content.company') },
    { key: 'contractName', label: t('content.contract'), cellClass: 'max-w-72 whitespace-normal' },
    { key: 'invoiceId', label: t('table.invoiceNumber') },
    { key: 'periodStart', label: t('table.dateFrom') },
    { key: 'periodEnd', label: t('table.dateTo') },
    { key: 'statusName', label: t('table.status') },
    { key: 'action', label: t('table.action') },
  ])

  function getInvoiceNumber(invoiceId: number | null): string {
    if (invoiceId == null) return '-'

    return t('content.invoiceWithNumber', { number: invoiceId })
  }

  function openReviewWizard(review: MonthlyContract): void {
    selectedReview.value = review
    isReviewWizardPopupOpen.value = true
  }

  function openDeletePopup(review: MonthlyContract): void {
    reviewToDelete.value = review
    deleteErrorMessage.value = ''
    isDeletePopupOpen.value = true
  }

  async function deleteMonthlyContract(): Promise<void> {
    const snapshotId = reviewToDelete.value?.snapshotId
    if (!snapshotId || isLoading.value) return

    deleteErrorMessage.value = ''
    setLoading(true)

    const { success, message } = await monthlyContractStore.deleteSnapshot(snapshotId)

    setLoading(false)

    if (!success) {
      deleteErrorMessage.value = message || t('error.networkError')

      return
    }

    isDeletePopupOpen.value = false
    reviewToDelete.value = null
    fetchMonthlyContracts()
  }
</script>

<style>

</style>
