import { useContractStore } from '@/stores/admin-dashboard/contract'
import { useLoading } from '@/composables/useLoading'
import type { SlotCalendarDay } from '@/types/admin-dashboard/response/contract/included-slot-charges-calculate'
import type { TableColumn } from '@/types/ui'
import { ContractSlotDayDisposition } from '@/types/admin-dashboard/response/contract/monthly-contract/monthly-contract.enums'

export interface ContractSlotsTableRow {
  index: number
  slotId: number
  vehicleLabel: string
  [key: `day_${string}`]: SlotCalendarDay | null | undefined
}

interface UseContractSlotsStatusOptions {
  contractId: Ref<string>
  startDate: Ref<string>
  endDate: Ref<string>
  skipContractFetch?: boolean
}

export function useSlotsCharges({
  contractId,
  startDate,
  endDate,
  skipContractFetch = false,
}: UseContractSlotsStatusOptions) {
  const { t } = useI18n()
  const { setLoading } = useLoading()

  const contractStore = useContractStore()
  const { contract, slotCharges } = storeToRefs(contractStore)

  const errorMessage = ref('')

  function formatDayLabel(dateStr: string): string {
    const date = new Date(`${dateStr}T00:00:00`)
    const month = date.toLocaleDateString('en-US', { month: 'short' })
    const day = date.getDate()

    return `${month} ${day}`
  }

  function getSlotLabel(slotId: number, index: number): string {
    const vehicle = slotCharges.value?.vehicles[String(slotId)]

    if (vehicle?.plateNo) {
      return vehicle.plateLetters
        ? `${vehicle.plateLetters} ${vehicle.plateNo}`
        : vehicle.plateNo
    }

    return t('content.carNumber', { number: index + 1 })
  }

  const tableColumns = computed<TableColumn[]>(() => {
    const calendar = slotCharges.value?.calendar ?? []
    const dates = calendar[0]?.days.map(day => day.date) ?? []

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

  function getSlotDayAmount(slotId: number, date: string): number | null {
    const lines = slotCharges.value?.lines ?? []
    const dayLines = lines.filter(
      line => line.contractSlotId === slotId && line.date === date,
    )

    if (!dayLines.length) return null

    return dayLines.reduce((total, line) => total + line.amount, 0)
  }

  const tableRows = computed<ContractSlotsTableRow[]>(() => {
    const calendar = slotCharges.value?.calendar ?? []

    if (!calendar.length) return []

    return calendar.filter(slot =>
      slot.days.some(day => day.disposition !== ContractSlotDayDisposition.NotActive)
    )
    .map((slot, index) => {
      const row: ContractSlotsTableRow = {
        index: index + 1,
        slotId: slot.slotId,
        vehicleLabel: getSlotLabel(slot.slotId, index),
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

  async function fetchContract(): Promise<void> {
    if (skipContractFetch) return

    const { success, message } = await contractStore.getContractById(contractId.value)

    if (success) return
    errorMessage.value = message || t('error.networkError')
  }

  async function fetchSlotCharges(): Promise<void> {
    const result = await contractStore.calculateSlotCharges(contractId.value, {
      periodStart: startDate.value,
      periodEnd: endDate.value,
    })
    if (!result.success) {
      errorMessage.value = result.message || t('error.networkError')
    }
  }

  async function initCalculatedSlot(): Promise<void> {
    errorMessage.value = ''

    if (!contractId.value) {
      errorMessage.value = t('error.networkError')

      return
    }

    setLoading(true)
    await fetchContract()
    await fetchSlotCharges()
    setLoading(false)
  }

  return {
    contract,
    errorMessage,
    initCalculatedSlot,
    slotCharges,
    tableColumns,
    tableRows,
  }
}
