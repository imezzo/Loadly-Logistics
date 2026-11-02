<template>
  <LazyPopupError
    v-if="showError"
    v-model="showError"
    :message="errorMessage"
    :errors="formErrors"
  />

  <LazyPopupSuccess
    v-if="isSuccess"
    :model-value="isSuccess"
    @update:model-value="onSuccess"
  >
    <template #message>
      <p class="text-center text-2xl font-bold text-neutral-10">
        {{ isUpdateMode ? $t('success.driverUpdated') : $t('success.driverCreated') }}
      </p>
    </template>
  </LazyPopupSuccess>

  <DriverInformation
    v-if="currentStep === 1"
    ref="informationStepRef"
    :validation-schema="validationSchema"
    :initial-values="initialValues"
    :groups="fieldsGroup"
    :errors="formErrors"
    :is-update-mode="isUpdateMode"
    @next="next"
    @prev="prev"
  />

  <DriverDocuments
    v-if="currentStep === 2"
    :next-button-label="nextButtonLabel"
    :next-button-disabled="isLoading"
    :initial-documents="(initialValues.documents as DriverDocumentItem[] | undefined) ?? driver?.documents"
    @submit="submit"
    @prev="prev"
  />
</template>

<script setup lang="ts">
  import type { DriverWizardRequest } from '@/types/admin-dashboard/request/driver/driver-wizard'
  import type { DriverDocumentRequest } from '@/types/admin-dashboard/request/driver/driver-wizard'
  import type { ApiDocumentInfo } from '@/types/admin-dashboard/response/document'
  import { driverWizardInformationAdapter } from '@/types/admin-dashboard/request/driver/driver-wizard'
  import { useDriverStore } from '@/stores/admin-dashboard/driver'
  import { useVehicleStore } from '@/stores/admin-dashboard/vehicle'
  import { useLoading } from '@/composables/useLoading'
  import { useLookups } from '@/composables/useLookups'
  import { useDriverForm } from '../composables/useDriverForm'
  import DriverInformation from './DriverInformation/DriverInformation.vue'
  import DriverDocuments from './DriverDocuments/DriverDocuments.vue'
  import type { DriverDocumentItem } from '../composables/useDriverDocuments'

  const LazyPopupSuccess = defineAsyncComponent(() => import('@/components/popup/PopupSuccess.vue'))
  const LazyPopupError = defineAsyncComponent(() => import('@/components/popup/PopupError.vue'))

  const driverStore = useDriverStore()
  const { driver } = storeToRefs(driverStore)
  const vehicleStore = useVehicleStore()
  const { vehicleTypesList } = storeToRefs(vehicleStore)
  const route = useRoute()
  const router = useRouter()
  const { isLoading, setLoading } = useLoading()
  const { t } = useI18n()
  const { getGovernments, getCities } = useLookups()

  const informationStepRef = useTemplateRef<InstanceType<typeof DriverInformation>>('informationStepRef')

  const formValues = computed(() => informationStepRef.value?.formRef?.form?.values)
  const initialValues = ref<Record<string, unknown>>({})

  const {
    initialValues: formInitialValues,
    validationSchema,
    fieldsGroup,
    fetchDriver,
    fetchDistricts,
    driverId,
  } = useDriverForm(formValues, initialValues)

  const isUpdateMode = computed(() => route.name === 'DriverEdit')
  const nextButtonLabel = computed(() => {
    if (isLoading.value) {
      return isUpdateMode.value ? t('form.updating') : t('form.creating')
    }

    return isUpdateMode.value ? t('button.updateDriver') : t('button.addDriver')
  })
  const formErrors = ref<Record<string, string[]> | undefined>()
  const errorMessage = ref<string | null>(null)
  const isSuccess = ref(false)
  const currentStep = ref(1)
  const wizardData = ref<Record<string, unknown>>({})

  const showError = computed({
    get: () => !!errorMessage.value,
    set: (value: boolean) => {
      if (!value) {
        errorMessage.value = null
        formErrors.value = undefined
      }
    },
  })

  onMounted(async () => {
    await getGovernments()
    const { data: vehicleTypes } = await vehicleStore.getVehicleTypes({ page: 1, pageSize: 100 })
    vehicleTypesList.value = vehicleTypes

    if (isUpdateMode.value && driverId.value) {
      await fetchDriver()

      initialValues.value = formInitialValues.value
      const cityId = formInitialValues.value?.cityId
      if (cityId && typeof cityId === 'string') {
        await fetchDistricts(Number(cityId))
      }
    }
  })

  watch(() => formValues.value?.governerateId, async (newGovId) => {
    if (!(newGovId && typeof newGovId === 'string')) return

    await getCities(Number(newGovId))
  })

  watch(() => formValues.value?.cityId, async (newCityId, oldCityId) => {
    if (newCityId !== oldCityId && oldCityId !== undefined) {
      informationStepRef.value?.formRef?.form?.setFieldValue('districtId', '')
    }
    if (newCityId && typeof newCityId === 'string') {
      await fetchDistricts(Number(newCityId))
    }
  })

  function handleSubmissionResponse(
    success: boolean,
    message: string | null,
    errors?: Record<string, string[]>
  ) {
    isSuccess.value = success
    if (!success) {
      errorMessage.value = message
      formErrors.value = errors
    }
  }

  async function submitUpdate(values: Record<string, unknown>): Promise<void> {
    if (!driverId.value) return

    const updateData: DriverWizardRequest = {
      id: driverId.value,
      ...driverWizardInformationAdapter(values),
      documents: (values.documents as DriverDocumentRequest[]) || [],
    }

    const { success, message, errors } = await driverStore.update(updateData)
    handleSubmissionResponse(success, message, errors)
  }

  async function submitCreate(values: Record<string, unknown>): Promise<void> {
    const createData: DriverWizardRequest = {
      ...driverWizardInformationAdapter(values),
      documents: values.documents as DriverWizardRequest['documents'] || [],
    }

    const { success, message, errors } = await driverStore.create(createData)
    handleSubmissionResponse(success, message, errors)
  }

  function next(values?: Record<string, unknown>) {
    wizardData.value = { ...(values ?? formInitialValues.value) }
    currentStep.value = 2
  }

  async function submit(data: Record<string, unknown>) {
    const documents = (data._documents as ApiDocumentInfo[]) || []

    const documentsData = documents.map((doc, index) => {
      const fileKey = `document_${index}` as keyof typeof data
      const file = data[fileKey] as File | undefined

      return {
        ...(doc.id ? { id: doc.id } : {}),
        documentId: doc.id || 0,
        file,
        expiryDate: doc.expirationDate,
        status: doc.status ? Number(doc.status) : undefined,
      }
    })

    const values = { ...wizardData.value, documents: documentsData }
    errorMessage.value = null
    formErrors.value = undefined
    setLoading(true)

    if (isUpdateMode.value && driverId.value) {
      await submitUpdate(values)
    } else {
      await submitCreate(values)
    }

    setLoading(false)
  }

  function prev(documents: DriverDocumentItem[]) {
    initialValues.value = { ...wizardData.value, documents }
    currentStep.value--
  }

  function onSuccess() {
    isSuccess.value = false
    router.push({name: 'Drivers'})
  }
</script>

<style scoped>

</style>
