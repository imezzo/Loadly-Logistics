<template>
  <div :class="wrapperClass">
    <IconSearch class="me-2 shrink-0 opacity-40" />

    <input
      v-model="model"
      type="text"
      :placeholder="$attrs.placeholder?.toString() || $t('form.searchHere')"
      class="
        w-full flex-1 border-none bg-transparent text-black/40 outline-none
        placeholder:text-black/40
      "
      v-bind="$attrs"
    >
  </div>
</template>

<script setup lang="ts">
  defineOptions({
    inheritAttrs: false,
  })

  defineProps<{
    wrapperClass?: string
  }>()

  const model = defineModel<string>()
</script>

<style scoped>

</style>
