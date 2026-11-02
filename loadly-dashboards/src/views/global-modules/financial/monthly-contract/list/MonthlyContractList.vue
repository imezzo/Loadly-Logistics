<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <ListHeader @refresh="fetchMonthlyContracts" />
    <ListFilters />
    <ListTable />
  </div>
</template>

<script setup lang="ts">
  import ListFilters from './components/ListFilters.vue'
  import ListHeader from './components/ListHeader.vue'
  import ListTable from './components/ListTable.vue'
  import { useBillingSnapshotsList } from './composables/useBillingSnapshotsList.ts'

  const { t } = useI18n()
  const {
    currentPage,
    itemsPerPage,
    fetchMonthlyContracts,
  } = useBillingSnapshotsList()

  const breadcrumbItems = computed(() => ([
    { label: t('sidebar.financialReports') },
    { label: t('title.reviewMonthlyContracts') },
  ]))

  watch([currentPage, itemsPerPage], () => {
    fetchMonthlyContracts()
  })

  onMounted(fetchMonthlyContracts)
</script>

<style>

</style>
