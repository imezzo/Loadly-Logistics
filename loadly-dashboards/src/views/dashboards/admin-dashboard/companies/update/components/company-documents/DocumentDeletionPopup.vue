<template>
  <BasePopup
    v-model="model"
    :show-close-button="false"
    content-class="max-w-3/4! md:max-w-200!"
  >
    <div class="flex w-full flex-col items-center px-6 py-4">
      <div class="mb-6 flex flex-col items-center">
        <div class="mb-4 flex items-center justify-center">
          <IconColoredTrash />
        </div>
        <h2 class="text-2xl font-bold text-neutral-800">
          {{ $t('content.documentDeletion') }}
        </h2>
      </div>

      <p class="mb-6 text-center text-neutral-600">
        {{ $t('content.documentDeletionWarning') }}
      </p>

      <div class="mb-6 w-full">
        <label class="mb-2 block text-sm font-medium text-neutral-10">
          {{ $t('form.reasonLogging') }}
        </label>
        <BaseTextarea
          v-model="reason"
          :wrapper-class="'w-full'"
          placeholder=""
          rows="4"
        />
      </div>

      <BaseActions
        container-class="flex w-full items-center justify-end gap-3"
        :prev-button-options="{ label: t('button.discard') }"
        :next-button-options="nextButtonOptions"
        @prev="discard"
        @next="onDelete"
      />
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  const model = defineModel<boolean>('modelValue', { required: true })

  const emit = defineEmits<{
    delete: [reason?: string]
  }>()

  const { t } = useI18n()

  const reason = ref<string>('')

  const nextButtonOptions = computed(() => ({
    label: t('button.delete'),
    class: 'flex h-13.5 items-center justify-center rounded-2xl bg-error-600 px-6 py-3 text-sm font-medium text-white transition-colors hover:bg-error-500 md:w-52.5',
  }))

  function onDelete(): void {
    emit('delete', reason.value || undefined)
    resetForm()
    model.value = false
  }

  function discard(): void {
    resetForm()
    model.value = false
  }

  function resetForm(): void {
    reason.value = ''
  }

  watch(model, (newValue) => {
    if (!newValue) {
      resetForm()
    }
  })
</script>

<style scoped>

</style>

