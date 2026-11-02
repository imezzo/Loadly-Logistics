<template>
  <BaseBreadcrumb
    :items="breadcrumbItems"
    class="mb-2"
  />

  <div
    v-if="showVehicleFleetStatus"
    class="mb-6 flex justify-end"
  >
    <BaseLink
      :to="vehicleFleetStatusRoute"
      class="
        inline-flex shrink-0 items-center gap-2 rounded-full bg-primary px-4 py-2 text-sm
        font-medium text-white transition-colors
        hover:bg-primary/90
      "
    >
      {{ $t('button.vehicleByDays') }}
      <IconTruck class="size-4" />
    </BaseLink>
  </div>

  <div class="mb-6 flex items-center justify-between gap-4">
    <h1 class="text-xl font-bold text-neutral-800">
      {{ pageTitle }}
    </h1>

    <div class="flex flex-col items-end gap-3">
      <BaseChip
        v-if="(isUpdateMode || isViewMode) && contract"
        :label="$t('content.contractStatusLabel', {
          status: getDisplayName('ContractStatus', contract.status)
        })"
        :variant="contractStatusVariants[Number(contract.status)]"
      />

      <ContractNumber
        v-if="(isUpdateMode || isViewMode) && contract"
        :contract-number="contract?.referenceNumber.toString()"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
  import ContractNumber from './ContractNumber.vue'
  import { useContractWizard } from '../composables/useContractWizard'
  import { ContractType } from '@/types/admin-dashboard/response/contract/contract-details'
  import { useContractStore } from '@/stores/admin-dashboard/contract'
  import { usePermission } from '@/composables/admin-dashboard/usePermission'
  import { Contract, Permission } from '@/types/response/permission-group'
  import { contractStatusVariants } from '@/utils/statusChipVariants.ts'
  import { useEnum } from '@/composables/useEnum'

  const { t } = useI18n()
  const route = useRoute()
  const { isUpdateMode, isViewMode } = useContractWizard()
  const { hasPermission: hasViewSlotActivityPermission } = usePermission(
    Permission.Contract,
    Contract.ViewSlotActivity,
  )
  const { getDisplayName } = useEnum()

  const contractStore = useContractStore()
  const { contract } = storeToRefs(contractStore)

  const breadcrumbItems = computed(() => [
    { label: t('title.contracts'), to: '/contracts' },
    { label: isUpdateMode.value
        ? t('title.updateContract')
        : t('title.createNewContracts')
    },
  ])

  const pageTitle = computed(() => {
    if ((isUpdateMode.value || isViewMode.value) && contract.value?.nameEn) {
      return contract.value.nameEn as string
    }

    return isUpdateMode.value
        ? t('title.updateContract')
        : t('title.addNewContract')
  })

  const showVehicleFleetStatus = computed(() =>
    hasViewSlotActivityPermission.value
    && (isUpdateMode.value || isViewMode.value)
    && (contract.value?.contractType === ContractType.Included
    || contract.value?.contractType === ContractType.Excluded
  ))

  const vehicleFleetStatusRoute = computed(() => ({
    name: 'ContractSlotsStatus',
    params: { id: route.params.id },
    query: { returnTo: String(route.name ?? 'ViewContract') },
  }))
</script>

<style scoped>

</style>
