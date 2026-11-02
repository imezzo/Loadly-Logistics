import { array, mixed, number, object, string } from 'yup'
import type { FieldsGroup } from '@/types/form'
import type { BroadcastNotification } from '@/types/admin-dashboard/response/broadcast-notification'
import {
  API_CHANNEL_MAP,
  BroadcastNotificationChannel,
  BroadcastNotificationModelType,
} from '@/types/admin-dashboard/broadcast-notification'
import { useBroadcastNotificationStore } from '@/stores/admin-dashboard/broadcast-notification'
import { useLookupStore } from '@/stores/lookup'
import { useLookups } from '@/composables/useLookups'
import { useVeeValidate } from '@/composables/useVeeValidate'
import { useLoading } from '@/composables/useLoading'
import type { BroadcastNotificationSendRequest } from '@/types/admin-dashboard/request/broadcast-notifications'

export function useBroadcastNotificationWizard() {
  const { t } = useI18n()
  const route = useRoute()
  useVeeValidate()
  const { isLoading, setLoading } = useLoading()

  const broadcastNotificationStore = useBroadcastNotificationStore()
  const { broadcastNotification } = storeToRefs(broadcastNotificationStore)

  const lookupStore = useLookupStore()
  const {
    admins,
    companies,
    drivers,
    getAdmins,
    getCompanies,
    getDrivers,
    hasMoreAdmins,
    hasMoreCompanies,
    hasMoreDrivers,
    loadMoreAdmins,
    loadMoreCompanies,
    loadMoreDrivers,
  } = useLookups()

  const isViewMode = computed(() => route.name === 'ViewBroadcastNotification')
  const isFormDisabled = computed(() => isViewMode.value)

  const selectedModelType = ref<string>(BroadcastNotificationModelType.COMPANY)

  const formErrors = ref<Record<string, string[]> | undefined>()
  const errorMessage = ref<string | null>(null)
  const isSuccess = ref(false)
  const formRef = ref<{ form?: { setFieldValue: (name: string, value: unknown) => void } } | null>(null)

  const initialValues = ref<BroadcastNotificationSendRequest>({
    attachments: null,
    body: '',
    channel: BroadcastNotificationChannel.IN_APP,
    modelIds: [],
    modelType: BroadcastNotificationModelType.COMPANY,
    subject: '',
  })

  const validationSchema = object({
    attachments: mixed<File>().nullable().optional(),
    body: string().required(),
    channel: number().required(),
    modelIds: array().of(string()).optional(),
    modelType: string().required(),
    subject: string().required(),
  })

  const channelOptions = computed(() => [
    { value: BroadcastNotificationChannel.EMAIL, label: t('form.email') },
    { value: BroadcastNotificationChannel.SMS, label: t('form.sms') },
    { value: BroadcastNotificationChannel.IN_APP, label: t('form.inApp') },
  ])

  const modelTypeOptions = computed(() => [
    { value: BroadcastNotificationModelType.DISPATCHER, label: t('form.dispatchers') },
    { value: BroadcastNotificationModelType.COMPANY, label: t('title.companies') },
    { value: BroadcastNotificationModelType.DRIVER, label: t('title.drivers') },
  ])

  async function fetchLookupForModelType(modelType: string) {
    switch (modelType) {
      case BroadcastNotificationModelType.DISPATCHER:
        await getAdmins(1)
        break
      case BroadcastNotificationModelType.COMPANY:
        await getCompanies(1)
        break
      case BroadcastNotificationModelType.DRIVER:
        await getDrivers({ page: 1 })
        break
    }
  }

  function onModelTypeChange(value: string | number) {
    const modelType = String(value)

    if (selectedModelType.value === modelType) return

    selectedModelType.value = modelType
    formRef.value?.form?.setFieldValue('modelIds', [])
    fetchLookupForModelType(modelType)
  }

  const getModelIdsField = computed(() => {
    const fields = getModelIdsFieldByType.value

    if (!isViewMode.value) return fields

    return fields.map(field => ({
      ...field,
      options: broadcastNotification.value?.models ?? [],
    }))
  })

  const getModelIdsFieldByType = computed(() => {
    const baseField = {
      name: 'modelIds',
      as: 'BaseMultiSelect',
      containerClass: 'w-full',
      searchable: true,
      disabled: isFormDisabled.value,
    }

    switch (selectedModelType.value) {
      case BroadcastNotificationModelType.DISPATCHER:
        return [{
          ...baseField,
          label: t('form.selectDispatchers'),
          options: admins.value?.items ?? [],
          onSearch: (searchKeyword: string) => getAdmins(1, searchKeyword),
          onScrollToEnd: () => {
            if (hasMoreAdmins.value) {
              loadMoreAdmins()
            }
          },
        }]
      case BroadcastNotificationModelType.COMPANY:
        return [{
          ...baseField,
          label: t('form.selectCompanies'),
          options: companies.value?.items ?? [],
          onSearch: (searchKeyword: string) => getCompanies(1, searchKeyword),
          onScrollToEnd: () => {
            if (hasMoreCompanies.value) {
              loadMoreCompanies()
            }
          },
        }]
      case BroadcastNotificationModelType.DRIVER:
        return [{
          ...baseField,
          label: t('form.selectDrivers'),
          options: drivers.value?.items ?? [],
          onSearch: async (searchKeyword: string) => {
            const { data } = await lookupStore.getDrivers({ page: 1, search: searchKeyword })
            drivers.value = data
          },
          onScrollToEnd: () => {
            if (hasMoreDrivers.value) {
              loadMoreDrivers()
            }
          },
        }]
      default:
        return []
    }
  })

  const fieldsGroup = computed<Record<string, FieldsGroup>>(() => ({
    channel: {
      title: t('table.channel'),
      class: 'mb-6 grid grid-cols-1 gap-6 md:grid-cols-[280px_1fr]',
      titleClass: 'text-neutral-10 font-bold',
      fields: [
        {
          name: 'channel',
          as: 'BaseRadioGroup',
          options: channelOptions.value,
          label: t('form.chooseChannel'),
          labelRequired: true,
          containerClass: 'w-full',
          disabled: isFormDisabled.value,
        },
      ],
    },
    whoYouSend: {
      title: t('content.whoYouSend'),
      class: 'mb-6 grid grid-cols-1 gap-6 md:grid-cols-[280px_1fr]',
      titleClass: 'text-neutral-10 font-bold',
      wrapperClass: 'flex flex-col gap-6',
      fields: [
        {
          name: 'modelType',
          as: 'BaseRadioGroup',
          options: modelTypeOptions.value,
          label: t('form.chooseUserTypes'),
          labelRequired: true,
          containerClass: 'w-full',
          disabled: isFormDisabled.value,
          onChange: onModelTypeChange,
        },
        ...getModelIdsField.value,
      ],
    },
    notificationMessage: {
      title: t('content.notificationMessage'),
      class: 'mb-6 grid grid-cols-1 gap-6 md:grid-cols-[280px_1fr]',
      titleClass: 'text-neutral-10 font-bold',
      wrapperClass: 'flex flex-col gap-6',
      fields: [
        {
          name: 'subject',
          as: 'BaseInput',
          type: 'text',
          label: t('form.subject'),
          labelRequired: true,
          containerClass: 'w-full',
          disabled: isFormDisabled.value,
        },
        {
          name: 'body',
          as: 'BaseTextarea',
          label: t('form.notificationMessage'),
          labelRequired: true,
          containerClass: 'w-full',
          rows: 5,
          disabled: isFormDisabled.value,
        },
        {
          name: 'attachments',
          as: 'BaseUploader',
          label: t('content.uploadAttachment'),
          class: 'w-full',
          containerClass: 'w-full',
          disabled: isFormDisabled.value,
          fileUrl: isViewMode.value ? broadcastNotification.value?.attachments[0]?.url : null,
          showPreview: isViewMode.value,
        },
      ],
    },
  }))

  function setInitialValues(data: BroadcastNotification) {
    selectedModelType.value = data.modelType
    initialValues.value = {
      body: data.body,
      channel: API_CHANNEL_MAP[data.channel] ?? BroadcastNotificationChannel.IN_APP,
      modelIds: data.models.map(model => model.value),
      modelType: data.modelType,
      subject: data.subject,
    }
  }

  async function initLookups() {
    await fetchLookupForModelType(selectedModelType.value)
  }

  async function getNotification() {
    const notificationId = route.params.id?.toString()
    if (!notificationId) return

    setLoading(true)
    const { success, data, message, errors } = await broadcastNotificationStore.get(notificationId)
    setLoading(false)

    if (success) {
      broadcastNotification.value = data
      setInitialValues(data)

      return
    }

    errorMessage.value = message
    formErrors.value = errors
  }

  async function onSubmit(values: Record<string, unknown>): Promise<void> {
    if (isViewMode.value) return

    errorMessage.value = null
    formErrors.value = undefined
    setLoading(true)

    const { success, message, errors } = await broadcastNotificationStore.send({
      attachments: values.attachments,
      body: values.body,
      channel: values.channel,
      modelIds: values.modelIds,
      modelType: values.modelType,
      subject: values.subject,
    } as BroadcastNotificationSendRequest)

    setLoading(false)

    if (success) {
      isSuccess.value = true

      return
    }

    errorMessage.value = message
    formErrors.value = errors
  }

  return {
    broadcastNotification,
    errorMessage,
    fieldsGroup,
    formErrors,
    formRef,
    getNotification,
    initLookups,
    initialValues,
    isFormDisabled,
    isLoading,
    isSuccess,
    isViewMode,
    onSubmit,
    validationSchema,
  }
}
