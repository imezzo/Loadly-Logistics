<template>
  <BasePopup
    v-model="model"
    :show-close-button="false"
  >
    <section class="w-full px-6 py-5">
      <h2 class="text-xl font-bold text-neutral-800">
        {{ $t('button.manageTripCharge') }}
      </h2>

      <BaseAlert
        :model-value="!!errorMessage"
        :message="errorMessage || ''"
        variant="error"
        class="mt-4 w-full"
      />

      <div
        v-if="charges.length"
        class="mt-6 max-h-70 divide-y divide-gray-100 overflow-y-auto"
      >
        <div
          v-for="(item, index) in charges"
          :key="item.chargeType"
          class="
            flex flex-col gap-4 py-4 pe-1
            sm:flex-row sm:items-center sm:justify-between
          "
        >
          <div class="flex flex-wrap items-center gap-2 text-sm font-medium text-neutral-800">
            <span class="font-bold">{{ item.chargeTypeName }}</span>

            <span
              v-if="item.totalBillable"
              class="text-content-secondary"
            >
              {{ formatValueWithCurrency(item.totalBillable) }}
            </span>
          </div>

          <BaseRadioGroup
            :model-value="item.invoiceType"
            :options="invoiceTypeOptions"
            wrapper-class="gap-15"
            @update:model-value="updateChargeSelection(index, $event)"
          />
        </div>
      </div>

      <p
        v-else
        class="mt-6 text-sm text-neutral-10"
      >
        {{ $t('content.noData') }}
      </p>

      <InvoiceTotals :charges="charges" />

      <BaseActions
        container-class="mt-4 justify-end gap-3"
        :prev-button-options="discardButtonOptions"
        :next-button-options="proceedButtonOptions"
        @prev="discard"
        @next="proceed"
      />
    </section>
  </BasePopup>
</template>

<script setup lang="ts">
  import { useLoading } from '@/composables/useLoading'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import type { InvoiceItem, TripInvoiceSummary } from '@/types/admin-dashboard/response/trip/trip-invoice'
  import { formatValueWithCurrency } from '@/utils/currency'
  import { InvoiceType } from '@/types/invoice.enums'
  import InvoiceTotals from './InvoiceTotals.vue'

  type ChargeItem = InvoiceItem & { invoiceType: InvoiceType }

  const props = defineProps<{
    tripIds?: string[]
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })

  const emit = defineEmits<{
    refresh: []
  }>()

  const { t } = useI18n()
  const { setLoading } = useLoading()

  const tripStore = useTripStore()
  const { tripsInvoiceSummary } = storeToRefs(tripStore)

  const errorMessage = ref<string | null>(null)
  const charges = ref<ChargeItem[]>([])

  const invoiceTypeOptions = computed(() => [
    { label: t('form.invoice'), value: InvoiceType.Invoice },
    { label: t('form.claim'), value: InvoiceType.Claim },
  ])

  const discardButtonOptions = computed(() => ({
    class: 'btn-secondary h-13.5 md:w-52.5',
    label: t('button.discard'),
  }))

  const proceedButtonOptions = computed(() => ({
    class: 'btn-primary h-13.5 md:w-52.5',
    disabled: !charges.value.length,
    label: t('button.proceed'),
  }))

  function updateChargeSelection(index: number, value: string | number): void {
    if (!charges.value[index]) return

    charges.value[index].invoiceType = Number(value)
  }

  function getInvoiceCharges(invoiceType: InvoiceType, items?: InvoiceItem[]): ChargeItem[] {
    if (!items) return []

    return items.map(item => ({ ...item, invoiceType }))
  }

  function setCharges(summary?: TripInvoiceSummary | null): void {
    charges.value = [
      ...getInvoiceCharges(InvoiceType.Invoice, summary?.tripInvoice.items),
      ...getInvoiceCharges(InvoiceType.Claim, summary?.claimInvoice.items),
    ].filter((item, index, self) => {
      const sameType = self.filter(charge => charge.chargeType === item.chargeType)

      if (sameType.length === 1) return true

      const preferred = sameType.find(charge => charge.totalBillable > 0) ?? sameType[0]

      return preferred && index === self.indexOf(preferred)
    })
  }

  async function getInvoiceSummary(): Promise<void> {
    if (!props.tripIds?.length) return

    const { data, message, success } = await tripStore.getTripsInvoiceSummary(props.tripIds)

    if (!success || !data) {
      errorMessage.value = message || t('error.networkError')

      return
    }

    if (!props.tripIds) {
      tripsInvoiceSummary.value = data
    }

    setCharges(data)
  }

  async function proceed(): Promise<void> {
    if (!props.tripIds?.length || !charges.value.length) return

    errorMessage.value = null
    setLoading(true)

    const { message, success } = await tripStore.updateTripChargesInvoiceType({
      charges: charges.value.map(item => ({
        chargeType: item.chargeType,
        invoiceType: Number(item.invoiceType),
      })),
      tripIds: props.tripIds.map(Number),
    })

    setLoading(false)

    if (success) {
      emit('refresh')
      model.value = false

      return
    }

    errorMessage.value = message || t('error.networkError')
  }

  function discard(): void {
    errorMessage.value = null
    model.value = false
  }

  onMounted(async () => {
    errorMessage.value = null

    if (props.tripIds) {
      await getInvoiceSummary()

      return
    }

    setCharges(tripsInvoiceSummary.value)
  })
</script>

<style scoped>

</style>
