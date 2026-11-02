<template>
  <PopupError
    v-if="saveError"
    :model-value="saveError !== null"
    :message="saveError"
    :errors="saveErrors"
    @update:model-value="saveError = null"
  />

  <div class="mt-2">
    <div
      class="
        rounded-t-xl border-b border-gray-100 bg-neutral-30 px-4 py-3 text-sm font-semibold
        text-neutral-800
      "
    >
      {{ $t('content.tripChargeSection') }}
    </div>

    <div class="divide-y divide-gray-100">
      <div
        v-for="(line, index) in charges"
        :key="line.id"
        class="
          flex flex-col gap-3 px-4 py-3
          sm:flex-row sm:items-center sm:gap-2
        "
      >
        <span class="w-8 shrink-0 text-sm font-medium text-content-secondary">
          {{ index + 1 }}
        </span>

        <span class="min-w-0 flex-1 text-sm font-medium text-neutral-800">
          {{ line.chargeTypeName }}
        </span>

        <div
          class="
            grid w-full min-w-0 shrink-0 grid-cols-3 gap-2
            sm:ms-auto sm:max-w-1/2
          "
        >
          <div>
            <label class="text-sm font-medium text-neutral-800">
              {{ $t('form.total') }}
            </label>

            <BaseInput
              v-model="line.amount"
              type="number"
              comma-separated
              :suffix="$t('form.egp')"
              class="text-sm text-neutral-800"
              :disabled="disabled"
            />
          </div>

          <div>
            <label class="text-sm font-medium text-neutral-800">
              {{ $t('form.customer') }}
            </label>

            <BaseInput
              :model-value="line.amount - (line.companyShare ?? 0)"
              type="number"
              comma-separated
              :suffix="$t('form.egp')"
              class="text-sm text-neutral-800"
              container-classes="disabled"
              disabled
            />
          </div>

          <div>
            <label class="text-sm font-medium text-neutral-800">
              {{ $t('form.loadly') }}
            </label>

            <BaseInput
              v-model="line.companyShare"
              type="number"
              comma-separated
              :suffix="$t('form.egp')"
              class="text-sm text-neutral-800"
              :disabled="disabled"
            />
          </div>
        </div>

        <button
          v-if="!disabled"
          type="button"
          class="btn-action-error mt-auto size-11"
          @click="removeCharge(line.id)"
        >
          <IconCross class="size-4" />
        </button>
      </div>

      <TripChargesNewRow
        v-if="!disabled"
        v-model:charge-value="newChargeValue"
        v-model:amount="newChargeAmount"
        v-model:company-share="newChargeCompanyShare"
        :charge-type-options="tripChargeTypes"
      />
    </div>

    <TripChargesIndicators :charges />

    <div
      v-if="showFooterActions && !disabled"
      class="
        flex flex-col gap-3 border-t border-gray-100 p-4
        sm:flex-row sm:items-center sm:justify-between
      "
    >
      <button
        type="button"
        class="btn-primary-outline flex items-center justify-center gap-2"
        @click="addNewCharge"
      >
        <IconPlus class="size-4" />
        {{ $t('button.addNewCharge') }}
      </button>

      <button
        type="button"
        class="btn-primary ms-auto"
        @click="saveTripCharge"
      >
        {{ $t('button.saveTripCharge') }}
      </button>

      <button
        v-if="hasSaveAndBack"
        type="button"
        class="btn-success"
        @click="saveAndBack"
      >
        {{ $t('button.saveAndBack') }}
      </button>
    </div>

    <div
      v-else-if="!disabled"
      class="border-t border-gray-100 p-4"
    >
      <button
        type="button"
        class="btn-primary-outline flex items-center justify-center gap-2"
        @click="addNewCharge"
      >
        <IconPlus class="size-4" />
        {{ $t('button.addNewCharge') }}
      </button>
    </div>

    <PopupError
      v-if="saveError"
      :model-value="saveError !== null"
      :message="saveError"
      @update:model-value="saveError = null"
    />
  </div>
</template>

<script setup lang="ts">
  import TripChargesNewRow from './TripChargesNewRow.vue'
  import TripChargesIndicators from './TripChargesIndicators.vue'
  import { useLoading } from '@/composables/useLoading'
  import { useEnum } from '@/composables/useEnum'
  import { useTripStore } from '@/stores/admin-dashboard/trip'
  import type { TripWizardTripCharge } from '@/types/admin-dashboard/response/trip/trip'

  const props = withDefaults(defineProps<{
    disabled?: boolean
    showFooterActions?: boolean
  }>(), {
    disabled: false,
    showFooterActions: true,
  })

  const charges = defineModel<TripWizardTripCharge[]>('charges', { required: true })

  const { t } = useI18n()
  const { setLoading } = useLoading()
  const router = useRouter()
  const route = useRoute()

  const tripStore = useTripStore()
  const { trip } = storeToRefs(tripStore)

  const redirectPath = computed(() => localStorage.getItem('companyInvoices'))
  const hasSaveAndBack = computed(() => !!redirectPath.value && !!route.query.tab)

  const saveError = ref<string | null>(null)
  const saveErrors = ref<Record<string, string[]>>({})

  const { enums } = useEnum()

  const tripChargeTypes = computed(
    () => enums.value?.['TripChargeType']?.filter(
      item => !charges.value.some(
        charge => charge.chargeType === Number(item.value)
      )
    ) ?? [])

  const newChargeValue = ref('')
  const newChargeAmount = ref<number | null>(null)
  const newChargeCompanyShare = ref<number | null>(null)

  function removeCharge(id: number): void {
    charges.value = charges.value.filter(charge => charge.id !== id)
  }

  function addNewCharge(): void {
    const value = Number(newChargeValue.value)
    if (!Number.isFinite(value) || newChargeValue.value === '') return
    if (!tripChargeTypes.value.some(item => Number(item.value) === value)) return

    const label = tripChargeTypes.value.find(item => Number(item.value) === value)?.label ?? ''

    charges.value.push({
      id: -Date.now(),
      amount: Number(newChargeAmount.value),
      chargeType: value,
      chargeTypeName: label,
      extraFeeTypeId: null,
      companyShare: newChargeCompanyShare.value || 0,
      notes: null,
    })

    newChargeValue.value = ''
    newChargeAmount.value = null
    newChargeCompanyShare.value = null
  }

  async function saveTripCharge(): Promise<void> {
    if (!props.showFooterActions) return

    const tripId = Number(trip.value.id)
    if (!Number.isInteger(tripId) || tripId <= 0) return

    saveError.value = null
    setLoading(true)

    const { success, message, errors } = await tripStore.updateTripCharges(tripId, charges.value)

    if (success) {
      await tripStore.fetchTrip(tripId)
    } else {
      saveError.value = message || t('error.somethingWentWrong')
      saveErrors.value = errors ?? []
    }
    setLoading(false)
  }

  function redirectToPreviousPage(): void {
    if (!redirectPath.value) return

    localStorage.removeItem('companyInvoices')
    router.push(redirectPath.value)
  }

  function saveAndBack(): void {
    saveTripCharge()
    redirectToPreviousPage()
  }
</script>

<style scoped>

</style>
