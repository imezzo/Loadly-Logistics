<template>
  <div class="rounded-xl border border-gray-50 bg-white p-1">
    <h3 class="mb-1 px-3 py-2 text-lg font-bold text-neutral-10">
      <slot name="title">
        {{ title }}
      </slot>
    </h3>

    <slot>
      <div
        class="
          grid grid-cols-1 gap-4 rounded-[0.625rem] border border-gray-50 bg-neutral-30 p-3
          md:grid-cols-3
        "
      >
        <slot name="content" />
      </div>
    </slot>
  </div>
</template>

<script setup lang="ts">
  defineProps<{
    title: string
  }>()
</script>

<style scoped>

</style>
