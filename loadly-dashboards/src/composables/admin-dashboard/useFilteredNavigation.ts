import { computed, type ComputedRef } from 'vue'
import type { SidebarItem } from '@/types/sidebarNavigation'
import { usePermission } from '@/composables/admin-dashboard/usePermission'

function filterItems(items: SidebarItem[]): SidebarItem[] {
  return items.filter((item) => {
    if (item.children
      && item.children.length
      && !filterItems(item.children).length) {
      return false
    }

    if (!item.module && !item.permission) {
      return true
    }

    if (item.module || item.permission) {
      const { hasModule, hasPermission } = usePermission(
        item.module ?? '',
        item.permission ?? '',
      )

      if (
        (item.module && !hasModule.value)
        || (item.permission && !hasPermission.value)
      ) {
        return false
      }

      if (item.module && !item.permission && hasModule.value) {
        return true
      }
    }

    return true
  }).map((item) => {
    if (item.children && item.children.length) {
      return {
        ...item,
        children: filterItems(item.children),
      }
    }

    return item
  })
}

export function useFilteredNavigation(items: () => SidebarItem[]): ComputedRef<SidebarItem[]> {
  return computed(() => filterItems(items()))
}

