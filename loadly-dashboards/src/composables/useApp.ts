import { Platform } from "@/types/platform"

export function useApp() {
  const appName = import.meta.env.VITE_BRAND_NAME
  const defaultLanguage = import.meta.env.VITE_DEFAULT_LANGUAGE
  const baseUrl = import.meta.env.VITE_API_BASE_URL
  const baseMode = import.meta.env.VITE_API_BASE_MODE
  const modeBaseUrl = `${baseUrl}${baseMode}`

  const mode = import.meta.env.MODE
  const isAdminDashboard = mode === Platform.ADMIN_DASHBOARD
  const isAppDashboard = mode === Platform.APP_DASHBOARD
  const isCompanyDashboard = mode === Platform.COMPANY_DASHBOARD

  return {
    appName,
    baseMode,
    baseUrl,
    defaultLanguage,
    isAdminDashboard,
    isAppDashboard,
    isCompanyDashboard,
    modeBaseUrl,
  }
}
