<template>
  <div class="ms-auto mt-6 w-full max-w-90">
    <hr class="mb-4 border-neutral-500">

    <div class="flex items-center justify-between text-lg">
      <span class="font-bold text-neutral-10">
        {{ $t('content.total') }}
      </span>

      <span class="font-bold text-neutral-10">
        {{ formatValueWithCurrency(totalAmount) }}
      </span>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { formatValueWithCurrency } from '@/utils/currency'

  defineProps<{
    totalAmount: number
  }>()
</script>

<style scoped>

</style>
