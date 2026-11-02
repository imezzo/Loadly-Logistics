<template>
  <BasePopup
    v-model="model"
    content-class="max-w-1/2! gap"
  >
    <div class="w-full space-y-4 p-6">
      <h2 class="text-2xl font-bold text-neutral-800">
        {{ $t('title.importTrips') }}
      </h2>

      <ImportInstructions :required-headers="requiredHeaders" />

      <BaseAlert
        v-if="errorMessage"
        :model-value="!!errorMessage"
        :message="errorMessage"
        variant="error"
        class="max-h-40 w-full overflow-y-auto"
      />

      <ImportForm
        v-model:contract-id="contractId"
        v-model:vehicle-type-id="vehicleTypeId"
        :selected-contract-id
        @change-contract="onContractChange"
      />

      <BaseImportActions
        template-url="/sheets-templates/bulk-trips-template.xlsx"
        @file-change="handleFile"
      />

      <BaseTable
        v-if="selectedFile?.length"
        :columns="columns"
        :data="selectedFile"
        class="max-h-50 overflow-y-auto"
      >
        <template #cell-start-date="{ row }">
          {{ new Date(row['start-date'] as string).toLocaleDateString() }}
          {{ new Date(row['start-date'] as string).toLocaleTimeString() }}
        </template>

        <template #cell-end-date="{ row }">
          {{ new Date(row['end-date'] as string).toLocaleDateString() }}
          {{ new Date(row['end-date'] as string).toLocaleTimeString() }}
        </template>
      </BaseTable>

      <BaseActions
        v-if="hasSelectedRows && !errorMessage"
        container-class="justify-end gap-3"
        :prev-button-options
        :next-button-options
        @prev="onCancel"
        @next="submitBulkImport"
      />
    </div>
  </BasePopup>

  <PopupSuccess
    v-model="isSuccessPopupOpen"
    :message="successMessage"
  >
    <template #actions>
      <button
        type="button"
        class="btn-primary mt-4 px-6"
        @click="isSuccessPopupOpen = false"
      >
        {{ $t('button.close') }}
      </button>
    </template>
  </PopupSuccess>

  <PopupError
    v-model="isErrorPopupOpen"
    :message="popupErrorMessage"
    :errors="popupErrors"
  />
</template>

<script setup lang="ts">
  import { read, utils } from 'xlsx'
  import ImportInstructions from './components/ImportInstructions.vue'
  import ImportForm from './components/ImportForm.vue'
  import {
    mapRowsToTrips,
    normalizeSheetRowKeys,
    useImportTrips,
    type BulkImportTripRow,
  } from './composables/useImportTrips'
  import {
    EXPECTED_TRIP_SHEET_HEADERS,
    useBulkTrips,
  } from './composables/useBulkTrips'
  import { useLookups } from '@/composables/useLookups'
  import { useLoading } from '@/composables/useLoading'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import type { TableColumn } from '@/types/ui'

  const model = defineModel<boolean>('modelValue', { required: true })

  const selectedFile = ref<BulkImportTripRow[] | null>([])

  const { t } = useI18n()
  const tripStore = useTripStore()
  const { tripsList, filters } = storeToRefs(tripStore)
  const { isLoading, setLoading } = useLoading()
  const { getContractReferenceNumbers, getContractVehicleTypes } = useLookups()
  const { validateHeaders } = useBulkTrips()

  const contractId = ref('')
  const selectedContractId = ref<string | number>('')
  const vehicleTypeId = ref('')
  const successMessage = ref('')
  const popupErrorMessage = ref('')
  const popupErrors = ref<Record<string, string[]>>()
  const isSuccessPopupOpen = ref(false)
  const isErrorPopupOpen = ref(false)

  const requiredHeaders = computed((): string[] => [...EXPECTED_TRIP_SHEET_HEADERS.slice(0, 7)])

  const {
    validateRows,
    errorMessage,
  } = useImportTrips(requiredHeaders)

  const columns = ref<TableColumn[]>([
    { key: 'start-date', label: t('form.startDate') },
    { key: 'end-date', label: t('form.endDate') },
    { key: 'from', label: t('table.addressFrom') },
    { key: 'to', label: t('table.addressTo') },
    { key: 'distance-kms', label: t('table.distanceKms') },
    { key: 'contact-name', label: t('form.contactName') },
    { key: 'contact-mobile', label: t('table.contactMobile') },
    { key: 'helpers-count', label: t('table.helpersCount') },
  ])

  const hasSelectedRows = computed(() => {
    if (!selectedFile.value) return false

    return selectedFile.value.length > 0
  })

  const canSubmit = computed(() => Boolean(
    contractId.value
    && vehicleTypeId.value
    && hasSelectedRows.value
    && !errorMessage.value,
  ))

  const prevButtonOptions = computed(() => ({
    class: 'btn-secondary',
    label: t('button.cancel'),
  }))

  const nextButtonOptions = computed(() => ({
    class: 'btn-primary',
    disabled: !canSubmit.value || isLoading.value,
    label: isLoading.value ? t('content.uploading') : t('button.importTrips'),
  }))

  function reset(): void {
    contractId.value = ''
    selectedContractId.value = ''
    vehicleTypeId.value = ''
    selectedFile.value = []
    errorMessage.value = ''
    popupErrors.value = {}
  }

  function onCancel(): void {
    reset()
    model.value = false
  }

  async function onContractChange(value: string | number): Promise<void> {
    contractId.value = String(value)
    vehicleTypeId.value = ''
    selectedContractId.value = value

    if (!value) return

    await getContractVehicleTypes(value)
  }

  async function submitBulkImport(): Promise<void> {
    if (!canSubmit.value || !selectedFile.value) return

    setLoading(true)

    const { success, message, errors } = await tripStore.bulkCreateTrips({
      contractId: Number(contractId.value),
      vehicleTypeId: Number(vehicleTypeId.value),
      trips: selectedFile.value.map(mapRowsToTrips),
    })

    setLoading(false)

    if (success) {
      model.value = false
      reset()
      successMessage.value = t('success.importTripsSuccess')
      isSuccessPopupOpen.value = true

      await tripStore.fetchTrips({
        page: tripsList.value?.pagination.page ?? 1,
        size: tripsList.value?.pagination.size ?? 10,
        ...filters.value,
      })

      return
    }

    popupErrors.value = errors || {}
    popupErrorMessage.value = message || t('error.networkError')
    isErrorPopupOpen.value = true
  }

  function handleFile(e: Event) {
    const target = e.target as HTMLInputElement
    const file = target.files?.[0]
    if (!file) return

    const reader = new FileReader()

    reader.onload = (event) => {
      errorMessage.value = ''

      const data = new Uint8Array(event.target?.result as ArrayBuffer)
      const workbook = read(data, { type: 'array', cellDates: true })

      const sheetName = workbook.SheetNames[0]
      const sheet = workbook.Sheets[sheetName ?? '']

      const jsonData = utils.sheet_to_json(sheet ?? {}, { defval: '' });
      (e.target as HTMLInputElement).value = ''

      const rows = utils.sheet_to_json((sheet ?? {}), { header: 1 })

      const headers = rows[0] as string[]

      validateHeaders(headers)
      if (errorMessage.value) return
      const normalizedRows = (jsonData as Record<string, unknown>[]).map(normalizeSheetRowKeys)
      validateRows(normalizedRows)
      if (errorMessage.value) return

      selectedFile.value = normalizedRows
    }

    reader.readAsArrayBuffer(file)
  }

  watch(model, async (isOpen) => {
    if (isOpen) {
      await getContractReferenceNumbers()

      return
    }

    reset()
  })
</script>

<style scoped>

</style>
