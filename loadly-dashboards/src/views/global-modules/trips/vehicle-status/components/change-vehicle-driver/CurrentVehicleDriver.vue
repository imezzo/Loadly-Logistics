<template>
  <div class="space-y-4">
    <p class="text-sm text-neutral-600">
      {{ $t('content.assignDriverBasedOnChoices') }}
    </p>

    <div class="flex items-center gap-4 rounded-lg border border-neutral-300 bg-white p-4">
      <div
        class="
          flex size-12 shrink-0 items-center justify-center overflow-hidden rounded-full
          bg-neutral-200
        "
      >
        <span class="text-lg font-semibold text-neutral-600">
          {{ props.driverName?.charAt(0).toUpperCase() }}
        </span>
      </div>

      <div class="min-w-0 flex-1">
        <p class="text-xs font-medium text-neutral-600">
          {{ $t('content.currentDriver') }}
        </p>

        <p class="font-medium text-neutral-10">
          {{ driverName }}
        </p>
      </div>

      <button
        type="button"
        class="btn-action-error"
        :aria-label="$t('button.remove')"
        :disabled="disabled"
        @click="emit('remove')"
      >
        <IconTrash class="size-5" />
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
  const props = defineProps<{
    driverName?: string | null
    disabled?: boolean
  }>()

  const emit = defineEmits<{
    remove: []
  }>()
</script>

<style scoped>

</style>
