import { object, string } from 'yup'
import type { Sector } from '@/types/admin-dashboard/response/sector/sector'

export function useSectorsWizard(sector: ComputedRef<Sector | null>) {
  const { t } = useI18n()

  const initialValues = computed(() => ({
    nameEn: sector.value?.nameEn || '',
    nameAr: sector.value?.nameAr || '',
  }))

  const validationSchema = computed(() => object({
    nameEn: string().required().max(255),
    nameAr: string().required().max(255),
  }))

  const fieldsSchema = computed(() => [
    {
      name: 'nameEn',
      as: 'BaseInput',
      type: 'text',
      placeholder: t('form.nameEn'),
      label: t('form.nameEn'),
      labelRequired: true,
    },
    {
      name: 'nameAr',
      as: 'BaseInput',
      type: 'text',
      placeholder: t('form.nameAr'),
      label: t('form.nameAr'),
      labelRequired: true,
    },
  ])

  return {
    fieldsSchema,
    initialValues,
    validationSchema,
  }
}
