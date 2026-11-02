<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <ListHeader :filters />
    <ListFilters />
    <ListTable />
  </div>
</template>

<script setup lang="ts">
  import ListHeader from './components/ListHeader.vue'
  import ListFilters from './components/ListFilters.vue'
  import ListTable from './components/ListTable.vue'
  import { usePaymentList } from './composables/usePaymentList'

  const { t } = useI18n()
  const { fetchInvoices, filters } = usePaymentList()

  const breadcrumbItems = computed(() => ([
    { label: t('title.financialManagement'), to: '/financial' },
    { label: t('title.invoicesList') },
  ]))

  onMounted(() => {
    fetchInvoices()
  })
</script>

<style scoped>

</style>
