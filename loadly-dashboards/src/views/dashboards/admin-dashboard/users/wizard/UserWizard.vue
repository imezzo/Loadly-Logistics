<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <h1 class="mb-6 font-medium text-neutral-800">
      {{ isEditMode ? $t('title.editUser') : $t('title.addUsers') }}
    </h1>

    <UsersWizardForm />
  </div>
</template>

<script setup lang="ts">
  import { useUserStore } from '@/stores/admin-dashboard/user'
  import type { BreadcrumbItem } from '@/types/ui'
  import { useUserWizardForm } from '@/composables/modules/users/useUserWizardForm'

  const userStore = useUserStore()
  const { resourceUrl } = storeToRefs(userStore)
  resourceUrl.value = '/admins'

  const { t } = useI18n()
  const { fetchRoles, isEditMode } = useUserWizardForm()

  const breadcrumbItems = computed<BreadcrumbItem[]>(() => [
    { label: t('title.users'), to: '/users' },
    { label: isEditMode.value ? t('title.editUser') : t('title.addUser') },
  ])

  onMounted(() => fetchRoles(1, '', 'admin'))
</script>

<style scoped>

</style>
