<template>
  <div class="mb-4 flex justify-center gap-2.5 text-sm">
    <div class="flex flex-col">
      <span
        class="
          flex h-11.5 w-12 items-center justify-center rounded-md border border-gray-600 font-bold
          text-neutral-50 shadow-[inset_0px_-3px_1.8px_0px_--theme(--color-gray-700/0.2)]
        "
      >
        {{ timerUnites(seconds).minutes }}
      </span>
      <span class="mt-1.5 text-content-secondary">{{ $t('form.minutes') }}</span>
    </div>

    <span class="pt-3 font-bold">:</span>

    <div class="flex flex-col">
      <span
        class="
          flex h-11.5 w-12 items-center justify-center rounded-md border border-gray-600 font-bold
          text-neutral-50 shadow-[inset_0px_-3px_1.8px_0px_--theme(--color-gray-700/0.2)]
        "
      >
        {{ timerUnites(seconds).seconds }}
      </span>
      <span class="mt-1.5 text-content-secondary">{{ $t('form.seconds') }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
  import { timerUnites } from '@/utils/time'

  defineProps<{
    seconds: number
  }>()
</script>

<style scoped>

</style>

