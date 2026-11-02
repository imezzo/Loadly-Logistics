<template>
  <div class="flex flex-col">
    <ContractDestinationsDraftForm
      ref="destinationDraftFormRef"
      :errors="props.errors"
      @submit-draft="onDraftSubmit"
      @discard="onDraftDiscard"
    />

    <ContractDestinationsList
      :rows="destinationRows"
      @remove="removeRow"
      @edit="editRow"
    />

    <BaseActions
      class="mt-4"
      :prev-button-options="{ hasScrollTop: true }"
      :next-button-options="{ label: saveButtonTitle, hasScrollTop: true }"
      @prev="emit('prev')"
      @next="persistAndAdvance"
    />
  </div>
</template>

<script setup lang="ts">
  import type { ContractWizardDestination } from '@/types/request/contract/contract-destinations.types'
  import {
    apiDestinationPointToState,
    type ApiContractDestinationPoint,
  } from '@/types/admin-dashboard/response/contract/contract-details'
  import { draftValuesToWizardRow } from '../../composables/contract-destinations/useContractDestinationsForm'
  import { useContractWizard } from '../../composables/useContractWizard'
  import ContractDestinationsDraftForm from './components/contract-destinations-form/ContractDestinationsDraftForm.vue'
  import ContractDestinationsList from './components/contract-destinations-form/ContractDestinationsList.vue'

  const props = defineProps<{
    errors?: Record<string, string[]>
  }>()

  const emit = defineEmits<{
    update: [data: Record<string, unknown>]
    next: []
    prev: []
    discard: []
  }>()

  const { formData, saveButtonTitle } = useContractWizard()

  const destinationDraftFormRef = useTemplateRef<InstanceType<typeof ContractDestinationsDraftForm>>('destinationDraftFormRef')

  const destinationRows = ref<ContractWizardDestination[]>([])
  const editingClientId = ref<string | null>(null)

  function normalizeWizardDestinations(raw: unknown): ContractWizardDestination[] {
    if (!Array.isArray(raw)) return []

    return raw.map((item) => {
      const row = item as ContractWizardDestination & { clientId?: string }

      return {
        clientId: row.clientId,
        id: row.id,
        description: row.description,
        start: apiDestinationPointToState(row.start as unknown as ApiContractDestinationPoint),
        end: apiDestinationPointToState(row.end as unknown as ApiContractDestinationPoint),
        nameAr: row.nameAr ?? '',
        nameEn: row.nameEn ?? '',
        createdAt: row.createdAt,
        updatedAt: row.updatedAt,
        expectedKms: row.expectedKms,
        ...(row.returnExpectedKms != null && Number.isFinite(Number(row.returnExpectedKms))
          ? { returnExpectedKms: Number(row.returnExpectedKms) }
          : {}),
        numberOfDays: typeof row.numberOfDays === 'number' && Number.isFinite(row.numberOfDays)
          ? row.numberOfDays
          : Math.max(0, Math.floor(Number(row.numberOfDays) || 0)),
        ...(row.pricePerTon != null && Number.isFinite(Number(row.pricePerTon))
          ? { pricePerTon: Number(row.pricePerTon) }
          : {}),
        ...(row.fixedTripPrice != null && Number.isFinite(Number(row.fixedTripPrice))
          ? { fixedTripPrice: Number(row.fixedTripPrice) }
          : {}),
        ...(row.contractorPricePerTon != null && Number.isFinite(Number(row.contractorPricePerTon))
          ? { contractorPricePerTon: Number(row.contractorPricePerTon) }
          : {}),
        ...(row.contractorFixedTripPrice != null && Number.isFinite(Number(row.contractorFixedTripPrice))
          ? { contractorFixedTripPrice: Number(row.contractorFixedTripPrice) }
          : {}),
      }
    })
  }

  function removeRow(rowToRemove: ContractWizardDestination): void {
    destinationRows.value = destinationRows.value.filter((row) => {
      if (rowToRemove.id != null && row.id != null) return row.id !== rowToRemove.id
      if (rowToRemove.clientId && row.clientId) return row.clientId !== rowToRemove.clientId

      return row !== rowToRemove
    })
    if (editingClientId.value && rowToRemove.clientId && editingClientId.value === rowToRemove.clientId) {
      editingClientId.value = null
      destinationDraftFormRef.value?.resetDraft()
    }
    emit('update', { destinations: [...destinationRows.value] })
  }

  function editRow(row: ContractWizardDestination): void {
    editingClientId.value = row.clientId
    nextTick(() => {
      destinationDraftFormRef.value?.loadDraft(row)
    })
  }

  function onDraftDiscard(): void {
    editingClientId.value = null
  }

  function onDraftSubmit(values: Record<string, unknown>): void {
    const prevRow = editingClientId.value
      ? destinationRows.value.find(r => r.clientId === editingClientId.value)
      : undefined
    const row = draftValuesToWizardRow(values, (editingClientId.value ?? destinationRows.value.length).toString())

    if (prevRow) {
      if (prevRow.id != null) row.id = prevRow.id
      row.description = prevRow.description
    }

    if (editingClientId.value) {
      const idx = destinationRows.value.findIndex(r => r.clientId === editingClientId.value)
      if (idx >= 0) destinationRows.value[idx] = row
    } else {
      destinationRows.value = [...destinationRows.value, row]
    }

    editingClientId.value = null
    destinationDraftFormRef.value?.resetDraft()
    emit('update', { destinations: [...destinationRows.value] })
  }

  function persistAndAdvance(): void {
    emit('update', { destinations: [...destinationRows.value] })
    emit('next')
  }

  function submitStep(): void {
    persistAndAdvance()
  }

  defineExpose({ submitStep })

  watch(
    () => formData.value?.destinations,
    (next) => {
      destinationRows.value = normalizeWizardDestinations(next ?? [])
    },
    { deep: true, immediate: true },
  )
</script>

<style scoped>

</style>
