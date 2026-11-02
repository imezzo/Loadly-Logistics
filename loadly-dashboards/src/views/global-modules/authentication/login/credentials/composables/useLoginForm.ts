import { object, string } from 'yup'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useAuth } from '@/composables/admin-dashboard/useAuth'
import { useAccountLockout } from './useAccountLockout'
import { createPasswordSchema } from '@/utils/validation/password'
import { createEmailSchema } from '@/utils/validation/email'
import type { LoginRequest } from '@/types/admin-dashboard/request/auth'
import { MfaChannel } from '@/types/request/mfa'
import { LoginMethod } from '@/types/admin-dashboard/response/auth'
import { useApp } from '@/composables/useApp'
import { createPhoneSchema } from '@/utils/validation/phone'
import { useMfaStore } from '@/stores/admin-dashboard/mfa'

export function useLoginForm() {
  const { t } = useI18n()
  const router = useRouter()
  const { isAdminDashboard } = useApp()
  const { login } = useAuth()
  const {
    isAccountLocked,
    lockoutTimer,
    onLoginSuccess,
    onLoginFailure,
  } = useAccountLockout()

  const { sendMfaCode } = useMfaStore()

  const isLoading = ref(false)
  const errorMessage = ref<string | null>(null)
  const selectedMethod = ref<LoginMethod>(LoginMethod.USERNAME)

  const loginMethodOptions = computed(() => [
    selectedMethod.value === LoginMethod.USERNAME ?
      {
        name: 'username',
        as: 'BaseInput',
        type: 'text',
        leftIcon: 'IconUser',
        label: t('form.userName'),
        placeholder: t('form.userName'),
        disabled: isAccountLocked.value,
      } : {
        name: 'phone',
        as: 'BaseInputPhone',
        type: 'tel',
        leftIcon: 'IconPhone',
        label: t('form.phoneNumber'),
        placeholder: t('form.phoneNumber'),
        disabled: isAccountLocked.value,
      }
  ])

  const schema = computed(() => [
    ...(isAdminDashboard ?
      loginMethodOptions.value : [
      {
        name: 'email',
        as: 'BaseInput',
        type: 'email',
        leftIcon: 'IconEnvelope',
        placeholder: t('form.email'),
        disabled: isAccountLocked.value,
      },
    ]),
    {
      name: 'password',
      as: 'BaseInput',
      type: 'password',
      wrapperClass: 'mt-4',
      leftIcon: 'IconLock',
      placeholder: t('form.passwordPlaceholder'),
      disabled: isAccountLocked.value,
    },
  ])

  const adminDashboardSchema = computed(() => ({
    ...(selectedMethod.value === LoginMethod.USERNAME ? {
      username: string().required().min(3),
    } : {
      phone: createPhoneSchema(t, true),
    }),
  }))

  const validationSchema = computed(() => object({
    ...(isAdminDashboard
      ? adminDashboardSchema.value
      : {
        email: createEmailSchema(t),
      }),
    password: createPasswordSchema(t),
  }))

  async function navigateAfterLogin(method: MfaChannel) {
    if (method === MfaChannel.NONE) {
      router.push('/')

      return
    }

    if (isAdminDashboard) {
      const { success } = await sendMfaCode({ method })

      if (!success) return
      router.push({ name: 'LoginVerifyCode', params: { method } })

      return
    }

    router.push({name: 'LoginVerificationMethod'})
  }

  async function onSubmit(formData: Record<string, unknown>): Promise<void> {
    if (isLoading.value) return

    errorMessage.value = null
    isLoading.value = true

    const { success, data, message } = await login(formData as unknown as LoginRequest)
    isLoading.value = false

    if (!success) {
      onLoginFailure()
      errorMessage.value = message ?? t('error.somethingWentWrong')

      return
    }

    onLoginSuccess()
    navigateAfterLogin(data.user?.mfaChannel)
  }

  onBeforeUnmount(() => {
    isLoading.value = false
  })

  return {
    schema,
    validationSchema,
    isLoading,
    errorMessage,
    isAccountLocked,
    selectedMethod,
    lockoutTimer,
    onSubmit,
  }
}

