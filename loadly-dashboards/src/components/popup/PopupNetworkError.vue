<template>
  <BasePopup
    :model-value="isNetworkError"
    @update:model-value="onNetworkErrorUpdate"
  >
    <div class="flex w-full flex-col items-center justify-center px-3.25">
      <slot name="icon">
        <IconNetworkError />
      </slot>

      <div class="mt-3 w-full rounded-[1.25rem] bg-neutral-95 p-2 py-8 text-center">
        <slot name="title">
          <p class="text-2xl font-bold text-neutral-10">
            {{ $t('error.networkServerError') }}
          </p>
        </slot>

        <slot name="message">
          <p>
            {{ $t('error.networkServerErrorMessage') }}
          </p>
        </slot>
      </div>
    </div>
  </BasePopup>
</template>

<script setup lang="ts">
  import { useNetworkStore } from '@/stores/admin-dashboard/network'

  const networkStore = useNetworkStore()
  const { isNetworkError } = storeToRefs(networkStore)

  function onNetworkErrorUpdate(value: boolean) {
    if (!value) {
      networkStore.clearNetworkError()
    }
  }
</script>

<style scoped>

</style>

