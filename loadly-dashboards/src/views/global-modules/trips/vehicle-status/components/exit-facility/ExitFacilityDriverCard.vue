<template>
  <article
    class="flex flex-col gap-3 rounded-xl border border-neutral-500 bg-neutral-30 p-3"
    :class="{ 'border-primary ring-1 ring-primary': selected }"
  >
    <div class="flex items-center gap-3">
      <span
        class="
          flex size-10 shrink-0 items-center justify-center overflow-hidden rounded-full bg-theme
        "
        aria-hidden="true"
      >
        <IconUser class="size-5 text-black" />
      </span>

      <div class="min-w-0 flex-1">
        <p class="text-xs font-bold text-neutral-600">
          {{ driver.fullName }}
        </p>

        <p class="truncate text-sm text-neutral-800">
          {{ driver.phone }}
        </p>
      </div>
    </div>

    <button
      type="button"
      class="btn-primary w-full"
      @click="$emit('choose', driver)"
    >
      {{ $t('content.chooseThisDriver') }}
    </button>
  </article>
</template>

<script setup lang="ts">
  import type { Driver } from '@/types/admin-dashboard/response/driver/driver'

  defineProps<{
    driver: Driver
    selected?: boolean
  }>()

  defineEmits<{
    choose: [driver: Driver]
  }>()
</script>

<style scoped>

</style>
