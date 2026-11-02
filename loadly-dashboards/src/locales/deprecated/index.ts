import { createI18n } from 'vue-i18n'
import { Platform } from '@/types/platform'
import { useApp } from '@/composables/useApp'

interface LocaleModule {
  default: Record<string, string>
}

type LocaleMessages = Record<string, Record<string, Record<string, string>>>

interface ParsedPath {
  platformFolder: string
  namespace: string
  lang: string
}

const allowedLanguages = ['ar', 'en'] as const
type AllowedLanguage = typeof allowedLanguages[number]

const messages: LocaleMessages = {}
const modules = import.meta.glob('@/locales/**/*.{json,ts,js}', { eager: true })
const currentMode = import.meta.env.MODE as Platform

const isAllowedLanguage = (lang: string): lang is AllowedLanguage => {
  return allowedLanguages.includes(lang as AllowedLanguage)
}

const hasDefaultExport = (module: unknown): module is LocaleModule => {
  return (
    module !== null
    && typeof module === 'object'
    && 'default' in module
    && typeof module.default === 'object'
    && module.default !== null
  )
}

const parseLocalePath = (path: string): ParsedPath | null => {
  const pathParts = path.split('/')
  const [platformFolder, fileName] = pathParts.slice(-2)

  if (!platformFolder || !fileName) {
    console.warn(`[i18n] Invalid path structure: ${path}`)

    return null
  }

  // Remove file extension and split by dots
  const fileNameWithoutExt = fileName.replace(/\.(ts|js|json)$/, '')
  const fileNameParts = fileNameWithoutExt.split('.')

  if (fileNameParts.length !== 2) {
    console.warn(`[i18n] Invalid filename format (expected namespace.lang): ${fileName}`)

    return null
  }

  const [namespace, lang] = fileNameParts

  if (!namespace || !lang) {
    console.warn(`[i18n] Missing namespace or language in: ${fileName}`)

    return null
  }

  return { platformFolder, namespace, lang }
}

const shouldLoadModule = (platformFolder: string): boolean => {
  if (platformFolder === currentMode) return true

  const platformFolders = [
    Platform.ADMIN_DASHBOARD,
    Platform.APP_DASHBOARD,
    Platform.COMPANY_DASHBOARD
  ]

  return !platformFolders.includes(platformFolder as Platform)
}

for (const path in modules) {
  const module = modules[path]

  if (!hasDefaultExport(module)) {
    console.warn(`[i18n] Invalid module format at ${path}`)
    continue
  }

  const parsed = parseLocalePath(path)
  if (!parsed) continue

  const { platformFolder, namespace, lang } = parsed

  if (!shouldLoadModule(platformFolder)) continue

  if (!isAllowedLanguage(lang)) {
    console.warn(
      `[i18n] Unsupported language '${lang}' in ${path}. Allowed languages: ${allowedLanguages.join(', ')}`)
    continue
  }

  messages[lang] ??= {}
  messages[lang][namespace] ??= {}

  messages[lang][namespace] = {
    ...messages[lang][namespace],
    ...module.default
  }
}

function getStoredLocale(): string | undefined {
  try {
    const stored = typeof localStorage !== 'undefined' && localStorage.getItem('locale')

    return (stored && isAllowedLanguage(stored)) ? stored : undefined
  } catch {
    return undefined
  }
}

const getInitialLocale = (): string => getStoredLocale() ?? useApp().defaultLanguage

export const i18n = createI18n({
  legacy: false,
  locale: getInitialLocale(),
  fallbackLocale: useApp().defaultLanguage,
  messages,
})

export function applyStoredLocale(): void {
  const stored = getStoredLocale()
  if (!stored) return

  i18n.global.locale.value = stored
  if (document) document.documentElement.lang = stored
}
