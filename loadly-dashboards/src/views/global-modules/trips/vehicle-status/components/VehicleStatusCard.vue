<template>
  <article class="flex flex-col rounded-2xl border border-neutral-500 bg-white p-4">
    <span class="mb-4 text-3xl font-bold text-neutral-800">
      {{ count }}
    </span>

    <span
      class="
        relative flex items-center gap-2 ps-3.25 font-medium
        after:absolute after:start-0 after:top-1/2 after:block after:size-2 after:shrink-0
        after:-translate-y-1/2 after:rounded-full after:bg-current after:content-['']
      "
      :class="labelClass"
    >
      {{ label }}
    </span>
  </article>
</template>

<script setup lang="ts">
  defineProps<{
    count: number
    label: string
    labelClass: string
  }>()
</script>

<style scoped>

</style>
