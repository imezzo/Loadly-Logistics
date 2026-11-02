<template>
  <header
    v-if="isMobile"
    class="
      fixed start-0 end-0 top-0 z-30 flex h-16 items-center justify-between bg-white px-4 shadow-sm
    "
  >
    <button
      type="button"
      :aria-label="$t('a11y.toggle')"
      class="
        flex items-center justify-center p-2 text-neutral-800 transition-colors
        hover:text-primary
      "
      @click="$emit('openMobileSidebar')"
    >
      <IconMenu class="size-6" />
    </button>

    <RouterLink
      to="/"
      class="mx-auto my-6.5 flex items-center justify-center"
    >
      <IconLogo
        class="[&>svg]:h-12"
        with-name
      />
    </RouterLink>

    <div class="flex items-center gap-2">
      <BaseLanguageSwitcher />
      <NotificationButton
        :has-notification="hasUnreadNotifications"
        @click="showNotificationMenu = true"
      />
      <NotificationMenu
        v-model="showNotificationMenu"
        :notifications="notifications?.data ?? []"
        :is-loading="isNotificationLoading"
        @scroll-to-end="onScrollToEnd"
        @mark-all-as-read="markAllAsRead"
      />
    </div>
  </header>

  <header
    v-else
    class="
      fixed start-sidebar end-0 top-0 z-51 flex h-header items-center justify-between bg-white px-6
      py-4
    "
  >
    <BaseInputSearch wrapper-class="bg-neutral-30 flex max-w-100.5 flex-1 items-center rounded-3xl p-4" />

    <div class="relative flex items-center gap-6">
      <BaseLanguageSwitcher />

      <NotificationButton
        :has-notification="hasUnreadNotifications"
        :notification-count="unreadCount"
        @click="showNotificationMenu = true"
      />
      <NotificationMenu
        v-model="showNotificationMenu"
        :notifications="notifications?.data ?? []"
        :is-loading="isNotificationLoading"
        @scroll-to-end="onScrollToEnd"
        @mark-all-as-read="markAllAsRead"
      />

      <BaseUserProfile
        :name="profile?.fullName || $t('content.user')"
        :role="profile?.userType || $t('content.user')"
        :avatar-alt="$t('a11y.userAvatar')"
      />
    </div>
  </header>
</template>

<script setup lang="ts">
  import { useMediaQuery } from '@vueuse/core'
  import { useProfileStore } from '@/stores/admin-dashboard/profile'
  import { useNotifications } from '@/composables/useNotifications'
  import { useNotificationStore } from '@/stores/notification'

  defineEmits<{
    openMobileSidebar: []
  }>()

  const isMobile = useMediaQuery('(max-width: 64rem)')

  const profileStore = useProfileStore()
  const { profile } = storeToRefs(profileStore)
  const notificationStore = useNotificationStore()
  const { unreadCount } = storeToRefs(notificationStore)

  const {
    hasUnreadNotifications,
    isNotificationLoading,
    markAllAsRead,
    notifications,
    onScrollToEnd,
    showNotificationMenu,
  } = useNotifications({ exclude: 1 })
</script>

<style scoped>

</style>
