<template>
  <div class="h-full rounded-xl bg-neutral-100 p-4">
    <div class="flex items-center gap-3">
      <img
        :src="defaultAvatar"
        :alt="$t('a11y.userAvatar')"
        class="size-12 rounded-full object-cover"
        loading="lazy"
      >

      <div class="min-w-0">
        <p class="text-lg font-bold text-neutral-10">
          {{ $t('content.currentDriver') }}
        </p>

        <p class="text-md text-content-secondary">
          <span class="me-2">
            {{ driverName || '—' }}
          </span>

          <span>
            {{ driverPhone || '—' }}
          </span>
        </p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
  import defaultAvatar from '@/assets/images/default-avatar/user.webp'

  defineProps<{
    driverName?: string
    driverPhone?: string
  }>()
</script>

<style scoped>

</style>
