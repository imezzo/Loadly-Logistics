# Deprecated i18n bootstrap (`index.ts`)

Legacy Vue I18n setup that **eager-loads** locale modules from the whole `@/locales` tree via Vite’s `import.meta.glob`. The running app uses **`src/locales/index.ts`** instead (`main.ts` imports from `./locales`).

## What `index.ts` does

1. **Glob** — `import.meta.glob('@/locales/**/*.{json,ts,js}', { eager: true })` pulls every matching file into the bundle at build time.

2. **Filename convention** — Paths are parsed as `.../<platformFolder>/<namespace>.<lang>.ext` (last two path segments). The file name must be exactly two dot-separated parts after removing the extension: `namespace` and `lang` (e.g. `content.en.ts` → namespace `content`, lang `en`). Invalid paths log `[i18n]` warnings.

3. **Platform filtering** — `import.meta.env.MODE` is treated as `Platform`. A module is included if:
   - its folder equals `currentMode`, **or**
   - its folder is **not** one of `ADMIN_DASHBOARD`, `APP_DASHBOARD`, `COMPANY_DASHBOARD` (shared / non-dashboard locales load in every mode).

4. **Languages** — Only `ar` and `en` are accepted; others are skipped with a warning.

5. **Merge** — Each module’s `default` export (flat `Record<string, string>`) is merged into `messages[lang][namespace]`.

6. **Instance** — `createI18n` with `legacy: false`, `locale` from `localStorage.locale` (if allowed) or `useApp().defaultLanguage`, `fallbackLocale` from `useApp().defaultLanguage`, and the built `messages` object.

7. **`applyStoredLocale()`** — If a valid locale is in `localStorage`, sets `i18n.global.locale` and `document.documentElement.lang`.

## Replacement

| Topic | This file | Current `src/locales/index.ts` |
|--------|-----------|----------------------------------|
| Loading | Eager glob over entire tree | Explicit `loadLocaleMessages(locale)` with targeted dynamic imports |
| Initial `messages` | Fully populated at import | `{}` until `loadLocaleMessages` runs |
| Platform folders | Filtered by `MODE` | Not used |

## Removal

Safe to delete this folder once nothing imports it and you no longer need the glob + platform-folder behavior for reference or migration.
