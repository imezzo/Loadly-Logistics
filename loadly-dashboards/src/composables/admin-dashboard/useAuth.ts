import { useAuthStore } from "@/stores/admin-dashboard/auth";
import type { LoginRequest } from "@/types/admin-dashboard/request/auth";
import { MfaChannel } from "@/types/request/mfa";
import { cookies } from "@/utils/cookies"
import router from "@/router"
import { useProfileStore } from "@/stores/admin-dashboard/profile";
import { useEnum } from "../useEnum";

export function useAuth() {
  const authStore = useAuthStore()
  const { user } = storeToRefs(authStore)
  const profileStore = useProfileStore()
  const { initEnums } = useEnum()

  const isLoggedIn = computed(() => !!cookies.get('accessToken'))

  const isTempLoggedIn = computed(() => {
    const tempAccessToken = cookies.get('tempAccessToken')
    const tempRefreshToken = cookies.get('tempRefreshToken')

    return !!tempAccessToken || !!tempRefreshToken
  })

  function resetTokens() {
    cookies.removeAll()
    user.value = null
  }

  async function login(data: LoginRequest) {
    const response = await authStore.login(data)
    if (response.success) {
      resetTokens()
      user.value = response.data

      if (user.value?.user.mfaChannel === MfaChannel.NONE) {
        cookies.set('accessToken', response.data.accessToken)
        cookies.set('refreshToken', response.data.refreshToken)
        await profileStore.getProfile()
        await initEnums()
      } else {
        cookies.set('tempAccessToken', response.data.accessToken)
        cookies.set('tempRefreshToken', response.data.refreshToken)
      }
    }

    return response
  }

  async function logout() {
    await authStore.logout()
    user.value = null
    profileStore.profile = null

    resetTokens()
    router.push({name: 'Login'})
  }

  async function getRefreshToken(): Promise<void> {
    const refreshToken = cookies.get('refreshToken')
    if (!refreshToken) router.push({name: 'Login'})

    const { success, data } = await useAuthStore().refreshToken(refreshToken)
    cookies.removeAll()

    if (success) {
      cookies.set('accessToken', data.accessToken)
      cookies.set('refreshToken', data.refreshToken)

      return
    }

    router.push({name: 'Login'})
  }

  return {
    getRefreshToken,
    isLoggedIn,
    isTempLoggedIn,
    login,
    logout,
    resetTokens,
    user,
  }
}
