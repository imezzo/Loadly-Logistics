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
  import { useContractList } from './composables/useContractList'
  import { useListFilter } from './composables/useListFilter.ts'

  const { t } = useI18n()
  const route = useRoute()
  const { fetchContracts } = useContractList()
  const { resetFilters } = useListFilter()

  const breadcrumbItems = [
    { label: t('title.contracts'), to: '/contracts' },
  ]

  onMounted(fetchContracts)

  onUnmounted(() => {
    if (!route.path.includes('/contract')) {
      resetFilters()
    }
  })
</script>

<style scoped>

</style>
