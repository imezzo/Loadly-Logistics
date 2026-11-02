import { object, string } from 'yup'
import type { Schema } from '@/types/form'
import { useLookups } from '@/composables/useLookups'
import { useDistrictStore } from '@/stores/admin-dashboard/district'
import { createEmailSchema } from '@/utils/validation/email'
import { createArabicTextSchema } from '@/utils/validation/arabic'
import { createPhoneSchema } from '@/utils/validation/phone'

export function useCompanyRegistration(formValues?: ComputedRef<Record<string, unknown> | undefined>) {
  const {
    cities,
    governments,
    hasMoreCities,
    hasMoreGovernments,
    loadMoreCities,
    loadMoreGovernments,
  } = useLookups()
  const districtStore = useDistrictStore()
  const { districtsList } = storeToRefs(districtStore)
  const { t } = useI18n()

  async function fetchDistricts(cityId?: number): Promise<void> {
    const { success, data } = await districtStore.getList({
      pagination: { page: 1, size: 100 },
      ...(cityId != null ? { cityId } : {}),
    })

    if (success && data) {
      districtsList.value = data
    }
  }

  const initialValues = computed(() => ({
    registrationNumber: formValues?.value?.registrationNumber || '',
    contactPersonName: formValues?.value?.contactPersonName || '',
    userName: formValues?.value?.userName || '',
    email: formValues?.value?.email || '',
    phone: formValues?.value?.phone || '+20',
    nameEn: formValues?.value?.nameEn || '',
    nameAr: formValues?.value?.nameAr || '',
    taxId: formValues?.value?.taxId || '',
    street: formValues?.value?.street || '',
    district: formValues?.value?.district || '',
    city: formValues?.value?.city || '',
    governorate: formValues?.value?.governorate || '',
    postalCode: formValues?.value?.postalCode || '',
  }))

  const validationSchema = computed(() => object({
    registrationNumber: string().optional().nullable(),
    contactPersonName: string().required().max(50),
    userName: string().required(),
    email: createEmailSchema(t),
    phone: createPhoneSchema(t, true),
    nameEn: string().required().max(100),
    nameAr: createArabicTextSchema(t, 100),
    taxId: string().required(),
    street: string().required().max(200),
    district: string().optional(),
    city: string().required(),
    governorate: string().required(),
    postalCode: string().optional(),
  }))

  const fieldsSchema = computed<Schema[]>(() => [
    {
      name: 'nameEn',
      as: 'BaseInput',
      type: 'text',
      containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
      label: t('form.fullNameEn'),
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
      labelRequired: true,
    },
    {
      name: 'nameAr',
      as: 'BaseInput',
      type: 'text',
      containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
      label: t('form.fullNameAr'),
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
      labelRequired: true,
    },
    {
      name: 'registrationNumber',
      as: 'BaseInput',
      type: 'text',
      label: t('form.businessRegistrationNumber'),
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
      containerClass: 'w-full',
      fieldSlot: true,
    },
    {
      name: 'taxId',
      as: 'BaseInput',
      type: 'text',
      label: t('form.taxId'),
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
      containerClass: 'w-full',
      fieldSlot: true,
      labelRequired: true,
    },
    {
      name: 'contactPersonName',
      as: 'BaseInput',
      type: 'text',
      label: 'Contact person name',
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
      labelRequired: true,
      containerClass: 'w-full',
    },
    {
      name: 'userName',
      as: 'BaseInput',
      type: 'text',
      label: t('form.userName'),
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
      labelRequired: true,
      containerClass: 'w-full',
      fieldSlot: true,
    },
    {
      name: 'email',
      as: 'BaseInput',
      type: 'email',
      label: t('form.emailAddress'),
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
      labelRequired: true,
      containerClass: 'w-full',
    },
    {
      name: 'phone',
      as: 'BaseInputPhone',
      type: 'tel',
      label: t('form.phoneNumber'),
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
      labelRequired: true,
      containerClass: 'w-full',
      fieldSlot: true,
      allowedCountryCode: '+20',
    },
    {
      name: 'governorate',
      as: 'BaseSelect',
      containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
      label: t('form.governorate'),
      labelRequired: true,
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
      options: governments.value,
      onScrollToEnd: () => {
        if (hasMoreGovernments.value) {
          loadMoreGovernments()
        }
      },
    },
    {
      name: 'city',
      as: 'BaseSelect',
      containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
      label: t('form.city'),
      labelRequired: true,
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
      options: cities.value,
      onScrollToEnd: () => {
        if (hasMoreCities.value) {
          loadMoreCities()
        }
      },
    },
    {
      name: 'district',
      as: 'BaseSelect',
      options: districtsList.value?.districts.map((d) => ({ value: String(d.id), label: d.name })) ?? [],
      containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
      label: t('form.district'),
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
    },
    {
      name: 'street',
      as: 'BaseInput',
      type: 'text',
      containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
      label: t('form.street'),
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
      labelRequired: true,
    },
    {
      name: 'postalCode',
      as: 'BaseInput',
      type: 'text',
      label: t('form.postalCode'),
      labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
      containerClass: 'w-full',
    },
  ])

  return {
    initialValues,
    validationSchema,
    fieldsSchema,
    fetchDistricts,
  }
}

