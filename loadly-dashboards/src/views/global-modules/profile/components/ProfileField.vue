<template>
  <div>
    <p class="mb-1 text-sm font-bold text-neutral-10">
      {{ label }}
    </p>
    <p class="flex items-center gap-2 text-neutral-800">
      {{ text }}
      <span
        v-if="verified"
        class="bg-success-20 text-success-60 rounded-sm px-2 py-0.5 text-xs font-medium"
      >
        {{ $t('content.status.verified') }}
      </span>
    </p>
  </div>
</template>

<script setup lang="ts">
  defineProps<{
    label: string
    text: string
    verified?: boolean
  }>()
</script>
