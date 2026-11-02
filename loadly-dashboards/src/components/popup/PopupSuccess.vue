<template>
  <BasePopup
    v-model="model"
  >
    <div class="flex w-full flex-col items-center justify-center px-3.25">
      <slot name="icon">
        <IconSuccess />
      </slot>

      <div class="mt-3 w-full rounded-[1.25rem] bg-success-200 p-2 py-8 text-center">
        <slot name="message">
          <p v-if="message">
            {{ message }}
          </p>
        </slot>
      </div>

      <slot name="actions" />
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  defineProps<{
    message?: string
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })
</script>

<style scoped>

</style>
