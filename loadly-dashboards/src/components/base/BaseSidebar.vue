<template>
  <Transition
    v-if="isMobile"
    name="mobile-sidebar"
  >
    <div v-if="isMobileOpen">
      <div
        class="fixed inset-0 z-40 bg-black/50"
        @click="closeMobileSidebar"
      />

      <aside
        class="
          fixed start-0 top-0 z-50 flex h-screen w-sidebar flex-1 flex-col overflow-y-auto bg-white
          shadow-lg transition-transform duration-300
        "
        role="navigation"
        :aria-label="$t('a11y.sidebarNavigation')"
      >
        <div class="flex min-h-27 items-start justify-between gap-4 px-10.25 pt-9">
          <RouterLink
            to="/"
            class="mx-auto my-2 flex items-center justify-center"
          >
            <IconLogo
              class="w-30"
              with-name
            />
          </RouterLink>

          <button
            type="button"
            class="
              p-2 text-neutral-800 transition-colors
              hover:text-primary
            "
            @click="closeMobileSidebar"
          >
            <IconCross class="size-6" />
          </button>
        </div>

        <nav class="px-2 py-8">
          <ul>
            <SidebarItem
              v-for="item in mainNavigation"
              :key="item.id"
              :item="item"
              :is-active="item.routeName === $route.name"
              :is-expanded="!!expandedItems[item.id]"
              @toggle="toggleItem(item.id)"
            />
          </ul>

          <p class="mb-2 p-1 text-sm font-medium tracking-wider text-content-secondary">
            {{ $t('sidebar.settings') }}
          </p>

          <ul>
            <SidebarItem
              v-for="item in filteredSettingsNavigation"
              :key="item.id"
              :item="item"
              :is-active="item.routeName === $route.name"
              :is-expanded="!!expandedItems[item.id]"
              @toggle="toggleItem(item.id)"
            />
          </ul>
        </nav>
      </aside>
    </div>
  </Transition>

  <aside
    v-else
    class="
      fixed start-0 top-0 z-50 flex h-screen w-sidebar flex-1 flex-col overflow-y-auto bg-white
    "
    role="navigation"
    :aria-label="$t('a11y.sidebarNavigation')"
  >
    <RouterLink
      to="/"
      class="mx-auto my-6.5 flex size-50 h-auto max-h-fit items-center justify-center"
    >
      <IconLogo with-name />
    </RouterLink>

    <nav class="px-2 py-8">
      <ul>
        <SidebarItem
          v-for="item in mainNavigation"
          :key="item.id"
          :item="item"
          :is-active="isActiveItem(item)"
          :is-expanded="!!expandedItems[item.id]"
          @toggle="toggleItem(item.id)"
        />
      </ul>

      <p class="mb-2 p-1 text-sm font-medium tracking-wider text-content-secondary">
        {{ $t('sidebar.settings') }}
      </p>

      <ul>
        <SidebarItem
          v-for="item in filteredSettingsNavigation"
          :key="item.id"
          :item="item"
          :is-active="isActiveItem(item)"
          :is-expanded="!!expandedItems[item.id]"
          @toggle="toggleItem(item.id)"
        />
      </ul>
    </nav>
  </aside>
</template>

<script setup lang="ts">
  import { useMediaQuery } from '@vueuse/core'
  import { useAuth } from '@/composables/admin-dashboard/useAuth'
  import { useFilteredNavigation } from '@/composables/admin-dashboard/useFilteredNavigation'
  import type { SidebarItem as SidebarItemType } from '@/types/sidebarNavigation'
  import SidebarItem from '@/components/base/sidebar/SidebarItem.vue'
  import { mainNavigation as mainNav, settingsNavigation as settingsNav } from '@/types/sidebarNavigation'

  const isMobileOpen = defineModel<boolean>({ default: false })
  const isMobile = useMediaQuery('(max-width: 64rem)')
  const route = useRoute()

  const { logout } = useAuth()

  const expandedItems = reactive<Record<string, boolean>>({})

  const filteredMainNav = useFilteredNavigation(mainNav)
  const filteredSettingsNav = useFilteredNavigation(settingsNav)

  const mainNavigation = computed<SidebarItemType[]>(() => {
    return filteredMainNav.value
  })

  const filteredSettingsNavigation = computed<SidebarItemType[]>(() => {
    return filteredSettingsNav.value.map(item => {
      if (item.id === 'logout') {
        return {
          ...item,
          action: logout,
        }
      }

      return item
    })
  })

  function toggleItem(itemId: string): void {
    expandedItems[itemId] = !expandedItems[itemId]
  }

  function closeMobileSidebar(): void {
    isMobileOpen.value = false
  }

  function isActiveItem(item: SidebarItemType): boolean {
    const routePath = route.path.replace('/', '')
    const itemPath = item.href?.replace('/', '')

    if (!routePath) {
      return route.name === item.routeName
    }

    return itemPath
      ? routePath.concat('/').startsWith(itemPath.concat('/'))
      : false
  }
</script>

<style scoped>
  .mobile-sidebar-enter-active,
  .mobile-sidebar-leave-active {
    transition: opacity 300ms;
  }

  .mobile-sidebar-enter-active aside,
  .mobile-sidebar-leave-active aside {
    transition: transform 300ms;
  }

  .mobile-sidebar-enter-from {
    opacity: 0;
  }

  .mobile-sidebar-enter-from aside {
    transform: translateX(-100%);
  }

  .mobile-sidebar-leave-to {
    opacity: 0;
  }

  .mobile-sidebar-leave-to aside {
    transform: translateX(-100%);
  }
</style>
