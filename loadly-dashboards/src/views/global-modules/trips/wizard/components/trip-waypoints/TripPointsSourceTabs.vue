<template>
  <div class="mb-4 grid grid-cols-2 rounded-lg">
    <button
      type="button"
      class="rounded-s-md border px-4 py-2 text-sm font-medium transition"
      :class="isContractMode
        ? 'text-primary-500 border-primary bg-white'
        : 'border-gray-50 text-neutral-600'"
      :disabled="isExternalTripMode"
      @click="!isExternalTripMode && $emit('switch-to-contract')"
    >
      {{ $t('content.contractDestinations') }}
    </button>

    <button
      type="button"
      class="rounded-e-md border px-4 py-2 text-sm font-medium transition"
      :class="isContractMode
        ? 'border-gray-50 text-neutral-600'
        : 'border-primary bg-white text-primary'"
      @click="$emit('switch-to-manual')"
    >
      {{ $t('content.manualDestination') }}
    </button>
  </div>
</template>

<script setup lang="ts">
  import { useTripWizard } from '../../composables/useTripWizard'

  defineProps<{
    isContractMode: boolean
  }>()

  defineEmits<{
    'switch-to-contract': []
    'switch-to-manual': []
  }>()

  const { isExternalTripMode } = useTripWizard()
</script>

<style scoped>

</style>
