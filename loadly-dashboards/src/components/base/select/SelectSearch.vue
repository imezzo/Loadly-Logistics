<template>
  <li
    class="
      sticky inset-x-0 top-0 flex w-full items-center gap-2 border-b border-neutral-500 bg-white p-2
    "
  >
    <IconSearch class="size-4 shrink-0 text-neutral-800 opacity-40" />
    <input
      v-model="searchKeyword"
      type="text"
      :placeholder="$t('form.search')"
      :aria-label="$t('a11y.searchOptions')"
      role="searchbox"
      class="
        flex-1 border-none bg-transparent text-sm text-neutral-800 outline-none
        placeholder:text-neutral-800/40
      "
      @click.stop
      @keydown.stop
      @input="onInput"
    >
  </li>
</template>

<script setup lang="ts">
  import { useDebounceFn } from '@vueuse/core'

  const emit = defineEmits<{
    search: [searchKeyword: string]
  }>()

  const searchKeyword = defineModel<string>({ required: true })

  const debouncedOnSearch = useDebounceFn(() => {
    emit('search', searchKeyword.value)
  }, 600)

  function onInput(): void {
    if (searchKeyword.value.length && searchKeyword.value.length < 3) return

    debouncedOnSearch()
  }
</script>

<style scoped>

</style>
