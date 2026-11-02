<template>
  <div class="page-content">
    <BaseBreadcrumb :items="breadcrumbItems" />

    <ListHeader />

    <ContractorInvoiceStats v-if="statistics" />

    <ListFilters />
    <ContractorsTable />
  </div>
</template>

<script setup lang="ts">
  import ListHeader from './components/ListHeader.vue'
  import ContractorInvoiceStats from './components/ContractorInvoiceStats.vue'
  import ListFilters from './components/ListFilters.vue'
  import ContractorsTable from './components/ContractorsTable.vue'
  import { useContractorInvoices } from './composables/useContractorInvoices'

  const { t } = useI18n()
  const { fetchList, statistics } = useContractorInvoices()

  const breadcrumbItems = [
    { label: t('title.financialManagement'), to: '/financial' },
    { label: t('title.contractorInvoices') },
  ]

  onMounted(fetchList)
</script>

<style scoped>

</style>
