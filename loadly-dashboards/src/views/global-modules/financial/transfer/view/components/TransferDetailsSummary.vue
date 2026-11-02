<template>
  <section class="rounded-xl bg-neutral-100 p-6">
    <h1 class="mb-6 text-2xl font-bold text-neutral-10">
      {{ transfer.payerName }}
    </h1>

    <div
      class="
        grid grid-cols-1 gap-6
        md:grid-cols-3
      "
    >
      <div>
        <CardPreviewLabel
          :label="$t('content.transferAmount')"
          :content="formatValueWithCurrency(transfer.amount)"
        />
      </div>

      <div>
        <CardPreviewLabel
          :label="$t('content.remainingAmount')"
          :content="formatValueWithCurrency(transfer.remainingAmount)"
        />
      </div>

      <div>
        <CardPreviewLabel
          :label="$t('form.method')"
          :content="paymentMethodName"
        />
      </div>

      <div>
        <CardPreviewLabel
          :label="$t('content.referenceNumber')"
          :content="transfer.externalPaymentReferenceNumber || '-'"
        />
      </div>

      <div>
        <CardPreviewLabel
          :label="$t('content.dateOfTransfer')"
          :content="transfer.issueDate"
        />
      </div>

      <div
        v-if="transfer.fileUrl"
        class="rounded-lg border border-neutral-500 p-3"
      >
        <div class="flex items-center gap-3">
          <UploaderFileExtension :file-url="transfer.fileUrl" />

          <p class="min-w-0 flex-1 truncate text-sm font-medium text-neutral-700">
            {{ fileName }}
          </p>

          <BaseLink
            :to="transfer.fileUrl"
            target="_blank"
            rel="noopener noreferrer"
            class="
              flex shrink-0 items-center gap-2 rounded-full bg-primary/30 px-4 py-2 text-sm
              font-medium text-primary transition-colors
              hover:bg-primary/40
            "
          >
            <IconEye
              visible
              class="[&>path]:stroke-primary"
            />
            {{ $t('content.preview') }}
          </BaseLink>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
  import type { BankTransferDetail } from '@/types/response/financial/bank-transfer'
  import { useEnum } from '@/composables/useEnum'
  import { formatValueWithCurrency } from '@/utils/currency'

  const props = defineProps<{
    transfer: BankTransferDetail
  }>()

  const { getDisplayName } = useEnum()

  const paymentMethodName = computed(() => getDisplayName('PaymentMethod', props.transfer.paymentMethod))

  const fileName = computed(() => {
    if (!props.transfer.fileUrl) return ''

    const urlParts = props.transfer.fileUrl.split('/')

    return urlParts[urlParts.length - 1] || ''
  })
</script>

<style scoped>

</style>
