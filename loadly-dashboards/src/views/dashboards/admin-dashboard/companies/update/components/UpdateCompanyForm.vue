<template>
  <div class="flex flex-col">
    <NavigationTabs
      :tabs="tabs"
      :active-tab="activeTab"
      @update:active-tab="activeTab = $event"
    />

    <div class="flex-1">
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
            {{ successMessage || t('success.companyUpdated') }}
          </p>
        </template>
      </PopupSuccess>

      <CompanyRegistrationForm
        v-if="activeTab === 0"
        :form-values="formData"
        :errors="formErrors"
        @update="updateFormData"
        @prev="prevStep"
        @next="nextStep"
      />

      <LazyBusinessInformation
        v-if="activeTab === 1"
        :form-values="formData"
        :errors="formErrors"
        @update="updateFormData"
        @prev="prevStep"
        @next="nextStep"
      />

      <LazyDocumentUpload
        v-if="activeTab === 2"
        :form-values="formData"
        @update="updateFormData"
        @prev="prevStep"
        @next="nextStep"
      />

      <LazyCompanyProfile
        v-if="activeTab === 3"
        :form-values="formData"
        :original-form-data="props.initialFormData"
        @update="updateFormData"
        @prev="prevStep"
        @next="nextStep"
        @submit="onSubmit"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
  import { useLoading } from '@/composables/useLoading'
  import { useCompanyStore } from '@/stores/admin-dashboard/company'
  import { formDataToCompanyUpdateRequestAdapter } from '@/types/admin-dashboard/request/company-update'
  import IconEditDocument from '@/components/icon/IconEditDocument.vue'
  import IconCreateFolder from '@/components/icon/IconCreateFolder.vue'
  import IconBuildings from '@/components/icon/IconBuildings.vue'
  import IconBriefcase from '@/components/icon/IconBriefcase.vue'
  import CompanyRegistrationForm from './company-registration/CompanyRegistrationForm.vue'
  import FormErrorPopup from './FormErrorPopup.vue'

  const LazyBusinessInformation = defineAsyncComponent(() => import('./business-information/BusinessInformation.vue'))
  const LazyDocumentUpload = defineAsyncComponent(() => import('./company-documents/CompanyDocuments.vue'))
  const LazyCompanyProfile = defineAsyncComponent(() => import('./company-preview/CompanyPreview.vue'))

  const activeTab = ref(0)

  const props = defineProps<{
    initialFormData?: Record<string, unknown>
  }>()

  const formData = ref<Record<string, unknown>>(props.initialFormData || {})

  watch(() => props.initialFormData, (newData) => {
    if (newData && Object.keys(newData).length > 0) {
      formData.value = { ...newData }
    }
  }, { deep: true, immediate: true })

  const router = useRouter()
  const route = useRoute()
  const { setLoading } = useLoading()
  const companyStore = useCompanyStore()

  const errorMessage = ref<string | null>(null)
  const isErrorPopupOpen = ref(false)
  const successMessage = ref<string | null>(null)
  const isSuccessPopupOpen = ref(false)
  const formErrors = ref<Record<string, string[]>>({})

  const { t } = useI18n()

  const tabs = computed(() => [
    {
      title: t('content.companyRegistration'),
      icon: IconBuildings,
    },
    {
      title: t('content.businessInformation'),
      icon: IconBriefcase,
    },
    {
      title: t('content.documentUploading'),
      icon: IconEditDocument,
    },
    {
      title: t('content.companyProfileGeneration'),
      icon: IconCreateFolder,
    },
  ])

  function nextStep(): void {
    if (activeTab.value < tabs.value.length - 1) {
      activeTab.value++
    }
  }

  function prevStep(): void {
    if (activeTab.value > 0) {
      activeTab.value--
    }
  }

  function updateFormData(data: Record<string, unknown>): void {
    formData.value = { ...formData.value, ...data }
  }

  function resetFormState(): void {
    errorMessage.value = null
    isErrorPopupOpen.value = false
    successMessage.value = null
    isSuccessPopupOpen.value = false
    formErrors.value = {}
  }

  async function onSubmit(): Promise<void> {
    resetFormState()
    setLoading(true)

    const id = route.params.id as string
    const adaptedCompany = formDataToCompanyUpdateRequestAdapter(formData.value)

    const { success, message, errors } = await companyStore.update(id, adaptedCompany)
    setLoading(false)

    if (!success) {
      errorMessage.value = message
      formErrors.value = errors || {}
      isErrorPopupOpen.value = true

      return
    }

    successMessage.value = message
    isSuccessPopupOpen.value = true
  }

  function onSuccess(): void {
    resetFormState()
    isSuccessPopupOpen.value = false
    router.push({name: 'CompaniesList'})
  }
</script>

<style scoped>

</style>
