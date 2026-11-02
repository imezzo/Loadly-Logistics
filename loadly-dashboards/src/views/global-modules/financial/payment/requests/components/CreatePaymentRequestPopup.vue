<template>
  <BasePopup
    v-model="model"
    :show-close-button="false"
  >
    <div class="w-full px-6 py-4">
      <h2 class="mb-6 text-2xl font-bold text-neutral-800">
        {{ $t('button.createPaymentRequest') }}
      </h2>

      <BaseAlert
        :model-value="!!errorMessage"
        :message="errorMessage || ''"
        variant="error"
        class="mb-4 w-full"
      />

      <BaseForm
        ref="form"
        container-class="w-full flex flex-wrap gap-2"
        :validation-schema
        :initial-values
        :fields-schema
        :errors="formErrors"
        @submit="onSubmit"
      >
        <template #submit>
          <BaseActions
            container-class="mt-6 justify-center gap-3"
            :prev-button-options="prevButtonOptions"
            :next-button-options="nextButtonOptions"
            @prev="discard"
          />
        </template>
      </BaseForm>
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import { useCreatePaymentRequestPopup } from '../composables/useCreatePaymentRequestPopup'

  const props = defineProps<{
    invoiceId: number
  }>()

  const emit = defineEmits<{
    refresh: []
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })

  const {
    errorMessage,
    fieldsSchema,
    form,
    formErrors,
    initialValues,
    nextButtonOptions,
    onSubmit,
    prevButtonOptions,
    validationSchema,
    discard,
  } = useCreatePaymentRequestPopup({
    invoiceId: toRef(props, 'invoiceId'),
    onClose: () => {
      model.value = false
    },
    onSuccess: () => {
      emit('refresh')
    },
  })
</script>

<style scoped>

</style>
