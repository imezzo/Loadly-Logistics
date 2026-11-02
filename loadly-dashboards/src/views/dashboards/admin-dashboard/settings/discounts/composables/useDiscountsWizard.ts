import { number, object, string } from 'yup'
import type { Discount } from '@/types/admin-dashboard/response/discount/discount'
import { useEnum } from '@/composables/useEnum'

export function useDiscountsWizard(discount: ComputedRef<Discount | null>) {
  const { t } = useI18n()
  const { enums } = useEnum()

  const discountTypeOptions = computed(() => enums.value?.DiscountType ?? [])
  const statusOptions = computed(() => enums.value?.DiscountStatus ?? [])

  const initialValues = computed(() => ({
    nameEn: discount.value?.nameEn || '',
    nameAr: discount.value?.nameAr || '',
    type: discount.value?.type,
    value: discount.value?.value ?? '',
    status: discount.value?.status,
  }))

  const validationSchema = computed(() => object({
    nameEn: string().required().max(255),
    nameAr: string().required().max(255),
    type: string().required(),
    value: number().required().min(0),
    status: string().required(),
  }))

  const fieldsSchema = computed(() => [
    {
      name: 'nameEn',
      as: 'BaseInput',
      type: 'text',
      label: t('form.nameEn'),
      labelRequired: true,
    },
    {
      name: 'nameAr',
      as: 'BaseInput',
      type: 'text',
      label: t('form.nameAr'),
      labelRequired: true,
    },
    {
      name: 'type',
      as: 'BaseSelect',
      options: discountTypeOptions.value,
      searchable: true,
      label: t('form.discountType'),
      labelRequired: true,
    },
    {
      name: 'value',
      as: 'BaseInput',
      type: 'number',
      label: t('form.discountValue'),
      labelRequired: true,
    },
    {
      name: 'status',
      as: 'BaseSelect',
      options: statusOptions.value,
      searchable: true,
      label: t('table.status'),
      labelRequired: true,
      containerClass: 'col-span-2',
    },
  ])

  return {
    fieldsSchema,
    initialValues,
    validationSchema,
  }
}
