import { ref, onBeforeUnmount, onMounted } from 'vue'
import { useIntervalFn } from '@vueuse/core'

const MAX_FAILED_ATTEMPTS = 3
const LOCKOUT_DURATION = 15 * 60
const LOCKOUT_KEY = 'account_lockout_end'
const FAILED_ATTEMPTS_KEY = 'failed_login_attempts'

export function useAccountLockout() {
  const isAccountLocked = ref(false)
  const lockoutTimer = ref(LOCKOUT_DURATION)
  const failedAttempts = ref(0)

  const { pause, resume, isActive } = useIntervalFn(
    () => {
      if (lockoutTimer.value > 0) {
        lockoutTimer.value--
      } else {
        stopLockout()
      }
    },
    1000,
    { immediate: false },
  )

  function startLockout(seconds: number = LOCKOUT_DURATION) {
    isAccountLocked.value = true
    lockoutTimer.value = seconds

    const lockoutEndTime = Date.now() + (seconds * 1000)
    localStorage.setItem(LOCKOUT_KEY, lockoutEndTime.toString())

    resume()
  }

  function stopLockout() {
    isAccountLocked.value = false
    pause()

    localStorage.removeItem(LOCKOUT_KEY)
  }

  function restoreLockoutFromStorage() {
    const lockoutEndTime = localStorage.getItem(LOCKOUT_KEY)
    const storedAttempts = localStorage.getItem(FAILED_ATTEMPTS_KEY)

    if (storedAttempts) {
      failedAttempts.value = Number.parseInt(storedAttempts, 10) || 0
    }

    if (lockoutEndTime) {
      const endTime = Number.parseInt(lockoutEndTime, 10)
      const now = Date.now()
      const remainingSeconds = Math.ceil((endTime - now) / 1000)

      if (remainingSeconds > 0) {
        startLockout(remainingSeconds)
      } else {
        localStorage.removeItem(LOCKOUT_KEY)
      }
    }
  }

  function onLoginSuccess() {
    failedAttempts.value = 0
    localStorage.removeItem(FAILED_ATTEMPTS_KEY)
  }

  function onLoginFailure() {
    failedAttempts.value++
    localStorage.setItem(FAILED_ATTEMPTS_KEY, failedAttempts.value.toString())

    if (failedAttempts.value >= MAX_FAILED_ATTEMPTS) {
      startLockout(LOCKOUT_DURATION)
      failedAttempts.value = 0
      localStorage.removeItem(FAILED_ATTEMPTS_KEY)
    }
  }

  onMounted(() => {
    restoreLockoutFromStorage()
  })

  onBeforeUnmount(() => {
    if (isActive.value) {
      pause()
    }
  })

  return {
    isAccountLocked,
    lockoutTimer,
    failedAttempts,
    startLockout,
    stopLockout,
    onLoginSuccess,
    onLoginFailure,
    MAX_FAILED_ATTEMPTS,
  }
}

