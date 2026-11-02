<template>
  <BasePopup
    v-model="model"
    :show-close-button="true"
  >
    <div class="flex w-full flex-col items-center justify-center px-3.25">
      <div class="flex items-center justify-center">
        <IconError />
      </div>

      <div class="mt-3 w-full rounded-[1.25rem] bg-error-50 p-2 py-8 text-center">
        <p class="text-2xl font-bold text-neutral-10">
          {{ $t('error.cantDeleteRole') }}
        </p>

        <p class="mt-4 text-content-secondary">
          {{ message }}
        </p>
      </div>
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  defineProps<{
    message: string
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })
</script>

<style scoped>

</style>

