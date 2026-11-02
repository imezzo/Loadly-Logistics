import { i18n } from "@/locales";

export function formatValueWithCurrency(value?: number | null, currency?: string): string {
  return value != null
    ? `${Number(value ?? 0).toLocaleString('en-US')} ${currency ?? i18n.global.t('form.egp')}`
    : '-'
}

export function formatCompactCurrency(value: number, currency?: string): string {
  const curr = currency ?? i18n.global.t('form.egp')
  const abs = Math.abs(value)

  if (abs >= 1_000_000_000) {
    return `${(value / 1_000_000_000).toFixed(3)}B ${curr}`
  }

  if (abs >= 1_000_000) {
    return `${(value / 1_000_000).toFixed(3)}M ${curr}`
  }

  if (abs >= 1_000) {
    return `${(value / 1_000).toFixed(1)}K ${curr}`
  }

  return formatValueWithCurrency(value, curr)
}
