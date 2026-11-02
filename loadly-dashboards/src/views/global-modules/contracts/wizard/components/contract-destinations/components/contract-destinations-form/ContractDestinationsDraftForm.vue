<template>
  <div class="flex flex-col rounded-lg border border-neutral-200 p-4">
    <h1 class="mb-4 font-bold text-neutral-10">
      {{ $t('content.contractDestinations') }}
    </h1>

    <BaseForm
      ref="formRef"
      :initial-values
      :validation-schema
      :groups
      :errors
      @submit="onSubmit"
    >
      <template #submit>
        <BaseActions
          container-class="flex-wrap justify-end gap-3 my-8"
          :prev-button-options="{ label: $t('button.discard'), disabled: isFormDisabled }"
          :next-button-options="{
            label: isCreateForm ? $t('button.add') : $t('button.edit'),
            asSubmit: true,
            hasIcon: true,
            disabled: isFormDisabled,
          }"
          @prev="onDiscardClick"
        />
      </template>
    </BaseForm>
  </div>
</template>

<script setup lang="ts">
  import { useMapsStore } from '@/stores/admin-dashboard/maps'
  import type { ContractWizardDestination } from '@/types/request/contract/contract-destinations.types'
  import { useDebounceFn } from '@vueuse/core'
  import {
    emptyContractDestinationDraft,
    useContractDestinationsForm,
    wizardRowToDraftValues,
  } from '@/views/global-modules/contracts/wizard/composables/contract-destinations/useContractDestinationsForm'
  import { useContractWizard } from '@/views/global-modules/contracts/wizard/composables/useContractWizard'

  defineProps<{
    errors?: Record<string, string[]>
  }>()

  const emit = defineEmits<{
    'submit-draft': [values: Record<string, unknown>]
    discard: []
  }>()

  const mapsStore = useMapsStore()

  const formRef = useTemplateRef<{
    form?: {
      getValues: () => Record<string, unknown>
      setValues: (v: Record<string, unknown>) => void
      resetForm: () => void
    }
    submitForm?: () => Promise<void>
  }>('formRef')

  const isCreateForm = ref(true)

  function applyStartFromExtract(url: string): void {
    void (async () => {
      const res = await mapsStore.extractPlace(url)
      const form = formRef.value?.form
      if (!res.success || !res.data || !form) return
      const current = form.getValues()
      form.setValues({
        ...current,
        startLatitude: String(res.data.lat),
        startLongitude: String(res.data.lng),
        startAddressName: res.data.address,
        startGooglePlaceId: res.data.placeId,
      })
    })()
  }

  function applyEndFromExtract(url: string): void {
    void (async () => {
      const res = await mapsStore.extractPlace(url)
      const form = formRef.value?.form
      if (!res.success || !res.data || !form) return
      const current = form.getValues()
      form.setValues({
        ...current,
        endLatitude: String(res.data.lat),
        endLongitude: String(res.data.lng),
        endAddressName: res.data.address,
        endGooglePlaceId: res.data.placeId,
      })
    })()
  }

  const debouncedStartExtract = useDebounceFn(applyStartFromExtract, 450)
  const debouncedEndExtract = useDebounceFn(applyEndFromExtract, 450)

  const { isFormDisabled } = useContractWizard()

  const { groups, validationSchema } = useContractDestinationsForm(
    (url) => debouncedStartExtract(url),
    (url) => debouncedEndExtract(url),
  )

  const initialValues = computed(() => ({
    ...emptyContractDestinationDraft(),
  }))

  function onSubmit(values: Record<string, unknown>): void {
    emit('submit-draft', values)
  }

  function onDiscardClick(): void {
    resetDraft()
    emit('discard')
  }

  function resetDraft(): void {
    formRef.value?.form?.resetForm?.()
    isCreateForm.value = true
  }

  function loadDraft(row: ContractWizardDestination): void {
    formRef.value?.form?.setValues(wizardRowToDraftValues(row))
    isCreateForm.value = false
  }

  defineExpose({
    loadDraft,
    resetDraft,
  })
</script>

<style scoped>

</style>
