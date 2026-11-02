<template>
  <BasePopup v-model="model">
    <div class="flex flex-col items-center justify-center gap-3 py-5">
      <IconInfo class="size-15 text-error-600" />

      <p class="text-2xl font-bold text-neutral-10">
        {{ $t('content.confirmTripAuditSaveTitle') }}
      </p>

      <p class="mt-4 text-content-secondary">
        {{ $t('content.confirmTripAuditSaveDescription') }}
      </p>

      <BaseActions
        container-class="mt-4 grid grid-cols-2 gap-3"
        :prev-button-options="prevButtonOptions"
        :next-button-options="nextButtonOptions"
        @prev="model = false"
        @next="onConfirm"
      />
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  const { t } = useI18n()
  const model = defineModel<boolean>('modelValue', { required: true })

  const emit = defineEmits<{
    confirm: []
  }>()

  const prevButtonOptions = computed(() => ({
    label: t('button.cancel'),
    class: 'btn-secondary h-13.5',
  }))

  const nextButtonOptions = computed(() => ({
    label: t('button.yesSave'),
    class: 'btn-primary h-13.5',
  }))

  function onConfirm(): void {
    model.value = false
    emit('confirm')
  }
</script>

<style scoped>

</style>
