<template>
  <PopupError
    v-if="modelValue"
    v-model="model"
  >
    <template #title>
      <p class="text-2xl font-bold text-neutral-10">
        {{ $t('error.deleteDistrict') }}
      </p>
    </template>

    <template #message>
      <p class="mt-4 text-content-secondary">
        {{ $t('error.deleteDistrictConfirmation', { name: districtName }) }}
      </p>

      <BaseActions
        container-class="mt-4 flex-wrap items-center justify-center gap-3 sm:gap-6 md:flex-nowrap"
        :prev-button-options="prevButtonOptions"
        :next-button-options="nextButtonOptions"
        @prev="model = false"
        @next="$emit('confirm')"
      />
    </template>
  </PopupError>
</template>

<script setup lang="ts">
  const props = defineProps<{
    districtName: string
    isLoading?: boolean
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })

  defineEmits<{
    confirm: []
  }>()

  const { t } = useI18n()

  const prevButtonOptions = computed(() => ({
    label: t('button.discard'),
    class: 'flex h-13.5 w-full items-center justify-center gap-2 rounded-2xl bg-black/15 px-4 py-4.25 font-medium text-content-secondary transition-colors hover:bg-black/20 md:w-52.5',
  }))

  const nextButtonOptions = computed(() => ({
    label: t('button.delete'),
    class: 'ms-0 flex w-full items-center justify-center gap-2 rounded-2xl bg-primary py-3.75 font-medium text-white transition-colors hover:bg-primary/90 md:ms-2 md:w-75',
    disabled: props.isLoading,
  }))
</script>

<style scoped>

</style>

