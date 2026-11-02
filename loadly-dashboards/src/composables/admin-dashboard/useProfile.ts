import { useAuth } from '@/composables/admin-dashboard/useAuth'
import { useProfileStore } from '@/stores/admin-dashboard/profile'

export function useProfile() {
  const { isLoggedIn } = useAuth()
  const profileStore = useProfileStore()
  const { profile } = storeToRefs(profileStore)

  async function initProfile(): Promise<void> {
    if (isLoggedIn.value) {
      await profileStore.getProfile()
    }
  }

  return {
    initProfile,
    profile,
  }
}

