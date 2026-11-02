<template>
  <RouterLink
    :to="notificationUrl"
    class="flex gap-3 rounded-lg p-3 transition-colors"
    :class="isRead ? 'bg-white' : 'bg-error-300/25'"
  >
    <div class="shrink-0 pt-0.5">
      <IconMedicineBottle class="size-10" />
    </div>

    <div class="min-w-0 flex-1">
      <p class="text-sm font-medium text-neutral-800">
        {{ notification.title }}
      </p>

      <p class="mt-0.5 text-xs text-primary">
        {{ notification.date }} • {{ notification.type }}
      </p>

      <div
        class="
          mt-2 rounded-lg border border-neutral-200 bg-white px-3 py-2 text-sm text-neutral-600
        "
      >
        {{ notification.body }}
      </div>
    </div>
  </RouterLink>
</template>

<script setup lang="ts">
  import type { Notification } from '@/types/response/notification'

  const props = defineProps<{
    notification: Notification
    isRead: boolean
  }>()

  const notificationActions = computed<Record<string, { url: string }>>(() => {
    return {
      'open_trip': {
        url: `/trip/edit/${props.notification.resourceId}`,
      },
    }
  })

  const notificationUrl = computed(() => notificationActions.value[props.notification.clickAction]?.url ?? '')
</script>

<style scoped>

</style>
