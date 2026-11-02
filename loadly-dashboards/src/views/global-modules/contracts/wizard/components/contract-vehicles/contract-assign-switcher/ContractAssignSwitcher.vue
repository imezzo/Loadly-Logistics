<template>
  <div>
    <h2 class="mb-3 font-medium text-neutral-10">
      {{ $t('content.assignmentType') }}
    </h2>

    <div class="mb-6 flex gap-6">
      <BaseRadio
        :model-value="Boolean(formData.autoAssignMode)"
        :label="$t('content.autoAssignCars')"
        :disabled="isAssignmentTypeDisabled"
        @change="changeAssignmentType(true)"
      />

      <BaseRadio
        :model-value="!Boolean(formData.autoAssignMode)"
        :label="$t('content.manualAssignCars')"
        :disabled="isAssignmentTypeDisabled"
        @change="changeAssignmentType(false)"
      />
    </div>

    <AutoAssignConfirmPopup
      v-model="isConfirmPopupOpen"
      @confirm="formData.autoAssignMode = true"
    />
  </div>
</template>

<script setup lang="ts">
  import { useContractWizard } from '../../../composables/useContractWizard.ts'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { CONTRACT_STATUS } from '@/types/admin-dashboard/contract'
  import { Contract, Permission } from '@/types/response/permission-group'

  const AutoAssignConfirmPopup = defineAsyncComponent(() => import('./components/AutoAssignConfirmPopup.vue'))

  const {
    contract,
    formData,
    isFormDisabled,
    isViewMode,
  } = useContractWizard()
  const { hasPermission: canEditActiveContract } = usePermission(Permission.Contract, Contract.EditActive)

  const isConfirmPopupOpen = ref(false)

  const isAssignmentTypeDisabled = computed(
    () =>
      isFormDisabled.value
      || isViewMode.value
      || (contract.value?.status === CONTRACT_STATUS.ACTIVE && !canEditActiveContract.value),
  )

  function changeAssignmentType(value: boolean): void {
    if (Boolean(formData.value.autoAssignMode) === value) return

    if (value) {
      isConfirmPopupOpen.value = true

      return
    }

    formData.value.autoAssignMode = value
  }
</script>

<style scoped>

</style>
