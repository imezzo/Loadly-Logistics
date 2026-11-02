<template>
  <PopupSuccess
    v-model="model"
    content-class="max-w-2xl"
  >
    <template #message>
      <div class="flex w-full flex-col px-3.25 py-2 text-center">
        <p class="text-2xl font-bold text-neutral-10">
          {{ $t('content.saveTripAsDraftTitle') }}
        </p>
        <p class="mt-4 text-balance text-content-secondary">
          {{ $t('content.saveTripAsDraftDescription') }}
        </p>

        <BaseActions
          container-class="mt-5 flex w-full flex-wrap items-center justify-center gap-3"
          :prev-button-options="prevButtonOptions"
          :next-button-options="nextButtonOptions"
          @prev="model = false"
          @next="onConfirm"
        />
      </div>
    </template>
  </PopupSuccess>
</template>

<script setup lang="ts">
  const model = defineModel<boolean>('modelValue', { required: true })
  const { t } = useI18n()

  const emit = defineEmits<{
    confirm: []
  }>()

  const prevButtonOptions = computed(() => ({
    class: 'btn-error-outline h-13.5 min-w-37.5 md:w-52.5',
    label: t('button.cancel'),
  }))

  const nextButtonOptions = computed(() => ({
    class: 'btn-success h-13.5 min-w-37.5 md:w-75',
    label: t('button.save'),
  }))

  function onConfirm(): void {
    emit('confirm')
    model.value = false
  }
</script>

<style scoped>

</style>
