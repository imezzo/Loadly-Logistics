<template>
  <div
    class="
      mb-6 grid grid-cols-1 gap-4
      md:grid-cols-3
      lg:grid-cols-4
    "
  >
    <div class="col-span-full flex items-center gap-2">
      <button
        type="button"
        class="tab-primary-outline shrink-0"
        :class="{ active: filters.payerType === PayerType.COMPANY }"
        @click="changePayerType(PayerType.COMPANY)"
      >
        {{ $t('content.company') }}
      </button>

      <button
        type="button"
        class="tab-primary-outline shrink-0"
        :class="{ active: filters.payerType === PayerType.LEAD }"
        @click="changePayerType(PayerType.LEAD)"
      >
        {{ $t('content.externalLead') }}
      </button>
    </div>

    <BaseInputSearch
      :model-value="filters.search"
      wrapper-class="input-container flex items-center border shrink-0 border-neutral-500 rounded-xl px-3 h-11"
      :placeholder="$t('form.search')"
      @update:model-value="updateFilters({ search: $event ?? '' })"
    />

    <BaseSelect
      :model-value="filters.payerIds"
      :options="payerOptions"
      searchable
      @update:model-value="updateFilters({ payerIds: String($event ?? '') })"
    />

    <BaseSelect
      :model-value="filters.paymentMethod"
      :options="paymentMethodOptions"
      @update:model-value="updateFilters({ paymentMethod: String($event ?? '') })"
    />

    <div class="flex items-center gap-2">
      <button
        type="button"
        class="btn-primary w-full min-w-32"
        @click="applyFilters"
      >
        {{ $t('button.search') }}
      </button>

      <button
        v-if="hasActiveFilters"
        type="button"
        class="btn-link"
        @click="resetFilters"
      >
        {{ $t('button.clearFilters') }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
  import type { SelectOption } from '@/types/ui'
  import { useEnum } from '@/composables/useEnum'
  import { useLookups } from '@/composables/useLookups'
  import { useTransferList } from '../composables/useTransferList'
  import { PayerType } from '@/types/bank-transfer.enums'

  const { t } = useI18n()
  const {
    filters,
    updateFilters,
    applyFilters,
    resetFilters,
    hasActiveFilters,
  } = useTransferList()
  const {
    companies,
    leads,
    getCompanies,
    getLeads,
  } = useLookups()
  const { enums } = useEnum()

  const payerOptions = computed<SelectOption[]>(() => {
    const options = filters.value.payerType === PayerType.COMPANY
      ? companies.value?.items ?? []
      : leads.value?.items ?? []

    return [
      { value: '', label: filters.value.payerType === PayerType.COMPANY ? t('content.company') : t('content.lead') },
      ...options,
    ]
  })

  const paymentMethodOptions = computed<SelectOption[]>(() => [
    { value: '', label: t('form.method') },
    ...(enums.value?.PaymentMethod ?? []),
  ])

  function changePayerType(payerType: PayerType): void {
    updateFilters({ payerType, payerIds: '' })

    if (payerType === PayerType.COMPANY) {
      getCompanies(1)

      return
    }
    getLeads(1)
  }

  onMounted(async () => {
    await getCompanies(1)
  })
</script>

<style scoped>

</style>
