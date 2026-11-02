import { object, string } from 'yup'
import type { FieldsGroup } from '@/types/form'
import { useLookups } from '@/composables/useLookups'
import { useDistrictStore } from '@/stores/admin-dashboard/district'
import { createEmailSchema } from '@/utils/validation/email'
import { createArabicTextSchema } from '@/utils/validation/arabic'

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
    nameEn: formValues?.value?.nameEn || '',
    nameAr: formValues?.value?.nameAr || '',
    registrationNumber: formValues?.value?.registrationNumber || '',
    taxId: formValues?.value?.taxId || '',
    contactPersonName: formValues?.value?.contactPersonName || '',
    email: formValues?.value?.email || '',
    phone: formValues?.value?.phone || '+20',
    street: formValues?.value?.street || '',
    district: formValues?.value?.district || '',
    city: formValues?.value?.city || '',
    governorate: formValues?.value?.governorate || '',
    postalCode: formValues?.value?.postalCode || '',
    gpsCoordinates: formValues?.value?.gpsCoordinates || '',
  }))

  const validationSchema = computed(() => object({
    nameEn: string().required().max(100),
    nameAr: createArabicTextSchema(t, 100),
    registrationNumber: string().optional(),
    taxId: string().required(),
    contactPersonName: string().required().max(50),
    email: createEmailSchema(t),
    phone: string()
      .required()
      .matches(/^\+20\d{10}$/, 'Phone number must be a valid Egyptian number (+20XXXXXXXXXX)'),
    street: string().required().max(200),
    district: string().optional(),
    city: string().required(),
    governorate: string().required(),
    postalCode: string().optional(),
    gpsCoordinates: string(),
  }))

  const groups = computed<Record<string, FieldsGroup>>(() => ({
    companyInfo: {
      title: '',
      description: '',
      class: 'mb-3',
      wrapperClass: 'flex flex-wrap justify-between',
      fields: [
        {
          name: 'nameEn',
          as: 'BaseInput',
          type: 'text',
          containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
          label: t('form.fullNameEn'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
          labelRequired: true,
        },
        {
          name: 'nameAr',
          as: 'BaseInput',
          type: 'text',
          containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
          label: 'Arabic name',
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
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
          disabled: true,
        },
        {
          name: 'taxId',
          as: 'BaseInput',
          type: 'text',
          label: t('form.taxId'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
          labelRequired: true,
          containerClass: 'w-full',
          fieldSlot: true,
        },
      ],
    },
    contacts: {
      title: t('content.contacts'),
      titleClass: 'font-bold text-neutral-10 mb-6',
      description: '',
      class: 'rounded-lg border border-neutral-200 bg-white p-4 mb-3',
      fields: [
        {
          name: 'contactPersonName',
          as: 'BaseInput',
          type: 'text',
          label: t('form.contactPersonName'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
          labelRequired: true,
          containerClass: 'w-full',
        },
        {
          name: 'email',
          as: 'BaseInput',
          type: 'email',
          label: 'Email address',
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
      ],
    },
    address: {
      title: t('content.addressManagement'),
      titleClass: 'font-bold text-neutral-10 mb-6',
      description: '',
      class: 'rounded-lg border border-neutral-200 bg-white p-4',
      wrapperClass: 'flex flex-wrap justify-between',
      fields: [
        {
          name: 'governorate',
          as: 'BaseSelect',
          containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
          label: 'Governorate',
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
          labelRequired: true,
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
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
          labelRequired: true,
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
          label: t('form.street'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
          containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
          labelRequired: true,
        },
        {
          name: 'postalCode',
          as: 'BaseInput',
          type: 'text',
          label: 'Postal code',
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
          containerClass: 'w-full',
        },
        {
          name: 'gpsCoordinates',
          as: 'BaseInput',
          type: 'text',
          label: t('form.gpsCoordinates'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
          containerClass: 'w-full',
          fieldSlot: true,
        },
      ],
    },
  }))

  return {
    initialValues,
    validationSchema,
    groups,
    fetchDistricts,
  }
}

