<template>
  <BaseBreadcrumb
    :items="breadcrumbItems"
    class="mb-2"
  />

  <div
    class="
      mb-6 flex flex-col gap-4
      sm:flex-row sm:items-center sm:justify-between
    "
  >
    <h1 class="text-xl font-bold text-neutral-800">
      {{ $t('title.notificationsCenter') }}
    </h1>

    <RouterLink
      :to="{ name: 'CreateBroadcastNotification' }"
      class="btn-primary-outline"
    >
      {{ $t('button.sendNotification') }}
      <IconSend />
    </RouterLink>
  </div>
</template>

<script setup lang="ts">
  const { t } = useI18n()

  const breadcrumbItems = [
    { label: t('title.notificationsCenter'), to: '/notifications' },
    { label: t('title.notificationsCenter') },
  ]
</script>

<style scoped>

</style>
