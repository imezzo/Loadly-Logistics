<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <h1 class="mb-6 text-xl font-bold text-neutral-800">
      {{ pageTitle }}
    </h1>

    <DriverForm />
  </div>
</template>

<script setup lang="ts">
  import DriverForm from './components/DriverForm.vue'

  const { t } = useI18n()
  const route = useRoute()

  const isUpdateMode = computed(() => route.name === 'DriverEdit')


  const pageTitle = computed(() =>
    isUpdateMode.value ? t('title.editDriver') : t('title.addNewDriver')
  )
  const breadcrumbItems = computed(() => [
    { label: t('title.drivers'), to: '/drivers' },
    { label: pageTitle.value },
  ])
</script>

<style scoped>

</style>
