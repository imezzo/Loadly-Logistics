<template>
  <PopupError v-model="model">
    <template #title>
      <p class="text-2xl font-bold text-neutral-10">
        {{ title }}
      </p>
    </template>

    <template #message>
      <p class="mt-4 text-content-secondary">
        {{ message }}
      </p>

      <BaseActions
        container-class="mt-4 grid grid-cols-2 gap-3"
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
    title: string
    message: string
    isLoading?: boolean
  }>()

  const { t } = useI18n()
  const model = defineModel<boolean>('modelValue', { required: true })

  defineEmits<{
    confirm: []
  }>()

  const prevButtonOptions = computed(() => ({
    label: t('button.discard'),
    class: 'btn-secondary h-13.5',
  }))

  const nextButtonOptions = computed(() => ({
    label: t('button.delete'),
    class: 'btn-primary h-13.5',
    disabled: props.isLoading,
  }))
</script>

<style scoped>

</style>
