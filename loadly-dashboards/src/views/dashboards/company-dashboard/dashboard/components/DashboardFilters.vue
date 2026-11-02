<template>
  <div class="mb-6 flex flex-wrap items-center gap-3">
    <h1 class="text-xl font-bold text-neutral-10">
      {{ $t('sidebar.dashboard') }}
    </h1>

    <div class="ms-auto flex flex-wrap items-center gap-3">
      <button
        type="button"
        class="rounded-full border px-4 py-2.5 text-sm transition-colors"
        :class="filters.isToday
          ? 'border-primary bg-primary/10 text-primary'
          : `
            border-gray-50 text-content-secondary
            hover:bg-gray-50
          `"
        @click="$emit('today')"
      >
        {{ $t('content.today') }}
      </button>

      <button
        type="button"
        class="rounded-full border px-4 py-2.5 text-sm transition-colors"
        :class="filters.isLastWeek
          ? 'border-primary bg-primary/10 text-primary'
          : `
            border-gray-50 text-content-secondary
            hover:bg-gray-50
          `"
        @click="$emit('lastWeek')"
      >
        {{ $t('content.lastWeek') }}
      </button>

      <BaseSelect
        :model-value="selectedYear"
        :options="yearOptions"
        wrapper-class="w-36"
        class="rounded-full border-0 bg-neutral-100"
        @update:model-value="$emit('yearChange', $event)"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
  import type { DashboardHomeRequest } from '@/types/company-dashboard/request/dashboard'
  import type { SelectOption } from '@/types/ui'

  defineProps<{
    filters: DashboardHomeRequest
    selectedYear: string
    yearOptions: SelectOption[]
  }>()

  defineEmits<{
    today: []
    lastWeek: []
    yearChange: [value: string]
  }>()
</script>

<style scoped>

</style>
