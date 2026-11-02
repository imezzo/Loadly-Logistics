import { object, string } from 'yup'
import type { Schema } from '@/types/form'
import { useLookups } from '@/composables/useLookups'

export function useBusinessInformation(formValues?: ComputedRef<Record<string, unknown> | undefined>) {
  const {
    industryTypes,
    communicationMethods,
    serviceRequirements,
    operatingHours,
  } = useLookups()
  const { t } = useI18n()

  const initialValues = computed(() => ({
    websiteUrl: formValues?.value?.websiteUrl || '',
    industryType: formValues?.value?.industryType || '',
    companySize: formValues?.value?.companySize || '',
    serviceRequirements: formValues?.value?.serviceRequirements || '',
    operatingHours: formValues?.value?.operatingHours || '',
    preferredCommunicationMethods: formValues?.value?.preferredCommunicationMethods || '',
  }))

  const validationSchema = computed(() => object({
    websiteUrl: string().optional().url(),
    industryType: string(),
    companySize: string(),
    serviceRequirements: string(),
    operatingHours: string(),
    preferredCommunicationMethods: string(),
  }))

  const fieldsSchema = computed<Schema[]>(() => [
    {
      name: 'websiteUrl',
      as: 'BaseInput',
      type: 'url',
      label: t('form.websiteUrl'),
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
    },
    {
      name: 'industryType',
      as: 'BaseSelect',
      label: 'Industry type',
      labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
      options: industryTypes.value,
    },
    {
      name: 'companySize',
      as: 'BaseSelect',
      label: t('form.companySize'),
      labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
      options: [
        { value: 'small', label: 'Small (1-50)' },
        { value: 'medium', label: 'Medium (51-200)' },
        { value: 'large', label: 'Large (201-1000)' },
        { value: 'enterprise', label: 'Enterprise (1000+)' },
      ],
    },
    {
      name: 'serviceRequirements',
      as: 'BaseSelect',
      label: t('form.serviceRequirements'),
      labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
      options: serviceRequirements.value,
    },
    {
      name: 'operatingHours',
      as: 'BaseSelect',
      label: 'Operating hours',
      labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
      options: operatingHours.value,
    },
    {
      name: 'preferredCommunicationMethods',
      as: 'BaseSelect',
      label: t('form.preferredCommunicationMethods'),
      labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
      options: communicationMethods.value,
    },
  ])

  return {
    fieldsSchema,
    initialValues,
    validationSchema,
  }
}

