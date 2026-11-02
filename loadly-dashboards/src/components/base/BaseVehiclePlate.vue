<template>
  <div class="w-25 min-w-0 flex-1">
    <div class="flex items-center justify-between rounded-t-sm bg-[#a21313] px-3 py-0.5">
      <span class="text-xs font-medium text-white">
        Egypt
      </span>
      <span class="text-xs font-medium text-white">
        مصر
      </span>
    </div>

    <div class="mb-1 flex items-center gap-2 p-0.5">
      <span class="me-auto text-sm font-medium text-neutral-10">
        {{ plateLetters || '***' }}
      </span>
      |
      <span class="ms-auto text-sm font-medium text-neutral-10">
        {{ plateNo || '***' }}
      </span>
    </div>
  </div>
</template>

<script setup lang="ts">
  defineProps<{
    plateLetters?: string
    plateNo?: string
  }>()
</script>
