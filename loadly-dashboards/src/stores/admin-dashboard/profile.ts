import type { Profile } from "@/types/admin-dashboard/response/profile"
import { profileResponseAdapter } from "@/types/admin-dashboard/response/profile"
import { api } from "@/utils/api"

export const useProfileStore = defineStore('profile', () => {
  const profile = ref<Profile | null>(null)

  async function getProfile() {
    const response = await api({
      url: '/auth/profile',
      method: 'GET',
      responseAdapter: profileResponseAdapter,
    })

    if (response.success && response.data) {
      profile.value = response.data as Profile | null
    }

    return response
  }

  return {
    getProfile,
    profile,
  }
})

