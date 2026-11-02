<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <h1 class="mb-6 text-xl font-bold text-neutral-800">
      {{ pageTitle }}
    </h1>

    <ContractorForm />
  </div>
</template>

<script setup lang="ts">
  import ContractorForm from './components/ContractorForm.vue'
  import type { BreadcrumbItem } from '@/types/ui'

  const { t } = useI18n()
  const route = useRoute()

  const isEditMode = computed(() => route.name === 'EditContractor')
  const isViewMode = computed(() => route.name === 'ViewContractor')

  const pageTitle = computed(() => {
    if (isViewMode.value) return t('title.viewContractor')

    return isEditMode.value ? t('title.editContractor') : t('title.addContractor')
  })

  const breadcrumbItems = computed<BreadcrumbItem[]>(() => [
    { label: t('title.contractors'), to: '/contractors' },
    { label: pageTitle.value },
  ])
</script>

<style scoped>

</style>
