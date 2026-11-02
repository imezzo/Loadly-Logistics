import { object, string } from 'yup'
import type { FieldsGroup } from '@/types/form'
import type { ContractorCreateRequest } from '@/types/admin-dashboard/request/contractor/contractor-wizard'
import type { Contractor } from '@/types/admin-dashboard/response/contractor/contractor'
import { useContractorStore } from '@/stores/admin-dashboard/contractor'
import { useVeeValidate } from '@/composables/useVeeValidate'
import { useLoading } from '@/composables/useLoading'
import { useEnum } from '@/composables/useEnum'
import { createPhoneSchema } from '@/utils/validation/phone'
import type { ApiResponse } from '@/types/admin-dashboard/response/response'

export function useContractorForm(contractorId?: string) {
  const { t } = useI18n()
  const route = useRoute()
  const contractorStore = useContractorStore()
  const { contractor } = storeToRefs(contractorStore)
  const { enums } = useEnum()
  useVeeValidate()
  const { isLoading, setLoading } = useLoading()

  const isEditMode = computed(() => route.name === 'EditContractor')
  const isViewMode = computed(() => route.name === 'ViewContractor')
  const isFormDisabled = computed(() => isViewMode.value)

  const formErrors = ref<Record<string, string[]> | undefined>()
  const errorMessage = ref<string | null>(null)
  const isSuccess = ref(false)

  const initialValues = ref<ContractorCreateRequest>({
    fullName: '',
    email: '',
    phone: '',
    secondPhoneNumber: '',
    nationalId: '',
    status: '',
    contractorStatus: false,
  })

  const validationSchema = object({
    fullName: string()
      .required()
      .min(3),
    phone: createPhoneSchema(t),
    secondPhoneNumber: createPhoneSchema(t),
    nationalId: string().required().min(14).max(14),
    status: string().required(),
  })

  const fieldsGroup = computed<Record<string, FieldsGroup>>(() => ({
    personalInformation: {
      title: t('content.personalInformationManagement'),
      description: '',
      class: 'mb-6 grid grid-cols-1 gap-8 md:grid-cols-[280px_1fr]',
      wrapperClass: 'font-medium',
      titleClass: 'mb-1 text-neutral-600',
      descriptionClass: 'text-neutral-700',
      fields: [
        {
          name: 'fullName',
          as: 'BaseInput',
          type: 'text',
          placeholder: t('form.fullName'),
          label: t('form.fullName'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
          labelRequired: true,
          disabled: isFormDisabled.value,
        },
        {
          name: 'phone',
          as: 'BaseInputPhone',
          type: 'tel',
          placeholder: t('form.phonePlaceholder'),
          label: t('form.phoneNumber'),
          labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
          labelRequired: true,
          disabled: isFormDisabled.value,
        },
        {
          name: 'secondPhoneNumber',
          as: 'BaseInputPhone',
          type: 'tel',
          placeholder: t('form.phonePlaceholder'),
          label: t('form.secondPhoneNumber'),
          labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
          disabled: isFormDisabled.value,
        },
        {
          name: 'email',
          as: 'BaseInput',
          type: 'email',
          placeholder: t('form.email'),
          label: t('form.email'),
          labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
          disabled: isEditMode.value || isViewMode.value,
          hint: isEditMode.value ? t('form.notEditableAfterCreation') : undefined,
          hintClass: isEditMode.value ? 'text-error-600 mt-1' : undefined,
        },
        {
          name: 'nationalId',
          as: 'BaseInput',
          type: 'text',
          placeholder: t('form.nationalId'),
          label: t('form.nationalId'),
          labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
          labelRequired: true,
          disabled: isFormDisabled.value,
          fieldSlot: true,
        },
        {
          name: 'status',
          as: 'BaseSelect',
          options: enums.value?.UserStatus ?? [],
          label: t('form.statusField'),
          labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
          labelRequired: true,
          disabled: isFormDisabled.value,
        },
        {
          name: 'contractorStatus',
          as: 'BaseCheckbox',
          label: t('form.isContractorActive'),
          labelClass: 'hidden',
          containerClass: 'mt-6',
          disabled: isFormDisabled.value,
        },
      ],
    },
  }))

  function setInitialValues(data: Contractor) {
    initialValues.value = {
      fullName: data.fullName,
      phone: data.phone,
      secondPhoneNumber: data.secondPhoneNumber,
      nationalId: data.nationalId,
      email: data.email,
      contractorStatus: Boolean(Number(data.contractorStatus)),
      status: String(data.status ?? ''),
    }
  }

  async function getContractor() {
    if (!contractorId) return

    setLoading(true)
    const { success, data, message, errors } = await contractorStore.get(contractorId)
    setLoading(false)

    if (success) {
      contractor.value = data
      setInitialValues(data)

      return
    }
    errorMessage.value = message
    formErrors.value = errors
  }

  async function onSubmit(values: Record<string, unknown>): Promise<void> {
    errorMessage.value = null
    formErrors.value = undefined

    let response: ApiResponse<Contractor | null>

    contractor.value = {
      ...contractor.value,
      ...values,
    } as Contractor

    setLoading(true)
    if (!isEditMode.value) {
      response = await contractorStore.create(contractor.value as unknown as ContractorCreateRequest)
    } else {
      response = await contractorStore.update({
        ...contractor.value as unknown as ContractorCreateRequest,
        id: contractorId as string,
      })
    }
    setLoading(false)

    const { success, data, message, errors } = response

    if (success) {
      isSuccess.value = true
      contractor.value = data

      return
    }
    errorMessage.value = message
    formErrors.value = errors
  }

  return {
    contractor,
    errorMessage,
    fieldsGroup,
    formErrors,
    getContractor,
    initialValues,
    isEditMode,
    isFormDisabled,
    isLoading,
    isSuccess,
    isViewMode,
    onSubmit,
    validationSchema,
  }
}
