<template>
  <button
    type="button"
    class="btn-info-outline absolute end-0 top-0"
    @click="copy"
  >
    <IconCheck
      v-if="isCopied"
      class="size-4"
    />

    <IconCopy
      v-else
      class="size-4"
    />
  </button>
</template>

<script setup lang="ts">
  import { useClipboard } from '@vueuse/core'

  const props = defineProps<{
    id: string
  }>()

  const { copy: copyToClipboard, copied } = useClipboard()

  const isCopied = computed(() => copied.value)

  function copy(): void {
    if (props.id) {
      copyToClipboard(props.id)
    }
  }
</script>

<style scoped>

</style>
