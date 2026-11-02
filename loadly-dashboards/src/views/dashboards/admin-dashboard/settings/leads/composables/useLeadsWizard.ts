import { object, string } from 'yup'
import type { Lead } from '@/types/admin-dashboard/response/lead/lead'
import { createPhoneSchema } from '@/utils/validation/phone'

export function useLeadsWizard(lead: ComputedRef<Lead | null>) {
  const { t } = useI18n()

  const initialValues = computed(() => ({
    fullName: lead.value?.fullName || '',
    phone: lead.value?.phone || '',
    email: lead.value?.email || '',
  }))

  const validationSchema = computed(() => object({
    fullName: string().required().max(255),
    phone: createPhoneSchema(t, true),
    email: string().optional().email().max(255),
  }))

  const fieldsSchema = computed(() => [
    {
      name: 'fullName',
      as: 'BaseInput',
      type: 'text',
      label: t('form.fullName'),
      labelRequired: true,
    },
    {
      name: 'phone',
      as: 'BaseInputPhone',
      type: 'text',
      label: t('form.phoneNumber'),
      labelRequired: true,
    },
    {
      name: 'email',
      as: 'BaseInput',
      type: 'email',
      label: t('form.email'),
    },
  ])

  return {
    fieldsSchema,
    initialValues,
    validationSchema,
  }
}
