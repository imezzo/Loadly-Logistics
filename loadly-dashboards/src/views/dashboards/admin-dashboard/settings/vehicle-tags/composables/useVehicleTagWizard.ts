import { boolean, object, string } from 'yup'
import type { VehicleTag } from '@/types/admin-dashboard/response/vehicle/vehicle-tag'

export function useVehicleTagWizard(vehicleTag: ComputedRef<VehicleTag | null>) {
  const { t } = useI18n()

  const initialValues = computed(() => ({
    isPricing: vehicleTag.value?.isPricing ?? true,
    nameAr: vehicleTag.value?.nameAr ?? '',
    nameEn: vehicleTag.value?.nameEn ?? '',
  }))

  const validationSchema = computed(() => object({
    isPricing: boolean(),
    nameAr: string().required().max(255),
    nameEn: string().required().max(255),
  }))

  const fieldsSchema = computed(() => [
    {
      name: 'nameEn',
      as: 'BaseInput',
      type: 'text',
      label: t('form.tagName'),
      labelRequired: true,
      placeholder: t('form.tagName'),
    },
    {
      name: 'nameAr',
      as: 'BaseInput',
      type: 'text',
      label: t('form.tagNameAr'),
      labelRequired: true,
      placeholder: t('form.tagName'),
    },
    {
      name: 'isPricing',
      as: 'BaseCheckbox',
      label: t('form.thisIsPricing'),
      labelClass: 'hidden',
      containerClass: 'col-span-full',
    },
  ])

  return {
    fieldsSchema,
    initialValues,
    validationSchema,
  }
}
