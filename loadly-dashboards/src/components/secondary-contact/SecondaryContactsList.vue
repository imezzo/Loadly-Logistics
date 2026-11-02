<template>
  <div
    v-for="(contact, index) in contacts"
    :key="index"
    class="mb-4 flex items-start gap-6 rounded-lg border border-neutral-200 bg-white p-4"
  >
    <div class="flex items-start gap-3">
      <IconUser class="mt-0.5 size-6 shrink-0 text-neutral-600" />

      <p class="font-semibold text-neutral-10">
        {{ contact.name }}
      </p>
    </div>

    <div class="flex items-start gap-2">
      <IconPhone class="mt-0.5 size-6 shrink-0 text-neutral-600" />

      <div>
        <p class="text-sm font-semibold text-neutral-10">
          {{ $t('form.phoneNumber') }}
        </p>

        <p class="text-sm text-neutral-600">
          {{ contact.phone }}
        </p>
      </div>
    </div>

    <div class="flex items-start gap-2">
      <IconEmail
        :is-open="false"
        class="mt-0.5 size-6 shrink-0 text-neutral-600"
      />

      <div>
        <p class="text-sm font-semibold text-neutral-10">
          {{ $t('form.email') }}
        </p>

        <p class="text-sm text-neutral-600">
          {{ contact.email }}
        </p>
      </div>
    </div>

    <button
      v-if="!isReadonly"
      type="button"
      class="btn-action-error ms-auto"
      @click="$emit('remove', index)"
    >
      <IconTrash class="size-4" />
    </button>
  </div>
</template>

<script setup lang="ts">
  import type { SecondaryContact } from '@/types/secondary-contact.types'

  defineProps<{
    contacts: SecondaryContact[]
    isReadonly?: boolean
  }>()

  defineEmits<{
    remove: [index: number]
  }>()
</script>

<style scoped>

</style>
