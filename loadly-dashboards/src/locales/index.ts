import { createI18n } from 'vue-i18n'
import { useApp } from '@/composables/useApp'

const allowedLanguages = ['ar', 'en'] as const
export type AllowedLanguage = typeof allowedLanguages[number]

const isAllowedLanguage = (lang: string): lang is AllowedLanguage =>
  allowedLanguages.includes(lang as AllowedLanguage)

function getStoredLocale(): AllowedLanguage | undefined {
  try {
    const stored = typeof localStorage !== 'undefined' && localStorage.getItem('locale')

    return stored && isAllowedLanguage(stored) ? stored : undefined
  } catch {
    return undefined
  }
}

export const i18n = createI18n({
  legacy: false,
  locale: getStoredLocale() ?? useApp().defaultLanguage,
  fallbackLocale: useApp().defaultLanguage,
  messages: {},
})

export async function loadLocaleMessages(locale: AllowedLanguage): Promise<void> {
  const [
    a11y, button, content, error, form,
    loader, sidebar, success, table, title, validation,
  ] = await Promise.all([
    import(`@/locales/a11y.${locale}.ts`),
    import(`@/locales/button.${locale}.ts`),
    import(`@/locales/content.${locale}.ts`),
    import(`@/locales/error.${locale}.ts`),
    import(`@/locales/form.${locale}.ts`),
    import(`@/locales/loader.${locale}.ts`),
    import(`@/locales/sidebar.${locale}.ts`),
    import(`@/locales/success.${locale}.ts`),
    import(`@/locales/table.${locale}.ts`),
    import(`@/locales/title.${locale}.ts`),
    import(`@/locales/validation.${locale}.ts`),
  ])

  i18n.global.setLocaleMessage(locale, {
    a11y: a11y.default,
    button: button.default,
    content: content.default,
    error: error.default,
    form: form.default,
    loader: loader.default,
    sidebar: sidebar.default,
    success: success.default,
    table: table.default,
    title: title.default,
    validation: validation.default,
  })
}

export function applyStoredLocale(): void {
  const stored = getStoredLocale()
  if (!stored) return

  i18n.global.locale.value = stored
  if (document) document.documentElement.lang = stored
}
