<template>
  <div
    class="
      mb-6 grid grid-cols-1 gap-4
      lg:grid-cols-12 lg:items-end
    "
  >
    <div class="lg:col-span-4">
      <BaseInputSearch
        id="top-drivers-search"
        v-model="search"
        :placeholder="t('form.searchByDrivers')"
        wrapper-class="
          flex h-11 w-full shrink-0 items-center rounded-lg border border-neutral-300
          bg-white px-3
        "
        class="w-full"
      />
    </div>

    <button
      type="button"
      class="
        btn-primary h-11 w-full shrink-0 px-6
        lg:col-span-2
      "
      @click="emit('search')"
    >
      {{ t('button.search') }}
    </button>
  </div>
</template>

<script setup lang="ts">
  const emit = defineEmits<{
    search: []
  }>()

  const search = defineModel<string>('search', { required: true })

  const { t } = useI18n()
</script>

<style scoped>

</style>
