import { object, string, ref as yupRef } from 'yup'
import { useVeeValidate } from '@/composables/useVeeValidate'
import { createPasswordSchema } from '@/utils/validation/password'

export const OTP_LENGTH = 6

export function useResetForm() {
  const { t } = useI18n()

  useVeeValidate()

  const codeSchema = computed(() => [
    {
      name: 'code',
      as: 'BaseInputOtp',
      type: 'text',
      length: OTP_LENGTH,
      label: t('form.verificationCode'),
      labelClass: 'mb-1.5 block font-medium text-neutral-50',
      class: 'mb-6',
    },
  ])

  const passwordSchema = computed(() => [
    {
      name: 'password',
      as: 'BaseInput',
      type: 'password',
      label: t('form.resetYourPassword'),
      labelClass: 'mb-1.5 block font-medium text-neutral-50',
      leftIcon: 'IconLock',
      placeholder: t('form.password'),
      fieldSlot: true,
    },
    {
      name: 'confirmPassword',
      as: 'BaseInput',
      type: 'password',
      label: t('form.confirmNewPassword'),
      labelClass: 'mb-1.5 block font-medium text-neutral-50',
      leftIcon: 'IconLock',
      placeholder: t('form.passwordPlaceholder'),
      fieldSlot: true,
    },
  ])

  const initialValuesCode = { code: '' }
  const initialValuesPassword = { password: '', confirmPassword: '' }

  const validationSchemaCode = object({
    code: string()
      .required(t('validation.defined'))
      .length(OTP_LENGTH, t('validation.length', { length: OTP_LENGTH })),
  })

  const validationSchemaPassword = object({
    password: createPasswordSchema(t),
    confirmPassword: string()
      .required()
      .oneOf([yupRef('password')], t('validation.passwordIsMatching')),
  })

  function hasMinLength(password: string): boolean {
    return password.length >= 8
  }

  function hasCapital(password: string): boolean {
    return /[A-Z]/.test(password)
  }

  function hasSymbolAndNumber(password: string): boolean {
    return /[!@#$%^&*(),.?":{}|<>]/.test(password) && /\d/.test(password)
  }

  return {
    codeSchema,
    passwordSchema,
    initialValuesCode,
    initialValuesPassword,
    validationSchemaCode,
    validationSchemaPassword,
    hasMinLength,
    hasCapital,
    hasSymbolAndNumber,
  }
}

