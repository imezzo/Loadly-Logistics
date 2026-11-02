import { array, boolean, object, string } from 'yup'
import type { FieldsGroup } from '@/types/form'
import { useLookups } from '@/composables/useLookups'
import { useVeeValidate } from '@/composables/useVeeValidate'
import { useLoading } from '@/composables/useLoading'
import { createEmailSchema } from '@/utils/validation/email'
import { createArabicTextSchema } from '@/utils/validation/arabic'
import { createPhoneSchema } from '@/utils/validation/phone'
import type { SelectOption } from '@/types/ui'
import { useEnum } from '@/composables/useEnum'
import { useDriverStore } from '@/stores/admin-dashboard/driver'
import { useDistrictStore } from '@/stores/admin-dashboard/district'
import { ContractorStatus } from '@/types/admin-dashboard/response/contractor/contractor.enums'

export function useDriverForm(
  formValues?: ComputedRef<Record<string, unknown> | undefined>,
  initialValuesRef?: Ref<Record<string, unknown>>
) {
  useVeeValidate()
  const { t } = useI18n()
  const route = useRoute()
  const router = useRouter()
  const driverStore = useDriverStore()
  const districtStore = useDistrictStore()
  const { driver } = storeToRefs(driverStore)
  const { districtsList } = storeToRefs(districtStore)
  const { setLoading } = useLoading()
  const { enums } = useEnum()
  const {
    cities,
    contractors,
    getCities,
    getContractors,
    governments,
    hasMoreCities,
    hasMoreContractors,
    hasMoreGovernments,
    loadMoreCities,
    loadMoreContractors,
    loadMoreGovernments,
  } = useLookups()

  async function fetchDistricts(cityId?: number): Promise<void> {
    const { success, data } = await districtStore.getList({
      pagination: { page: 1, size: 100 },
      ...(cityId != null ? { cityId } : {}),
    })

    if (success && data) {
      districtsList.value = data
    }
  }

  const driverId = computed(() => route.params.id as string | undefined)

  function toSpokenLanguagesArray(value: string | string[] | undefined): string[] {
    if (Array.isArray(value)) return value
    if (typeof value === 'string' && value.trim()) {
      return value.split(',').map((s) => s.trim()).filter(Boolean)
    }

    return []
  }

  const hasContractor = ref(false)

  watch(
    () => formValues?.value?.hasContractor,
    (value) => {
      if (value != null) {
        hasContractor.value = Boolean(value)
      }
    },
    { immediate: true },
  )

  const initialValues = computed(() => ({
    hasContractor: Boolean(
      formValues?.value?.hasContractor
      ?? (driverId.value && driver.value?.contractorId),
    ),
    contractorId: formValues?.value?.contractorId?.toString()
      ?? (driverId.value && driver.value?.contractorId ? String(driver.value.contractorId) : ''),
    fullNameEn: formValues?.value?.fullNameEn || '',
    fullNameAr: formValues?.value?.fullNameAr || '',
    nationalId: formValues?.value?.nationalId || '',
    userName: formValues?.value?.userName || '',
    email: formValues?.value?.email || '',
    phone: formValues?.value?.phone || '+20',
    dateOfBirth: formValues?.value?.dateOfBirth || '',
    governerateId: formValues?.value?.governerateId || '',
    cityId: formValues?.value?.cityId || '',
    districtId: formValues?.value?.districtId || '',
    streetName: formValues?.value?.streetName || '',
    postalCode: formValues?.value?.postalCode || '',
    spokenLanguages: toSpokenLanguagesArray(
      ((driverId.value && driver.value)
        ? driver.value?.spokenLanguages
        : formValues?.value?.spokenLanguages) as string | string[] | undefined
    ),
    emergencyContactName: formValues?.value?.emergencyContactName || '',
    emergencyContactPhone: formValues?.value?.emergencyContactPhone || '',
    licenseNo: formValues?.value?.licenseNo || '',
    licenseCategory:
      (driverId.value && driver.value)
        ? (driver.value?.licenseCategory ?? '')
        : (formValues?.value?.licenseCategory || ''),
    licenseIssueDate: formValues?.value?.licenseIssueDate || '',
    licenseExpirationDate: formValues?.value?.licenseExpirationDate || '',
  }))

  const validationSchema = computed(() => object({
    hasContractor: boolean(),
    contractorId: string().when('hasContractor', {
      is: true,
      then: (schema) => schema.required(),
      otherwise: (schema) => schema.optional(),
    }),
    fullNameEn: string().required().max(100),
    fullNameAr: createArabicTextSchema(t, 100),
    nationalId: string().required().min(14).max(14),
    userName: string().required().min(3),
    email: createEmailSchema(t),
    phone: createPhoneSchema(t, true),
    dateOfBirth: string().required(),
    governerateId: string().required(),
    cityId: string().required(),
    districtId: string().optional(),
    streetName: string().required(),
    postalCode: string().optional(),
    spokenLanguages: array().of(string()),
    emergencyContactName: string().nullable(),
    emergencyContactPhone: createPhoneSchema(t).nullable(),
    licenseNo: string().required(),
    licenseCategory: string(),
    licenseIssueDate: string().required(),
    licenseExpirationDate: string().required(),
  }))

  const licenseCategoryOptions = computed<SelectOption[]>(
    () => enums.value?.LicenseCategory ?? []
  )

  const fieldsGroup = computed<Record<string, FieldsGroup>>(() => ({
    contractorRelated: {
      title: t('form.contractorRelated'),
      description: '',
      class: 'mb-6 grid grid-cols-1 gap-8 md:grid-cols-[280px_1fr]',
      wrapperClass: 'flex justify-between flex-wrap',
      titleClass: 'mb-1 text-neutral-600',
      descriptionClass: 'text-neutral-700',
      fields: [
        {
          name: 'hasContractor',
          as: 'BaseCheckbox',
          label: t('form.driverRelatedToContractors'),
          labelClass: 'hidden',
          containerClass: 'w-full',
          onCheckedChange: (value: boolean) => {
            hasContractor.value = value
          },
        },
        ...(hasContractor.value
          ? [{
              name: 'contractorId',
              as: 'BaseSelect' as const,
              options: contractors.value?.items ?? [],
              placeholder: t('form.select'),
              searchable: true,
              label: t('form.chooseContractorName'),
              labelClass: 'mb-1 mt-4 flex items-center font-medium text-neutral-10',
              labelRequired: true,
              containerClass: 'w-full',
              onSearch: (searchKeyword: string) => {
                getContractors(1, ContractorStatus.ACTIVE, searchKeyword)
              },
              onScrollToEnd: () => {
                if (hasMoreContractors.value) {
                  loadMoreContractors()
                }
              },
            }]
          : []),
      ],
    },
    personalInformation: {
      title: t('content.personalInformationManagement'),
      description: '',
      class: 'mb-6 grid grid-cols-1 gap-8 md:grid-cols-[280px_1fr]',
      wrapperClass: 'flex justify-between flex-wrap',
      titleClass: 'mb-1 text-neutral-600',
      descriptionClass: 'text-neutral-700',
      fields: [
        {
          name: 'userName',
          as: 'BaseInput',
          type: 'text',
          label: t('form.userName'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
          labelRequired: true,
          containerClass: 'w-full',
        },
        {
          name: 'fullNameEn',
          as: 'BaseInput',
          type: 'text',
          label: t('form.fullNameEn'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
          labelRequired: true,
          containerClass: 'w-full',
        },
        {
          name: 'fullNameAr',
          as: 'BaseInput',
          type: 'text',
          label: t('form.fullNameAr'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
          labelRequired: true,
          containerClass: 'w-full',
        },
        {
          name: 'nationalId',
          as: 'BaseInput',
          type: 'text',
          label: t('form.nationalId'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
          labelRequired: true,
          containerClass: 'w-full',
        },
        {
          name: 'dateOfBirth',
          as: 'BaseInput',
          type: 'date',
          label: t('form.dateOfBirth'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
          labelRequired: true,
          containerClass: 'w-full',
          placeholder: 'dd-mm-yyyy',
        },
        {
          name: 'phone',
          as: 'BaseInputPhone',
          type: 'tel',
          label: t('form.phoneNumber'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
          labelRequired: true,
          containerClass: 'w-full',
          allowedCountryCode: '+20',
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
          name: 'governerateId',
          as: 'BaseSelect',
          containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
          label: t('form.governorate'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
          labelRequired: true,
          options: governments.value,
          onScrollToEnd: () => {
            if (hasMoreGovernments.value) {
              loadMoreGovernments()
            }
          },
        },
        {
          name: 'cityId',
          as: 'BaseSelect',
          containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
          label: t('form.city'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
          labelRequired: true,
          options: cities.value,
          onScrollToEnd: () => {
            if (hasMoreCities.value) {
              loadMoreCities()
            }
          },
        },
        {
          name: 'districtId',
          as: 'BaseSelect',
          options: districtsList.value?.districts.map((d) => ({ value: String(d.id), label: d.name })) ?? [],
          containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
          label: t('form.district'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
        },
        {
          name: 'streetName',
          as: 'BaseInput',
          type: 'text',
          containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
          label: t('form.streetName'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
          labelRequired: true,
        },
        {
          name: 'postalCode',
          as: 'BaseInput',
          type: 'text',
          containerClass: 'w-full',
          label: t('form.postalCode'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
        },
        {
          name: 'spokenLanguages',
          as: 'BaseMultiSelect',
          options: enums.value?.SpokenLanguage ?? [],
          containerClass: 'w-full',
          label: t('form.spokenLanguages'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
          searchable: true,
        },
        {
          name: 'emergencyContactName',
          as: 'BaseInput',
          type: 'text',
          label: t('form.emergencyContactName'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
          labelRequired: true,
          containerClass: 'w-full',
        },
        {
          name: 'emergencyContactPhone',
          as: 'BaseInputPhone',
          type: 'tel',
          label: t('form.emergencyContactPhone'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
          labelRequired: true,
          containerClass: 'w-full',
          allowedCountryCode: '+20',
        },
      ],
    },
    driverQualifications: {
      title: t('content.driverQualifications'),
      description: '',
      class: 'mb-6 grid grid-cols-1 gap-8 md:grid-cols-[280px_1fr]',
      wrapperClass: 'flex justify-between flex-wrap',
      titleClass: 'mb-1 text-neutral-600',
      descriptionClass: 'text-neutral-700',
      fields: [
        {
          name: 'licenseNo',
          as: 'BaseInput',
          type: 'text',
          label: t('form.licenseNumber'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
          labelRequired: true,
          containerClass: 'w-full',
        },
        {
          name: 'licenseCategory',
          as: 'BaseSelect',
          containerClass: 'w-full',
          label: t('form.licenseCategory'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
          options: licenseCategoryOptions.value,
        },
        {
          name: 'licenseIssueDate',
          as: 'BaseInput',
          type: 'date',
          containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
          label: t('form.issueDate'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
          labelRequired: true,
          placeholder: 'dd-mm-yyyy',
        },
        {
          name: 'licenseExpirationDate',
          as: 'BaseInput',
          type: 'date',
          containerClass: 'w-full md:w-[calc(50%-0.75rem)]',
          label: t('form.expiryDate'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10 mt-6',
          labelRequired: true,
          placeholder: 'dd-mm-yyyy',
        },
      ],
    },
  }))

  async function fetchDriver(): Promise<void> {
    if (!driverId.value || !initialValuesRef) return

    setLoading(true)
    const { success, data } = await driverStore.get(driverId.value)
    setLoading(false)

    if (success && data) {
      driver.value = data
      initialValuesRef.value = data
      hasContractor.value = Boolean(data.contractorId)

      if (data.governerateId) {
        await getCities(data.governerateId)
      }

      return
    }

    router.push({name: 'Drivers'})
  }

  return {
    initialValues,
    validationSchema,
    fieldsGroup,
    fetchDriver,
    fetchDistricts,
    driverId,
  }
}
