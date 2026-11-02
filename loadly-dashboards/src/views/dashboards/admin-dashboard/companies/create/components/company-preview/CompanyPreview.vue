<template>
  <div class="space-y-6">
    <CompanyDetailsCard
      :form-data="formData"
      :company-name="companyName"
      :cities="cities"
      :governments="governments"
    />

    <BusinessInformationCard
      :form-data="formData"
      :industry-types="industryTypes"
      :service-requirements="serviceRequirements"
      :communication-methods="communicationMethods"
      :operating-hours="operatingHours"
    />

    <DocumentsCard :document-list="documentList" />

    <BaseActions
      :prev-button-options="{ label: t('button.back'), hasScrollTop: true }"
      :next-button-options="nextButtonOptions"
      @prev="$emit('prev')"
      @next="$emit('submit')"
    />
  </div>
</template>

<script setup lang="ts">
  import CompanyDetailsCard from './CompanyDetailsCard.vue'
  import BusinessInformationCard from './BusinessInformationCard.vue'
  import DocumentsCard from './DocumentsCard.vue'
  import { useLookups } from '@/composables/useLookups'
  import { useDocuments } from '../../composables/useDocuments'
  import { getLocalizedValue } from '@/utils/i18n'
  import type { DocumentInfo } from '@/types/admin-dashboard/response/document'

  const props = defineProps<{
    formValues?: Record<string, unknown>
  }>()

  const emit = defineEmits<{
    update: [data: Record<string, unknown>]
    prev: []
    next: []
    submit: []
  }>()

  const { t } = useI18n()

  const nextButtonOptions = computed(() => ({
    label: t('button.addCompany'),
    class: 'flex h-13.5 items-center justify-center gap-2 rounded-2xl bg-success-500 px-6 py-3 text-sm font-semibold text-white transition-colors hover:bg-success-500/90 md:w-52.5',
  }))

  const {
    industryTypes,
    serviceRequirements,
    communicationMethods,
    operatingHours,
    governments,
    cities,
    getGovernments,
    getCities,
    getIndustryTypes,
    getServiceRequirements,
    getCommunicationMethods,
  } = useLookups()

  const formData = computed(() => props.formValues || {})

  const { documents: documentsList, fetchDocuments } = useDocuments(
    (event: 'update', data: Record<string, unknown>) => emit(event, data)
  )

  onMounted(async () => {
    await Promise.all([
      getIndustryTypes(),
      getServiceRequirements(),
      getCommunicationMethods(),
      getGovernments(),
      fetchDocuments(),
    ])

    if (formData.value.governorate) {
      await getCities(Number(formData.value.governorate))
    }
  })

  const companyName = computed(() => {
    if (formData.value.companyName) {
      return String(formData.value.companyName)
    }
    if (formData.value.nameEn || formData.value.nameAr) {
      return getLocalizedValue(
        String(formData.value.nameEn || ''),
        String(formData.value.nameAr || '')
      )
    }

    return ''
  })

  interface DocumentItem {
    label: string
    file?: File
    fileUrl?: string | null
    status?: string
    id?: number
    expirationDate?: string
  }

  const documentList = computed<DocumentItem[]>(() => {
    const filesData: Record<string, File | undefined> = {}
    documentsList.value.forEach((doc, index) => {
      filesData[`document_${index}`] = doc.file
    })

    return documentsList.value.map((doc, index) => {
      const fileKey = `document_${index}` as keyof typeof formData.value
      const file = formData.value[fileKey] as File | undefined
      const document = (props.formValues?._documents as DocumentInfo[] | undefined)?.find((item) => item.id === doc.id)

      return {
        label: doc.label,
        file,
        fileUrl: null,
        status: document?.status,
        id: doc.id,
        expirationDate: document?.expirationDate,
      }
    })
  })
</script>

<style scoped>

</style>
