<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <h1 class="mb-6 text-xl font-bold text-neutral-800">
      {{ $t('title.addNewCompanies') }}
    </h1>

    <CreateCompanyForm />
  </div>
</template>

<script setup lang="ts">
  import CreateCompanyForm from './components/CreateCompanyForm.vue'
  import { useLookups } from '@/composables/useLookups'

  const { t } = useI18n()

  const breadcrumbItems = [
    { label: t('title.companies'), to: '/companies' },
    { label: t('title.addNewCompanies') },
  ]

  const {
    getIndustryTypes,
    getCommunicationMethods,
    getServiceRequirements,
    getGovernments,
    getCities,
    getOperatingHours,
  } = useLookups()

  onMounted(async () => {
    await getIndustryTypes()
    await getCommunicationMethods()
    await getServiceRequirements()
    await getGovernments()
    await getCities()
    await getOperatingHours()
  })
</script>

<style scoped>

</style>

