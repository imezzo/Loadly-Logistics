import { array, object, string } from 'yup'
import { ApiUserType, UserRole, UserStatus } from '@/types/user'
import { useVeeValidate } from '@/composables/useVeeValidate'
import type { FieldsGroup } from '@/types/form'
import type { UserCreateRequest } from '@/types/request/user'
import { useUserStore } from '@/stores/admin-dashboard/user'
import type { User } from '@/types/response/user'
import { useLoading } from '@/composables/useLoading'
import { createEmailSchema } from '@/utils/validation/email'
import { useEnum } from '@/composables/useEnum'
import { createPhoneSchema } from '@/utils/validation/phone'
import { useLookups } from '@/composables/useLookups'
import { useApp } from '@/composables/useApp'

export function useUserWizardForm() {
  const { t } = useI18n()
  const route = useRoute()
  const { isAdminDashboard } = useApp()

  const userStore = useUserStore()
  const { user } = storeToRefs(userStore)
  const { enums } = useEnum()
  useVeeValidate()
  const { isLoading, setLoading } = useLoading()
  const {
    getRoles,
    hasMoreRoles,
    loadMoreRoles,
    roles,
  } = useLookups()

  const isCollectorModule = computed(() => ['CreateCollector', 'EditCollector', 'ViewCollector'].includes(route.name as string))
  const isEditMode = computed(() => ['EditUser', 'EditCollector'].includes(route.name as string))
  const isViewMode = computed(() => ['ViewUser', 'ViewCollector'].includes(route.name as string))
  const isFormDisabled = computed(() => isViewMode.value || (isEditMode.value && user.value?.status === UserStatus.DISABLED))
  const roleGuardName = computed(() => isCollectorModule.value ? 'collector' : 'admin')

  const userId = computed(() => route.params.id as string | undefined)
  const formErrors = ref<Record<string, string[]> | undefined>()
  const errorMessage = ref<string | null>(null)
  const isSuccess = ref(false)
  type UserFormValues = Omit<UserCreateRequest, 'roleIds' | 'roleId'> & {
    roleIds?: string[]
    roleId?: string
  }

  const initialValues = ref<UserFormValues>({
    fullName: '',
    userName: '',
    phone: '',
    email: '',
    type: UserRole.ADMIN,
    status: 1,
    mfaChannel: '0',
    ...(isCollectorModule.value ? { roleId: '' } : { roleIds: [] }),
  })

  const validationSchema = object({
    fullName: string()
      .required()
      .min(3),
    ...(isAdminDashboard ? { userName: string()
      .required()
      .min(3) } : {}),
    phone: createPhoneSchema(t, isCollectorModule.value),
    email: createEmailSchema(t, !isCollectorModule.value),
    type: string()
      .required(),
    mfaChannel: string()
      .required(),
    ...(isCollectorModule.value
      ? {
        roleId: string()
          .required(),
      }
      : {
        roleIds: array()
          .of(string()),
      }),
  })

  const fieldsGroup = computed<Record<string, FieldsGroup>>(() => ({
    personalInformation: {
      title: t('title.personalInformation'),
      description: t('form.editYourPersonalInformation'),
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
          fieldSlot: true,
          labelRequired: true,
          labelInfo: true,
          disabled: isFormDisabled.value,
        },
        ...(isAdminDashboard ? [{
          name: 'userName',
          as: 'BaseInput',
          type: 'text',
          placeholder: t('form.userName'),
          label: t('form.userName'),
          labelClass: 'mt-6 flex items-center font-medium text-neutral-10',
          fieldSlot: true,
          labelRequired: true,
          labelInfo: true,
          disabled: isFormDisabled.value,
        }] : []),
        {
          name: 'phone',
          as: 'BaseInputPhone',
          type: 'tel',
          placeholder: t('form.phonePlaceholder'),
          label: t('form.phoneNumber'),
          labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
          fieldSlot: true,
          labelRequired: isCollectorModule.value,
          disabled: isFormDisabled.value,
        },
        {
          name: 'email',
          as: 'BaseInput',
          type: 'email',
          placeholder: t('form.email'),
          label: t('form.email'),
          labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
          fieldSlot: true,
          labelRequired: !isCollectorModule.value,
          labelInfo: true,
          disabled: isEditMode.value || isViewMode.value,
          hint: isEditMode.value ? t('form.notEditableAfterCreation') : undefined,
          hintClass: isEditMode.value ? 'text-error-600 mt-1' : undefined,
        },
      ],
    },
    contactInformation: {
      title: t('title.role'),
      description: t('content.rolesAndPermissionsManagement'),
      class: 'my-6 grid grid-cols-1 gap-8 md:grid-cols-[280px_1fr]',
      wrapperClass: 'font-medium',
      titleClass: 'mb-1 text-neutral-600',
      descriptionClass: 'text-neutral-700',
      fields: [
        ...(isCollectorModule.value
          ? [{
            name: 'roleId',
            as: 'BaseSelect',
            options: roles.value ?? [],
            label: t('form.chooseRoleAndPermission'),
            labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
            fieldSlot: true,
            labelRequired: true,
            labelInfo: true,
            searchable: true,
            disabled: isFormDisabled.value,
            onSearch: (searchKeyword: string) => {
              getRoles(1, searchKeyword, roleGuardName.value)
            },
            onScrollToEnd: () => {
              if (hasMoreRoles.value) {
                loadMoreRoles()
              }
            },
          }]
          : [{
            name: 'roleIds',
            as: 'BaseMultiSelect',
            options: roles.value ?? [],
            label: t('form.chooseRoleAndPermission'),
            labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
            fieldSlot: true,
            labelRequired: true,
            labelInfo: true,
            searchable: true,
            disabled: isFormDisabled.value,
            onSearch: (searchKeyword: string) => {
              getRoles(1, searchKeyword, roleGuardName.value)
            },
            onScrollToEnd: () => {
              if (hasMoreRoles.value) {
                loadMoreRoles()
              }
            },
          }]),
        ...(isEditMode.value || isViewMode.value
          ? [{
            name: 'status',
            as: 'BaseSelect',
            options: enums.value?.UserStatus ?? [],
            label: t('form.statusField'),
            labelClass: 'mt-6 mb-1 flex items-center font-medium text-neutral-10',
            fieldSlot: true,
            disabled: isFormDisabled.value,
          }]
          : []),
        {
          name: 'mfaChannel',
          as: 'BaseSelect',
          options: [
            { value: '0', label: t('form.none') },
            ...(isCollectorModule.value ? [{ value: '1', label: t('form.email') }] : []),
            { value: '2', label: t('form.sms') },
          ],
          containerClass: 'mt-6',
          label: t('form.mfaChannel'),
          labelClass: 'mb-1 flex items-center font-medium text-neutral-10',
          labelRequired: true,
        },
      ],
    },
  }))

  function setInitialValues(data: User) {
    initialValues.value = {
      fullName: data.fullName,
      userName: data.userName,
      phone: data.phone,
      email: data.email,
      type: data.type === ApiUserType.ADMIN
        ? UserRole.ADMIN
        : UserRole.SUPERADMIN,
      status: data.status,
      mfaChannel: data.mfaChannel.toString(),
      ...(isCollectorModule.value
        ? { roleId: data.roles[0]?.id ?? '' }
        : { roleIds: data.roles.map((role) => role.id) }),
    }
  }

  async function getUser() {
    if (!userId.value) return

    setLoading(true)
    const { success, data, message, errors } = await userStore.get(userId.value)
    setLoading(false)

    if (success) {
      user.value = data
      setInitialValues(data)

      return
    }
    errorMessage.value = message
    formErrors.value = errors
  }

  const isDeactivateConfirmed = ref(false)
  const isDeactivateConfirmNeeded = ref(false)

  async function onSubmit(values: Record<string, unknown>): Promise<void> {
    errorMessage.value = null
    formErrors.value = undefined

    if (!isEditMode.value) {
      setLoading(true)
      const { success, data, message, errors } = await userStore.create(values as unknown as UserCreateRequest)
      setLoading(false)

      if (success) {
        isSuccess.value = true
        user.value = data

        return
      }
      errorMessage.value = message
      formErrors.value = errors

      return
    }

    if (!user.value?.status) {
      values.status = 1 as unknown as number
    }

    if (!values.status && !isDeactivateConfirmed.value) {
      isDeactivateConfirmed.value = false
      isDeactivateConfirmNeeded.value = true

      return
    }

    setLoading(true)
    const { success, message, errors } = await userStore.update({
      ...values as unknown as UserCreateRequest,
      id: userId.value as string,
    })

    setLoading(false)
    isSuccess.value = success
    isDeactivateConfirmed.value = false
    isDeactivateConfirmNeeded.value = false
    if (!success) {
      errorMessage.value = message
      formErrors.value = errors
    }
  }

  return {
    errorMessage,
    fetchRoles: getRoles,
    fieldsGroup,
    formErrors,
    getUser,
    initialValues,
    isDeactivateConfirmNeeded,
    isDeactivateConfirmed,
    isEditMode,
    isLoading,
    isSuccess,
    onSubmit,
    user,
    validationSchema,
    isViewMode,
  }
}
