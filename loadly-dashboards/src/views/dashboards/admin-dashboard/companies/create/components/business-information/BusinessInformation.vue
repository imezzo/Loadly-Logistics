<template>
  <BaseForm
    :initial-values="initialValues"
    :validation-schema="validationSchema"
    :fields-schema="fieldsSchema"
    :errors="props.errors"
    @submit="onSubmit"
  >
    <template #submit>
      <BaseActions
        :prev-button-options="{ hasScrollTop: true }"
        :next-button-options="{ asSubmit: true, hasScrollTop: true }"
        @prev="emit('prev')"
        @next="emit('next')"
      />
    </template>
  </BaseForm>
</template>

<script setup lang="ts">
  import { useVeeValidate } from '@/composables/useVeeValidate'
  import { useBusinessInformation } from '../../composables/useBusinessInformation'

  useVeeValidate()

  const props = defineProps<{
    formValues?: Record<string, unknown>
    errors?: Record<string, string[]>
  }>()

  const emit = defineEmits<{
    update: [data: Record<string, unknown>]
    prev: []
    next: []
    submit: []
  }>()

  const { initialValues, validationSchema, fieldsSchema } = useBusinessInformation(
    computed(() => props.formValues)
  )

  function onSubmit(values: Record<string, unknown>): void {
    emit('update', values)
    emit('next')
  }
</script>

<style scoped>

</style>

