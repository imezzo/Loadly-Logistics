<template>
  <PopupError
    v-model="model"
  >
    <template #title>
      <p class="text-2xl font-bold text-neutral-10">
        {{ $t('error.deleteDraftTrip') }}
      </p>
    </template>

    <template #message>
      <p class="mt-4 text-content-secondary">
        {{ $t('error.deleteDraftTripConfirmation', { id: tripIdLabel }) }}
      </p>

      <BaseActions
        container-class="mt-4 flex-wrap items-center justify-center! gap-3 sm:gap-6 md:flex-nowrap"
        :prev-button-options="prevButtonOptions"
        :next-button-options="nextButtonOptions"
        @prev="model = false"
        @next="emit('confirm')"
      />
    </template>
  </PopupError>
</template>

<script setup lang="ts">
  const props = defineProps<{
    tripIdLabel: string
    isLoading?: boolean
  }>()

  const emit = defineEmits<{
    confirm: []
  }>()

  const { t } = useI18n()
  const model = defineModel<boolean>({ required: true })

  const prevButtonOptions = computed(() => ({
    label: t('button.cancel'),
    class: 'btn-secondary-outline h-13.5 py-3.75 md:me-2 md:w-75',
  }))

  const nextButtonOptions = computed(() => ({
    label: t('button.delete'),
    class: 'btn-primary h-13.5 py-3.75 md:ms-2 md:w-75',
    disabled: props.isLoading,
  }))
</script>

<style scoped>

</style>
