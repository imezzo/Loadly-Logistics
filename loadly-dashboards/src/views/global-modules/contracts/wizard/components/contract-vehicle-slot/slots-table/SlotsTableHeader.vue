<template>
  <div class="mb-2 flex flex-wrap items-center justify-between gap-2">
    <BaseInputSearch
      v-model="search"
      :placeholder="$t('form.searchByPlateNumber')"
      wrapper-class="
        flex h-11 w-full max-w-100 items-center rounded-lg border border-neutral-300
        bg-white px-3
      "
    />

    <button
      v-if="canAdd"
      type="button"
      class="btn-primary ms-auto"
      :aria-label="$t('button.add')"
      @click="$emit('add')"
    >
      <IconPlus class="size-4" />
      {{ $t('button.add') }}
    </button>
  </div>
</template>

<script setup lang="ts">
  defineProps<{
    canAdd: boolean
  }>()

  defineEmits<{
    add: []
  }>()

  const search = defineModel<string>('search', { required: true })
</script>
