<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-4"
    />

    <DashboardReportsHeader :title="$t('sidebar.invoiceAndBillingInformation')" />
    <InvoiceBillingStatistics />
    <InvoiceReportsList />
  </div>
</template>

<script setup lang="ts">
  import InvoiceBillingStatistics from './components/InvoiceBillingStatistics.vue'
  import InvoiceReportsList from './components/InvoiceReportsList.vue'

  const { t } = useI18n()
  const breadcrumbItems = computed(() => ([
    { label: t('sidebar.dashboardReports') },
    { label: t('sidebar.invoiceAndBillingInformation') },
  ]))
</script>

<style scoped>

</style>
