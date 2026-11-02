<template>
  <BaseForm
    v-if="!showPasswordStep && !isTokenMode"
    :key="'code'"
    :fields-schema="codeSchema"
    :validation-schema="validationSchemaCode"
    :initial-values="initialValuesCode"
    :errors="errors"
    :submit-attrs="{
      class: 'h-13 w-full rounded-2xl bg-primary leading-6 font-medium tracking-tight text-white transition-colors hover:bg-primary/90'
    }"
    class="w-full max-w-96"
    @submit="onCodeSubmit"
  >
    <template #submit-text>
      {{ $t('button.continue') }}
    </template>
  </BaseForm>

  <BaseForm
    v-else
    :key="'password'"
    :fields-schema="passwordSchema"
    :validation-schema="validationSchemaPassword"
    :initial-values="initialValuesPassword"
    :errors="errors"
    :submit-attrs="{
      class: 'h-13 w-full rounded-2xl bg-primary leading-6 font-medium tracking-tight text-white transition-colors hover:bg-primary/90'
    }"
    class="w-full max-w-96"
    @submit="onPasswordSubmit"
  >
    <template #field-password="{ value }">
      <div class="mt-4 mb-8 space-y-2 text-neutral-60">
        <BaseRadio
          :model-value="hasMinLength(value)"
          :label="$t('validation.charactersMinimum')"
          :variant="hasMinLength(value) ? 'success' : 'default'"
          class="cursor-default"
          disabled
          tabindex="-1"
        />

        <BaseRadio
          :model-value="hasCapital(value)"
          :label="$t('validation.atLeastOneCapital')"
          :variant="hasCapital(value) ? 'success' : 'default'"
          class="cursor-default"
          disabled
          tabindex="-1"
        />

        <BaseRadio
          :model-value="hasSymbolAndNumber(value)"
          :label="$t('validation.oneSymbolAndOneNumber')"
          :variant="hasSymbolAndNumber(value) ? 'success' : 'default'"
          class="cursor-default"
          disabled
          tabindex="-1"
        />
      </div>
    </template>

    <template #field-confirmPassword="{ value, formValues }">
      <BaseRadio
        :model-value="!!(value && formValues?.password === value)"
        :label="$t('validation.passwordIsMatching')"
        :variant="!!(value && formValues?.password === value) ? 'success' : 'default'"
        class="mt-4 mb-8 cursor-default"
        disabled
        tabindex="-1"
      />
    </template>

    <template #submit-text>
      {{ isLoading ? $t('loader.submitting') : $t('button.confirmPassword') }}
    </template>
  </BaseForm>
</template>

<script setup lang="ts">
  import type {
    ResetPasswordRequest,
    ResetPasswordWithCodeRequest,
  } from '@/types/admin-dashboard/request/auth'
  import { useResetForm } from '../composables/useResetForm'

  defineProps<{
    isLoading: boolean
    errors?: Record<string, string[]>
  }>()

  const emit = defineEmits<{
    submit: [values: ResetPasswordWithCodeRequest | ResetPasswordRequest]
  }>()

  const route = useRoute()
  const identifier = (route.query.identifier as string) ?? ''
  const token = (route.query.token as string) ?? ''
  const isTokenMode = computed(() => !!token)

  const showPasswordStep = ref(false)
  const codeValue = ref('')

  const {
    codeSchema,
    passwordSchema,
    initialValuesCode,
    initialValuesPassword,
    validationSchemaCode,
    validationSchemaPassword,
    hasMinLength,
    hasCapital,
    hasSymbolAndNumber,
  } = useResetForm()

  function onCodeSubmit(values: Record<string, unknown>) {
    codeValue.value = String(values.code ?? '')
    showPasswordStep.value = true
  }

  function onPasswordSubmit(values: Record<string, unknown>) {
    if (isTokenMode.value) {
      emit('submit', {
        token,
        password: String(values.password ?? ''),
        confirmPassword: String(values.confirmPassword ?? ''),
      })

      return
    }

    emit('submit', {
      identifier,
      code: codeValue.value,
      newPassword: String(values.password ?? ''),
      confirmPassword: String(values.confirmPassword ?? ''),
    })
  }
</script>

<style scoped>

</style>
