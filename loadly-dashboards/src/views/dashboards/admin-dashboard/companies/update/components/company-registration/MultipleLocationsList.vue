<template>
  <div
    v-if="locations.length"
    class="mb-6 space-y-4"
  >
    <div
      v-for="(location, index) in locations"
      :key="index"
      class="flex items-center gap-6 rounded-lg border border-neutral-200 bg-white p-4"
    >
      <div class="flex items-start gap-2">
        <IconLocation class="mt-0.5 size-6 shrink-0 text-neutral-600" />
        <div>
          <p class="text-sm font-semibold text-neutral-10">
            {{ $t('form.city') }}
          </p>
          <p class="text-sm text-neutral-600">
            {{ location.city }}
          </p>
        </div>
      </div>

      <div class="flex items-start gap-2">
        <IconLocation class="mt-0.5 size-6 shrink-0 text-neutral-600" />
        <div>
          <p class="text-sm font-semibold text-neutral-10">
            {{ $t('form.governorate') }}
          </p>
          <p class="text-sm text-neutral-600">
            {{ location.governorate }}
          </p>
        </div>
      </div>

      <div class="flex items-start gap-2">
        <IconPin class="mt-0.5 size-6 shrink-0 text-neutral-600" />
        <div>
          <p class="text-sm font-semibold text-neutral-10">
            {{ $t('form.district') }}
          </p>
          <p class="text-sm text-neutral-600">
            {{ location.district }}
          </p>
        </div>
      </div>

      <button
        type="button"
        class="btn-action-error ms-auto"
        @click="$emit('remove', index)"
      >
        <IconTrash class="size-4" />
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
  defineProps<{
    locations: {
      city: string
      governorate: string
      district: string
    }[]
  }>()

  defineEmits<{
    remove: [index: number]
  }>()
</script>

<style scoped>

</style>
