<template>
  <FormHeader
    :sub-header="$t('title.resetYourPassword')"
    :description="$t('form.createYourNewPassword')"
  />

  <BaseAlert
    v-if="successMessage || errorMessage"
    :model-value="!!(successMessage || errorMessage)"
    :message="successMessage || errorMessage || ''"
    :variant="successMessage ? 'success' : 'error'"
    class="mb-4 w-full max-w-100"
  />

  <ResetForm
    v-if="!successMessage"
    :is-loading="isLoading"
    :errors="formErrors"
    @submit="onSubmit"
  />

  <RouterLink
    v-else
    :to="{ name: 'Login' }"
    class="
      flex h-13 w-full max-w-100 cursor-pointer items-center justify-center rounded-2xl bg-primary
      leading-6 font-medium tracking-tight text-white
      disabled:cursor-not-allowed disabled:opacity-50
    "
  >
    {{ $t('button.backToLogin') }}
  </RouterLink>
</template>

<script setup lang="ts">
  import { useAuthStore } from '@/stores/admin-dashboard/auth'
  import type {
    ResetPasswordRequest,
    ResetPasswordWithCodeRequest,
  } from '@/types/admin-dashboard/request/auth'
  import FormHeader from '../components/FormHeader.vue'
  import ResetForm from './components/ResetForm.vue'

  const { resetPassword, resetPasswordWithCode } = useAuthStore()
  const { t } = useI18n()

  const successMessage = ref<string | null>(null)
  const errorMessage = ref<string | null>(null)
  const isLoading = ref(false)
  const formErrors = ref<Record<string, string[]> | undefined>()

  async function onSubmit(
    values: ResetPasswordWithCodeRequest | ResetPasswordRequest
  ): Promise<void> {
    if (isLoading.value) return

    successMessage.value = null
    errorMessage.value = null
    isLoading.value = true

    const isTokenFlow = 'token' in values && values.token
    const { success, message, errors } = isTokenFlow
      ? await resetPassword(values as ResetPasswordRequest)
      : await resetPasswordWithCode(values as ResetPasswordWithCodeRequest)

    isLoading.value = false
    if (success) {
      successMessage.value = message ?? t('success.passwordReset')

      return
    }
    errorMessage.value = message ?? t('error.somethingWentWrong')
    formErrors.value = errors
  }
</script>

<style scoped>

</style>

