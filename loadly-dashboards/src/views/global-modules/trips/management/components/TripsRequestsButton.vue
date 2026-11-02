<template>
  <!-- TODO: toggle class based on unread count -->
  <RouterLink
    to="/trips/requests"
    class="
      btn-secondary-outline flex items-center gap-3 rounded-full bg-neutral-30 py-2.5 ps-4 pe-3
    "
  >
    <IconTrip class="size-4 shrink-0" />

    <div class="flex flex-col items-start">
      <span class="text-sm font-bold">{{ $t('content.tripsRequests') }}</span>

      <span class="flex items-center gap-0.5 text-xs font-medium text-content-secondary">
        <!-- TODO: Replace with unread count when implemented -->
        {{ $t('content.checkNewTrip') }}
        <span class="size-1.5 shrink-0 rounded-full bg-primary text-center text-xs text-white" />
      </span>
    </div>
  </RouterLink>
</template>

<script setup lang="ts">

</script>

<style scoped>

</style>

