import { object, string } from 'yup'
import type { FieldsGroup } from '@/types/form'
import type { VehicleFacilityFormValues } from '@/types/admin-dashboard/request/vehicle-facility'
import type { SecondaryContact } from '@/types/secondary-contact.types'
import { useVehicleFacilityStore } from '@/stores/admin-dashboard/vehicle-facility'
import { useVeeValidate } from '@/composables/useVeeValidate'
import { useLoading } from '@/composables/useLoading'
import { useEnum } from '@/composables/useEnum'
import { useLookups } from '@/composables/useLookups'
import { createPhoneSchema } from '@/utils/validation/phone'
import { VehicleFacilityType } from '@/types/admin-dashboard/response/vehicle-facility.enums'

export function useVehicleFacilityForm(
  onGoogleMapsUrlInput?: (url: string) => void | Promise<void>
) {
  useVeeValidate()
  const { t } = useI18n()
  const route = useRoute()
  const router = useRouter()
  const { enums } = useEnum()
  const {
    communicationMethods, companies, getCompanies, hasMoreCompanies, loadMoreCompanies
  } = useLookups()
  const { isLoading, setLoading } = useLoading()

  const vehicleFacilityStore = useVehicleFacilityStore()
  const { facility } = storeToRefs(vehicleFacilityStore)

  const isCreateMode = computed(() => route.name === 'CreateVehicleFacility')
  const isEditMode = computed(() => route.name === 'EditVehicleFacility')
  const isViewMode = computed(() => route.name === 'ViewVehicleFacility')
  const isFormDisabled = computed(() => isViewMode.value)
  const facilityId = computed(() => Number(route.params.id))
  const secondaryContacts = computed<SecondaryContact[]>(() => facility.value?.secondaryContacts ?? [])

  const selectedFacilityType = ref<VehicleFacilityType | null>(facility.value?.facilityType ?? null)
  const formErrors = ref<Record<string, string[]> | undefined>()
  const errorMessage = ref<string | null>(null)
  const isSuccess = ref(false)
  const coordinates = ref<{ lat: string, lng: string } | null>(null)

  const initialValues = computed(() => ({
    facilityType: facility.value?.facilityType.toString() ?? '',
    name: facility.value?.name ?? '',
    companyId: facility.value?.companyId?.toString() ?? '',
    status: facility.value?.status.toString() ?? '',
    contactPersonName: facility.value?.contactPersonName ?? '',
    email: facility.value?.email ?? '',
    phone: facility.value?.phone ?? '',
    position: facility.value?.position ?? '',
    communicationMethod: facility.value?.communicationMethod ?? '',
    address: facility.value?.address ?? '',
    googleMapsUrl: facility.value?.googleMapsUrl ?? '',
  }))

  const validationSchema = object({
    facilityType: string().required(),
    name: string().required().min(2),
    companyId: selectedFacilityType.value === VehicleFacilityType.Parking
      ? string().required()
      : string().optional(),
    status: string().required(),
    contactPersonName: string().optional(),
    email: string().optional().email(),
    phone: createPhoneSchema(t, true),
    position: string().optional(),
    communicationMethod: string().optional(),
    address: string().optional(),
    googleMapsUrl: string().required().url(),
  })

  const fieldsGroup = computed<Record<string, FieldsGroup>>(() => ({
    facilityType: {
      class: 'mb-6',
      wrapperClass: 'mt-4',
      fields: [
        {
          name: 'facilityType',
          as: 'BaseRadioGroup',
          label: t('form.chooseTheType'),
          labelClass: 'mb-3 inline-block font-medium text-neutral-10',
          labelRequired: true,
          options: enums.value?.VehicleFacilityType ?? [],
          wrapperClass: 'flex flex-row flex-wrap gap-30',
          disabled: isFormDisabled.value,
          onChange: (value: string) => {
            selectedFacilityType.value = Number(value)
          },
        },
      ],
    },
    basicInfo: {
      class: 'mb-6',
      fields: [
        {
          name: 'name',
          as: 'BaseInput',
          type: 'text',
          placeholder: t('form.name'),
          label: t('form.name'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
          labelRequired: true,
          containerClass: 'mb-4',
          disabled: isFormDisabled.value,
        },
        ...(selectedFacilityType.value === VehicleFacilityType.Parking ? [
          {
            name: 'companyId',
            as: 'BaseSelect',
            options: companies.value?.items ?? [],
            placeholder: t('form.selectCompany'),
            label: t('form.selectCompany'),
            labelRequired: true,
            searchable: true,
            containerClass: 'mb-4',
            disabled: isFormDisabled.value,
            onScrollToEnd: () => {
              if (hasMoreCompanies.value) loadMoreCompanies()
            },
            onSearch: (searchKeyword: string) => {
              getCompanies(1, searchKeyword)
            },
          },
        ] : []),
        {
          name: 'status',
          as: 'BaseRadioGroup',
          options: enums.value?.VehicleFacilityStatus ?? [],
          label: t('form.statusField'),
          labelClass: 'mb-3 inline-block font-medium text-neutral-10',
          labelRequired: true,
          wrapperClass: 'flex flex-row flex-wrap gap-30',
          disabled: isFormDisabled.value,
        },
      ],
    },
    contacts: {
      title: t('content.contacts'),
      titleClass: 'mb-6 font-bold text-neutral-10',
      class: 'mb-6 rounded-lg border border-neutral-200 bg-white p-4',
      wrapperClass: 'grid grid-cols-1 gap-4 md:grid-cols-2',
      fields: [
        {
          name: 'contactPersonName',
          as: 'BaseInput',
          type: 'text',
          label: t('form.contactPersonName'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
          disabled: isFormDisabled.value,
        },
        {
          name: 'email',
          as: 'BaseInput',
          type: 'email',
          label: t('form.emailAddress'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
          disabled: isFormDisabled.value,
        },
        {
          name: 'phone',
          as: 'BaseInputPhone',
          type: 'tel',
          placeholder: t('form.phonePlaceholder'),
          label: t('form.phoneNumber'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
          labelRequired: true,
          containerClass: 'w-full',
          allowedCountryCode: '+20',
          disabled: isFormDisabled.value,
        },
        {
          name: 'position',
          as: 'BaseInput',
          type: 'text',
          placeholder: t('form.position'),
          label: t('form.position'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
          disabled: isFormDisabled.value,
        },
        {
          name: 'communicationMethod',
          as: 'BaseSelect',
          options: communicationMethods.value,
          placeholder: t('form.communicationMethod'),
          label: t('form.communicationMethod'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
          containerClass: 'w-full col-span-full',
          disabled: isFormDisabled.value,
        },
      ],
    },
    address: {
      title: t('content.addressManagement'),
      titleClass: 'mb-6 font-bold text-neutral-10',
      class: 'mb-6 rounded-lg border border-neutral-200 bg-white p-4',
      wrapperClass: 'flex flex-wrap justify-between',
      fields: [
        {
          name: 'address',
          as: 'BaseInput',
          type: 'text',
          label: t('form.manuallyAddress'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
          containerClass: 'w-full',
          disabled: isFormDisabled.value,
        },
        {
          name: 'googleMapsUrl',
          as: 'BaseInput',
          type: 'text',
          label: t('form.gpsCoordinates'),
          labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
          labelRequired: true,
          containerClass: 'w-full',
          disabled: isFormDisabled.value,
          onInput: (e: InputEvent) => {
            if (isFormDisabled.value) return
            const url = (e.target as HTMLInputElement)?.value?.trim?.()
            if (url) onGoogleMapsUrlInput?.(url)
          },
        },
      ],
    },
  }))

  async function fetchFacility(): Promise<void> {
    if ((!isEditMode.value && !isViewMode.value) || !facilityId.value) return

    setLoading(true)
    const { success, data, message, errors } = await vehicleFacilityStore.getById(facilityId.value)
    setLoading(false)

    if (!success || !data) {
      errorMessage.value = message
      formErrors.value = errors
    }
  }

  async function onSubmit(values: Record<string, unknown>): Promise<void> {
    errorMessage.value = null
    formErrors.value = undefined

    const payload = {
      ...values as unknown as VehicleFacilityFormValues,
      ...(coordinates.value
        ? coordinates.value
        : {}),
    }

    setLoading(true)
    const { success, message, errors } = isEditMode.value
      ? await vehicleFacilityStore.update(facilityId.value, payload)
      : await vehicleFacilityStore.create(payload)
    setLoading(false)

    if (success) {
      isSuccess.value = true

      return
    }

    errorMessage.value = message
    formErrors.value = errors
  }

  function onSuccess(): void {
    isSuccess.value = false
    router.push({ name: 'VehicleFacilitiesLocationsList' })
  }

  return {
    coordinates,
    errorMessage,
    fetchFacility,
    fieldsGroup,
    formErrors,
    initialValues,
    isCreateMode,
    isEditMode,
    isLoading,
    isSuccess,
    isViewMode,
    onSubmit,
    onSuccess,
    secondaryContacts,
    validationSchema,
  }
}
