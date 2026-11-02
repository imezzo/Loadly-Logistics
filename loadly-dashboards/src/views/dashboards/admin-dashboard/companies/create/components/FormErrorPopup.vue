<template>
  <PopupError
    v-model="model"
    :message="message || undefined"
  >
    <template
      v-if="formErrors && Object.keys(formErrors).length > 0"
      #message
    >
      <div v-if="message">
        <p class="mb-2">
          {{ message }}
        </p>
      </div>
      <div
        v-if="Object.keys(formErrors).length > 0"
        class="mt-2"
      >
        <p class="mb-2 font-semibold">
          {{ $t('error.validationErrors') || 'Validation Errors:' }}
        </p>
        <ul class="list-inside list-disc space-y-1">
          <li
            v-for="(fieldErrors, fieldName) in formErrors"
            :key="fieldName"
          >
            <span class="font-medium">{{ fieldName }}:</span>
            {{ Array.isArray(fieldErrors) ? fieldErrors.join(', ') : fieldErrors }}
          </li>
        </ul>
      </div>
    </template>
  </PopupError>
</template>

<script setup lang="ts">
  defineProps<{
    message?: string | null
    formErrors?: Record<string, string[]>
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })
</script>

<style scoped>

</style>

