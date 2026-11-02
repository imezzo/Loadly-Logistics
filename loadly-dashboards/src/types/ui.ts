import type { RouterLinkProps } from 'vue-router'

export interface BreadcrumbItem {
  label: string
  to?: RouterLinkProps['to'] | string
}

export interface TableColumn {
  key: string
  label?: string
  class?: string
  cellClass?: string
}

export interface SelectOption {
  label: string
  value: string
  content?: () => string
  searchableText?: string
}

export type ChipVariant = 'success' | 'active' | 'error' | 'warning' | 'info' | 'neutral' | 'disabled' | 'purple' | 'indigo' | 'amber' | 'lime'

export type AlertVariant = 'error' | 'success' | 'warning' | 'info' | 'default'

export interface DropdownMenuOption {
  label?: string
  icon?: string
  action?: () => void
  to?: RouterLinkProps['to']
}
