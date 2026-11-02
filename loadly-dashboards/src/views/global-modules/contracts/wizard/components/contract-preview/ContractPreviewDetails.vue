<template>
  <CardPreview :title="$t('content.contractDetails')">
    <template #content>
      <CardPreviewLabel
        :label="$t('content.contractReferenceNumber')"
        :content="String(contractReferenceNumber || $t('content.notAvailable'))"
      />

      <CardPreviewLabel
        :label="$t('form.contractType')"
        :content="String(contractTypeName || $t('content.notAvailable'))"
      />

      <CardPreviewLabel
        :label="`${$t('form.startDate')} - ${$t('form.endDate')}`"
        :content="formData.startDate + ' - ' + formData.endDate"
      />

      <CardPreviewLabel
        :label="$t('form.salesRepresentative')"
        :content="String(salesRepName || $t('content.notAvailable'))"
      />
    </template>
  </CardPreview>
</template>

<script setup lang="ts">
  import { useUserStore } from '@/stores/admin-dashboard/user'
  import { useContractWizard } from '../../composables/useContractWizard'
  import type { SalesRep } from '@/types/admin-dashboard/response/contract/contract-details'
  import { useEnum } from '@/composables/useEnum'

  const { formData } = useContractWizard()
  const userStore = useUserStore()
  const { enums } = useEnum()
  const { usersListAsLookup } = storeToRefs(userStore)

  const contractReferenceNumber = computed(
    () =>
      (formData.value?.referenceNumber as string) ||
      (formData.value?.contractReferenceNumber as string) ||
      ''
  )

  const contractTypeName = computed(() => {
    const id = formData.value?.contractType
    if (id == null || id === '') return ''

    const types = enums.value?.ContractType ?? []
    const type = types.find((type) => type.value === String(id))

    return type?.label ?? ''
  })

  const salesRepName = computed(() => {
    const id = formData.value?.salesRepId
    const salesRep = formData.value?.salesRep as SalesRep
    if (id == null || id === '') return ''

    const items = usersListAsLookup.value?.items ?? []
    const rep = items.find((item) => item.value === String(id))

    return rep?.label ?? salesRep?.fullName ?? ''
  })
</script>

<style scoped>

</style>

