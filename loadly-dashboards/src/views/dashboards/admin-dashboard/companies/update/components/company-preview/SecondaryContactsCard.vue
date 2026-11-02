<template>
  <CardPreview
    v-if="hasSecondaryContacts"
    :title="$t('content.secondaryContacts')"
  >
    <template #content>
      <div
        v-for="(contact, index) in secondaryContacts"
        :key="index"
        class="space-y-2 rounded-lg border border-gray-200 p-4"
      >
        <h4 class="mb-3 font-medium text-neutral-800">
          {{ contact.name }}
        </h4>

        <CardPreviewLabel
          :label="$t('content.emailAddress')"
          :content="contact.email || $t('content.notAvailable')"
        />

        <CardPreviewLabel
          :label="$t('content.phoneNumber')"
          :content="contact.phone || $t('content.notAvailable')"
        />

        <CardPreviewLabel
          :label="$t('form.position')"
          :content="contact.position || $t('content.notAvailable')"
        />
      </div>
    </template>
  </CardPreview>
</template>

<script setup lang="ts">
  import type { SecondaryContact } from '@/types/secondary-contact.types'

  const props = defineProps<{
    secondaryContacts: SecondaryContact[]
  }>()

  const hasSecondaryContacts = computed(() => props.secondaryContacts.length)
</script>

<style scoped>

</style>

