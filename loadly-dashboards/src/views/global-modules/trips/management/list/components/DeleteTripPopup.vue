<template>
  <BasePopup
    v-model="model"
    :show-close-button="false"
    content-class="max-w-md!"
  >
    <div class="flex w-full flex-col items-center p-4">
      <span class="flex size-12 items-center justify-center rounded-xl bg-error-50">
        <IconTrash class="size-6 text-error-600" />
      </span>

      <h2 class="mt-4 text-2xl font-bold text-error-600">
        {{ $t('error.deleteTrip') }}
      </h2>

      <p class="mt-2 text-content-secondary">
        {{ message ? message : $t('error.deleteTripConfirmation') }}
      </p>

      <span
        v-if="isLoading"
        class="loader mt-4"
      />

      <BaseActions
        container-class="mt-6 grid w-full grid-cols-2 gap-3"
        :prev-button-options="prevButtonOptions"
        :next-button-options="message ? undefined : nextButtonOptions"
        @prev="cancel"
        @next="$emit('confirm')"
      />
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  const props = defineProps<{
    isLoading?: boolean
    message?: string
  }>()

  const emit = defineEmits<{
    confirm: []
    cancel: []
  }>()

  const { t } = useI18n()
  const model = defineModel<boolean>({ required: true })

  const prevButtonOptions = computed(() => ({
    label: t('button.cancel'),
    class: 'btn-secondary-outline h-13.5 w-full',
  }))

  const nextButtonOptions = computed(() => ({
    label: t('button.yesDelete'),
    class: 'btn-error h-13.5 w-full',
    disabled: props.isLoading,
  }))

  function cancel(): void {
    model.value = false
    emit('cancel')
  }
</script>

<style scoped>

</style>
