<template>
  <form
    class="
      mb-6 grid grid-cols-1 items-end gap-4
      md:grid-cols-4
    "
    @submit.prevent
  >
    <div class="flex flex-col gap-2">
      <label
        for="vehicleId"
        class="text-sm font-medium text-neutral-700"
      >
        {{ $t('table.vehicle') }}
      </label>

      <BaseSelect
        id="vehicleId"
        v-model="vehicleId"
        searchable
        :options="options"
        :placeholder="$t('table.vehicle')"
      />
    </div>
  </form>
</template>

<script setup lang="ts">
  import type { SelectOption } from '@/types/ui'

  const props = defineProps<{
    vehicleOptions: SelectOption[]
  }>()

  const { t } = useI18n()
  const vehicleId = defineModel<string>({ required: true })

  const options = computed<SelectOption[]>(() => [
    { label: t('form.select'), value: '' },
    ...props.vehicleOptions,
  ])
</script>

<style>

</style>
