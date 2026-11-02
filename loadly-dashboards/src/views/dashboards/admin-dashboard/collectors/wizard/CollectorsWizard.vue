<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <h1 class="mb-6 font-medium text-neutral-800">
      {{ isEditMode ? $t('title.editCollector') : $t('title.addCollector') }}
    </h1>

    <UsersWizardForm list-route-name="CollectorsList" />
  </div>
</template>

<script setup lang="ts">
  import { useUserWizardForm } from '@/composables/modules/users/useUserWizardForm'
  import { useUserStore } from '@/stores/admin-dashboard/user'
  import type { BreadcrumbItem } from '@/types/ui'

  const userStore = useUserStore()
  const { resourceUrl } = storeToRefs(userStore)
  resourceUrl.value = '/collectors'

  const { t } = useI18n()
  const { fetchRoles, isEditMode } = useUserWizardForm()

  const breadcrumbItems = computed<BreadcrumbItem[]>(() => [
    { label: t('title.collectors'), to: '/collectors' },
    { label: isEditMode.value ? t('title.editCollector') : t('title.addCollector') },
  ])

  onMounted(() =>fetchRoles(1, '', 'collector'))
</script>

<style scoped>

</style>
