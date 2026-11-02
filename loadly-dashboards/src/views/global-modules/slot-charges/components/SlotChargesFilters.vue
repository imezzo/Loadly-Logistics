<template>
  <div
    class="
      mb-6 grid grid-cols-1 items-end gap-4
      md:grid-cols-3
    "
  >
    <div>
      <label class="mb-2 block text-sm font-medium text-neutral-800">
        {{ $t('form.startDate') }}
      </label>

      <BaseInput
        v-model="startDate"
        type="date"
      />
    </div>

    <div>
      <label class="mb-2 block text-sm font-medium text-neutral-800">
        {{ $t('form.endDate') }}
      </label>

      <BaseInput
        v-model="endDate"
        type="date"
        :min="startDate"
      />
    </div>

    <button
      type="button"
      class="btn-primary min-w-40"
      @click="$emit('search')"
    >
      {{ $t('button.search') }}
    </button>
  </div>
</template>

<script setup lang="ts">
  const startDate = defineModel<string>('startDate', { required: true })
  const endDate = defineModel<string>('endDate', { required: true })

  defineEmits<{
    search: []
  }>()
</script>

<style scoped>

</style>
