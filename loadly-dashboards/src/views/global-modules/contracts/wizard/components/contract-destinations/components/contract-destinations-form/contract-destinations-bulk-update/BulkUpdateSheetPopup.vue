<template>
  <BasePopup
    v-model="model"
    :show-close-button="true"
    content-class="max-w-1/2!"
  >
    <div class="w-full px-6 py-4">
      <h2 class="mb-4 text-2xl font-bold text-neutral-800">
        {{ $t('title.bulkUpdateDestinations') }}
      </h2>

      <BulkUpdateInstructions :required-headers="requiredHeaders" />

      <BaseAlert
        v-if="errorMessage"
        :model-value="!!errorMessage"
        variant="error"
        class="mb-4 max-h-1/2! w-full"
      >
        <!-- eslint-disable-next-line vue/no-v-html -->
        <span v-html="errorMessage" />
      </BaseAlert>

      <div class="flex gap-3">
        <button
          type="button"
          class="btn-primary mb-4 w-1/4"
          @click="fileInputRef?.click()"
        >
          {{ $t('button.import') }}
          <IconPlus />
        </button>

        <input
          ref="fileInputRef"
          type="file"
          accept=".xlsx,.xls"
          class="hidden"
          @change="handleFile"
        >
      </div>

      <BaseTable
        v-if="selectedFile?.length"
        :columns="columns"
        :data="selectedFile"
        class="max-h-[150px] overflow-y-auto"
      >
        <template #header-checkbox>
          <BaseCheckbox
            v-if="selectedFile"
            :model-value="isAllSelected"
            :indeterminate="isIndeterminate"
            @update:model-value="selectAll($event as boolean)"
          />
        </template>

        <template #cell-checkbox="{ row }">
          <BaseCheckbox v-model="(row.checked as boolean)" />
        </template>
      </BaseTable>

      <button
        v-if="hasSelectedRows && !errorMessage"
        type="button"
        class="btn-primary mt-6 w-full"
        @click="applyBulkUpdate"
      >
        {{ $t('button.updateDestinations') }}
      </button>
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import { read, utils } from 'xlsx'
  import { useContractDestinationsForm } from '@/views/global-modules/contracts/wizard/composables/contract-destinations/useContractDestinationsForm'
  import BulkUpdateInstructions from './BulkUpdateInstructions.vue'
  import {
    mergeSheetRowIntoDestination,
    useBulkUpdateDestinations,
  } from '@/views/global-modules/contracts/wizard/composables/contract-destinations/useBulkUpdateDestinations'
  import { normalizeSheetRowKeys } from '@/views/global-modules/contracts/wizard/composables/contract-destinations/useImportDestinations'
  import type { TableColumn } from '@/types/ui'
  import type { ContractWizardDestination } from '@/types/request/contract/contract-destinations.types'
  import { useContractWizard } from '@/views/global-modules/contracts/wizard/composables/useContractWizard'
  import { useContractStore } from '@/stores/admin-dashboard/contract'
  import { EXPECTED_DESTINATION_SHEET_HEADERS } from '@/views/global-modules/contracts/wizard/composables/contract-destinations/useBulkDestinations'

  const props = withDefaults(
    defineProps<{
      existingDestinations?: ContractWizardDestination[]
    }>(),
    { existingDestinations: () => [] },
  )

  const model = defineModel<boolean>('modelValue', { required: true })

  const selectedFile = ref<Record<string, string | boolean>[] | null>([])
  const fileInputRef = useTemplateRef<HTMLInputElement>('fileInputRef')

  const { t } = useI18n()
  const {
    showPricePerTonField,
    showFixedTripPriceField,
    requireNumberOfDaysAndKms,
  } = useContractDestinationsForm()
  const { formData } = useContractWizard()
  const contractStore = useContractStore()

  const requiredHeaders = computed((): string[] => {
    const headers = [...EXPECTED_DESTINATION_SHEET_HEADERS.slice(0, 4)]
    if (requireNumberOfDaysAndKms.value) {
      headers.push('number-of-days', 'kms')
    }
    if (showPricePerTonField.value) {
      return [...headers, 'id', 'price-per-ton', 'contractor-price-per-ton']
    }
    if (showFixedTripPriceField.value) {
      return [...headers, 'id', 'fixed-destination-price', 'contractor-fixed-destination-price']
    }

    return [...headers, 'id']
  })

  const columns = ref<TableColumn[]>([
    { key: 'checkbox', class: 'w-12 px-6 py-4' },
    { key: 'id', label: t('table.id') },
    { key: 'name-en', label: t('table.nameEn') },
    { key: 'name-ar', label: t('table.nameAr') },
    { key: 'address-from', label: t('table.addressFrom') },
    { key: 'address-to', label: t('table.addressTo') },
    { key: 'number-of-days', label: t('table.numberOfDays') },
    { key: 'kms', label: t('table.kms') },
    { key: 'return-expected-kms', label: t('table.returnExpectedKms') },
    { key: 'fixed-destination-price', label: t('table.fixedDestinationPrice') },
    { key: 'contractor-fixed-destination-price', label: t('table.contractorFixedTripPrice') },
    { key: 'price-per-ton', label: t('table.pricePerTon') },
    { key: 'contractor-price-per-ton', label: t('table.contractorPricePerTon') },
  ])

  const {
    validateBulkUpdateSheet,
    errorMessage,
  } = useBulkUpdateDestinations(requiredHeaders, toRef(props, 'existingDestinations'))

  const isAllSelected = computed(() => {
    if (!selectedFile.value) return false

    return selectedFile.value.length === selectedFile.value.filter(row => row.checked).length
  })

  const isIndeterminate = computed(() => {
    if (!selectedFile.value) return false

    return selectedFile.value.length > 0
      && selectedFile.value.filter(row => row.checked).length > 0
      && selectedFile.value.filter(row => row.checked).length < selectedFile.value.length
  })

  const hasSelectedRows = computed(() => {
    if (!selectedFile.value) return false

    return selectedFile.value.some(row => row.checked)
  })

  function applyBulkUpdate(): void {
    const destinations = formData.value?.destinations as ContractWizardDestination[] || []
    const rowsToApply = selectedFile.value?.filter(row => row.checked as boolean) ?? []
    const updateById = new Map(
      rowsToApply.map(row => [Number(row.id), row]),
    )

    const updatedDestinations = destinations.map((dest) => {
      const sheetRow = dest.id != null ? updateById.get(dest.id) : undefined
      if (!sheetRow) return dest

      return mergeSheetRowIntoDestination(dest, sheetRow)
    })

    contractStore.wizardFormData = { ...formData.value, destinations: updatedDestinations }
    model.value = false
  }

  function selectAll(value: boolean): void {
    if (selectedFile.value) {
      selectedFile.value.forEach((row) => {
        row.checked = value
      })
    }
  }

  function handleFile(e: Event) {
    const target = e.target as HTMLInputElement
    const file = target.files?.[0]
    if (!file) return

    const reader = new FileReader()

    reader.onload = (event) => {
      errorMessage.value = ''

      const data = new Uint8Array(event.target?.result as ArrayBuffer)
      const workbook = read(data, { type: 'array' })

      const sheetName = workbook.SheetNames[0]
      const sheet = workbook.Sheets[sheetName ?? '']

      const jsonData = utils.sheet_to_json(sheet ?? {})
      ;(e.target as HTMLInputElement).value = ''

      const rows = utils.sheet_to_json((sheet ?? {}), { header: 1 })
      const headers = rows[0] as string[]
      const normalizedRows = (jsonData as Record<string, string>[]).map(normalizeSheetRowKeys)

      if (!validateBulkUpdateSheet(normalizedRows, headers)) return

      selectedFile.value = normalizedRows
      selectAll(true)
    }

    reader.readAsArrayBuffer(file)
  }
</script>

<style scoped>

</style>
