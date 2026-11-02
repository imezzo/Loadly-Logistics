<template>
  <FormHeader
    :header="t('form.startWork')"
    :sub-header="t('form.signInNow')"
    :description="t('form.enterEmailAndPassword')"
  />

  <LockoutTimer
    v-if="isAccountLocked"
    :seconds="lockoutTimer"
  />

  <BaseAlert
    :model-value="isAccountLocked || !!errorMessage"
    variant="error"
    :message="isAccountLocked
      ? t('error.accountLocked', { minutes: Math.ceil(lockoutTimer / 60) })
      : errorMessage ?? ''"
    class="w-full max-w-100"
  />

  <div
    v-if="isAdminDashboard"
    class="flex w-full max-w-100 flex-col gap-1"
  >
    <VerificationOption
      :label="t('form.userName')"
      :description="''"
      :is-active="selectedMethod === LoginMethod.USERNAME"
      @click="selectedMethod = LoginMethod.USERNAME"
    >
      <template #icon>
        <IconUser class="size-5" />
      </template>
    </VerificationOption>

    <VerificationOption
      :label="$t('form.phoneNumber')"
      :description="''"
      :is-active="selectedMethod === LoginMethod.PHONE"
      @click="selectedMethod = LoginMethod.PHONE"
    >
      <template #icon>
        <IconChat class="size-5" />
      </template>
    </VerificationOption>
  </div>

  <BaseForm
    :fields-schema="schema"
    :validation-schema="validationSchema"
    :submit-attrs="{
      class: 'bg-primary w-full mt-8 rounded-2xl cursor-pointer h-13 text-white font-medium leading-6 tracking-tight',
      disabled: isAccountLocked || isLoading
    }"
    class="mt-4 w-full max-w-100"
    @submit="onSubmit"
  >
    <RouterLink
      :to="{ name: 'ForgotPassword' }"
      class="mt-4 inline-block font-medium text-primary"
    >
      {{ t('form.forgotPasswordOrResendInvitationCode') }}
    </RouterLink>

    <template #submit-text>
      {{ isLoading ? t('form.signingIn') : t('form.signIn') }}
    </template>
  </BaseForm>
</template>

<script setup lang="ts">
  import { useLoginForm } from './composables/useLoginForm'
  import { useAuth } from '@/composables/admin-dashboard/useAuth'
  import FormHeader from '../../components/FormHeader.vue'
  import LockoutTimer from './components/LockoutTimer.vue'
  import VerificationOption from '../verification-method/components/VerificationOption.vue'
  import { LoginMethod } from '@/types/admin-dashboard/response/auth'
  import { useApp } from '@/composables/useApp'

  const { t } = useI18n()

  const {
    errorMessage,
    isAccountLocked,
    isLoading,
    lockoutTimer,
    onSubmit,
    schema,
    selectedMethod,
    validationSchema,
  } = useLoginForm()
  const { isTempLoggedIn, resetTokens } = useAuth()
  const { isAdminDashboard } = useApp()

  onMounted(() => {
    if (isTempLoggedIn.value) {
      resetTokens()
    }
  })
</script>

<style scoped>

</style>
