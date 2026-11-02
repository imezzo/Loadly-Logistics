<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <h1 class="mb-6 text-xl font-bold text-neutral-800">
      {{ pageTitle }}
    </h1>

    <BroadcastNotificationForm />
  </div>
</template>

<script setup lang="ts">
  import BroadcastNotificationForm from './components/BroadcastNotificationForm.vue'
  import type { BreadcrumbItem } from '@/types/ui'

  const { t } = useI18n()
  const route = useRoute()

  const isViewMode = computed(() => route.name === 'ViewBroadcastNotification')

  const pageTitle = computed(() =>
    isViewMode.value ? t('title.viewNotification') : t('title.sendNotification'),
  )

  const breadcrumbItems = computed<BreadcrumbItem[]>(() => [
    { label: t('title.notificationsCenter'), to: '/notifications' },
    { label: pageTitle.value },
  ])
</script>

<style scoped>

</style>
