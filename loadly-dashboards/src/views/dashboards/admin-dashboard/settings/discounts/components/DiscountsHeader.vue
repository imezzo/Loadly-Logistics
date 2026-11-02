<template>
  <BaseBreadcrumb :items="breadcrumbItems" />

  <div
    class="
      my-4 flex flex-col gap-4
      sm:flex-row sm:justify-between
    "
  >
    <h1 class="text-xl font-bold text-neutral-800">
      {{ $t('title.discounts') }}
    </h1>

    <button
      v-if="hasCreatePermission"
      type="button"
      class="btn-primary"
      @click="$emit('openWizard')"
    >
      {{ $t('button.addDiscount') }}
      <IconPlus />
    </button>
  </div>
</template>

<script setup lang="ts">
  import { Permission, Discount as DiscountPermission } from '@/types/response/permission-group'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'

  defineEmits<{
    openWizard: []
  }>()

  const { t } = useI18n()

  const {
    hasPermission: hasCreatePermission,
  } = usePermission(Permission.Discount, DiscountPermission.Create)

  const breadcrumbItems = [
    { label: t('title.settings'), to: '/settings' },
    { label: t('title.discounts') },
  ]
</script>

<style scoped>

</style>
