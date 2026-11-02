export const useNetworkStore = defineStore('network', () => {
  const isNetworkError = ref(false)
  const networkErrorMessage = ref<string | null>(null)

  function setNetworkError(error: boolean, message?: string) {
    isNetworkError.value = error
    networkErrorMessage.value = message || null
  }

  function clearNetworkError() {
    isNetworkError.value = false
    networkErrorMessage.value = null
  }

  return {
    isNetworkError,
    networkErrorMessage,
    setNetworkError,
    clearNetworkError,
  }
})

