<template>
  <BasePopup
    v-model="model"
  >
    <div class="flex w-full flex-col items-center justify-center px-3.25">
      <slot name="icon">
        <IconError />
      </slot>

      <div class="mt-3 w-full rounded-[1.25rem] bg-error-50 p-2 py-8 text-center">
        <slot name="title">
          <p class="text-2xl font-bold text-neutral-10">
            {{ $t('error.systemError') }}
          </p>
        </slot>

        <slot name="message">
          <p v-if="message">
            {{ message }}
          </p>

          <ul
            v-if="errors && Object.keys(errors).length"
            class="mt-3 list-inside list-disc text-start text-sm text-neutral-10"
          >
            <li
              v-for="(msgs, key) in errors"
              :key="key"
              class="mt-1"
            >
              <span class="font-medium">{{ key }}:</span>
              {{ Array.isArray(msgs) ? msgs.join(', ') : msgs }}
            </li>
          </ul>
        </slot>
      </div>
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  defineProps<{
    message?: string | null
    errors?: Record<string, string[]>
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })
</script>

<style scoped>

</style>

