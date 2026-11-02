<template>
  <CodeInput
    :method="verificationMethod"
    :error-message="errorMessage"
    @back="back"
    @verify="verifyCode"
  />
</template>

<script setup lang="ts">
  import { useMfaStore } from '@/stores/admin-dashboard/mfa'
  import { MfaChannel } from '@/types/request/mfa'
  import { cookies } from '@/utils/cookies'
  import CodeInput from './components/CodeInput.vue'
  import { useApp } from '@/composables/useApp'

  const router = useRouter()
  const route = useRoute()
  const { isAdminDashboard } = useApp()

  const { verifyMfaCode } = useMfaStore()

  const verificationMethod = computed(() => {
    const method = Number(route.params.method) as MfaChannel | undefined

    return method === MfaChannel.EMAIL ? MfaChannel.EMAIL : MfaChannel.SMS
  })

  const errorMessage = ref<string | null>(null)

  async function verifyCode(code: string) {
    const { success, data, message } = await verifyMfaCode(code)
    if (success) {
      cookies.removeAll()
      cookies.set('accessToken', data.accessToken)
      cookies.set('refreshToken', data.refreshToken)
      router.push('/')
    } else {
      errorMessage.value = message
    }
  }

  function back() {
    router.push({
      name: isAdminDashboard ? 'Login' : 'LoginVerificationMethod',
    })
  }

  onMounted(() => {
    if (!route.params.method) {
      router.push({name: 'Login'})
    }
  })
</script>

<style scoped>

</style>

