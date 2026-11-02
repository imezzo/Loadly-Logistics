<template>
  <div
    class="
      mb-6 flex flex-col gap-4
      sm:flex-row sm:items-center sm:justify-between
    "
  >
    <h1 class="text-xl font-bold text-neutral-800">
      {{ $t('title.reviewMonthlyContracts') }}
    </h1>

    <button
      v-if="hasCreatePermission"
      type="button"
      class="btn-primary"
      @click="isReviewWizardPopupOpen = true"
    >
      {{ $t('button.addNewReview') }}
      <IconPlus class="size-5" />
    </button>

    <ReviewWizardPopup
      v-model="isReviewWizardPopupOpen"
      @refresh="emit('refresh')"
    />
  </div>
</template>

<script setup lang="ts">
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { ContractBillingSnapshot, Permission } from '@/types/response/permission-group'

  const ReviewWizardPopup = defineAsyncComponent(() => import('./ReviewWizardPopup.vue'))

  const emit = defineEmits<{
    refresh: []
  }>()

  const { hasPermission: hasCreatePermission } = usePermission(
    Permission.ContractBillingSnapshot,
    ContractBillingSnapshot.Create,
  )

  const isReviewWizardPopupOpen = ref(false)
</script>

<style>

</style>
