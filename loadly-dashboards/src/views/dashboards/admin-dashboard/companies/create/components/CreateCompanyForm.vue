<template>
  <div
    class="
      flex flex-col gap-6
      lg:flex-row lg:gap-12
    "
  >
    <NavigationMenu
      :steps="steps"
      :current-step="currentStep"
    />

    <div
      class="
        flex-1 border-gray-200 ps-6
        md:border-s
      "
    >
      <FormErrorPopup
        v-model="isErrorPopupOpen"
        :message="errorMessage"
        :form-errors="formErrors"
      />

      <PopupSuccess
        v-model="isSuccessPopupOpen"
        @update:model-value="onSuccess"
      >
        <template #message>
          <p class="text-center text-2xl font-bold text-neutral-10">
            {{ successMessage || t('success.companyCreated') }}
          </p>
        </template>
      </PopupSuccess>

      <CompanyRegistration
        v-if="currentStep === 0"
        :form-values="formData"
        :errors="formErrors"
        @update="updateFormData"
        @prev="prevStep"
        @next="nextStep"
      />

      <LazyBusinessInformation
        v-if="currentStep === 1"
        :form-values="formData"
        :errors="formErrors"
        @update="updateFormData"
        @prev="prevStep"
        @next="nextStep"
      />

      <LazyDocumentUpload
        v-if="currentStep === 2"
        :form-values="formData"
        @update="updateFormData"
        @prev="prevStep"
        @next="nextStep"
      />

      <LazyCompanyProfile
        v-if="currentStep === 3"
        :form-values="formData"
        @update="updateFormData"
        @prev="prevStep"
        @next="nextStep"
        @submit="submitForm"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
  import { useCompanyStore } from '@/stores/admin-dashboard/company'
  import { useLoading } from '@/composables/useLoading'
  import IconEditDocument from '@/components/icon/IconEditDocument.vue'
  import IconCreateFolder from '@/components/icon/IconCreateFolder.vue'
  import IconBuildings from '@/components/icon/IconBuildings.vue'
  import IconBriefcase from '@/components/icon/IconBriefcase.vue'
  import CompanyRegistration from './company-registration/CompanyRegistration.vue'
  import FormErrorPopup from './FormErrorPopup.vue'
  import type { CompanyCreateRequest } from '@/types/admin-dashboard/request/company'

  const LazyBusinessInformation = defineAsyncComponent(() => import('./business-information/BusinessInformation.vue'))
  const LazyDocumentUpload = defineAsyncComponent(() => import('./company-documents/CompanyDocuments.vue'))
  const LazyCompanyProfile = defineAsyncComponent(() => import('./company-preview/CompanyPreview.vue'))

  enum CompanyWizardStep {
    CompanyRegistration,
    BusinessInformation,
    DocumentUploading,
    CompanyProfileGeneration,
  }

  const currentStep = ref(0)

  const formData = ref<Record<string, unknown>>({})

  const companyStore = useCompanyStore()
  const router = useRouter()
  const { setLoading } = useLoading()

  const errorMessage = ref<string | null>(null)
  const formErrors = ref<Record<string, string[]>>({})
  const isErrorPopupOpen = ref(false)
  const successMessage = ref<string | null>(null)
  const isSuccessPopupOpen = ref(false)

  const { t } = useI18n()

  const STEP_FIELD_MAPPINGS: Record<number, string[]> = {
    0: [
      'nameEn',
      'nameAr',
      'registrationNumber',
      'taxId',
      'websiteUrl',
      'contactPersonName',
      'userName',
      'email',
      'phone',
      'street',
      'buildingNumber',
      'district',
      'city',
      'governorate',
      'postalCode',
      'secondaryContacts',
    ],
    1: [
      'industryType',
      'companySize',
      'serviceRequirements',
      'operatingHours',
      'preferredCommunicationMethods',
    ],
    2: [
      'documents',
      'document_0',
      'document_1',
      'document_2',
      'document_3',
      '_documents',
    ],
  }

  const steps = computed(() => [
    { title: t('content.companyRegistration'), icon: IconBuildings, id: CompanyWizardStep.CompanyRegistration },
    { title: t('content.businessInformation'), icon: IconBriefcase, id: CompanyWizardStep.BusinessInformation },
    { title: t('content.documentUploading'), icon: IconEditDocument, id: CompanyWizardStep.DocumentUploading },
    { title: t('content.companyProfileGeneration'), icon: IconCreateFolder, id: CompanyWizardStep.CompanyProfileGeneration },
  ])

  function nextStep(): void {
    if (currentStep.value < steps.value.length - 1) {
      currentStep.value++
    }
  }

  function prevStep(): void {
    if (currentStep.value > 0) {
      currentStep.value--
    }
  }

  function updateFormData(data: Record<string, unknown>): void {
    formData.value = { ...formData.value, ...data }
  }

  function resetFormState(): void {
    errorMessage.value = null
    formErrors.value = {}
    isErrorPopupOpen.value = false
    successMessage.value = null
    isSuccessPopupOpen.value = false
  }

  async function submitForm(): Promise<void> {
    resetFormState()
    setLoading(true)

    const { success, message, errors } = await companyStore.create(formData.value as unknown as CompanyCreateRequest)
    setLoading(false)

    if (!success) {
      errorMessage.value = message
      formErrors.value = errors || {}
      isErrorPopupOpen.value = true

      if (!errors || !Object.keys(errors).length) return

      const targetStep = findStepWithErrors(errors)
      if (targetStep !== null) currentStep.value = targetStep

      return
    }

    successMessage.value = message || null
    isSuccessPopupOpen.value = true
  }

  function findStepWithErrors(errors: Record<string, string[]>): number | null {
    const errorFields = Object.keys(errors)

    for (const [stepIndex, fields] of Object.entries(STEP_FIELD_MAPPINGS)) {
      const step = Number(stepIndex)
      const hasError = errorFields.some(field => fields.includes(field))

      if (hasError) {
        return step
      }
    }

    return errorFields.length > 0 ? 0 : null
  }

  function onSuccess(): void {
    isSuccessPopupOpen.value = false
    router.push({name: 'CompaniesList'})
  }
</script>

<style scoped>

</style>
