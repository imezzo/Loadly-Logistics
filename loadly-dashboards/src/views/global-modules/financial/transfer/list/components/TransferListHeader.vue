<template>
  <div
    class="
      mb-6 flex flex-col gap-4
      sm:flex-row sm:items-center sm:justify-between
    "
  >
    <h1 class="text-xl font-bold text-neutral-800">
      {{ $t('title.transfer') }}
    </h1>

    <div class="flex items-center gap-3">
      <button
        type="button"
        class="btn-primary"
        @click="emit('onCreateTransfer')"
      >
        {{ $t('button.createTransfer') }}

        <IconReceipt class="size-4" />
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
  const emit = defineEmits<{
    onCreateTransfer: [],
  }>()
</script>

<style scoped>

</style>
