<template>
  <div
    class="
      flex flex-col gap-3 px-4 py-3
      sm:flex-row sm:items-center sm:gap-4
    "
  >
    <span
      class="
        w-8 shrink-0 text-sm font-bold text-primary
        sm:w-auto sm:min-w-8
      "
    >
      {{ $t('content.newChargeRow') }}
    </span>

    <BaseSelect
      v-model="chargeValue"
      :options="selectOptions"
      :placeholder="$t('content.chooseChargeTypes')"
      searchable
      wrapper-class="w-full flex-1 sm:max-w-1/2"
    />

    <BaseInput
      v-model="amount"
      type="number"
      comma-separated
      :suffix="$t('form.egp')"
      placeholder="0"
      class="text-end text-sm text-neutral-800"
      wrapper-class="w-1/4"
      container-classes="!border-primary focus-within:!ring-2 focus-within:ring-primary/30"
    />

    <BaseInput
      v-model="companyShare"
      type="number"
      comma-separated
      :suffix="$t('form.egp')"
      placeholder="0"
      class="text-end text-sm text-neutral-800"
      wrapper-class="w-1/4"
      container-classes="!border-primary focus-within:!ring-2 focus-within:ring-primary/30"
    />
  </div>
</template>

<script setup lang="ts">
  import type { EnumItem } from '@/types/response/enum'
  import type { SelectOption } from '@/types/ui'

  const props = defineProps<{
    chargeTypeOptions: EnumItem[]
  }>()

  const { t } = useI18n()

  const selectOptions = computed<SelectOption[]>(() => {
    const placeholder = t('content.chooseChargeTypes')

    return [
      { value: '', label: placeholder, searchableText: placeholder },
      ...props.chargeTypeOptions.map(item => ({
        value: item.value,
        label: item.label,
        searchableText: item.label,
      })),
    ]
  })

  const chargeValue = defineModel<string>('chargeValue', { default: '' })
  const amount = defineModel<number | null>('amount', { default: null })
  const companyShare = defineModel<number | null>('companyShare', { default: null })
</script>

<style scoped>

</style>
