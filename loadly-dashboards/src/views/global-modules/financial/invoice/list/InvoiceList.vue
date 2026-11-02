<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <ListHeader />
    <ListFilters />
    <ListTable />
  </div>
</template>

<script setup lang="ts">
  import ListHeader from './components/ListHeader.vue'
  import ListFilters from './components/ListFilters.vue'
  import ListTable from './components/ListTable.vue'
  import { useInvoiceList } from './composables/useInvoiceList'

  const { t } = useI18n()
  const { fetchContracts } = useInvoiceList()

  const breadcrumbItems = [
    { label: t('title.financialManagement'), to: '/financial' },
    { label: t('title.financialManagement') },
  ]

  onMounted(fetchContracts)
</script>

<style scoped>

</style>
