<template>
  <div class="mb-6 flex items-start justify-between gap-4">
    <div>
      <h4 class="mb-1 font-bold text-neutral-10">
        {{ $t('content.multipleLocations') }}
      </h4>
      <p class="text-sm text-neutral-600">
        {{ $t('content.multipleLocationsDescription') }}
      </p>
    </div>

    <button
      type="button"
      class="btn-primary-outline"
      @click="emit('addLocation')"
    >
      {{ $t('button.addLocations') }}
    </button>
  </div>
</template>

<script setup lang="ts">
  const emit = defineEmits<{
    addLocation: []
  }>()
</script>

<style scoped>

</style>

