<template>
  <CardPreview :title="$t('content.companyInformation')">
    <template #content>
      <CardPreviewLabel
        :label="$t('form.companyName')"
        :content="String(companyName || $t('content.notAvailable'))"
      />

      <CardPreviewLabel
        :label="$t('form.contactPersonName')"
        :content="String(companyContactName || $t('content.notAvailable'))"
      />

      <CardPreviewLabel
        :label="$t('content.emailAddress')"
        :content="String(companyEmail || $t('content.notAvailable'))"
      />

      <CardPreviewLabel
        :label="$t('content.phoneNumber')"
        :content="String(companyPhone || $t('content.notAvailable'))"
      />

      <CardPreviewLabel
        :label="$t('content.commercialRegisterNumber')"
        :content="String(companyCommercialRegister || $t('content.notAvailable'))"
      />
    </template>
  </CardPreview>
</template>

<script setup lang="ts">
  import { getLocalizedValue } from '@/utils/i18n'
  import type { ApiCompany } from '@/types/admin-dashboard/response/company-detail'
  import { useCompanyStore } from '@/stores/admin-dashboard/company'
  import { useContractWizard } from '../../composables/useContractWizard'

  const {
    formData,
    contract,
    isViewMode,
    isUpdateMode
  } = useContractWizard()
  const companyStore = useCompanyStore()

  const companyDetails = ref<ApiCompany | null>(null)

  const hasContractCompany = computed(() => {
    if (isUpdateMode.value && contract.value?.company) {
      return formData.value?.companyId != null
        && String(formData.value?.companyId) === String(contract.value.companyId)
    }

    return isViewMode.value
  })

  const companyName = computed(() => {
    if (hasContractCompany.value && contract.value?.company) {
      return getLocalizedValue(contract.value.company.nameEn, contract.value.company.nameAr)
    }

    const detail = companyDetails.value?.companyDetail

    return detail?.nameEn || detail?.nameAr || formData.value?.companyName || ''
  })

  const companyContactName = computed(() => {
    return hasContractCompany.value
      ? contract.value?.company?.fullName
      : (companyDetails.value?.contacts?.find(contact => contact.primary)?.name || companyDetails.value?.fullName)
  })

  const companyEmail = computed(() => {
    return hasContractCompany.value
      ? contract.value?.company?.email
      : (formData.value?.email || companyDetails.value?.email)
  })

  const companyPhone = computed(() => {
    return hasContractCompany.value
      ? contract.value?.company?.phone
      : (formData.value?.phoneNumber || companyDetails.value?.phone)
  })

  const companyCommercialRegister = computed(() => {
    return hasContractCompany.value
      ? contract.value?.company?.registrationNumber
      : (formData.value?.commercialRegisterNumber || companyDetails.value?.companyDetail?.registrationNumber)
  })

  async function fetchCompany(id: string | number): Promise<void> {
    const { data } = await companyStore.get(id)
    companyDetails.value = data
  }

  watch(
    () => formData.value?.companyId,
    async (companyId) => {
      if (hasContractCompany.value) return
      companyDetails.value = null

      if (!Number.isNaN(Number(companyId))) {
        await fetchCompany(Number(companyId))
      }
    },
    { immediate: true }
  )
</script>

<style scoped>

</style>

