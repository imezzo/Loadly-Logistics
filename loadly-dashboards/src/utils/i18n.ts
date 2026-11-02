import { i18n } from '@/locales'

export function getLocalizedValue(enValue: string, arValue: string): string {
  const currentLocale = i18n.global.locale.value

  return currentLocale === 'ar' ? arValue : enValue
}
