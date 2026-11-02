import { number, object, string } from 'yup'
import type { Tax } from '@/types/admin-dashboard/response/tax/tax'
import { useEnum } from '@/composables/useEnum'

export function useTaxesWizard(tax: ComputedRef<Tax | null>) {
  const { t } = useI18n()
  const { enums } = useEnum()

  const taxTypeOptions = computed(() => enums.value?.TaxType ?? [])
  const statusOptions = computed(() => enums.value?.TaxStatus ?? [])

  const initialValues = computed(() => ({
    nameEn: tax.value?.nameEn || '',
    nameAr: tax.value?.nameAr || '',
    type: tax.value?.type,
    value: tax.value?.value ?? '',
    status: tax.value?.status,
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
      options: taxTypeOptions.value,
      searchable: true,
      label: t('form.taxType'),
      labelRequired: true,
    },
    {
      name: 'value',
      as: 'BaseInput',
      type: 'number',
      label: t('form.taxValue'),
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
