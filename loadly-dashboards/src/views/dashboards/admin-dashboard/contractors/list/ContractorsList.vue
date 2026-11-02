<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <ListHeader />
    <ListFilters />
    <ContractorsTable />
  </div>
</template>

<script setup lang="ts">
  import ListHeader from './components/ListHeader.vue'
  import ListFilters from './components/ListFilters.vue'
  import ContractorsTable from './components/ContractorsTable.vue'
  import { useList } from './composables/useList'

  const { t } = useI18n()
  const route = useRoute()
  const { fetchContractors, resetFilters } = useList()

  const breadcrumbItems = [{ label: t('title.contractors'), to: '/contractors' }]

  onMounted(fetchContractors)
  onUnmounted(() => {
    if (!route.path.includes('/contractor')) {
      resetFilters()
    }
  })
</script>

<style scoped>

</style>
