<template>
  <div
    v-if="contractConditions?.fileUrl"
    class="mb-4 flex flex-wrap items-center gap-3"
  >
    <p class="text-sm font-medium text-neutral-700">
      {{ contractConditions.name }}
    </p>

    <img
      :src="contractConditions.fileUrl"
      :alt="contractConditions.name"
      class="size-12 shrink-0 rounded-lg object-cover"
    >

    <BaseLink
      :to="contractConditions.fileUrl"
      target="_blank"
      rel="noopener noreferrer"
      class="
        inline-flex shrink-0 items-center gap-2 rounded-full bg-primary/30 px-4 py-2 text-sm
        font-medium text-primary transition-colors
        hover:bg-primary/40
      "
    >
      <IconEye
        visible
        class="
          size-4
          [&>path]:stroke-primary
        "
      />
      {{ $t('content.preview') }}
    </BaseLink>
  </div>
</template>

<script setup lang="ts">
  import type { TripContractConditions } from '@/types/admin-dashboard/response/trip/trip'

  defineProps<{
    contractConditions?: TripContractConditions | null
  }>()
</script>

<style scoped>

</style>
