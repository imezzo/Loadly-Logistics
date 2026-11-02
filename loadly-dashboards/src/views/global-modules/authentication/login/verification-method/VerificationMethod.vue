<template>
  <VerificationSelector @continue="selectVerificationMethod" />
</template>

<script setup lang="ts">
  import { useAuth } from '@/composables/admin-dashboard/useAuth'
  import { useMfaStore } from '@/stores/admin-dashboard/mfa'
  import { MfaChannel } from '@/types/request/mfa'
  import VerificationSelector from './components/VerificationSelector.vue'

  type VerificationMethod = MfaChannel | null

  const { resetTokens } = useAuth()
  const router = useRouter()
  const { sendMfaCode } = useMfaStore()

  async function selectVerificationMethod(method: VerificationMethod) {
    if (method === null) return

    const { success } = await sendMfaCode({ method })
    if (success) {
      router.push({
        name: 'LoginVerifyCode',
        params: { method }
      })

      return
    }

    resetTokens()
    router.push({name: 'Login'})
  }
</script>

<style scoped>

</style>

