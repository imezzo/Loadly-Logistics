<template>
  <div
    v-if="duration"
    class="mt-4 flex flex-col gap-2"
  >
    <div class="flex w-fit items-center gap-2 rounded-full bg-black/3 px-3 py-1">
      <IconClock class="size-4 text-black/40" />

      <p class="font-medium text-neutral-10">
        {{ $t('form.duration') }}: {{ duration }}
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
  defineProps<{
    duration?: string | null
  }>()
</script>

<style scoped>

</style>
