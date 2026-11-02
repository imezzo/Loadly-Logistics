<template>
  <BasePopup
    v-model="model"
    :show-close-button="false"
    content-class="max-w-2xl"
  >
    <div class="flex w-full flex-col px-3.25 py-2 text-center">
      <p class="text-2xl font-bold text-neutral-10">
        {{ $t('content.contractVehicleSlotEmptyConfirmTitle') }}
      </p>

      <p class="mt-4 text-content-secondary">
        {{ $t('content.contractVehicleSlotEmptyConfirmDescription') }}
      </p>

      <BaseActions
        container-class="mt-6 flex flex-wrap items-center justify-center gap-3"
        :prev-button-options="{ label: $t('button.cancel') }"
        :next-button-options="{ label: $t('button.continue') }"
        @prev="model = false"
        @next="onConfirm"
      />
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  const model = defineModel<boolean>('modelValue', { required: true })

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
