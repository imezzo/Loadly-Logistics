<template>
  <BasePopup
    v-model="model"
    :show-close-button="false"
    content-class="max-w-2xl"
  >
    <div class="flex w-full flex-col px-3.25 py-2 text-center">
      <p class="text-2xl font-bold text-neutral-10">
        {{ $t('content.saveAsDraftTitle') }}
      </p>

      <p class="mt-4 text-content-secondary">
        {{ saveDraftMessage }}
      </p>

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

  const props = defineProps<{
    contractReferenceNumber?: string
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })
  const { t } = useI18n()

  const prevButtonOptions = computed(() => ({
    label: t('button.cancel'),
  }))

  const nextButtonOptions = computed(() => ({
    label: t('button.yesSave'),
    class: 'flex h-13.5 items-center justify-center gap-2 rounded-2xl bg-success-500 px-6 py-3 text-sm font-semibold text-white transition-colors hover:bg-success-500/90 px-30',
  }))

  const saveDraftMessage = computed(() =>
    props.contractReferenceNumber
      ? t('content.saveAsDraftConfirmation', { referenceNumber: props.contractReferenceNumber })
      : t('content.saveAsDraftConfirmationNew')
  )

  const emit = defineEmits<{
    confirm: []
  }>()

  function onConfirm(): void {
    model.value = false
    emit('confirm')
  }
</script>

<style scoped>

</style>
