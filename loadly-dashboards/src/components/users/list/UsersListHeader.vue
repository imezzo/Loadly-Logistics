<template>
  <BaseBreadcrumb :items="breadcrumbItems" />

  <div
    class="
      mt-2 mb-4 flex flex-col gap-4
      sm:flex-row sm:items-center sm:justify-between
    "
  >
    <h1 class="text-xl text-neutral-800">
      {{ title ?? $t('title.users') }}
    </h1>

    <BaseLink
      v-if="hasCreatePermission"
      :to="{ name: createRouteName ?? 'CreateUser' }"
      class="btn-primary"
    >
      {{ buttonLabel ?? $t('button.addUser') }}

      <IconPlus />
    </BaseLink>
  </div>
</template>

<script setup lang="ts">
  import type { BreadcrumbItem } from '@/types/ui'

  const props = defineProps<{
    breadcrumb?: BreadcrumbItem[]
    title?: string
    createRouteName?: string
    buttonLabel?: string
    hasCreatePermission?: boolean
  }>()

  const { t } = useI18n()

  const breadcrumbItems = computed<BreadcrumbItem[]>(() => props.breadcrumb ?? [
    { label: t('title.users'), to: '/users' },
    { label: t('title.userList') },
  ])
</script>

<style scoped>

</style>

