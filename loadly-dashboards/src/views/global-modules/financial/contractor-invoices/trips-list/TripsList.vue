<template>
  <div class="page-content">
    <BaseBreadcrumb :items="breadcrumbItems" />

    <ListHeader :contractor-name="contractorName" />

    <ContractorTripStats
      v-if="statistics"
      :contractor-name="contractorName"
      :statistics="statistics"
    />

    <ListTabs />

    <RouterView />
  </div>
</template>

<script setup lang="ts">
  import ListHeader from './components/ListHeader.vue'
  import ContractorTripStats from './components/ContractorTripStats.vue'
  import ListTabs from './components/ListTabs.vue'
  import { useContractorTrips } from './composables/useContractorTrips'

  const { t } = useI18n()
  const {
    contractorName,
    fetchStatistics,
    statistics,
  } = useContractorTrips()

  const breadcrumbItems = computed(() => [
    { label: t('title.financialManagement'), to: '/financial' },
    { label: t('title.contractorInvoices'), to: '/financial/contractor-invoices' },
    { label: contractorName.value || '…' },
  ])

  onMounted(fetchStatistics)
</script>

<style scoped>

</style>
