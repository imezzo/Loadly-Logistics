<template>
  <section class="rounded-xl border border-neutral-200 bg-white p-4">
    <h2 class="mb-6 text-lg font-bold text-neutral-10">
      {{ $t('content.approvalHistory') }}
    </h2>

    <div
      v-for="status in approvalHistory"
      :key="status.id"
      class="mb-4 flex flex-wrap items-start justify-between gap-4"
    >
      <div class="min-w-0 shrink-0">
        <p class="font-medium text-neutral-10">
          {{ status.stageName }}
        </p>

        <p
          v-if="status.approverName"
          class="text-sm text-content-secondary"
        >
          {{ $t('content.user') }}: {{ status.approverName }}
        </p>

        <p
          v-if="status.comments"
          class="mt-2 text-sm text-content-secondary"
        >
          {{ $t('content.reason') }}: {{ status.comments }}
        </p>
      </div>

      <BaseChip
        :label="status.statusName"
        :variant="contractApprovalStatusVariants[Number(status.status)]"
      />
    </div>

    <div
      v-if="!approvalHistory.length"
      class="text-sm text-neutral-500"
    >
      {{ $t('content.noData') }}
    </div>
  </section>

  <BaseActions
    :prev-button-options="{ hasScrollTop: true }"
    :next-button-options="{ hasScrollTop: true }"
    @prev="emit('prev')"
    @next="emit('next')"
  />
</template>

<script setup lang="ts">
  import { useContractStore } from '@/stores/admin-dashboard/contract'
  import { contractApprovalStatusVariants } from '@/utils/statusChipVariants'

  const emit = defineEmits<{
    prev: []
    next: []
  }>()

  const contractStore = useContractStore()
  const { contract } = storeToRefs(contractStore)

  const approvalHistory = computed(() => contract.value?.approvalHistory ?? [])
</script>

<style scoped>

</style>
