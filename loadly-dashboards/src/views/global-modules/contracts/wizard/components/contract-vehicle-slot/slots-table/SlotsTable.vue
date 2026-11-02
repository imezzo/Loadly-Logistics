<template>
  <section>
    <SlotsTableHeader
      v-model:search="search"
      :can-add="canAdd"
      @add="addRow"
    />

    <BaseTable
      class="max-h-120 min-h-100"
      :columns="tableColumns"
      :data="rows"
    >
      <template #cell-index="{ index }">
        {{ index + 1 }}
      </template>

      <template #cell-vehicleTypeId="{ row }">
        <BaseSelect
          searchable
          :model-value="row.vehicleTypeId"
          :options="vehicleTypeOptions"
          :placeholder="$t('form.select')"
          :disabled="isFormDisabled"
          wrapper-class="min-w-48"
          @update:model-value="setVehicleType(row.index, $event)"
        />
      </template>

      <template #cell-vehicle="{ row }">
        <div
          v-if="row.vehicleId"
          class="flex items-center gap-2"
        >
          <BaseVehiclePlate
            :plate-letters="row.plateLetters"
            :plate-no="row.plateNo"
            class="max-w-fit"
          />

          <button
            v-if="canRemove"
            type="button"
            class="rounded-md border border-primary bg-white p-0.5"
            :aria-label="$t('button.change')"
            :title="$t('button.change')"
            @click="openAssignPopup(row.index)"
          >
            <IconReload class="size-5 text-primary" />
          </button>
        </div>

        <button
          v-else-if="canRemove && row.slotVehicleTypeId"
          type="button"
          class="
            btn-success flex h-8 shrink-0 items-center gap-1 rounded-lg px-2 py-1 text-xs font-bold
          "
          :aria-label="$t('button.assignVehicle')"
          @click="openAssignPopup(row.index)"
        >
          <IconPlus class="size-4" />
          {{ $t('button.assignVehicle') }}
        </button>
        <span v-else>-</span>
      </template>

      <template #cell-action="{ row }">
        <div class="flex items-center gap-2">
          <button
            v-if="canAdd"
            type="button"
            class="btn-action-success"
            :aria-label="$t('button.duplicateSlot')"
            :title="$t('button.duplicateSlot')"
            @click="duplicateRow(row.index)"
          >
            <IconCopy class="size-4" />
          </button>

          <button
            v-if="canRemove"
            type="button"
            class="btn-action-error"
            :aria-label="$t('button.remove')"
            :title="$t('button.remove')"
            @click="emit('remove', row.index)"
          >
            <IconTrash class="size-4" />
          </button>
        </div>
      </template>
    </BaseTable>

    <AssignVehiclePopup
      v-model="isAssignPopupOpen"
      :vehicle-type-id="assigningVehicleTypeId"
      :exclude-ids="excludeVehicleIds"
      @select="selectVehicle"
    />
  </section>
</template>

<script setup lang="ts">
  import type { SelectOption, TableColumn } from '@/types/ui'
  import type { VehicleType } from '@/types/admin-dashboard/response/contract/contract-details'
  import type { ContractVehicleSlot } from '@/types/admin-dashboard/response/contract/contract-slots'
  import type { Vehicle } from '@/types/admin-dashboard/response/vehicle/vehicle'
  import { ContractType } from '@/types/admin-dashboard/response/contract/contract-details'
  import { ContractVehicleSlotStatus } from '@/types/admin-dashboard/response/contract/contract-slots.enums'
  import { ConsumptionType } from '@/types/request/contract/consumption-types.enums'
  import { useContractWizard } from '../../../composables/useContractWizard.ts'
  import { extractISODate } from '@/utils/date'
  import { formatValueWithCurrency } from '@/utils/currency'
  import { computeSlotDailyValue, computeSlotTripsPerDay } from '@/types/request/contract/contract-slots.mappers'

  import SlotsTableHeader from './SlotsTableHeader.vue'

  const AssignVehiclePopup = defineAsyncComponent(() => import('./AssignVehiclePopup.vue'))

  const emit = defineEmits<{
    add: []
    assign: [
      index: number,
      vehicle: Pick<ContractVehicleSlot, 'vehicleId' | 'plateNo' | 'plateLetters'>
    ]
    duplicate: [index: number]
    remove: [index: number]
    setType: [index: number, vehicleTypeId: number]
  }>()

  const { t } = useI18n()
  const { formData, isFormDisabled, isViewMode } = useContractWizard()

  const isAssignPopupOpen = ref(false)
  const assigningIndex = ref<number | null>(null)
  const search = ref('')

  const canRemove = computed(() => !isFormDisabled.value && !isViewMode.value)

  const vehicleTypes = computed(() => (formData.value.vehicleTypes as VehicleType[]) ?? [])
  const vehicleSlots = computed(() => (formData.value.vehicleSlots as ContractVehicleSlot[]) ?? [])

  const tableColumns = computed<TableColumn[]>(() => {
    const contractType = Number(formData.value.contractType)
    const consumptionType = Number(formData.value.consumptionType)
    const isOnDemand = contractType === ContractType.OnDemand
    const isIncluded = contractType === ContractType.Included
    const isExcluded = contractType === ContractType.Excluded
    const isTripBased = contractType === ContractType.Excluded
      || contractType === ContractType.Seasonal
    const isKilometers = isOnDemand && consumptionType === ConsumptionType.Kilometers
    const isCargo = isOnDemand && consumptionType === ConsumptionType.CargoWeight
    const showRentalMetrics = !isOnDemand || isKilometers

    return [
      { key: 'index', label: '#', visible: true },
      { key: 'id', label: t('table.id'), visible: true },
      { key: 'assignedAt', label: t('table.assignedAt'), cellClass: 'min-w-36', visible: true },
      { key: 'vehicleTypeId', label: t('table.vehicleType'), cellClass: 'min-w-48', visible: true },
      { key: 'vehicle', label: t('table.vehicle'), cellClass: 'min-w-48', visible: true },
      { key: 'consumptionRate', label: t('form.consumptionRate'), visible: isTripBased || isKilometers },
      { key: 'dailyValue', label: t('table.dailyValue'), visible: showRentalMetrics },
      {
        key: 'monthlyRental',
        label: isOnDemand
          ? t('form.fixedCarRent')
          : t('form.monthlyFeesPerVehicle'),
        visible: showRentalMetrics
      },
      {
        key: 'extraTrip',
        label: isIncluded || isOnDemand
          ? t('form.kmFees')
          : t('form.extraTrip'),
        visible: showRentalMetrics,
      },
      { key: 'extraWorkingDayFees', label: t('form.extraWorkingDaysFees'), visible: isIncluded || isExcluded },
      { key: 'minBillableWeightTons', label: t('form.minBillableWeightTons'), visible: isCargo },
      { key: 'overWeightPricePerTon', label: t('form.OverWeightPricePerTon'), visible: isCargo },
      { key: 'contractorOverWeightPricePerTon', label: t('form.contractorOverweightPricePerTon'), visible: isCargo },
      { key: 'maxKms', label: t('form.kilometersPerMonth'), visible: isIncluded },
      { key: 'tripsPerDay', label: t('table.tripsPerDay'), visible: isTripBased },
      {
        key: 'tripsPerMonth',
        label: contractType === ContractType.Seasonal
          ? t('form.totalTripsPerContractPeriod')
          : t('form.tripsPerMonthPerVehicle'),
        visible: isTripBased,
      },
      { key: 'maxPayloadTons', label: t('form.maxPayloadTons'), visible: isCargo },
      { key: 'action', label: t('table.action'), visible: canRemove.value },
    ]
      .filter((column) => column.visible)
      .map((column) => ({
        key: column.key,
        label: column.label,
        cellClass: column.cellClass,
      }))
  })

  const vehicleTypeOptions = computed<SelectOption[]>(() =>
    vehicleTypes.value.map((vehicleType) => ({
      label: vehicleType.name,
      value: String(vehicleType.vehicleTypeId ?? vehicleType.id ?? ''),
    }))
  )

  const activeVehicleSlots = computed(() =>
    vehicleSlots.value.filter((slot) => slot.status !== ContractVehicleSlotStatus.Archived)
  )

  const assigningVehicleTypeId = computed(() => {
    if (assigningIndex.value == null) return 0

    return vehicleSlots.value[assigningIndex.value]?.vehicleTypeId ?? 0
  })

  const excludeVehicleIds = computed(() =>
    vehicleSlots.value
      .filter((slot, index) =>
        index !== assigningIndex.value
        && slot.status !== ContractVehicleSlotStatus.Archived
      )
      .map((slot) => slot.vehicleId)
      .filter((id): id is number => Boolean(id))
  )

  const isSlotsAtCapacity = computed(() => {
    const raw = formData.value.vehiclesCount
    if (raw == null || String(raw).trim() === '') return false

    return Number.isFinite(Number(raw)) && activeVehicleSlots.value.length >= Number(raw)
  })

  const canAdd = computed(() => canRemove.value && !isSlotsAtCapacity.value)

  const rows = computed(() => {
    const keyword = search.value.trim().toLowerCase().replace(/\s+/g, '')

    return vehicleSlots.value
      .map((slot, index) => getSlotsFormattedRow(slot, index))
      .filter((row) => {
        const slot = vehicleSlots.value[row.index]
        if (slot?.status === ContractVehicleSlotStatus.Archived) return false

        if (!keyword) return true

        const plateNo = (row.plateNo ?? '').toLowerCase()
        const plateLetters = (row.plateLetters ?? '').toLowerCase()
        const combined = `${plateNo}${plateLetters}`.replace(/\s+/g, '')

        return plateNo.includes(keyword)
          || plateLetters.includes(keyword)
          || combined.includes(keyword)
      })
      .reverse()
  })

  function getSlotsFormattedRow(slot: ContractVehicleSlot, index: number) {
    const vehicleType = vehicleTypes.value.find((item) =>
      (item.vehicleTypeId ?? item.id) === slot.vehicleTypeId
    )

    return {
      assignedAt: extractISODate(slot.assignedAt) ?? '',
      consumptionRate: getValidProperty(vehicleType, 'consumptionRate'),
      dailyValue: vehicleType
        ? formatValueWithCurrency(computeSlotDailyValue({
            contractType: Number(formData.value.contractType),
            monthlyRental: vehicleType.monthlyRental,
            monthlyWorkingDays: Number(formData.value.monthlyWorkingDays),
            tripsPerMonth: vehicleType.tripsPerMonth,
          }))
        : '',
      monthlyRental: getValidProperty(vehicleType, 'monthlyRental'),
      extraTrip: getValidProperty(vehicleType, 'extraTrip'),
      extraWorkingDayFees: getValidProperty(vehicleType, 'ExtraWorkingDayFees'),
      id: slot.id,
      index,
      isNew: !slot.id,
      maxKms: getValidProperty(vehicleType, 'maxKms'),
      maxPayloadTons: getValidProperty(vehicleType, 'maxPayloadTons'),
      minBillableWeightTons: getValidProperty(vehicleType, 'minBillableWeightTons'),
      overWeightPricePerTon: getValidProperty(vehicleType, 'OverWeightPricePerTon'),
      contractorOverWeightPricePerTon: getValidProperty(vehicleType, 'contractorOverWeightPricePerTon'),
      plateLetters: slot.plateLetters,
      plateNo: slot.plateNo,
      slotVehicleTypeId: slot.vehicleTypeId,
      tripsPerDay: String(
        computeSlotTripsPerDay(
          vehicleType?.tripsPerMonth,
          Number(formData.value.monthlyWorkingDays),
        ) ?? ''
      ),
      tripsPerMonth: getValidProperty(vehicleType, 'tripsPerMonth'),
      vehicleId: slot.vehicleId,
      vehicleTypeId: slot.vehicleTypeId ? String(slot.vehicleTypeId) : '',
      vehicleTypeName: vehicleType?.name || '',
    }
  }

  function getValidProperty(vehicleType: VehicleType | undefined, key: keyof VehicleType): string {
    if (!vehicleType) return ''

    const value = vehicleType[key]

    return value == null ? '' : String(value)
  }

  function addRow(): void {
    if (!canAdd.value) return

    emit('add')
  }

  function duplicateRow(index: number): void {
    if (!canAdd.value) return

    emit('duplicate', index)
  }

  function setVehicleType(index: number, value?: string | number | null): void {
    if (value == null || value === '') return

    const vehicleTypeId = Number(value)
    if (!vehicleTypeId) return

    clearVehicle(index)

    emit('setType', index, vehicleTypeId)
  }

  function openAssignPopup(index: number): void {
    assigningIndex.value = index
    isAssignPopupOpen.value = true
  }

  function selectVehicle(vehicle: Vehicle): void {
    if (assigningIndex.value == null) return

    emit('assign', assigningIndex.value, {
      vehicleId: vehicle.id,
      plateNo: vehicle.plateNo,
      plateLetters: vehicle.plateLetters,
    })
    assigningIndex.value = null
  }

  function clearVehicle(index: number): void {
    const slot = (formData.value.vehicleSlots as ContractVehicleSlot[])?.[index]
    if (slot) {
      slot.vehicleId = undefined
      slot.plateNo = undefined
      slot.plateLetters = undefined
    }
  }

  watch(isAssignPopupOpen, (isOpen) => {
    if (isOpen) return

    assigningIndex.value = null
  })
</script>

<style scoped>

</style>
