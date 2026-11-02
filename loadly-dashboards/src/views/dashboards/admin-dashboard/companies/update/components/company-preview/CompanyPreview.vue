<template>
  <div class="space-y-6">
    <CompanyPreviewHeader />

    <CompanyDetailsCard
      :form-data="formData"
      :original-form-data="originalFormData"
      :company-name="companyName"
      :cities="cities"
      :governments="governments"
    />

    <MultipleLocationsCard
      :multiple-locations="multipleLocations"
      :cities="cities"
      :governments="governments"
    />

    <SecondaryContactsCard :secondary-contacts="secondaryContacts" />

    <BusinessInformationCard
      :form-data="formData"
      :original-form-data="originalFormData"
      :industry-types="industryTypes"
      :service-requirements="serviceRequirements"
      :communication-methods="communicationMethods"
      :operating-hours="operatingHours"
    />

    <DocumentsCard :document-list="documentList" />

    <BaseActions
      :prev-button-options="{ hasScrollTop: true }"
      :next-button-options="{ label: $t('button.saveChanges') }"
      @prev="$emit('prev')"
      @next="$emit('submit')"
    />
  </div>
</template>

<script setup lang="ts">
  import type { CompanyAddressFormData } from '@/types/admin-dashboard/response/company-detail'
  import type { SecondaryContact } from '@/types/secondary-contact.types.ts'
  import CompanyPreviewHeader from './CompanyPreviewHeader.vue'
  import CompanyDetailsCard from './CompanyDetailsCard.vue'
  import MultipleLocationsCard from './MultipleLocationsCard.vue'
  import SecondaryContactsCard from './SecondaryContactsCard.vue'
  import BusinessInformationCard from './BusinessInformationCard.vue'
  import DocumentsCard from './DocumentsCard.vue'
  import { useLookups } from '@/composables/useLookups'
  import { useDocuments } from '../../composables/useDocuments'
  import { getLocalizedValue } from '@/utils/i18n'
  import type { DocumentInfo } from '@/types/admin-dashboard/response/document'

  const props = defineProps<{
    formValues?: Record<string, unknown>
    originalFormData?: Record<string, unknown>
  }>()

  defineEmits<{
    update: [data: Record<string, unknown>]
    prev: []
    next: []
    submit: []
  }>()

  const {
    industryTypes,
    serviceRequirements,
    communicationMethods,
    operatingHours,
    governments,
    cities,
    getCities,
  } = useLookups()

  const formData = computed(() => props.formValues || {})
  const originalFormData = computed(() => props.originalFormData || {})

  const { documents: documentsList, fetchDocuments } = useDocuments(() => {
    // TODO: Implement document fetching
  })

  onMounted(async () => {
    await fetchDocuments()
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

  const multipleLocations = computed(() => {
    const addresses = formData.value.addresses as CompanyAddressFormData[] | undefined
    if (!Array.isArray(addresses) || addresses.length <= 1) {
      return []
    }

    return addresses.slice(1)
  })

  const secondaryContacts = computed(() => {
    return (formData.value.secondaryContacts as SecondaryContact[] | undefined) || []
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
    const companyDocs = formData.value.documents as DocumentInfo[] | undefined
    if (!Array.isArray(companyDocs) || companyDocs.length === 0) {
      return []
    }

    const companyDocsMap = new Map<number, DocumentInfo>()
    companyDocs.forEach((doc: DocumentInfo) => {
      companyDocsMap.set(doc.documentId || 0, doc)
    })

    const mergedDocs: DocumentItem[] = []

    documentsList.value.forEach((docTemplate, index) => {
      const companyDoc = companyDocsMap.get(docTemplate.id || 0)
      if (companyDoc) {
        const fileKey = `document_${index}` as keyof typeof formData.value
        const file = formData.value[fileKey] as File | undefined

        mergedDocs.push({
          label: docTemplate.label,
          file,
          fileUrl: companyDoc.fileUrl || null,
          status: companyDoc.status,
          id: companyDoc.id,
          expirationDate: companyDoc.expirationDate,
        })
      }
    })

    return mergedDocs
  })
</script>

<style scoped>

</style>
