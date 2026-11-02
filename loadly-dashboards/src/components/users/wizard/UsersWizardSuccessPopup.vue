<template>
  <PopupSuccess
    :model-value="model"
    @update:model-value="$emit('success')"
  >
    <template #message>
      <p class="text-center text-2xl font-bold text-neutral-10">
        {{ isEditMode ? $t('success.accountUpdated') : $t('success.accountCreated') }}
      </p>

      <p
        v-if="user"
        class="text-center text-content-secondary"
      >
        {{
          isEditMode
            ? $t('success.accountUpdatedAndEmailSent', { name: user.fullName, email: user.email })
            : $t('success.accountCreatedAndEmailSent', { name: user.fullName, email: user.email })
        }}
      </p>
    </template>
  </PopupSuccess>
</template>

<script setup lang="ts">
  import type { User } from '@/types/user'

  defineProps<{
    isEditMode: boolean
    user: User | null
  }>()

  defineEmits<{
    success: []
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })
</script>

<style scoped>

</style>
