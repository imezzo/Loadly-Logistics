<template>
  <div class="mb-6 flex items-start justify-between gap-4">
    <div>
      <h4 class="mb-1 font-bold text-neutral-10">
        {{ $t('content.secondaryContacts') }}
      </h4>
      <p class="text-sm text-neutral-600">
        {{ $t('content.secondaryContactDescription') }}
      </p>
    </div>

    <button
      v-if="!isReadonly"
      type="button"
      class="btn-primary-outline"
      @click="$emit('addContact')"
    >
      {{ $t('button.addContact') }}
    </button>
  </div>
</template>

<script setup lang="ts">
  defineProps<{
    isReadonly?: boolean
  }>()

  defineEmits<{
    addContact: []
  }>()
</script>

<style scoped>

</style>
