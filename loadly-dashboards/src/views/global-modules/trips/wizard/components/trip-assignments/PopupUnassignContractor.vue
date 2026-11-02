<template>
  <BasePopup
    v-model="model"
    :show-close-button="false"
    content-class="max-w-md!"
  >
    <div class="flex w-full flex-col items-center p-4 text-center">
      <span class="flex size-12 items-center justify-center rounded-xl bg-error-50">
        <IconTrash class="size-6 text-error-600" />
      </span>

      <h2 class="mt-4 text-2xl font-bold text-error-600">
        {{ $t('error.unassignContractor') }}
      </h2>

      <p class="mt-2 text-content-secondary">
        {{ $t('error.unassignContractorConfirmation') }}
      </p>

      <BaseActions
        container-class="mt-6 grid w-full grid-cols-2 gap-3"
        :prev-button-options="prevButtonOptions"
        :next-button-options="nextButtonOptions"
        @prev="model = false"
        @next="emit('confirm')"
      />
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  const emit = defineEmits<{
    confirm: []
  }>()

  const { t } = useI18n()
  const model = defineModel<boolean>('modelValue', { required: true })

  const prevButtonOptions = computed(() => ({
    label: t('button.cancel'),
    class: 'btn-secondary-outline h-13.5 w-full',
  }))

  const nextButtonOptions = computed(() => ({
    label: t('button.yesIKnow'),
    class: 'btn-primary h-13.5 w-full',
  }))
</script>

<style scoped>

</style>
