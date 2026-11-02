<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <CompanyListHeader @import-click="isImportPopupOpen = true" />

    <CompanyGrid
      :companies="companies"
      :pagination="pagination"
    />

    <ImportCompaniesPopup
      v-model="isImportPopupOpen"
      @refresh="onImportRefresh"
    />
  </div>
</template>

<script setup lang="ts">
  import { useList } from './composables/useList'
  import CompanyListHeader from './components/CompanyListHeader.vue'
  import CompanyGrid from './components/CompanyGrid.vue'
  import ImportCompaniesPopup from './components/ImportCompaniesPopup.vue'

  const { t } = useI18n()

  const breadcrumbItems = [
    { label: t('title.companies'), to: '/companies' },
    { label: t('title.clients') },
  ]

  const {
    fetchCompanies,
    companies,
    pagination,
  } = useList()

  const isImportPopupOpen = ref(false)

  function onImportRefresh(): void {
    fetchCompanies()
  }

  onMounted(() => {
    fetchCompanies()
  })
</script>

<style scoped>

</style>

