import { useBankTransferStore } from '@/stores/financial/bank-transfer'
import { PayerType } from '@/types/bank-transfer.enums'
import type { BankTransferListItem } from '@/types/response/financial/bank-transfer'
import type { InvoicePayment } from '@/types/request/financial/invoice'
import { useContractStore } from '@/stores/admin-dashboard/contract'
import { useInvoicePaymentTotals } from './useInvoicePaymentTotals'

export interface InvoiceLinkedTransferRow {
  id: string
  bankTransferId: number
  invoicePaymentId?: number
  amount: number | ''
  label: string
  reference: string
  totalAmount: number
  remainingAmount: number
}

export function useInvoiceLinkedTransfers() {
  const bankTransferStore = useBankTransferStore()
  const { transfersList } = storeToRefs(bankTransferStore)

  const contractStore = useContractStore()
  const { contract } = storeToRefs(contractStore)
  const companyId = computed(() => contract.value?.companyId ?? null)

  const transferRows = ref<InvoiceLinkedTransferRow[]>([])
  const newTransferId = ref('')

  const { payableTotal, remainingTotal } = useInvoicePaymentTotals(transferRows)

  const availableTransferOptions = computed(() => {
    const usedIds = new Set(transferRows.value.map(row => String(row.bankTransferId)))

    return (transfersList.value?.data ?? []).map((transfer: BankTransferListItem) => ({
      label: transfer.label,
      value: String(transfer.id),
    })).filter(option => !usedIds.has(option.value))
  })

  async function fetchTransfers(): Promise<void> {
    if (!companyId.value) return

    const { data } = await bankTransferStore.getList({
      page: 1,
      size: 100,
      payerType: PayerType.COMPANY,
      payerIds: String(companyId.value),
    })

    transfersList.value = data
  }

  async function onNewTransferSelect(transferId: string | number | null | undefined): Promise<void> {
    if (!transferId) return

    const { data, success } = await bankTransferStore.getById(Number(transferId))
    if (!success || !data) return

    transferRows.value.push({
      id: crypto.randomUUID(),
      bankTransferId: data.id,
      label: data.label,
      amount: Math.min(data.remainingAmount, Number(remainingTotal.value.toFixed(2))),
      reference: data.externalPaymentReferenceNumber || data.payerName,
      totalAmount: data.amount,
      remainingAmount: data.remainingAmount,
    })
    newTransferId.value = ''
  }

  function removeTransferRow(rowId: string): void {
    transferRows.value = transferRows.value.filter(row => row.id !== rowId)
  }

  function maxAmountForRow(rowId: string): number {
    const row = transferRows.value.find(item => item.id === rowId)
    const remainingAmount = row?.remainingAmount ?? 0
    const amount = row?.amount ?? 0
    const totalAmount = Number((remainingTotal.value + Number(amount)).toFixed(2))

    return Math.min(remainingAmount, totalAmount);
  }

  function updateTransferAmount(rowId: string, amount: number | ''): void {
    const row = transferRows.value.find(item => item.id === rowId)
    if (!row) return

    if (amount && payableTotal.value) {
      row.amount = Math.min(Number(amount), maxAmountForRow(rowId))

      return
    }

    row.amount = 0
  }

  function resetTransferRows(): void {
    transferRows.value = []
    newTransferId.value = ''
  }

  async function initializeTransferRowsFromPayments(payments: InvoicePayment[]): Promise<void> {
    transferRows.value = []

    for (const payment of payments) {
      const { data, success } = await bankTransferStore.getById(payment.bankTransferId)
      if (!success || !data) continue

      transferRows.value.push({
        id: crypto.randomUUID(),
        bankTransferId: data.id,
        ...(payment.invoicePaymentId != null ? { invoicePaymentId: payment.invoicePaymentId } : {}),
        label: data.label,
        amount: payment.amount,
        reference: data.externalPaymentReferenceNumber || data.payerName,
        totalAmount: data.amount,
        remainingAmount: data.remainingAmount,
      })
    }
  }

  return {
    availableTransferOptions,
    fetchTransfers,
    initializeTransferRowsFromPayments,
    maxAmountForRow,
    newTransferId,
    onNewTransferSelect,
    removeTransferRow,
    resetTransferRows,
    transferRows,
    updateTransferAmount,
  }
}
