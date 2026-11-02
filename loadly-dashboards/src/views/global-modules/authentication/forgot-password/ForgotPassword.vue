<template>
  <FormHeader
    :header="t('form.startWork')"
    :sub-header="pageSubHeader"
    :description="pageDescription"
  />

  <div
    v-if="isCompanyDashboard"
    class="mb-4 flex w-full max-w-100 flex-col gap-2"
  >
    <VerificationOption
      :label="$t('form.email')"
      :description="$t('form.emailVerification')"
      :is-active="channel === MfaChannel.EMAIL"
      @click="channel = MfaChannel.EMAIL"
    >
      <template #icon>
        <IconEmail class="size-5" />
      </template>
    </VerificationOption>

    <VerificationOption
      :label="$t('form.phoneNumber')"
      :description="$t('form.phoneNumberVerification')"
      :is-active="channel === MfaChannel.SMS"
      @click="channel = MfaChannel.SMS"
    >
      <template #icon>
        <IconChat class="size-5" />
      </template>
    </VerificationOption>
  </div>

  <BaseAlert
    v-if="errorMessage"
    :model-value="!!errorMessage"
    :message="errorMessage || ''"
    variant="error"
    class="mb-4 w-full max-w-100"
  />

  <BaseForm
    :key="channel"
    :fields-schema="schema"
    :validation-schema="validationSchema"
    :errors="formErrors"
    :submit-attrs="{
      class: 'bg-primary w-full mt-8 rounded-2xl cursor-pointer h-13 flex items-center justify-center gap-2 text-white font-medium leading-6 tracking-tight hover:bg-primary/90',
      disabled: isLoading
    }"
    class="w-full max-w-100"
    @submit="onSubmit"
  >
    <template #submit-text>
      {{ isLoading ? t('loader.sending') : t('form.sendResetLink') }}
      <IconArrow
        v-if="!isLoading"
        class="size-5 fill-white!"
      />
    </template>
  </BaseForm>
</template>

<script setup lang="ts">
  import { object } from 'yup'
  import { useAuthStore } from '@/stores/admin-dashboard/auth'
  import type { ForgotPasswordRequest } from '@/types/admin-dashboard/request/auth'
  import { MfaChannel } from '@/types/request/mfa'
  import { createEmailSchema } from '@/utils/validation/email'
  import { createPhoneSchema } from '@/utils/validation/phone'
  import FormHeader from '../components/FormHeader.vue'
  import VerificationOption from '../login/verification-method/components/VerificationOption.vue'
  import { useApp } from '@/composables/useApp.ts'

  const { t } = useI18n()
  const router = useRouter()
  const { isCompanyDashboard } = useApp()

  const authStore = useAuthStore()

  const channel = ref<MfaChannel>(isCompanyDashboard ? MfaChannel.EMAIL : MfaChannel.SMS)
  const isLoading = ref(false)
  const errorMessage = ref<string | null>(null)
  const formErrors = ref<Record<string, string[]> | undefined>()

  const pageSubHeader = computed(() =>
    isCompanyDashboard ? t('title.enterYourEmail') : t('form.phoneNumber')
  )

  const pageDescription = computed(() =>
    isCompanyDashboard ? t('form.enterEmailToResetPassword') : t('form.enterPhoneToResetPassword')
  )

  const schema = computed(() =>
    channel.value === MfaChannel.EMAIL
      ? [
          {
            name: 'identifier',
            as: 'BaseInput',
            type: 'email',
            leftIcon: 'IconEnvelope',
            placeholder: t('form.email'),
            disabled: isLoading.value,
          },
        ]
      : [
          {
            name: 'identifier',
            as: 'BaseInputPhone',
            type: 'tel',
            leftIcon: 'IconChat',
            placeholder: t('form.phoneNumber'),
            disabled: isLoading.value,
            allowedCountryCode: '+20',
          },
        ]
  )

  const validationSchema = computed(() =>
    object({
      identifier:
        channel.value === MfaChannel.EMAIL
          ? createEmailSchema(t)
          : createPhoneSchema(t, true),
    })
  )

  async function onSubmit(values: Record<string, unknown>) {
    if (isLoading.value) return

    const identifier = String(values?.identifier ?? '')
    errorMessage.value = null
    isLoading.value = true

    const payload: ForgotPasswordRequest = {
      identifier,
      channel: channel.value,
    }
    const { success, message, errors } = await authStore.forgotPassword(payload)
    isLoading.value = false

    if (success) {
      router.push({ name: 'ResetPassword', query: { identifier } })

      return
    }
    errorMessage.value = message ?? t('error.somethingWentWrong')
    formErrors.value = errors
  }
</script>

<style scoped>

</style>
