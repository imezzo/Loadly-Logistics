import {
  mainNavigation as adminMainNavigation,
  settingsNavigation as adminSettingsNavigation,
} from './admin-dashboard/sidebarNavigation'
import {
  mainNavigation as companyMainNavigation,
  settingsNavigation as companySettingsNavigation,
} from './company-dashboard/sidebarNavigation'
import { Platform } from './platform'

export interface SidebarItem {
  action?: () => void
  children?: SidebarItem[]
  comingSoon?: boolean
  href?: string
  icon?: string
  id: string
  isExpandable?: boolean
  label: string
  module?: string
  permission?: string
  routeName?: string
}

export const mainNavigation = (): SidebarItem[] => {
  switch (import.meta.env.MODE) {
    case Platform.ADMIN_DASHBOARD:
      return adminMainNavigation
    case Platform.COMPANY_DASHBOARD:
      return companyMainNavigation
    default:
      return []
  }
}

export const settingsNavigation = (): SidebarItem[] => {
  switch (import.meta.env.MODE) {
    case Platform.ADMIN_DASHBOARD:
      return adminSettingsNavigation
    case Platform.COMPANY_DASHBOARD:
      return companySettingsNavigation
    default:
      return []
  }
}
