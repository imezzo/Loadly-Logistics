<template>
  <div class="page-content">
    <BaseBreadcrumb
      :items="breadcrumbItems"
      class="mb-2"
    />

    <div class="mb-6 flex items-center justify-between">
      <h1 class="text-xl font-bold text-neutral-800">
        {{ t('title.editCompany', { name: companyName}) }}
      </h1>

      <BaseSelect
        :model-value="companyStatus"
        :options="statusOptionsWithContent"
        wrapper-class="[&>.select-button]:rounded-full [&>.select-button]:border [&>.select-button]:border-neutral-300 [&>.select-button]:bg-white [&>.select-button]:px-4 [&>.select-button]:py-2 [&>.select-button]:min-w-32"
        @update:model-value="onStatusChange"
      />
    </div>

    <UpdateCompanyForm
      :initial-form-data="initialFormData"
      @update:form-data="initialFormData = $event"
    />
  </div>
</template>

<script setup lang="ts">
  import UpdateCompanyForm from './components/UpdateCompanyForm.vue'
  import type { SelectOption } from '@/types/ui'
  import { useCompanyData } from './composables/useCompanyData'
  import { useLookups } from '@/composables/useLookups'
  import { useDocuments } from './composables/useDocuments'
  import { useEnum } from '@/composables/useEnum'

  const { t } = useI18n()
  const route = useRoute()
  const { fetchCompany } = useCompanyData()

  const {
    getIndustryTypes,
    getCommunicationMethods,
    getServiceRequirements,
    getGovernments,
    getCities,
    getOperatingHours,
  } = useLookups()
  const { enums } = useEnum()

  const { fetchDocuments } = useDocuments(() => {
    // No-op emit function for fetching documents
  })

  const companyName = ref<string>('')
  const companyStatus = ref<string>('')
  const initialFormData = ref<Record<string, unknown>>({})

  const breadcrumbItems = computed(() => [
    { label: t('title.companies'), to: '/companies' },
    { label: companyName.value || 'Company' },
  ])

  const statusOptions = computed<SelectOption[]>(() =>
    enums.value?.VerificationStatus ?? []
  )

  const statusOptionsWithContent = computed<SelectOption[]>(() => {
    return statusOptions.value.map((option) => ({
      ...option,
      content: () => {
        const textColor = getStatusTextClass(Number(option.value))

        return `
          <div class="flex items-center gap-2">
            <span class="text-sm font-medium ${textColor}">${option.label}</span>
          </div>
        `
      },
    }))
  })

  function getStatusTextClass(status: number): string {
    switch (status) {
      case 1:
        return 'text-green-600'
      case 0:
        return 'text-gray-800'
      case 2:
        return 'text-red-600'
      default:
        return 'text-gray-600'
    }
  }

  function onStatusChange(value: string): void {
    companyStatus.value = value

    if (initialFormData.value) {
      initialFormData.value = {
        ...initialFormData.value,
        verificationStatus: value,
      }
    }
  }

  onMounted(async () => {
    const companyId = route.params.id as string

    if (!companyId) {
      return
    }

    await Promise.all([
      getIndustryTypes(),
      getCommunicationMethods(),
      getServiceRequirements(),
      getGovernments(),
      getCities(),
      fetchDocuments(),
      getOperatingHours(),
    ])

    const result = await fetchCompany(companyId)

    if (!result) return
    companyName.value = result.companyName

    companyStatus.value = String(result.companyStatus)

    if (result.formData) {
      initialFormData.value = result.formData as Record<string, unknown>
      initialFormData.value.verificationStatus = result.companyStatus
    }

    if (result.formData?.governorate) {
      await getCities(Number(result.formData.governorate))
    }
  })
</script>

<style scoped>

</style>

