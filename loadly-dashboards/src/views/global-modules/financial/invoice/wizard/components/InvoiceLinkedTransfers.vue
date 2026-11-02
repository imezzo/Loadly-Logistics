<template>
  <div class="col-span-2 mb-8 w-full">
    <div class="rounded-xl border border-neutral-200">
      <div class="bg-neutral-100 px-4 py-3 text-sm font-medium text-neutral-800">
        {{ $t('content.transferList') }}
      </div>

      <div
        v-if="transferRows.length"
        class="divide-y divide-neutral-200"
      >
        <div
          v-for="row in transferRows"
          :key="row.id"
          class="
            grid grid-cols-1 items-center gap-4 p-4
            md:grid-cols-4
          "
        >
          <div class="col-span-full">
            <p class="text-xs font-bold text-neutral-10">
              {{ $t('content.referenceNumber') }}
            </p>
            <p class="text-neutral-800">
              {{ row.label }}
            </p>
          </div>

          <div>
            <p class="text-xs font-bold text-neutral-10">
              {{ $t('form.total') }}
            </p>
            <p class="text-neutral-800">
              {{ formatValueWithCurrency(row.totalAmount) }}
            </p>
          </div>

          <div>
            <p class="text-xs font-bold text-neutral-10">
              {{ $t('content.remainingAmount') }}
            </p>
            <p class="text-neutral-800">
              {{ formatValueWithCurrency(row.remainingAmount) }}
            </p>
          </div>

          <div class="col-span-2 flex gap-2">
            <div class="flex w-full flex-col">
              <BaseInput
                :model-value="row.amount"
                min="0"
                :max="maxAmountForRow(row.id)"
                :disabled="!maxAmountForRow(row.id)"
                :suffix="$t('form.egp')"
                @update:model-value="emit('updateAmount', row.id, $event as number | '')"
              />

              <BaseValidation
                v-if="!maxAmountForRow(row.id)"
                :message="$t('error.cannotAddAmountBeforeAdjustingTransfers')"
                custom-class="text-xs"
              />
            </div>

            <button
              type="button"
              class="btn-action-error size-11 justify-self-end"
              :aria-label="$t('button.remove')"
              @click="emit('removeRow', row.id)"
            >
              <IconTrash class="size-4" />
            </button>
          </div>
        </div>
      </div>

      <div
        class="
          grid grid-cols-1 items-center gap-4 p-4
          md:grid-cols-[auto_1fr]
        "
      >
        <span class="text-sm font-semibold text-primary">
          {{ $t('content.new') }}
        </span>

        <BaseSelect
          :model-value="newTransferId"
          :options="availableTransferOptions"
          :placeholder="$t('form.chooseTransfer')"
          wrapper-class="w-full"
          @update:model-value="emit('selectTransfer', $event)"
        />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { formatValueWithCurrency } from '@/utils/currency'
  import type { InvoiceLinkedTransferRow } from '../composables/useInvoiceLinkedTransfers'
  import type { SelectOption } from '@/types/ui'

  defineProps<{
    availableTransferOptions: SelectOption[]
    maxAmountForRow: (rowId: string) => number
    newTransferId: string
    transferRows: InvoiceLinkedTransferRow[]
  }>()

  const emit = defineEmits<{
    removeRow: [rowId: string]
    selectTransfer: [transferId: string | number | null | undefined]
    updateAmount: [rowId: string, amount: number | '']
  }>()
</script>

<style scoped>

</style>
