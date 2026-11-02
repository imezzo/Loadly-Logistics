<template>
  <div class="rounded-lg border border-neutral-200 p-4">
    <div class="mb-3 flex items-center justify-between gap-3">
      <p class="text-sm font-medium text-neutral-10">
        {{ $t('form.theContractors') }}
        <span class="text-error-600">*</span>
      </p>

      <button
        v-if="!disabled"
        type="button"
        class="
          flex shrink-0 items-center gap-2 rounded-lg bg-error-50 px-3 py-2 text-sm font-medium
          text-error-600 transition-colors
          hover:bg-error-100
        "
        @click="emit('unassign')"
      >
        <IconTrash class="size-4" />
        {{ $t('button.unassignContractor') }}
      </button>
    </div>

    <div class="flex items-center gap-3">
      <span
        class="
          flex size-12 shrink-0 items-center justify-center rounded-full bg-neutral-200 text-lg
          font-semibold text-neutral-600
        "
      >
        {{ initial }}
      </span>

      <div class="min-w-0">
        <p class="truncate font-bold text-neutral-10">
          {{ name }}
        </p>

        <p
          v-if="phone"
          class="text-sm text-content-secondary"
        >
          {{ phone }}
        </p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
  const props = defineProps<{
    disabled?: boolean
    name: string
    phone?: string
  }>()

  const emit = defineEmits<{
    unassign: []
  }>()

  const initial = computed(() => props.name?.charAt(0).toUpperCase() || '')
</script>

<style scoped>

</style>
