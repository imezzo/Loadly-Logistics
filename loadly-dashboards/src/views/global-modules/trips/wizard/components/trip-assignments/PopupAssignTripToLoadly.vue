<template>
  <BasePopup
    v-model="model"
    :show-close-button="false"
    content-class="max-w-md!"
  >
    <div class="flex w-full flex-col items-center p-4 text-center">
      <IconAlert class="size-16 text-warning-500" />

      <h2 class="mt-4 text-2xl font-bold text-warning-500">
        {{ $t('error.assignTripToLoadly') }}
      </h2>

      <p class="mt-2 text-content-secondary">
        {{ $t('error.assignTripToLoadlyConfirmation', { name: contractorName }) }}
      </p>

      <BaseActions
        container-class="mt-6 grid w-full grid-cols-2 gap-3"
        :prev-button-options="prevButtonOptions"
        :next-button-options="nextButtonOptions"
        @prev="onCancel"
        @next="emit('confirm')"
      />
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  defineProps<{
    contractorName: string
  }>()

  const emit = defineEmits<{
    confirm: []
    cancel: []
  }>()

  const { t } = useI18n()
  const model = defineModel<boolean>('modelValue', { required: true })

  const prevButtonOptions = computed(() => ({
    label: t('button.cancel'),
    class: 'btn-secondary-outline h-13.5 w-full',
  }))

  const nextButtonOptions = computed(() => ({
    label: t('button.okChooseLoadly'),
    class: 'btn-primary h-13.5 w-full',
  }))

  function onCancel(): void {
    model.value = false
    emit('cancel')
  }
</script>

<style scoped>

</style>
