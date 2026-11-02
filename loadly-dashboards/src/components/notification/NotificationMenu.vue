<template>
  <TransitionGroup
    enter-active-class="transition duration-200 ease-out"
    enter-from-class="opacity-0"
    enter-to-class="opacity-100"
    leave-active-class="transition duration-150 ease-in"
    leave-from-class="opacity-100"
    leave-to-class="opacity-0"
  >
    <div
      v-if="model"
      class="fixed inset-0 z-51 bg-black/50"
      @click="model = false"
    />

    <div
      v-if="model"
      class="
        absolute end-0 top-full z-51 mt-2 w-[min(24rem,90vw)] rounded-xl border border-neutral-200
        bg-white shadow-xl
      "
    >
      <div class="flex items-center justify-between border-b border-neutral-200 px-4 py-3">
        <h3 class="flex items-center gap-2 text-lg font-semibold text-neutral-800">
          {{ $t('a11y.notifications') }}
          <span
            v-if="menuUnreadBadge > 0"
            class="
              flex size-6 items-center justify-center rounded-xl bg-error-600 px-1.5 text-xs
              font-semibold text-white
            "
          >
            {{ menuUnreadBadge }}
          </span>
        </h3>

        <button
          type="button"
          class="btn-link ms-auto"
          @click="emit('markAllAsRead')"
        >
          {{ $t('content.markAllAsRead') }}
        </button>

        <button
          type="button"
          class="
            flex size-8 items-center justify-center rounded-lg text-neutral-600 transition-colors
            hover:bg-neutral-100 hover:text-neutral-800
          "
          :aria-label="$t('a11y.close')"
          @click="model = false"
        >
          <IconCross class="size-5" />
        </button>
      </div>

      <span
        v-if="isLoading"
        class="loader"
      />

      <div
        class="max-h-96 overflow-y-auto p-2"
        @scroll="onScroll"
      >
        <div
          v-if="unreadNotifications.length"
          class="mb-4 space-y-2"
        >
          <h4 class="mb-2 px-2 text-sm font-semibold text-neutral-800">
            {{ $t('content.unread') }}
          </h4>
          <NotificationItem
            v-for="item in unreadNotifications"
            :key="item.id"
            :notification="item"
            :is-read="false"
          />
        </div>

        <div
          v-if="readNotifications.length"
          class="space-y-2"
        >
          <h4 class="mb-2 px-2 text-sm font-semibold text-neutral-800">
            {{ $t('content.read') }}
          </h4>
          <NotificationItem
            v-for="item in readNotifications"
            :key="item.id"
            :notification="item"
            :is-read="true"
          />
        </div>

        <div
          v-if="!notifications.length"
          class="py-8 text-center text-sm text-content-secondary"
        >
          {{ $t('content.noNotifications') }}
        </div>
      </div>
    </div>
  </TransitionGroup>
</template>

<script setup lang="ts">
  import { useNotificationStore } from '@/stores/notification'
  import { createScrollToEndHandler } from '@/composables/ui/useScrollToEnd'
  import type { Notification } from '@/types/response/notification'

  const props = defineProps<{
    notifications: Notification[]
    isLoading: boolean
  }>()

  const emit = defineEmits<{
    scrollToEnd: []
    markAllAsRead: []
  }>()

  const model = defineModel<boolean>('modelValue', { required: true })

  const notificationStore = useNotificationStore()
  const { unreadCount } = storeToRefs(notificationStore)

  const menuUnreadBadge = computed(() => unreadCount.value)

  const unreadNotifications = computed(() => props.notifications.filter(notification => !notification.isRead))
  const readNotifications = computed(() => props.notifications.filter(notification => notification.isRead))

  const onScroll = createScrollToEndHandler(() => emit('scrollToEnd'))
</script>

<style scoped>

</style>
