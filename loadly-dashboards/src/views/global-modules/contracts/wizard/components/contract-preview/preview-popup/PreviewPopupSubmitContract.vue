<template>
  <BasePopup
    v-model="model"
    content-class="max-w-2xl bg-success-50 [&>div]:bg-success-50 [&>div]:px-10"
  >
    <div class="flex w-full flex-col px-3.25 py-2 text-center">
      <p class="text-2xl font-bold text-neutral-10">
        {{ $t('content.submitContractForApprovalTitle') }}
      </p>

      <ul class="mt-6 flex flex-col gap-3 text-start text-neutral-10">
        <li class="flex items-center gap-3">
          <IconCheckCircle class="size-5 shrink-0 text-success-500" />
          {{ $t('content.submitContractReadyForApproval') }}
        </li>
        <li class="flex items-center gap-3">
          <IconCheckCircle class="size-5 shrink-0 text-success-500" />
          {{ $t('content.submitContractWaitingForApprovers') }}
        </li>
        <li class="flex items-center gap-3">
          <IconCheckCircle class="size-5 shrink-0 text-success-500" />
          {{ $t('content.submitContractEstimatedApprovalTime') }}
        </li>
      </ul>

      <BaseActions
        container-class="mt-6 flex flex-wrap items-center justify-center gap-3"
        :prev-button-options="prevButtonOptions"
        :next-button-options="nextButtonOptions"
        @prev="model = false"
        @next="onConfirm"
      />
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import BasePopup from '@/components/base/BasePopup.vue'
  import IconCheckCircle from '@/components/icon/IconCheckCircle.vue'

  const { t } = useI18n()
  const model = defineModel<boolean>('modelValue', { required: true })

  const emit = defineEmits<{
    confirm: []
  }>()

  const prevButtonOptions = computed(() => ({
    label: t('button.cancel'),
  }))

  const nextButtonOptions = computed(() => ({
    label: t('button.yesSubmit'),
    class: 'flex h-13.5 items-center justify-center gap-2 rounded-2xl bg-success-500 px-6 py-3 text-sm font-semibold text-white transition-colors hover:bg-success-500/90 px-30',
  }))

  function onConfirm(): void {
    model.value = false
    emit('confirm')
  }
</script>

<style scoped>

</style>
