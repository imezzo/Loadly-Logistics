<template>
  <article class="rounded-2xl border border-neutral-500 bg-white p-6">
    <header>
      <h2 class="text-base font-bold text-neutral-10">
        {{ $t('content.invoiceSummary') }}
      </h2>
      <p class="mt-1 text-sm text-content-secondary">
        {{ $t('content.invoiceSummarySubtitle') }}
      </p>
    </header>

    <ul class="mt-4 space-y-4">
      <li class="flex items-center justify-between">
        <span class="text-sm font-medium text-neutral-10">
          {{ $t('content.pendingInvoices') }}
        </span>
        <span class="font-bold text-warning-300">
          {{ $t('content.invoicesCount', { count: pendingInvoices }) }}
        </span>
      </li>

      <li class="flex items-center justify-between">
        <span class="text-sm font-medium text-neutral-10">
          {{ $t('content.paidThisMonth') }}
        </span>
        <span class="font-bold text-success-500">
          {{ $t('content.invoicesCount', { count: paidThisMonth }) }}
        </span>
      </li>
    </ul>
  </article>
</template>

<script setup lang="ts">
  defineProps<{
    pendingInvoices: number
    paidThisMonth: number
  }>()
</script>

<style scoped>

</style>
