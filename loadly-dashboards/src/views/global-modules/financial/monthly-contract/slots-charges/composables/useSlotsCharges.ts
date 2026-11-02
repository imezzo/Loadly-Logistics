import { useLoading } from '@/composables/useLoading'
import { useMonthlyContractStore } from '@/stores/admin-dashboard/monthly-contract'
import type { MonthlyContractVehicle } from '@/types/admin-dashboard/response/contract/monthly-contract/monthly-contract'
import type { SlotCalendar, SlotCalendarDay } from '@/types/admin-dashboard/response/contract/included-slot-charges-calculate'
import type { ContractSlotsTableRow } from '@/views/global-modules/slot-charges/composables/useSlotsCharges'
import type { SelectOption, TableColumn } from '@/types/ui'
import { ContractSlotDayDisposition } from '@/types/admin-dashboard/response/contract/monthly-contract/monthly-contract.enums'

export function useMonthlyContractSlotsCharges() {
  const route = useRoute()
  const { t } = useI18n()
  const { isLoading, setLoading } = useLoading()

  const monthlyContractStore = useMonthlyContractStore()
  const { monthlyContract } = storeToRefs(monthlyContractStore)

  const errorMessage = ref('')
  const isEditPopupOpen = ref(false)
  const selectedDay = ref<SlotCalendarDay | null>(null)
  const selectedVehicleLabel = ref('')
  const vehicleId = ref('')
  const snapshotId = computed(() => String(route.params.id ?? ''))
  const slotCharges = computed(() => monthlyContract.value?.calculation)
  const vehicles = computed(() => monthlyContract.value?.calculation?.vehicles ?? {})

  const seatOptions = computed<SelectOption[]>(() => {
    const calendar = monthlyContract.value?.calculation.calendar ?? []
    const options = new Map<string, string>()

    calendar.forEach((slot, index) => {
      const id = String(slot.billingSnapshotSeatRootSlotId)
      if (options.has(id) || !isActiveSlot(slot)) return

      options.set(id, getVehicleLabel(slot.slotId, index))
    })

    return [...options.entries()].map(([value, label]) => ({ label, value }))
  })

  function isActiveSlot(slot: SlotCalendar): boolean {
    return slot.days.some(day => day.disposition !== ContractSlotDayDisposition.NotActive)
  }

  function formatDayLabel(dateStr: string): string {
    const date = new Date(`${dateStr}T00:00:00`)
    const month = date.toLocaleDateString('en-US', { month: 'short' })
    const day = date.getDate()

    return `${month} ${day}`
  }

  function getVehicleLabel(slotId: number, index: number): string {
    const vehicle = monthlyContract.value?.calculation.vehicles[String(slotId)]

    if (vehicle?.plateNo) {
      return vehicle.plateLetters
        ? `${vehicle.plateLetters} ${vehicle.plateNo}`
        : vehicle.plateNo
    }

    return t('content.carNumber', { number: index + 1 })
  }

  function getSlotDayAmount(slotId: number, date: string): number | null {
    const lines = monthlyContract.value?.calculation.lines ?? []
    const dayLines = lines.filter(line => line.contractSlotId === slotId && line.date === date)

    if (!dayLines.length) return null

    return dayLines.reduce((total, line) => total + line.amount, 0)
  }

  const vehicleOptions = computed<SelectOption[]>(() => {
    const options = new Map<string, string>()

    Object.values(vehicles.value).forEach((vehicle) => {
      const id = String(vehicle.id)
      if (options.has(id)) return

      options.set(id, getVehicleOptionLabel(vehicle))
    })

    return [...options.entries()]
      .map(([value, label]) => ({ label, value }))
      .sort((first, second) => first.label.localeCompare(second.label))
  })

  function getVehicleOptionLabel(vehicle: MonthlyContractVehicle): string {
    if (!vehicle.plateNo) return String(vehicle.id)

    return vehicle.plateLetters
      ? `${vehicle.plateLetters} - ${vehicle.plateNo}`
      : vehicle.plateNo
  }

  const tableColumns = computed<TableColumn[]>(() => {
    const dates = monthlyContract.value?.calculation.calendar[0]?.days.map(day => day.date) ?? []

    return [
      {
        key: 'vehicleLabel',
        label: t('table.vehicle'),
        class: 'px-2 py-3 text-sm font-semibold text-neutral-20',
        cellClass: 'bg-slate-100 font-medium text-neutral-800',
      },
      ...dates.map(date => ({
        key: `day_${date}`,
        label: formatDayLabel(date),
        class: 'px-2 py-3 text-center text-sm font-semibold text-neutral-20',
        cellClass: 'text-center',
      })),
    ]
  })

  function matchesSelectedVehicle(slotId: number): boolean {
    if (!vehicleId.value) return true

    const vehicle = vehicles.value[String(slotId)]

    return String(vehicle?.id ?? '') === vehicleId.value
  }

  const tableRows = computed<ContractSlotsTableRow[]>(() => {
    const calendar = monthlyContract.value?.calculation.calendar ?? []

    return calendar
      .filter(slot => matchesSelectedVehicle(slot.slotId) && isActiveSlot(slot))
      .map((slot, index) => {
        const row: ContractSlotsTableRow = {
          index: index + 1,
          slotId: slot.slotId,
          vehicleLabel: getVehicleLabel(slot.slotId, index),
        }

        slot.days.forEach(day => {
          row[`day_${day.date}`] = {
            ...day,
            amount: getSlotDayAmount(slot.slotId, day.date),
          }
        })

        return row
      })
  })

  async function updateReview(): Promise<void> {
    const contract = monthlyContract.value
    if (!contract || !snapshotId.value || isLoading.value) return

    errorMessage.value = ''
    setLoading(true)

    const { success, message } = await monthlyContractStore.update(
      contract.contractId,
      snapshotId.value,
      {
        contractSlotId: null,
        name: contract.name,
        periodEnd: contract.periodEnd,
        periodStart: contract.periodStart,
      },
    )

    if (!success) {
      setLoading(false)
      errorMessage.value = message || t('error.networkError')

      return
    }

    await fetchMonthlyContract()
  }

  async function fetchMonthlyContract(): Promise<void> {
    errorMessage.value = ''

    if (!snapshotId.value) {
      monthlyContract.value = null
      errorMessage.value = t('error.networkError')

      return
    }

    setLoading(true)

    const { success, message } = await monthlyContractStore.getById(snapshotId.value)

    setLoading(false)

    if (!success) {
      errorMessage.value = message || t('error.networkError')
    }
  }

  function openEditPopup({
    day,
    vehicleLabel,
  }: {
    day: SlotCalendarDay
    vehicleLabel: string
  }): void {
    if (!day.calendarDayId) return

    selectedDay.value = day
    selectedVehicleLabel.value = vehicleLabel
    isEditPopupOpen.value = true
  }

  watch(snapshotId, () => {
    vehicleId.value = ''
    fetchMonthlyContract()
  }, { immediate: true })

  return {
    errorMessage,
    fetchMonthlyContract,
    isEditPopupOpen,
    isLoading,
    monthlyContract,
    openEditPopup,
    seatOptions,
    selectedDay,
    selectedVehicleLabel,
    slotCharges,
    snapshotId,
    tableColumns,
    tableRows,
    updateReview,
    vehicleId,
    vehicleOptions,
  }
}
