<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <TransferListHeader @on-create-transfer="openWizard()" />
    <TransferListFilter />
    <TransferListTable @edit="openWizard" />

    <WizardTransferPopup
      v-if="isWizardPopupOpen"
      v-model="isWizardPopupOpen"
      :transfer-id="selectedTransferId"
      @close="selectedTransferId = null"
    />
  </div>
</template>

<script setup lang="ts">
  import TransferListHeader from './components/TransferListHeader.vue'
  import TransferListFilter from './components/TransferListFilter.vue'
  import TransferListTable from './components/TransferListTable.vue'
  import { useTransferList } from './composables/useTransferList'
  import { useBankTransferStore } from '@/stores/financial/bank-transfer'

  const WizardTransferPopup = defineAsyncComponent(() => import('./components/WizardTransferPopup.vue'))

  const { t } = useI18n()
  const route = useRoute()
  const { fetchTransfers } = useTransferList()

  const bankTransferStore = useBankTransferStore()

  const isWizardPopupOpen = ref(false)
  const selectedTransferId = ref<number | null>(null)

  function openWizard(transferId?: number): void {
    selectedTransferId.value = transferId ?? null
    isWizardPopupOpen.value = true
  }

  const breadcrumbItems = computed(() => [
    { label: t('title.financialManagement'), to: '/financial' },
    { label: t('title.transfer') },
  ])

  onMounted(fetchTransfers)

  onUnmounted(() => {
    if (!route.path.includes('/financial')) {
      bankTransferStore.resetFilters()
    }
  })
</script>

<style scoped>

</style>
